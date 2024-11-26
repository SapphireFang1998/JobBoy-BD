using DatabaseLayer;
using JobBoyBD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobBoyBD.Controllers
{
    public class JobController : Controller
    {
        private JobBoyBD_DBEntities Db = new JobBoyBD_DBEntities();
        // GET: Job
        public ActionResult PostJob()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            var job = new PostJobMV();
            ViewBag.JobCategoryID = new SelectList(
                                    Db.JobCategoryTables.ToList(),
                                    "JobCategoryID",
                                    "JobCategory",
                                    "0");
            ViewBag.JobNatureID = new SelectList(
                                    Db.JobNatureTables.ToList(),
                                    "JobNatureID",
                                    "JobNature",
                                    "0");

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostJob(PostJobMV postJobMV)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            int userid = 0;
            int companyid = 0;
            int.TryParse(Convert.ToString(Session["UserID"]), out userid);
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyid);
            postJobMV.UserID = userid;
            postJobMV.CompanyID = companyid;

            if (ModelState.IsValid)
            {
                var post = new PostJobTable();

                post.UserID = postJobMV.UserID;
                post.CompanyID = postJobMV.CompanyID;
                post.JobCategoryID = postJobMV.JobCategoryID;
                post.JobTitle = postJobMV.JobTitle;
                post.JobDescription = postJobMV.JobDescription;
                post.MinSalary = postJobMV.MinSalary;
                post.MaxSalary = postJobMV.MaxSalary;
                post.Location = postJobMV.Location;
                post.Vacancy = postJobMV.Vacancy;
                post.JobNatureID = postJobMV.JobNatureID;
                post.PostDate = DateTime.Now;
                post.ApplicationLastDate = postJobMV.ApplicationLastDate;
                post.LastDate = postJobMV.ApplicationLastDate;
                post.JobStatusID = 1;
                post.WebURL = postJobMV.WebURL;

                Db.PostJobTables.Add(post);
                Db.SaveChanges();

                return RedirectToAction("CompanyJobsList");
            }

            ViewBag.JobCategoryID = new SelectList(
                                    Db.JobCategoryTables.ToList(),
                                    "JobCategoryID",
                                    "JobCategory",
                                    "0");
            ViewBag.JobNatureID = new SelectList(
                                    Db.JobNatureTables.ToList(),
                                    "JobNatureID",
                                    "JobNature",
                                    "0");

            return View(postJobMV);
        }

        public ActionResult CompanyJobsList()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            int userid = 0;
            int companyid = 0;
            int.TryParse(Convert.ToString(Session["UserID"]), out userid);
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyid);
            var allpost = Db.PostJobTables.Where(c => c.CompanyID == companyid && c.UserID == userid).ToList();

            return View(allpost);
        }

        public ActionResult AllCompanyPendingJobs()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            int userid = 0;
            int companyid = 0;
            int.TryParse(Convert.ToString(Session["UserID"]), out userid);
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyid);
            var allpost = Db.PostJobTables.ToList();
            if (allpost.Count() > 0)
            {
                allpost = allpost.OrderByDescending(o => o.PostJobID).ToList();
            }

            return View(allpost);
        }

        public ActionResult AddRequirements(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            var details = Db.JobRequirementDetailTables.Where(j => j.PostJobID == id).ToList();
            if (details.Count() > 0)
            {
                details = details.OrderBy(r => r.JobRequirementID).ToList();
            }

            var requirments = new JobRequirementsMV();
            requirments.Details = details;
            requirments.PostJobID = (int)id;

            ViewBag.JobRequirementID = new SelectList(Db.JobRequirementTables.ToList(), "JobRequirementID", "JobRequirementTitle", "0");

            return View(requirments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRequirements(JobRequirementsMV jobRequirementsMV)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            try
            {
                var requiments = new JobRequirementDetailTable();
                requiments.JobRequirementID = jobRequirementsMV.JobRequirementID;
                requiments.JobRequirementDetail = jobRequirementsMV.JobRequirementDetail;
                requiments.PostJobID = jobRequirementsMV.PostJobID;

                Db.JobRequirementDetailTables.Add(requiments);
                Db.SaveChanges();

                return RedirectToAction("AddRequirements", new {id = requiments.PostJobID });
            }
            catch (Exception ex)
            {
                var details = Db.JobRequirementDetailTables.Where(j => j.PostJobID == jobRequirementsMV.PostJobID).ToList();
                if (details.Count() > 0)
                {
                    details = details.OrderBy(r => r.JobRequirementID).ToList();
                }
                jobRequirementsMV.Details = details;
                ModelState.AddModelError("JobRequirementID", "Required*");
            }
            
            ViewBag.JobRequirementID = new SelectList(Db.JobRequirementTables.ToList(), "JobRequirementID", "JobRequirementTitle", jobRequirementsMV.JobRequirementID);

            return View(jobRequirementsMV);
        }

        public ActionResult DeleteRequirements(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            var jobpostid = Db.JobRequirementDetailTables.Find(id).PostJobID;
            var requirements = Db.JobRequirementDetailTables.Find(id);
            Db.Entry(requirements).State = System.Data.Entity.EntityState.Deleted;
            Db.SaveChanges();

            return RedirectToAction("AddRequirements", new { id = jobpostid });
        }

        public ActionResult DeleteJobPost(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            var jobpost = Db.PostJobTables.Find(id);
            Db.Entry(jobpost).State = System.Data.Entity.EntityState.Deleted;
            Db.SaveChanges();

            return RedirectToAction("CompanyJobsList");
        }

        public ActionResult ApprovedPost(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            var jobpost = Db.PostJobTables.Find(id);
            jobpost.JobStatusID = 2;
            Db.Entry(jobpost).State = System.Data.Entity.EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("AllCompanyPendingJobs");
        }

        public ActionResult CanceledPost(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            var jobpost = Db.PostJobTables.Find(id);
            jobpost.JobStatusID = 3;
            Db.Entry(jobpost).State = System.Data.Entity.EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("AllCompanyPendingJobs");
        }

        public ActionResult JobDetails(int? id)
        {
            var getpostjob = Db.PostJobTables.Find(id);
            var postjob = new PostJobDetailMV();

            postjob.PostJobID = getpostjob.PostJobID;
            postjob.Company = getpostjob.CompanyTable.CompanyName;
            postjob.JobCategory = getpostjob.JobCategoryTable.JobCategory;
            postjob.JobTitle = getpostjob.JobTitle;
            postjob.JobDescription = getpostjob.JobDescription;
            postjob.MinSalary = getpostjob.MinSalary;
            postjob.MaxSalary = getpostjob.MaxSalary;
            postjob.Location = getpostjob.Location;
            postjob.Vacancy = getpostjob.Vacancy;
            postjob.JobNature = getpostjob.JobNatureTable.JobNature;
            postjob.PostDate = getpostjob.PostDate;
            postjob.ApplicationLastDate = getpostjob.ApplicationLastDate;
            postjob.WebURL = getpostjob.WebURL;

            getpostjob.JobRequirementDetailTables = getpostjob.JobRequirementDetailTables.OrderBy(d => d.JobRequirementID).ToList();

            int jobrequirmentid = 0;
            var jobrequirement = new JobRequirementMV();
            foreach (var detail in getpostjob.JobRequirementDetailTables)
            {
                var jobrequirementdetails = new JobRequirementDetailMV();

                if (jobrequirmentid == 0)
                {
                    jobrequirement.JobRequirementID = detail.JobRequirementID;
                    jobrequirement.JobRequirementTitle = detail.JobRequirementTable.JobRequirementTitle;
                    jobrequirementdetails.JobRequirementID = detail.JobRequirementID;
                    jobrequirementdetails.JobRequirementDetail = detail.JobRequirementDetail;
                    jobrequirement.Details.Add(jobrequirementdetails);
                    jobrequirmentid = detail.JobRequirementID;
                }
                else if (jobrequirmentid == detail.JobRequirementID)
                {
                    jobrequirementdetails.JobRequirementID = detail.JobRequirementID;
                    jobrequirementdetails.JobRequirementDetail = detail.JobRequirementDetail;
                    jobrequirement.Details.Add(jobrequirementdetails);
                    jobrequirmentid = detail.JobRequirementID;
                }
                else if (jobrequirmentid != detail.JobRequirementID)
                {
                    postjob.Requirements.Add(jobrequirement);
                    jobrequirement = new JobRequirementMV();
                    jobrequirement.JobRequirementID = detail.JobRequirementID;
                    jobrequirement.JobRequirementTitle = detail.JobRequirementTable.JobRequirementTitle;
                    jobrequirementdetails.JobRequirementID = detail.JobRequirementID;
                    jobrequirementdetails.JobRequirementDetail = detail.JobRequirementDetail;
                    jobrequirement.Details.Add(jobrequirementdetails);
                    jobrequirmentid = detail.JobRequirementID;
                }
            }
            postjob.Requirements.Add(jobrequirement);

            return View(postjob);
        }

        public ActionResult FilterJob()
        {
            var obj = new FilterJobMV();
            var date = DateTime.Now.Date;
            var result = Db.PostJobTables.Where(r => r.ApplicationLastDate >= date && r.JobStatusID == 2).ToList();
            obj.Result = result;

            ViewBag.JobCategoryID = new SelectList(
                                    Db.JobCategoryTables.ToList(),
                                    "JobCategoryID",
                                    "JobCategory",
                                    "0");
            ViewBag.JobNatureID = new SelectList(
                                    Db.JobNatureTables.ToList(),
                                    "JobNatureID",
                                    "JobNature",
                                    "0");

            return View(obj); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FilterJob(FilterJobMV filterJobMV)
        {
            var date = DateTime.Now.Date;
            var result = Db.PostJobTables.Where(r => r.ApplicationLastDate >= date && r.JobStatusID == 2 && (r.JobCategoryID == filterJobMV.JobCategoryID && r.JobNatureID == filterJobMV.JobNatureID)).ToList();
            filterJobMV.Result = result;

            ViewBag.JobCategoryID = new SelectList(
                                    Db.JobCategoryTables.ToList(),
                                    "JobCategoryID",
                                    "JobCategory",
                                    filterJobMV.JobCategoryID);
            ViewBag.JobNatureID = new SelectList(
                                    Db.JobNatureTables.ToList(),
                                    "JobNatureID",
                                    "JobNature",
                                    filterJobMV.JobNatureID);

            return View(filterJobMV);
        }
    }
}
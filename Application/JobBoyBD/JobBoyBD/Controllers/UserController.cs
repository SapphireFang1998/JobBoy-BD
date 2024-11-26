using DatabaseLayer;
using JobBoyBD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobBoyBD.Controllers
{
    public class UserController : Controller
    {
        private JobBoyBD_DBEntities Db = new JobBoyBD_DBEntities();
        // GET: User
        public ActionResult NewUser()
        {
            return View(new UserMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUser(UserMV userMV)
        {
            if (ModelState.IsValid)
            {
                var checkuser = Db.UserTables.Where(u => u.EmailAddress == userMV.EmailAddress).FirstOrDefault();
                if (checkuser != null)
                {
                    ModelState.AddModelError("EmailAddress", "Email is Already Registered!");
                    return View(userMV);
                }

                checkuser = Db.UserTables.Where(u => u.UserName == userMV.UserName).FirstOrDefault();
                if (checkuser != null)
                {
                    ModelState.AddModelError("UserNames", "Username is Already Registered!");
                    return View(userMV);
                }

                using (var trans = Db.Database.BeginTransaction())
                {
                    try
                    {
                        var user = new UserTable();
                        user.UserName = userMV.UserName;
                        user.Password = userMV.Password;
                        user.ContactNo = userMV.ContactNo;
                        user.EmailAddress = userMV.EmailAddress;
                        user.UserTypeID = userMV.AreYouProvider == true ? 2 : 3; //In UserTypeTable ID 2 is Job Provider and ID 3 is Job Seeker.
                        Db.UserTables.Add(user);
                        Db.SaveChanges();

                        if (userMV.AreYouProvider == true)
                        {
                            var company = new CompanyTable();
                            company.UserID = user.UserID;

                            if(string.IsNullOrEmpty(userMV.Company.EmailAddress))
                            {
                                trans.Rollback();
                                ModelState.AddModelError("Company.EmailAddress", "Required*");
                                return View(userMV);
                            }
                            if (string.IsNullOrEmpty(userMV.Company.CompanyName))
                            {
                                trans.Rollback();
                                ModelState.AddModelError("Company.CompanyName", "Required*");
                                return View(userMV);
                            }
                            if (string.IsNullOrEmpty(userMV.Company.PhoneNo))
                            {
                                trans.Rollback();
                                ModelState.AddModelError("Company.PhoneNo", "Required*");
                                return View(userMV);
                            }
                            if (string.IsNullOrEmpty(userMV.Company.Description))
                            {
                                trans.Rollback();
                                ModelState.AddModelError("Company.Description", "Required*");
                                return View(userMV);
                            }

                            company.EmailAddress = userMV.Company.EmailAddress;
                            company.CompanyName = userMV.Company.CompanyName;
                            company.ContactNo = userMV.ContactNo;
                            company.PhoneNo = userMV.Company.PhoneNo;
                            company.Logo = "~/Content/assets/img/logo/logo.png";
                            company.Description = userMV.Company.Description;
                            Db.CompanyTables.Add(company);
                            Db.SaveChanges();
                        }

                        trans.Commit();
                        return RedirectToAction("Login");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "Please Provide Correct Details!");
                        trans.Rollback();
                    }
                }
            }
            return View(userMV);
        }

        public ActionResult Login()
        {
            return View(new UserLoginMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(UserLoginMV userLoginMV)
        {
            if(ModelState.IsValid) 
            {
                var user = Db.UserTables.Where(u=>u.UserName == userLoginMV.UserName && u.Password == userLoginMV.Password).FirstOrDefault();
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "UserName or Password is Incorrect!");
                    return View(userLoginMV);
                }

                Session["UserID"] = user.UserID;
                Session["UserName"] = user.UserName;
                Session["UserTypeID"] = user.UserTypeID;

                if(user.UserTypeID == 2)
                {
                    Session["CompanyID"] = user.CompanyTables.FirstOrDefault().CompanyID;
                }

                return RedirectToAction("Index", "Home");
            }
            
            return View(userLoginMV);
        }

        public ActionResult Logout()
        {
            Session["UserID"] = string.Empty;
            Session["UserName"] = string.Empty;
            Session["CompanyID"] = string.Empty;
            Session["UserTypeID"] = string.Empty;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AllUsers()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }

            var users = Db.UserTables.ToList();
            return View(users);
        }

        public ActionResult Forgot()
        {
            return View(new ForgotPasswordMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Forgot(ForgotPasswordMV forgotPasswordMV)
        {
            var user = Db.UserTables.Where(u => u.EmailAddress == forgotPasswordMV.Email).FirstOrDefault();

            if(user != null) 
            {
                string userandpassword = "User Name : " + user.UserName + "\n" + "Password " + user.Password;
                string body = userandpassword;
                bool IsSendEmail = JobBoyBD.Forgot.Email.Emailsend(user.EmailAddress, "Account Details", body, true);
                if (IsSendEmail)
                {
                    ModelState.AddModelError(string.Empty, "UserName and Password is Send!");
                }
                else
                {
                    ModelState.AddModelError("Email", "Your Email is Registered! Currently email sending is not working properly. Please try again after sometime.");
                }
            }
            else
            {
                ModelState.AddModelError("Email", "Email is not Registered!");
            }
            
            return View(forgotPasswordMV);
        }
    }
}
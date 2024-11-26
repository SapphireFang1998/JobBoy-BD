using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace JobBoyBD.Models
{
    public class JobRequirementMV
    {
        public JobRequirementMV() 
        { 
            Details = new List<JobRequirementDetailMV>();
        }
        
        public int JobRequirementID { get; set; }
        public string JobRequirementTitle { get; set; }

        public List<JobRequirementDetailMV> Details { get; set; }
    }
}
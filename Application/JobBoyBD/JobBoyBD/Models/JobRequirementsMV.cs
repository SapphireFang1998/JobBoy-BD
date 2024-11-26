using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace JobBoyBD.Models
{
    public class JobRequirementsMV
    {
        public JobRequirementsMV() 
        {
            Details = new List<JobRequirementDetailTable>(); 
        }

        [Required(ErrorMessage ="Required*")]
        public int JobRequirementID { get; set; }
        [Required(ErrorMessage = "Required*")]
        public string JobRequirementDetail { get; set; }
        public int PostJobID { get; set; }

        public List<JobRequirementDetailTable> Details { get; set; }
    }
}
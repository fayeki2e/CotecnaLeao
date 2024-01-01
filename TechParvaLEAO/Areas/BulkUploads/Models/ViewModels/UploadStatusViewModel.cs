using System;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace TechParvaLEAO.Models
{
    /*
     * View model for Upload
     */

    public class UploadStatusViewModel {
      

        public UploadStatusViewModel()
        {
            StatusListItems = new List<StatusViewModel>();
        }
        public virtual IList<StatusViewModel> StatusListItems { get; set; }
    }

    public class StatusViewModel
    {
        public int Row_ID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "UploadedOn")]
        public DateTime UploadedOn { get; set; }
        public string UploadedBy { get; set; }
       public int AddedRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public int FailedRecords { get; set; }
    }




}

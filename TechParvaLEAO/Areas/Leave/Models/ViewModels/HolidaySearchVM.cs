using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechParvaLEAO.Areas.Leave.Models.ViewModels
{
    /*
    * View Model for Holiday Search
    */
    public class HolidaySearchVM
    {
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Holiday Date")]
        public DateTime HolidayDate { get; set; }

        public string Reason { get; set; }

        [Display(Name = "Is Half Day")]
        public bool IsHalfDay { get; set; }

        public virtual int LocationId { get; set; }

    }
}

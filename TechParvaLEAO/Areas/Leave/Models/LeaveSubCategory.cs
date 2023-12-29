using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Leave.Models;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Leave.Models
{
    /*
 * Model for Leave Sub Category
 */
    public partial class LeaveSubCategory: Entity<int>
    {
        public LeaveSubCategory()
        { }
        [Display(Name = "Leave Sub Category")]
        public string Text { get; set; }

        public bool Deactivated { get; set; } = false;

    }

    /*
 * Model for Leave Type Leave Category Leave Sub Category Mapping
 */
    public partial class LeaveTypeLeaveCategoryLeaveSubCategoryMapping
    {
        public LeaveTypeLeaveCategoryLeaveSubCategoryMapping() { }

        public int Id { get; set; }

        [Display(Name = "Leave Type")]
        [ForeignKey("LeaveType")]
        public virtual int LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }

        [Display(Name = "Leave Category")]
        [ForeignKey("LeaveCategory")]
        public virtual int? LeaveCategoryId { get; set; }
        public virtual LeaveCategory LeaveCategory { get; set; }

        [Display(Name = "Leave Sub Category")]
        [ForeignKey("LeaveSubCategory")]
        public virtual int LeaveSubCategoryId { get; set; }
        public virtual LeaveSubCategory LeaveSubCategory { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}
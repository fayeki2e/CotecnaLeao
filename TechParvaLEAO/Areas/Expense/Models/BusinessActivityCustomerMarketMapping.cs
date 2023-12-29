using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Business Activity Customer Market Mapping
     */
    public partial class BusinessActivityCustomerMarketMapping : Entity<int>
    {
        public BusinessActivityCustomerMarketMapping()
        { }

        [Display(Name = "Business Activity")]
        [ForeignKey("BusinessActivity")]
        public virtual int BusinessActivityId { get; set; }
        public virtual BusinessActivity BusinessActivity { get; set; }

        [Display(Name = "Customer Market")]
        [ForeignKey("CustomerMarket")]
        public virtual int CustomerMarketId { get; set; }
        public virtual CustomerMarket CustomerMarket { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}
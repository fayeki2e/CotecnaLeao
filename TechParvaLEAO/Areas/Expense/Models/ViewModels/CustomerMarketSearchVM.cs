using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Expense.Models.ViewModels
{
    /*
     * View Model for Customer Market Search
     */
    public class CustomerMarketSearchVM
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public virtual int? LocationId { get; set; }
    }
}

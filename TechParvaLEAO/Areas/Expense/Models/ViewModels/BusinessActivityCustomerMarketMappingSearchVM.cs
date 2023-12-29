using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Expense.Models.ViewModels
{
    /*
     * View Model for Business Activity Customer Market Mapping Search
     */
    public class BusinessActivityCustomerMarketMappingSearchVM
    {
        public virtual int? BusinessActivityId { get; set; }
        public virtual int? CustomerMarketId { get; set; }
    }
}

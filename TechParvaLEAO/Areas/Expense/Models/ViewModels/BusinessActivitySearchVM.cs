using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Expense.Models.ViewModels
{
    /*
     * View Model for Business Activity Search
     */
    public class BusinessActivitySearchVM
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? BusinessUnitid { get; set; }
    }
}

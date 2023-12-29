using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechParvaLEAO.Areas.Organization.Models;

namespace TechParvaLEAO.Controllers
{
    public abstract class BaseDefaultController : ControllerBase
    {
        public Employee GetEmployee()
        {
            var employee = null as Employee;
            if (HttpContext.Items.ContainsKey("EMPLOYEE")){
                employee = (Employee)HttpContext.Items["EMPLOYEE"];
            }
            return employee;
        }
    }

    public abstract class BaseViewController : Controller
    {
        public Employee GetEmployee()
        {
            var employee = null as Employee;
            if (HttpContext.Items.ContainsKey("EMPLOYEE"))
            {
                employee = (Employee)HttpContext.Items["EMPLOYEE"];
            }
            return employee;
        }

    }

}
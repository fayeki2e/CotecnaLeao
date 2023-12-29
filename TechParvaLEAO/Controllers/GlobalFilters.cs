using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Techparva.GenericRepository;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;

namespace TechParvaLEAO.Controllers
{
    public class MySampleActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var User = context.HttpContext.User;
                var repository = context.HttpContext.RequestServices.GetService<IApplicationRepository>();
                var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
                var signInManager = context.HttpContext.RequestServices.GetService<SignInManager<ApplicationUser>>();

                if (User.Identity.Name != null)
                {
                    ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
                    if (user.EmployeeProfileId != null)
                    {
                        var employee = repository.GetById<Employee>(user.EmployeeProfileId);
                        context.HttpContext.Items["EMPLOYEE"] = employee;
                        var controller = context.Controller as Controller;
                        if (controller != null) controller.ViewData["Employee_Loggedin"] = employee.Id;
                        if (controller != null) controller.ViewData["Employee_CanApplyOnBehalf"] = 
                                (User.IsInRole(AuthorizationRoles.LOCATION_COORDINATOR)
                                || User.IsInRole(AuthorizationRoles.TIMESHEET))&&
                                controller.Request.Path.Value.Contains("OnBehalf")
                                ;

                        if (DateTime.Today - employee.LastWorkingDate > TimeSpan.FromDays(15))
                        {
                            await signInManager.SignOutAsync();
                        }
                    }
                }
            }
            await next();
        }
    }
}

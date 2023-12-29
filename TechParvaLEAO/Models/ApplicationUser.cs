using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TechParvaLEAO.Data;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Transactions;
using TechParvaLEAO.Areas.Organization.Models;

namespace TechParvaLEAO.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Employee ID")]
        public int? EmployeeProfileId { get; set; }
    }

    public class ApplicationSignInManager: SignInManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IApplicationRepository _repository;
        public ApplicationSignInManager(
        UserManager<ApplicationUser> userManager,
                IHttpContextAccessor contextAccessor,
                IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
                IOptions<IdentityOptions> optionsAccessor,
                ILogger<SignInManager<ApplicationUser>> logger,
                ApplicationDbContext dbContext,
                IAuthenticationSchemeProvider schemeProvider,
                IApplicationRepository repository
                )
                : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemeProvider)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _repository = repository;
        }

        override public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var result = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            var user = await UserManager.FindByNameAsync(userName);
            if (user?.EmployeeProfileId != null)
            {
                var employee = _repository.GetById<Employee>(user.EmployeeProfileId);
                if (employee == null ||
                    (DateTime.Today - employee.LastWorkingDate >= TimeSpan.FromDays(15) || employee.Deactivated))
            {
                    await SignOutAsync();
                    return SignInResult.Failed;
                }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechParvaLEAO.Models;
using System.IO;
using Microsoft.Extensions.FileProviders;
using AutoMapper;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Controllers;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using Cotecna.Email_Process;
using SmartBreadcrumbs.Extensions;
using Techparva.GenericRepository;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Services;
using Postal.AspNetCore;
using MediatR;
using System.Reflection;
using System.Globalization;
using FluentValidation.AspNetCore;
using FluentValidation;
using TechParvaLEAO.Notification;
using TechParvaLEAO.Areas.Reports.Services;
using TechParvaLEAO.Areas.Reports.Controllers;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.Runtime.Loader;
using TechParvaLEAO.Service;
using System.Data;
using TechParvaLEAO.Areas.BulkUploads.Services;

namespace TechParvaLEAO
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseLazyLoadingProxies()
                        .UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<ApplicationSignInManager>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 9;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
            });

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileuploads")));
            services.AddScoped<IEmployeeServices, EmployeeServices>();
            services.AddScoped<PaymentRequestService, PaymentRequestService>();
            services.AddScoped<LeaveRequestServices, LeaveRequestServices>();
            services.AddScoped<TimeSheetServices, TimeSheetServices>();
            services.AddScoped<PaymentRequestSequenceService, PaymentRequestSequenceService>();
            services.AddScoped<IApplicationRepository, ApplicationEntityFrameworkRepository>();
            services.AddScoped<LocationWorkdaysService, LocationWorkdaysService>();
            services.AddScoped<LeaveCreditAndUtilizationServices, LeaveCreditAndUtilizationServices>();

            services.AddScoped<FinanceReportsServices, FinanceReportsServices>();
            services.AddScoped<TimesheetReportsServices, TimesheetReportsServices>();
            services.AddScoped<LeaveReportsServices, LeaveReportsServices>();

            services.AddScoped<IAccountServices, AccountServices>();

            services.AddScoped<IAuditLogServices, AuditLog>();

            services.AddScoped<UploadService, UploadService>();
            services.AddAutoMapper(typeof(Startup));
            services.AddSession(options =>
                options.IdleTimeout = TimeSpan.FromMinutes(30)
            );

            services.AddMvc(options =>
                    options.Filters.Add(new MySampleActionFilter()))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddFluentValidation().
                AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddBreadcrumbs(GetType().Assembly, options =>
            {
                options.TagName = "nav";
                options.TagClasses = "d-print-none";
                options.OlClasses = "breadcrumb";
                options.LiClasses = "breadcrumb-item";
                options.ActiveLiClasses = "breadcrumb-item active";
                options.SeparatorElement = "<li class=\"separator\">/</li>";
            });

            // Add Hangfire services.
            //GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            services.AddHangfire((provider, configuration) =>
            {
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                            .UseSimpleAssemblyNameTypeSerializer()
                            .UseRecommendedSerializerSettings()
                            .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                            {
                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                QueuePollInterval = TimeSpan.Zero,
                                UseRecommendedIsolationLevel = true,
                                UsePageLocksOnDequeue = true,
                                DisableGlobalLocks = true,
                            });
                //configuration.UseFilter(provider.GetRequiredService<AutomaticRetryAttribute>());
            });

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailSender"));
            services.Configure<ReportsOptions>(Configuration.GetSection("Reports"));
            services.Configure<SharePointOptions>(Configuration.GetSection("SharePoint"));

            services.Configure<DBConnectionOptions>(Configuration.GetSection("ConnectionStrings"));

            services.AddPostal();
            services.AddScoped<IEmailSenderEnhance, EmailSenderImpl>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IsharepointEnhance, SharePoint_service>();

            services.AddScoped<IDBConnectionEnhance, DBConnection_service>();

            var loadcontext = new CustomAssemblyLoadContext();
            loadcontext.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IHostingEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
                RequestPath = "/Uploads"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "LeaveUploads")),
                RequestPath = "/LeaveUploads"
            });
            app.UseCookiePolicy();
            CultureInfo.CurrentCulture = new CultureInfo("en-IN");

            if (Configuration.GetValue<bool>("EnableHangfireDashboard"))
            {
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new LEAOAuthorizationFilter() },
                    IsReadOnlyFunc = (DashboardContext context) => Configuration.GetValue<bool>("EnableHangfireReadonly")
                });
            }
            //Finance
            //Advance
            RecurringJob.AddOrUpdate<HangfireReminderSender>("AdvanceReminder",
                x => x.ProcessRemindersAsync("AdvanceReminder"),
                Cron.Daily(1, 0));
            RecurringJob.AddOrUpdate<HangfireReminderSender>("AdvanceFinalReminder",
                x => x.ProcessRemindersAsync("AdvanceFinalReminder"),
                Cron.Daily(1, 15));
            RecurringJob.AddOrUpdate<HangfireReminderSender>("AdvanceReminderMoreThanThreeDays",
                x => x.ProcessRemindersAsync("AdvanceReminderMoreThanThreeDays"),
                Cron.Daily(1, 30));
            RecurringJob.AddOrUpdate<HangfireReminderSender>("FinanceAdvanceReminder",
                x => x.ProcessRemindersAsync("FinanceAdvanceReminder"),
                Cron.Daily(1, 45));


            //Expense
            RecurringJob.AddOrUpdate<HangfireReminderSender>("ExpenseReminder",
                x => x.ProcessRemindersAsync("ExpenseReminder"),
                Cron.Daily(2, 00));
            /*
            RecurringJob.AddOrUpdate<HangfireReminderSender>("ExpenseFinalReminder", 
                x => x.ProcessRemindersAsync("ExpenseFinalReminder"),
                Cron.Daily(2, 15));
                //There is no final reminder
                */
            RecurringJob.AddOrUpdate<HangfireReminderSender>("FinanceDocumentsPendingReminder",
                x => x.ProcessRemindersAsync("FinanceDocumentsPendingReminder"),
                Cron.Daily(2, 30));
            RecurringJob.AddOrUpdate<HangfireReminderSender>("FinanceExpenseReminder",
                x => x.ProcessRemindersAsync("FinanceExpenseReminder"),
                Cron.Daily(2, 45));

            //Leave
            RecurringJob.AddOrUpdate<HangfireReminderSender>("LeaveReminder",
                x => x.ProcessRemindersAsync("LeaveReminder"),
                Cron.Daily(3, 00));
            RecurringJob.AddOrUpdate<HangfireReminderSender>("LeaveFinalReminder",
                x => x.ProcessRemindersAsync("LeaveFinalReminder"),
                Cron.Daily(3, 15));

            //Timesheet
            RecurringJob.AddOrUpdate<HangfireReminderSender>("TimesheetReminder",
                x => x.ProcessRemindersAsync("TimesheetReminder"),
                Cron.Daily(3, 30));
            RecurringJob.AddOrUpdate<HangfireReminderSender>("TimesheetFinalReminder",
                x => x.ProcessRemindersAsync("TimesheetFinalReminder"),
                Cron.Daily(3, 45));

            //Auto cancel Leave Requests
            RecurringJob.AddOrUpdate<AutoCancelLeaves>("LeaveAutoCancel",
                x => x.ExecuteAutoCancelLeaves("LeaveAutoCancel"),
                Cron.Daily(4, 15));

            //Auto deactivate employees
            RecurringJob.AddOrUpdate<AutoDeactivateEmployee>("EmployeeAutoDeactivate",
                x => x.ProcessDeactivate(),
                Cron.Daily(0, 15));

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

        }
    }
    public class LEAOAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }

    internal class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}

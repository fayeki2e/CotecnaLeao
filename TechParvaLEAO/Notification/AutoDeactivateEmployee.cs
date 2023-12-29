using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;

namespace TechParvaLEAO.Notification
{
    public class AutoDeactivateEmployee
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediatR;

        public AutoDeactivateEmployee(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IMediator mediatR)
        {
            this._context = context;
            this._mapper = mapper;
            this._userManager = userManager;
            this._mediatR = mediatR;
        }

        public async Task ProcessDeactivate()
        {
            var employeeList = _context.Employees.Where(e => DateTime.Today - e.LastWorkingDate > TimeSpan.FromDays(15) 
                && e.Deactivated==false).ToList();
            foreach(var employee in employeeList)
            {
                employee.Deactivated = true;
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}

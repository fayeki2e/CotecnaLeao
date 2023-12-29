using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Controllers;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using TechParvaLEAO.Controllers;

namespace TechParvaLEAO
{
    public class MappingProfile: Profile
    {
        private class IntTypeConverter : ITypeConverter<string, int>
        {
            public int Convert(string source, int destination, ResolutionContext context)
            {
                if (source == null)
                    throw new AutoMapperMappingException("null string value cannot convert to non-nullable return type.");
                else
                    return Int32.Parse(source);
            }

        }

        public MappingProfile()
        {
            MappingProfileBasicTypes();
            MappingProfileOrganizationTypes();
            MappingProfileLeaveTypes();
            MappingProfilePaymentRequestTypes();
            MappingProfileEmployeeTypes();
        }

        private void MappingProfileBasicTypes()
        {
            CreateMap<string, string>().ConvertUsing(s => s ?? string.Empty);
            CreateMap<string, int>().ConvertUsing<IntTypeConverter>();

        }

        private void MappingProfileOrganizationTypes()
        {
            CreateMap<EmployeeCSVRecord, Employee>();
        }

        private void MappingProfileLeaveTypes()
        {
            CreateMap<NewLeaveViewModel, LeaveRequest>();
            CreateMap<LeaveRequest, NewLeaveViewModel>();
        }

        private void MappingProfilePaymentRequestTypes()
        {
            CreateMap<AdvanceViewModel, PaymentRequest>();
        }

        private void MappingProfileEmployeeTypes()
        {
            CreateMap<Employee, AddEditEmployeeVM>().ReverseMap();
        }
    }
}

using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Leave.Controllers;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Reports.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Services;
using static TechParvaLEAO.Areas.Organization.Controllers.EmployeesJsonController;
using DocumentFormat.OpenXml.Bibliography;
using TechParvaLEAO.Service;

namespace TechParvaLEAO.Areas.Reports.Services
{
    public class FinanceReportsServices
    {
        private readonly ApplicationDbContext dbContext;
        private readonly PaymentRequestService paymentRequestService;
        private readonly ApplicationDbContext context;
        private IApplicationRepository _context;
        private readonly IEmployeeServices employeeServices;

        private readonly IDBConnectionEnhance dbConnection;

        public FinanceReportsServices(ApplicationDbContext context,
            ApplicationDbContext dbContext, PaymentRequestService paymentRequestService, IApplicationRepository repository,
             IEmployeeServices employeeServices, IDBConnectionEnhance DBconnection
            )
        {
            this.context = context;
            this.dbContext = dbContext;
            this.paymentRequestService = paymentRequestService;
            this._context = repository;
            this.employeeServices = employeeServices;
            dbConnection = DBconnection;
        }

        private IEnumerable<PaymentRequest> ApplySearch(IEnumerable<PaymentRequest> query, FinanceReportSearchVm searchVm)
        {
            if (searchVm == null) return query;

            if (searchVm.is_reporting_exist == 1)
            {
                query = query.Where(l => l.Employee.ReportingToId == searchVm.current_emp_id);
            }

            if (searchVm.AdvanceFromDate != null)
            {
                query = query.Where(p => p.PaymentRequestCreatedDate >= searchVm.AdvanceFromDate);
            }
            if (searchVm.AdvanceToDate != null)
            {
                query = query.Where(p => p.PaymentRequestCreatedDate <= searchVm.AdvanceToDate.Value.AddDays(1));
            }

            if (searchVm.ExpenseFromDate != null)
            {
                query = query.Where(p => p.PaymentRequestCreatedDate >= searchVm.ExpenseFromDate);
            }
            if (searchVm.ExpenseToDate != null)
            {
                query = query.Where(p => p.PaymentRequestCreatedDate <= searchVm.ExpenseToDate.Value.AddDays(1));
            }

            if (searchVm.ApproveRejectFromDate != null)
            {
                query = query.Where(p => p.ActionDate >= searchVm.ApproveRejectFromDate);
            }
            if (searchVm.ApproveRejectToDate != null)
            {
                query = query.Where(p => p.ActionDate <= searchVm.ApproveRejectToDate.Value.AddDays(1));
            }
            if (searchVm.Branch != null)
            {
                query = query.Where(p => p.LocationId == searchVm.Branch);
            }
            if (searchVm.AmountGt != null)
            {
                query = query.Where(p => p.Amount >= searchVm.AmountGt);
            }
            if (searchVm.AmountLt != null)
            {
                query = query.Where(p => p.Amount <= searchVm.AmountLt);
            }
            if (searchVm.Employee != null)
            {
                query = query.Where(p => p.EmployeeId == searchVm.Employee);
            }
            if (searchVm.SubmittedBy != null)
            {
                query = query.Where(p => p.PaymentRequestCreatedById <= searchVm.SubmittedBy);
            }
            if (searchVm.ApproveRejectedBy != null)
            {
                query = query.Where(p => p.PaymentRequestCreatedById <= searchVm.ApproveRejectedBy);
            }
            if (searchVm.Status != null)
            {
                if (string.Equals(searchVm.Status, "APPROVED"))
                {
                    query = query.Where(p => p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                    p.Status == PaymentRequestStatus.POSTED.ToString() ||
                    p.Status == PaymentRequestStatus.PAID.ToString()
                    );

                }
                else if (string.Equals(searchVm.Status, "REJECTED"))
                {
                    query = query.Where(p => p.Status == PaymentRequestStatus.REJECTED.ToString());
                }
            }
            if (searchVm.Type != null)
            {
                query = query.Where(p => p.Type == searchVm.Type);
            }

            if (searchVm.AccountNumber != null)
            {
                query = query.Where(p => p.Employee.AccountNumber == searchVm.AccountNumber);
            }


            return query;
        }

        private double DateDiff(DateTime? dateFrom, DateTime? dateTo)
        {
            if (dateFrom.HasValue && dateTo.HasValue)
            {
                return (dateTo - dateFrom).Value.TotalDays;
            }
            return 0;
        }


        public IEnumerable<EmployeeExpenseTrackerReportVm> SearchEmployeeExpenseTracker(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                && (p.Status == PaymentRequestStatus.PAID.ToString() ||
                p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                p.Status == PaymentRequestStatus.POSTED.ToString()));

            var searchResult = ApplySearch(query, searchVm);

            var result = searchResult.Select((s, index) => new EmployeeExpenseTrackerReportVm
            {
                Id = s.Id,
                Month = Convert.ToDateTime(s.PaymentRequestCreatedDate.ToString("dd-MMM-yyyy")),
                str_date = s.PaymentRequestCreatedDate.ToString("dd-MMM-yyyy"),
                SerialNumber = index + 1,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                SubmissionDate = s.PaymentRequestCreatedDate,
                SubmissionMonth = s.PaymentRequestCreatedDate,
                ApprovalDate = s.ActionDate,
                PaidDate = s.PaidDate,
                Description = s.Comment,
                PaidInDays = Math.Round(s.PaidDate.HasValue ? DateDiff(s.ActionDate, s.PaidDate) : 0.0d, 2)
            });


            return result;
        }

        public IEnumerable<EmployeeAdvanceTrackerReportVm> SearchEmployeeAdvanceTracker(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.ADVANCE.ToString()
                && (p.Status == PaymentRequestStatus.PAID.ToString() ||
                p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                p.Status == PaymentRequestStatus.POSTED.ToString()));
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new EmployeeAdvanceTrackerReportVm
            {
                Id = s.Id,
                Month = s.PaymentRequestCreatedDate,
                SerialNumber = index + 1,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                SubmissionDate = s.PaymentRequestCreatedDate,
                ApprovalDate = s.ActionDate.HasValue ? s.ActionDate.Value : new DateTime(),
                PaidDate = s.PostedOn.HasValue ? s.PostedOn.Value : new DateTime(),
                Description = s.Comment,
                PaidInDays = Math.Round(s.PaidDate.HasValue ? DateDiff(s.ActionDate, s.PaidDate) : 0.0d, 2)

            });
            return result;
        }

        public IEnumerable<UnsettledAdvanceReportVm> SearchUnsettledAdvances(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }


            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.ADVANCE.ToString()
                && p.Status == PaymentRequestStatus.PAID.ToString()
                && p.Settled == false);
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new UnsettledAdvanceReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                PaidDate = s.PaidDate.HasValue ? s.PaidDate.Value : new DateTime(),
                DaysSincePaid = s.PaidDate.HasValue ? Convert.ToInt32(DateDiff(s.PaidDate.Value.Date, DateTime.Today)) : 0
            });
            return result;
        }

        public IEnumerable<SettledAdvanceReportVm> SearchSettledAdvances(FinanceReportSearchVm searchVm)
        {
            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.ADVANCE.ToString()
                        && p.Status == PaymentRequestStatus.PAID.ToString()
                        && p.Settled == true);
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new SettledAdvanceReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                ApprovalDate = s.ActionDate.Value,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                ClaimSubmissionDate = s.PaymentRequestCreatedDate,
                PaidDate = s.PaidDate.HasValue ? s.PaidDate.Value.Date : new DateTime(),
                DaysSincePaid = DateDiff(s.PaidDate.Value.Date, DateTime.Today)
            });
            return result;
        }

        public IEnumerable<HardcopyPendingReportVm> SearchHardcopyNotSubmitted(FinanceReportSearchVm searchVm)
        {
            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                && p.Status == PaymentRequestStatus.PAID.ToString()
                && p.SupportingDocumentsReceivedDate == null);
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new HardcopyPendingReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                ClaimSubmissionDate = s.PaymentRequestCreatedDate,
                ApprovalDate = s.ActionDate.HasValue ? s.ActionDate.Value : new DateTime(),
                DaysSincePaid = s.ActionDate.HasValue ? DateDiff(s.ActionDate.Value.Date, DateTime.Today) : 0
            });
            return result;
        }

        public IEnumerable<ExpenseTransactionSummaryReportVm> SearchExpenseTransactionSummary(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                && (p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                    p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString() ||
                    p.Status == PaymentRequestStatus.REJECTED.ToString() ||
                    p.Status == PaymentRequestStatus.POSTED.ToString() ||
                    p.Status == PaymentRequestStatus.PAID.ToString()));
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new ExpenseTransactionSummaryReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                ExpenseDate = s.PaymentRequestCreatedDate,
                Comment = s.Comment,
                ApprovedRejected = string.Equals(s.Status, PaymentRequestStatus.REJECTED.ToString()) ? "Rejected" : "Approved",
                Status = s.Status,
                SubmittedBy = s.PaymentRequestCreatedBy.Name,
                ApproveRejectedBy = s.PaymentRequestActionedBy != null ? s.PaymentRequestActionedBy.Name : "",
                ApprovedRejectedEscalated = s.EscalationStatus,
                ApproveRejectEscalatedTo = s.EscalationApprover,
                RejectionReason = s.RejectionReasons == null ? "" : s.RejectionReasons.Reason,
                AdjustedAgainst = s.AdvancePaymentRequestId != null ? s.AdvancePaymentRequest.RequestNumber : "",
                AmountOfClaim = s.Amount,
                AmountOfAdvance = 0.00,
                ForexRate = s.ExchangeRate,
                SupportingsInSystem = s.GetSupportingDocuments() != null ? s.GetSupportingDocuments().Count() : 0,
                HardcopySubmitted = s.SupportingDocumentsReceivedDate.HasValue ? "Yes" : "No",
                NumberOfLineItems = s.LineItems != null ? s.LineItems.Count() : 0,
                CreditCard = s.CreditCard ? "Yes" : "No",
                EmployeeStatus = s.Employee.Status.ToString(),
                ReportingManager = s.Employee.ReportingTo.Name
            });

            var resultList = result.ToList();
            foreach (var expense in resultList)
            {
                if (expense.AdjustedAgainst == null || "".Equals(expense.AdjustedAgainst))
                {
                    var mappedAdv = dbContext.AdvanceExpenseAdjustments.Where(adj => adj.ExpenseId == expense.Id);
                    expense.AdjustedAgainst = "";
                    foreach (var ma in mappedAdv)
                    {
                        expense.AdjustedAgainst += (ma.Advance.RequestNumber + " (" + ma.Advance.Currency.Name + ma.AdjustedAdvanceAmount + ") ");
                    }
                }
            }
            return resultList;
        }

        public IEnumerable<RejectedAdvanceReportVm> SearchRejectedAdvances(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }


            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.ADVANCE.ToString()
                && p.Status == PaymentRequestStatus.REJECTED.ToString());
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new RejectedAdvanceReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                RejectionReason = s.RejectionReasons == null ? "" : s.RejectionReasons.Reason,
                RejectedBy = s.PaymentRequestActionedBy.Name
            });
            return result;
        }

        public IEnumerable<RejectedExpensesReportVm> SearchRejectedExpenses(FinanceReportSearchVm searchVm, string empcode)

        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                && p.Status == PaymentRequestStatus.REJECTED.ToString());
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new RejectedExpensesReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                RejectionReason = s.RejectionReasons == null ? "" : s.RejectionReasons.Reason,
                RejectedBy = s.PaymentRequestActionedBy.Name
            });
            return result;
        }

        public IEnumerable<UnapprovedAdvanceExpensesReportVm> SearchUnapprovedAdvanceExpenses(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }


            var query = dbContext.PaymentRequests.Where(p =>
                (p.Status == PaymentRequestStatus.PENDING.ToString() ||
                p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString())
                );
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new UnapprovedAdvanceExpensesReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = string.Equals(s.Type, PaymentRequestType.REIMBURSEMENT.ToString()) ? s.RequestNumber : "",
                AdvancePaymentRequestNumber = string.Equals(s.Type, PaymentRequestType.ADVANCE.ToString()) ? s.RequestNumber : s.AdvancePaymentRequest != null ? s.AdvancePaymentRequest.RequestNumber : "",
                Amount = s.Amount,
                Currency = s.Currency.Name,
                SubmittedBy = s.PaymentRequestCreatedBy.Name,
                LastApprovedBy = s.PaymentRequestActionedBy != null ? s.PaymentRequestActionedBy.Name : "",
                Status = s.Status.ToString()
            });
            return result;
        }

        public IEnumerable<FNFAdvancesReportVm> SearchFNFPending(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            var query = dbContext.PaymentRequests.Where(p => (p.Employee.Status == EmployeeStatus.RESIGNED
                                                                        || p.Employee.Status == EmployeeStatus.SERVICETERMINATED)
                                    && p.Employee.SettlementDate == null &&
                                    ((p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                                    (p.Status == PaymentRequestStatus.PENDING.ToString()
                                    || p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()
                                    || p.Status == PaymentRequestStatus.APPROVED.ToString()
                                    || p.Status == PaymentRequestStatus.POSTED.ToString())) ||
                                    (p.Type == PaymentRequestType.ADVANCE.ToString() &&
                                        (p.Status == PaymentRequestStatus.POSTED.ToString() ||
                                        p.Status == PaymentRequestStatus.PAID.ToString()) && p.Settled == false)));
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new FNFAdvancesReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                ExpensePaymentRequestNumber = string.Equals(s.Type, PaymentRequestType.REIMBURSEMENT.ToString()) ? s.RequestNumber : "",
                AdvancePaymentRequestNumber = string.Equals(s.Type, PaymentRequestType.ADVANCE.ToString()) ? s.RequestNumber : s.AdvancePaymentRequest != null ? s.AdvancePaymentRequest.RequestNumber : "",
                Amount = s.Amount,
                Currency = s.Currency.Name,
                SubmittedBy = s.PaymentRequestCreatedBy.Name,
                LastApprovedBy = s.PaymentRequestActionedBy.Name,
                Status = s.Status.ToString()
            });
            return result;
        }

        public IEnumerable<ExpenseTransactionDetailReportVm> SearchExpenseTransactionDetail(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                && (p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                    p.Status == PaymentRequestStatus.REJECTED.ToString() ||
                    p.Status == PaymentRequestStatus.POSTED.ToString() ||
                    p.Status == PaymentRequestStatus.PAID.ToString()));

            // var query = dbContext.PaymentRequests.Where(p => p.RequestNumber == "EXP/MH/C3237/15/2022-23" || p.RequestNumber== "EXP/MH/C3237/16/2022-23") ;

            var searchResult = ApplySearch(query, searchVm);
            var result = new List<ExpenseTransactionDetailReportVm>();
            var index = 0;
            foreach (var s in searchResult)
            {
                foreach (var line in s.LineItems)
                {
                    index++;
                    var ot = "";

                    if (line.operationtype != null)
                    {

                        if (line.operationtype.ToString() == "1")
                        {
                            ot = "Miscellaneous";
                        }
                        else if (line.operationtype.ToString() == "2")
                        {
                            ot = "Job No";
                        }
                        else
                        {
                            ot = "";
                        }
                    }
                    else
                    {
                        ot = "";
                    }

                    result.Add(new ExpenseTransactionDetailReportVm
                    {
                        Id = s.Id,
                        SerialNumber = index,
                        Month = s.PaymentRequestCreatedDate,
                        Branch = s.Location.Name,
                        EmployeeCode = s.Employee.EmployeeCode,
                        EmployeeName = s.Employee.Name,
                        PaymentRequestNumber = s.RequestNumber,
                        ExpenseHead = line.ExpenseHead.ExpenseHeadDesc,
                        Amount = line.Amount,
                        Currency = s.Currency.Name,
                        ExpenseDate = line.ExpenseDate,
                        Comment = s.Comment,
                        ApprovedRejected = string.Equals(s.Status, PaymentRequestStatus.REJECTED.ToString()) ? "Rejected" : "Approved",
                        Status = s.Status,
                        SubmittedBy = s.PaymentRequestCreatedBy.Name,
                        ApproveRejectedBy = s.PaymentRequestActionedBy.Name,
                        ApproveRejectEscalated = s.EscalationStatus,
                        ApproveRejectEscalatedTo = s.EscalationApprover,
                        RejectionReason = s.RejectionReasons == null ? "" : s.RejectionReasons.Reason,
                        AdjustedAgainst = s.AdvancePaymentRequestId != null ? s.AdvancePaymentRequest.RequestNumber : "",
                        AmountOfClaim = line.Amount,
                        AmountOfAdvance = s.ClaimedAmount,
                        ForexRate = s.ExchangeRate,
                        SupportingsInSystem = s.GetSupportingDocuments().Count(),
                        HardcopySubmitted = s.SupportingDocumentsReceivedDate.HasValue ? "Yes" : "No",
                        NumberOfLineItems = s.LineItems.Count(),
                        CreditCard = s.CreditCard ? "Yes" : "No",
                        EmployeeStatus = s.Employee.Status.ToString(),
                        ReportingManager = s.Employee.ReportingTo.Name,
                        BusinessActivity = line.BusinessActivity.Name,
                        CustomerMarket = line.CustomerMarket.Name,
                        Description = line.VoucherDescription,
                        OperationNumber = line.VoucherDescription.Substring(0, line.VoucherDescription.Length > 15 ? 15 : line.VoucherDescription.Length),
                        operationtype = ot.ToString(),
                        operationno = line.operationno


                    });
                }
            }
            var resultList = result.ToList();
            foreach (var expense in resultList)
            {
                if (expense.AdjustedAgainst == null || "".Equals(expense.AdjustedAgainst))
                {
                    var mappedAdv = dbContext.AdvanceExpenseAdjustments.Where(adj => adj.ExpenseId == expense.Id);
                    expense.AdjustedAgainst = "";
                    foreach (var ma in mappedAdv)
                    {
                        expense.AdjustedAgainst += (ma.Advance.RequestNumber + " (" + ma.Advance.Currency.Name + ma.AdjustedAdvanceAmount + ") ");
                    }
                }
            }
            return resultList;
        }

        public IEnumerable<AdvanceTransactionDetailReportVm> SearchAdvanceTransactionDetail(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }


            var query = dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.ADVANCE.ToString());
            var searchResult = ApplySearch(query, searchVm);
            var result = searchResult.Select((s, index) => new AdvanceTransactionDetailReportVm
            {
                Id = s.Id,
                SerialNumber = index + 1,
                Month = s.PaymentRequestCreatedDate,
                Branch = s.Location.Name,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                PaymentRequestNumber = s.RequestNumber,
                Comment = s.Comment,
                Amount = s.Amount,
                Currency = s.Currency.Name,
                BusinessActivity = s.BusinessActivity.Name,
                CustomerMarket = s.CustomerMarket.Name,
                Status = s.Status.ToString(),
                ApproveRejectedBy = s.PaymentRequestActionedBy == null ? "" : s.PaymentRequestActionedBy.Name,
                RejectedReason = s.RejectionReasons == null ? "" : s.RejectionReasons.Reason,
            });
            return result;
        }

        public IEnumerable<BalancePayableOrReceivableReportVm> SearchBalancePayableOrReceivableDetail1(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }


            var query = dbContext.PaymentRequests.Where(p => p.Status == PaymentRequestStatus.PAID.ToString()).OrderBy(p => p.Employee.EmployeeCode); ;
            var searchResult = ApplySearch(query, searchVm);

            var result = searchResult.Select((s, index) => new BalancePayableOrReceivableReportVm
            {
                Id = s.Id,
                EmpId = s.Employee.Id,
                AccountNumber = s.Employee.AccountNumber,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                Branch = s.Location.Name,
                Balance = ""

            });



            return result;
        }

        public IEnumerable<BalancePayableOrReceivableReportVm> SearchBalancePayableOrReceivableDetail(FinanceReportSearchVm searchVm, string empcode)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }


            var query = dbContext.PaymentRequests.Where(p => p.Status == PaymentRequestStatus.PAID.ToString()).OrderBy(p => p.Employee.EmployeeCode); ;

            //string sql_query = "";


            //sql_query = "select * from PaymentRequests ";
            //sql_query = sql_query + " where Status='PAID' order by EmployeeId ";
            //var query = dbContext.PaymentRequests.FromSql(sql_query).ToList();



            var searchResult = ApplySearch(query, searchVm);

            var result = searchResult.Select((s, index) => new BalancePayableOrReceivableReportVm
            {
                Id = s.Id,
                EmpId = s.Employee.Id,
                AccountNumber = s.Employee.AccountNumber,
                EmployeeCode = s.Employee.EmployeeCode,
                EmployeeName = s.Employee.Name,
                Branch = s.Location.Name,
                Balance = ""

            });
            // var userType = dbContext.Set().FromSql("dbo.SomeSproc @Id = {0}, @Name = {1}", 45, "Ada");
            //var result = dbContext.BalancePayableOrReceivableReportVm.FromSql("dbo.sp_GetBalancePayable_Report @Id = {0}, @Name = {1}", 45, "Ada");

            return result;
        }

        public IEnumerable<Employee> SearchBalancePayableOrReceivableDetail_Emp(FinanceReportSearchVm searchVm, string empcode)
        {
            var result = dbContext.Employees.FromSql("dbo.sp_GetBalancePayable_Report");

            return result;
        }


        public DataSet GetBalancePayableOrReceivable_Report()
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetBalancePayable_Report", dbConnection.Get_DB_Connection());
                //cmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = Bank_Name;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adpt.Fill(dt);

                ds.Tables.Add(dt);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }





    }


}

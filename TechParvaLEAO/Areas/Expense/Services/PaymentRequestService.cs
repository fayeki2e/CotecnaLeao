using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Organization.Services;
using System.Collections;
using TechParvaLEAO.Areas.Expense.Models;
//using X.PagedList;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Newtonsoft.Json;
using AutoMapper;

namespace TechParvaLEAO.Areas.Expense.Services
{
    public class PendingDocumentPaymentRequests
    {
        public string PaymentRequestNumber;
        public int PaymentRequestId;
        public DateTime SubmittedDate;
    }


    public class PaymentRequestService
    {
        private IApplicationRepository _context;
        private readonly IApplicationRepository repository;
        private readonly IMapper mapper;
        private readonly PaymentRequestSequenceService paymentRequestSequenceService;

        private ApplicationDbContext _dbContext;
        private int MaxShowDays = 7;
        private int MaxShowDaysHR = 30;
        private readonly ApplicationDbContext dbContext;


        private List<string> InProgressPaymentRequestsStatus = new string[]
        {
                PaymentRequestStatus.PENDING.ToString(),
                PaymentRequestStatus.APPROVED_ESCALATED.ToString(),
                PaymentRequestStatus.APPROVED.ToString(),
                PaymentRequestStatus.POSTED.ToString(),
        }.ToList();
        private List<string> DonePaymentRequestsStatus = new string[]
        {
                PaymentRequestStatus.PAID.ToString(),
                PaymentRequestStatus.REJECTED.ToString()
        }.ToList();
        private List<string> PaidPaymentRequestsStatus = new string[]
        {
                PaymentRequestStatus.PAID.ToString(),
        }.ToList();


        public PaymentRequestService(IApplicationRepository context, ApplicationDbContext dbContext)
        {
            this._context = context;
            this._dbContext = dbContext;
            this.dbContext = dbContext;
        }
        
        private IEnumerable<PaymentRequest> ApplySearch(IEnumerable<PaymentRequest> query, PaymentRequestSearchViewModel searchModel)
        {
            if (searchModel == null) return query;

            if (searchModel.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }
            if (searchModel.FromAmount > 0)
            {
                query = query.Where(p => p.Amount >= searchModel.FromAmount);
            }
            if (searchModel.ToAmount > 0)
            {
                query = query.Where(p => p.Amount <= searchModel.ToAmount);
            }
            if (searchModel.EmployeeName != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Employee.Name, "%" + searchModel.EmployeeName.Trim() +"%"));
            }
            if (searchModel.EmployeeCode != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Employee.EmployeeCode, "%"+searchModel.EmployeeCode.Trim()+"%"));
            }
            if (searchModel.RequestNumber != null)
            {
                query = query.Where(p => EF.Functions.Like(p.RequestNumber, "%"+searchModel.RequestNumber.Trim()+ "%"));
            }
            if (searchModel.FromDate != null && searchModel.FromDate > new DateTime(2000, 01, 01))
            {
                //var fromDate = searchModel.FromDate - TimeSpan.FromDays(1) + -TimeSpan.FromDays(1);
                query = query.Where(p => p.PaymentRequestCreatedDate.Date >= searchModel.FromDate.Value);
            }
            if (searchModel.ToDate != null && searchModel.ToDate > new DateTime(2000, 01, 01))
            {
                query = query.Where(p => p.PaymentRequestCreatedDate.Date <= searchModel.ToDate);
            }
            if (!string.IsNullOrEmpty(searchModel.Status))
            {
                query = query.Where(p => p.Status == searchModel.Status);
            }
            if (searchModel.Currency != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Currency.Name, searchModel.Currency));
            }
            return query;

        }

        private IEnumerable<PaymentRequest> ApplySearch_advacnce(IEnumerable<PaymentRequest> query, PaymentRequestSearchViewModel searchModel)
        {
            if (searchModel == null) return query;

            if (searchModel.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }
            if (searchModel.FromAmount > 0)
            {
                query = query.Where(p => p.Amount >= searchModel.FromAmount);
            }
            if (searchModel.ToAmount > 0)
            {
                query = query.Where(p => p.Amount <= searchModel.ToAmount);
            }
            if (searchModel.EmployeeName != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Employee.Name, "%" + searchModel.EmployeeName.Trim() + "%"));
            }
            if (searchModel.EmployeeCode != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Employee.EmployeeCode, "%" + searchModel.EmployeeCode.Trim() + "%"));
            }
            if (searchModel.RequestNumber != null)
            {
                query = query.Where(p => EF.Functions.Like(p.RequestNumber, "%" + searchModel.RequestNumber.Trim() + "%"));
            }
            if (searchModel.FromDate != null && searchModel.FromDate > new DateTime(2000, 01, 01))
            {
                //var fromDate = searchModel.FromDate - TimeSpan.FromDays(1) + -TimeSpan.FromDays(1);
                query = query.Where(p => p.PaymentRequestCreatedDate.Date >= searchModel.FromDate.Value);
            }
            if (searchModel.ToDate != null && searchModel.ToDate > new DateTime(2000, 01, 01))
            {
                query = query.Where(p => p.PaymentRequestCreatedDate.Date <= searchModel.ToDate);
            }
            if (!string.IsNullOrEmpty(searchModel.Status))
            {
                query = query.Where(p => p.Status == searchModel.Status);
            }
            if (searchModel.Currency != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Currency.Name, searchModel.Currency));
            }
            return query;

        }

        public FinancialYear GetFinancialYear(DateTime date)
        {
            if (date == null) date = DateTime.Now;
            return _context.GetOne<FinancialYear>(fy => fy.StartDate <= date && fy.EndDate >= date);
        }

        public double GetApprovalLimit(Employee employee)
        {
            return employee.AuthorizationProfile.Approval_Limit;
        }

        public double GetApprovalLimit(int employeeId)
        {
            var employee = _context.GetById<Employee>(employeeId);
            return GetApprovalLimit(employee);
        }

        //-------------------Advances----------------------
        public IEnumerable<PaymentRequest> GetOwnAdvances(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            var query = _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id 
                        && p.Type==PaymentRequestType.ADVANCE.ToString(), 
                        q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;

        }
        public IEnumerable<PaymentRequest> GetPendingForexAdvances(Employee employee, int? currencyId)
        {
            var response = new List<PaymentRequest>();
            var query =  _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id && 
                p.Type == PaymentRequestType.ADVANCE.ToString() &&
                p.CurrencyId == currencyId && 
                p.Status == PaymentRequestStatus.PAID.ToString(),
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            foreach(var paymentRequest in query)
            {
                var settled = _context.Get<PaymentRequest>(p => p.AdvancePaymentRequestId == paymentRequest.Id && 
                    (p.Status==PaymentRequestStatus.POSTED.ToString()||
                    p.Status == PaymentRequestStatus.PAID.ToString())).Count();
                if (!(settled>0))
                {
                    response.Add(paymentRequest);
                }

            }
            return response;
        }
        public IEnumerable<PaymentRequest> GetOwnUnsettledAdvances(Employee employee)
        {
            return _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id 
            && p.Type == PaymentRequestType.ADVANCE.ToString(), 
            q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }
        public IEnumerable<PaymentRequest> GetOnBehalfAdvances(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            var query = _context.Get<PaymentRequest>(p => p.PaymentRequestCreatedById == employee.Id
                    && p.EmployeeId != employee.Id
                    && p.Type == PaymentRequestType.ADVANCE.ToString(),
                    q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;

        }
        public IEnumerable<PaymentRequest> GetForMyApprovalAdvances(Employee employee, PaymentRequestSearchViewModel searchModel)
        {

            //var query = from paymentrequests in _context.Get<PaymentRequest>()
            //            where paymentrequests.PaymentRequestActionedById == employee.Id &&
            //            paymentrequests.Type == PaymentRequestType.ADVANCE.ToString() ||
            //            paymentrequests.ApprovalActions.Any(a => a.ActionById == employee.Id &&
            //             a.PaymentRequest.Type == PaymentRequestType.ADVANCE.ToString() &&
            //             (a.Action == PaymentRequestActions.APPROVED.ToString() ||
            //             a.Action == PaymentRequestActions.REJECTED.ToString()))
            //            select paymentrequests;

            //var query = from paymentrequests in _context.Get<PaymentRequest>()
            //            where (paymentrequests.PaymentRequestActionedById == employee.Id &&
            //            paymentrequests.Type == PaymentRequestType.ADVANCE.ToString()) ||
            //            (paymentrequests.ApprovalActions.Any(a => a.ActionById == employee.Id &&
            //             a.PaymentRequest.Type == PaymentRequestType.ADVANCE.ToString() &&
            //             (a.Action == PaymentRequestActions.APPROVED.ToString() ||
            //             a.Action == PaymentRequestActions.REJECTED.ToString())))
            //            select paymentrequests;


            //   var query = _dbContext.PaymentRequests.Where(c => c.Id == 1);



            //var query = from paymentrequests in _context.Get<PaymentRequest>()
            //            where paymentrequests.PaymentRequestActionedById == employee.Id &&
            //            paymentrequests.Type == PaymentRequestType.ADVANCE.ToString() ||
            //            paymentrequests.ApprovalActions.Any(a => a.ActionById == employee.Id &&
            //             a.PaymentRequest.Type == PaymentRequestType.ADVANCE.ToString() &&
            //             (a.Action == PaymentRequestActions.APPROVED.ToString() ||
            //             a.Action == PaymentRequestActions.REJECTED.ToString()))
            //            select paymentrequests;
            //query = ApplySearch(query, searchModel);


            var query = from paymentrequests in _context.Get<PaymentRequest>()
                        where paymentrequests.PaymentRequestActionedById == employee.Id &&
                        paymentrequests.Type == PaymentRequestType.ADVANCE.ToString()  
                        select paymentrequests;
            //if(searchModel.Status == null)
            //{
            //    searchModel.Status = PaymentRequestStatus.PENDING.ToString();   
            //}
            query = ApplySearch(query, searchModel);




            //var sql_query = "select PR.Id,RequestNumber,VersionNumber,EmployeeId,Amount,ForexAmount,ExchangeRate,CreditCard";
            //sql_query = sql_query + " ,INRAmount,SettlementMode,BalanceAmount,PaidAmount,ClaimedAmount,CurrencyId,FinancialYearId,LocationId";
            //sql_query = sql_query + " ,BusinessActivityId,CustomerMarketId,PaymentRequestCreatedDate,PaymentRequestCreatedById,PaymentRequestActionedById";
            //sql_query = sql_query + " ,ActionDate,PostedById,PostedOn,PaidDate,PR.Type,Status";
            //sql_query = sql_query + " ,Comment,AdvancePaymentRequestId,RejectionReasonsId,RejectionReasonOther,FromDate";
            //sql_query = sql_query + " ,ToDate,SupportingDocumentsReceivedDate,SupportingDocumentsComment,SupportingDocumentsPath";
            //sql_query = sql_query + " ,Settled,operationno,operationtype from PaymentRequests  PR inner join PaymentRequestApprovalActions PRA ";
            //sql_query = sql_query + " on PR.Id = PRA.PaymentRequestId";
            //sql_query = sql_query + " where PR.PaymentRequestActionedById = " + employee.Id + " and PR.Type = 'ADVANCE' and(PRA.Action = 'APPROVED' or PRA.Action = 'REJECTED') or ";
            //sql_query = sql_query + " ( ";
            //sql_query = sql_query + "  PRA.ActionById = " + employee.Id + " and PRA.Type = 'ADVANCE' and(PRA.Action = 'APPROVED' or PRA.Action = 'REJECTED') ";
            //sql_query = sql_query + "   ) ";

            //var sql_query_result = _dbContext.PaymentRequests.FromSql(sql_query).ToList();

            ////////var query = from paymentrequests in _context.Get<PaymentRequest>()
            ////////            where paymentrequests.PaymentRequestActionedById == employee.Id
            ////////            select paymentrequests;

            //var query1 = ApplySearch((IEnumerable<PaymentRequest>)sql_query_result, searchModel);

            //if (searchModel.NoSearchRequested())
            //{
            //    query1 = query1.Where(p => p.Status == PaymentRequestStatus.PENDING.ToString() ||
            //                p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()
            //    );
            //}
            //query1 = query1.OrderByDescending(s => s.PaymentRequestCreatedDate);
            //return query1;


            //var sql_query = "select Id,RequestNumber,VersionNumber,EmployeeId,Amount,ForexAmount,ExchangeRate,CreditCard";
            //sql_query = sql_query + " ,INRAmount,SettlementMode,BalanceAmount,PaidAmount,ClaimedAmount,CurrencyId,FinancialYearId,LocationId";
            //sql_query = sql_query + " ,BusinessActivityId,CustomerMarketId,PaymentRequestCreatedDate,PaymentRequestCreatedById,PaymentRequestActionedById";
            //sql_query = sql_query + " ,ActionDate,PostedById,PostedOn,PaidDate,Type,Status";
            //sql_query = sql_query + " ,Comment,AdvancePaymentRequestId,RejectionReasonsId,RejectionReasonOther,FromDate";
            //sql_query = sql_query + " ,ToDate,SupportingDocumentsReceivedDate,SupportingDocumentsComment,SupportingDocumentsPath";
            //sql_query = sql_query + " ,Settled,operationno,operationtype from PaymentRequests ";
            //sql_query = sql_query + " where PaymentRequestActionedById = " + employee.Id + " and Type = 'ADVANCE'  and Status='PENDING' ";


            //var sql_query_result = _dbContext.PaymentRequests.FromSql(sql_query).ToList();

            ////////var query = from paymentrequests in _context.Get<PaymentRequest>()
            ////////            where paymentrequests.PaymentRequestActionedById == employee.Id
            ////////            select paymentrequests;

            //var query1 = ApplySearch((IEnumerable<PaymentRequest>)sql_query_result, searchModel);

            //if (searchModel.NoSearchRequested())
            //{
            //    query1 = query1.Where(p => p.Status == PaymentRequestStatus.PENDING.ToString() ||
            //                p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()
            //    );
            //}
            //query1 = query1.OrderByDescending(s => s.PaymentRequestCreatedDate);
            //return query1;

            if (searchModel.NoSearchRequested())
            {
                query = query.Where(p => p.Status == PaymentRequestStatus.PENDING.ToString() ||
                            p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()
                );
            }
            query = query.OrderByDescending(s => s.PaymentRequestCreatedDate);


            return query;
        }

     

        //AdvanceDashboard
        public IEnumerable<PaymentRequest> GetOwnAdvanceDashboard(Employee employee)
        {
            var query = _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
               p.Type == PaymentRequestType.ADVANCE.ToString() &&
               (InProgressPaymentRequestsStatus.Contains(p.Status) ||
               (DonePaymentRequestsStatus.Contains(p.Status) && 
               p.ActionDate > DateTime.Today.AddDays(-MaxShowDays))), 
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            return query;

        }

        public bool CheckTodaysPaymentRequest(Employee employee,double amount)
        {
            var query = _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
               p.Type == PaymentRequestType.REIMBURSEMENT.ToString() && p.PaymentRequestCreatedDate.ToString("dd/M/yyyy") == DateTime.Today.ToString("dd/M/yyyy") && p.Amount == amount,              
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
           // return query;

            if(query.ToList().Count >=1)
            {
                return true;
            }else
            {
                return false;
            }

        }

        public IEnumerable<PaymentRequest> GetOnBehalfAdvanceDashboard(Employee employee)
        {
            //p.Status == PaymentRequestStatus.PENDING.ToString() ||
            //    p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()

            //var query = _context.Get<PaymentRequest>(p => p.PaymentRequestCreatedById == employee.Id &&
            //    p.Type == PaymentRequestType.ADVANCE.ToString() && p.EmployeeId != employee.Id &&
            //   (  InProgressPaymentRequestsStatus.Contains(p.Status) ||
            //   (PaidPaymentRequestsStatus.Contains(p.Status) &&
            //   p.ActionDate > DateTime.Today.AddDays(-MaxShowDays))),
            //   q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));

            var query = _context.Get<PaymentRequest>(p => p.PaymentRequestCreatedById == employee.Id &&
            p.Type == PaymentRequestType.ADVANCE.ToString() && p.EmployeeId != employee.Id &&
           (p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString() ||
           p.Status == PaymentRequestStatus.PENDING.ToString())  ,
           q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));


            return query;
        }

        public IEnumerable<PaymentRequest> GetHRAdvanceDashboard(Employee employee)
        {
            var query = _context.Get<PaymentRequest>(p => 
                p.Type == PaymentRequestType.ADVANCE.ToString() &&
               (InProgressPaymentRequestsStatus.Contains(p.Status)),
               q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            return query;
        }

        public IEnumerable<PaymentRequest> GetOnBehalfExpensePendingApprovalDashboard(Employee employee)
        {
            var query = _context.Get<PaymentRequest>(p => p.PaymentRequestCreatedById == employee.Id &&
                p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                (p.Status == PaymentRequestStatus.PENDING.ToString() ||
                p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()) &&
                p.EmployeeId != employee.Id, q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            return query;
        }

        //i2e Changes - 
        public IEnumerable<PaymentRequest> GetForMyApprovalAdvanceDashboard(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            var query = _context.Get<PaymentRequest>(p => p.PaymentRequestActionedById == employee.Id && p.Type == PaymentRequestType.ADVANCE.ToString() && p.Status == PaymentRequestStatus.PENDING.ToString(),
               q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }


        //ExpenseDashboard
        public IEnumerable<PaymentRequest> GetOwnExpenseDashboard(Employee employee)
        {
            return _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
                                    p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                                (InProgressPaymentRequestsStatus.Contains(p.Status) ||
                                   (DonePaymentRequestsStatus.Contains(p.Status) &&
                                   p.ActionDate > DateTime.Today.AddDays(-MaxShowDays))),
                            q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }

        public IEnumerable<PaymentRequest> GetOnBehalfExpenseDashboard(Employee employee)
        {
            return _context.Get<PaymentRequest>(p => p.PaymentRequestCreatedById == employee.Id
                && p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                && (p.Status == PaymentRequestStatus.PENDING.ToString() ||
                p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString())
                && p.EmployeeId != employee.Id,// ||
                                               //p.Status == PaymentRequestStatus.APPROVED.ToString()),
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }
        public IEnumerable<PaymentRequest> GetHRExpenseDashboard(Employee employee)
        {
            return _context.Get<PaymentRequest>(p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                && (p.Status == PaymentRequestStatus.PENDING.ToString() ||
                p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString() ||
                p.Status == PaymentRequestStatus.APPROVED.ToString()),
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }

        //*** i2e changes
        public IEnumerable<PaymentRequest> GetForMyApprovalExpenseDashboard(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            //i2e changes
            var query = _context.Get<PaymentRequest>(p => p.PaymentRequestActionedById == employee.Id && p.Type == PaymentRequestType.REIMBURSEMENT.ToString() && p.Status == PaymentRequestStatus.PENDING.ToString(),
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }

        //-------------------Expense----------------------
        public IEnumerable<PaymentRequest> GetOwnExpenses(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            var query= _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id && p.Type == PaymentRequestType.REIMBURSEMENT.ToString() && p.Comment != "Advance entry from upload file to settle expenses", q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }
        public IEnumerable<PaymentRequest> GetOnBehalfExpenses(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            var query = _context.Get<PaymentRequest>(p => p.PaymentRequestCreatedById == employee.Id 
                    && p.EmployeeId != employee.Id 
                    && p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                    && p.Comment != "Advance entry from upload file to settle expenses", 
                    q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }
        public IEnumerable<PaymentRequest> GetForMyApprovalExpenses(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            //var query = from paymentrequests in _context.Get<PaymentRequest>()
            //                       where paymentrequests.PaymentRequestActionedById == employee.Id &&
            //                       paymentrequests.Type == PaymentRequestType.REIMBURSEMENT.ToString() 
            //                       ||
            //                       paymentrequests.ApprovalActions.Any(a => a.ActionById == employee.Id &&
            //                    a.PaymentRequest.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
            //                    (a.Action == PaymentRequestActions.APPROVED.ToString() ||
            //                    a.Action == PaymentRequestActions.REJECTED.ToString()))
            //                       select paymentrequests;
            //query = ApplySearch(query, searchModel);

            var query = from paymentrequests in _context.Get<PaymentRequest>()
                        where paymentrequests.PaymentRequestActionedById == employee.Id &&
                        paymentrequests.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                        select paymentrequests;

            
            query = ApplySearch(query, searchModel);


            if (searchModel.NoSearchRequested())
            {
                query = query.Where(p => p.Status == PaymentRequestStatus.PENDING.ToString() ||
                    p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()
                );
            }
            query = query.OrderByDescending(p => p.PaymentRequestCreatedDate);
            return query;
            
        }
        //-------------------Finance and others----------------------
        public IEnumerable<PaymentRequest> GetFinancePendingDocumentsList(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            //p => p.PaymentRequestActionedById == employee.Id && 
            var query =  _context.Get<PaymentRequest>(
                p=>p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                (p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                      p.Status == PaymentRequestStatus.POSTED.ToString() ||
                      p.Status == PaymentRequestStatus.PAID.ToString()) &&
                p.SupportingDocumentsReceivedDate==null, q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }
        public IEnumerable<PaymentRequest> GetFinanceApprovedAdvanceList(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            //&& p.ActionDate.Value.Date == DateTime.Now.Date
            var query = _context.Get<PaymentRequest>(p=>
                p.Type == PaymentRequestType.ADVANCE.ToString(),
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            if (string.IsNullOrEmpty(searchModel.Status)){
                query = query.Where(p => p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                        p.Status == PaymentRequestStatus.POSTED.ToString() ||
                        p.Status == PaymentRequestStatus.PAID.ToString());
            }
            query = ApplySearch(query, searchModel);
            return query;
        }
        public IEnumerable<PaymentRequest> GetFinanceDashboardApprovedAdvanceList(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            //&& p.ActionDate.Value.Date == DateTime.Now.Date
            var query = _context.Get<PaymentRequest>(
                p => (p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                p.Status == PaymentRequestStatus.POSTED.ToString())
                && p.Type == PaymentRequestType.ADVANCE.ToString(),
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }
        public IEnumerable<PaymentRequest> GetFinancePaymentRequestsList(int[] paymentRequests, PaymentRequestSearchViewModel searchModel)
        {
            var query = _context.Get<PaymentRequest>(
                p => (p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                      p.Status == PaymentRequestStatus.POSTED.ToString()),
                q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;

        }
        
        public IEnumerable<PaymentRequest> GetFinanceApprovedExpenseList(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            //&& p.ActionDate.Value.Date == DateTime.Now.Date
            var query = _context.Get<PaymentRequest>(p => 
                   p.Type == PaymentRequestType.REIMBURSEMENT.ToString(),
            q => q.OrderByDescending(s => s.ActionDate));
            if (string.IsNullOrEmpty(searchModel.Status))
            {
                query = query.Where(p =>
                (p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                p.Status == PaymentRequestStatus.POSTED.ToString() ||
                p.Status == PaymentRequestStatus.PAID.ToString()));
            }
            query = ApplySearch(query, searchModel);
            return query;           
        }
        public IEnumerable<PaymentRequest> GetFinanceDashboardApprovedExpenseList(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            //&& p.ActionDate.Value.Date == DateTime.Now.Date
            var query = _context.Get<PaymentRequest>(p => (p.Status == PaymentRequestStatus.APPROVED.ToString()||
                p.Status == PaymentRequestStatus.POSTED.ToString())
                && p.Type == PaymentRequestType.REIMBURSEMENT.ToString(),
            q => q.OrderByDescending(s => s.ActionDate));
            query = ApplySearch(query, searchModel);
            return query;

        }
        public IEnumerable<Employee> SearchSettlementEmployees(IEnumerable<Employee> query, PaymentRequestSearchViewModel searchModel)
        {
            if (searchModel == null) return query;
            if (searchModel.EmployeeName != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Name, "%" + searchModel.EmployeeName.Trim() + "%"));
            }
            if (searchModel.EmployeeCode != null)
            {
                query = query.Where(p => EF.Functions.Like(p.EmployeeCode, "%" + searchModel.EmployeeCode.Trim() + "%"));
            }
            if (searchModel.FromDate != null && searchModel.FromDate > new DateTime(2000, 01, 01))
            {
                //var fromDate = searchModel.FromDate - TimeSpan.FromDays(1) + -TimeSpan.FromDays(1);
                query = query.Where(p => p.LastWorkingDate >= searchModel.FromDate.Value);
            }
            if (searchModel.ToDate != null && searchModel.ToDate > new DateTime(2000, 01, 01))
            {
                query = query.Where(p => p.LastWorkingDate <= searchModel.ToDate);
            }
            return query;
        }

        public IEnumerable<Employee> GetForSettlementEmployees(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            var query = _context.Get<Employee>(e =>
                (e.Status == EmployeeStatus.RESIGNED || e.Status == EmployeeStatus.SERVICETERMINATED) 
                && e.SettlementDate == null,
                q => q.OrderByDescending(s => s.ResignationDate));
            query = SearchSettlementEmployees(query, searchModel);
            return query;
        }

        public IEnumerable<PaymentRequest> GetUnsettledPaymentRequests(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            var query = _context.Get<PaymentRequest>(p => p.EmployeeId==employee.Id && 
                p.Type==PaymentRequestType.ADVANCE.ToString() &&
                p.Status==PaymentRequestStatus.PENDING.ToString() &&
                p.AdvancePaymentRequest == null);
            query = ApplySearch(query, searchModel);
            return query;
        }

        public IEnumerable<PaymentRequest> GetUnsettledExpensePaymentRequests(Employee employee, PaymentRequestSearchViewModel searchModel)
        {
            var query = _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
                p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                (p.Status == PaymentRequestStatus.APPROVED.ToString() ||
                p.Status == PaymentRequestStatus.POSTED.ToString()) &&
                p.AdvancePaymentRequest == null);
            query = ApplySearch(query, searchModel);
            return query;
        }
        public IEnumerable<PaymentRequest> GetPaidAdvances(Employee employee, int? currencyId)
        {
            var currency = currencyId.HasValue ? currencyId.Value : 1;
            var query = _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
                p.Type == PaymentRequestType.ADVANCE.ToString() &&
                p.CurrencyId == currency &&
                (p.Status == PaymentRequestStatus.PAID.ToString()));
            return query;
        }
        public IEnumerable<PaymentRequest> GetApprovedReimbursements(Employee employee, int? currencyId)
        { // Removed p.Status == PaymentRequestStatus.APPROVED.ToString() ||
            var currency = currencyId.HasValue ? currencyId.Value : 1;
            return _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
                p.Type == PaymentRequestType.REIMBURSEMENT.ToString() && p.CurrencyId == currency &&
                (p.Status == PaymentRequestStatus.PAID.ToString()||
                    p.Status == PaymentRequestStatus.POSTED.ToString()));
        }
        public double GetUnsettledAdvanceAmount(Employee employee, int? currencyId)
        {
            var paidAdvaces = GetPaidAdvances(employee, currencyId);
            var approvedReimbursements = GetApprovedReimbursements(employee, currencyId);
            var paidAmount = 0.0d;
            var reimbusedAmount = 0.0d;
            foreach (var advance in paidAdvaces)
            {
                    paidAmount += advance.PaidAmount;
                
            }
            foreach (var reimb in approvedReimbursements)
            {
               
                    reimbusedAmount += (reimb.Amount - reimb.PaidAmount);
                
            }
            return paidAmount-reimbusedAmount;
        }
        public IEnumerable<PendingDocumentPaymentRequests> GetDocumentNotSubmittedExpenseReports(Employee employee)
        {
            return _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
                p.Type==PaymentRequestType.REIMBURSEMENT.ToString() &&
                p.Status == PaymentRequestStatus.PAID.ToString() &&
                p.SupportingDocumentsReceivedDate == null).Select(
                    p=> new PendingDocumentPaymentRequests
                    {
                        PaymentRequestNumber = p.RequestNumber,
                        PaymentRequestId = p.Id,
                        SubmittedDate = p.PaymentRequestCreatedDate
                    });
        }

        public IEnumerable<PaymentRequest> GetApprovedReimbursementUnpaid(Employee employee)
        {//&& p.EmployeeId != employee.Id
            return _context.Get<PaymentRequest>(
                p=>p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                p.Status== PaymentRequestStatus.APPROVED.ToString() 
                , q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }
        public IEnumerable<PaymentRequest> GetAdvanceRequestPaid(Employee employee)
        {
            return _context.Get<PaymentRequest>(
                p => p.Type == PaymentRequestType.ADVANCE.ToString() &&
                p.Status == PaymentRequestStatus.PAID.ToString() && 
                p.EmployeeId != employee.Id, q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }
        public IEnumerable<PaymentRequest> GetAdvanceRequestApproved(Employee employee)
        {
            return _context.Get<PaymentRequest>(
                p => p.Type == PaymentRequestType.ADVANCE.ToString() &&
                p.Status == PaymentRequestStatus.APPROVED.ToString() &&
                p.EmployeeId != employee.Id
                , q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }

        public IEnumerable<PaymentRequest> GetClaimedAmount(Employee employee)
        {
            //               p.Status == PaymentRequestStatus.PAID.ToString() || 
            return _context.Get<PaymentRequest>(
                p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
               (p.Status == PaymentRequestStatus.APPROVED.ToString() &&
               p.Status == PaymentRequestStatus.POSTED.ToString())&&
                p.Employee.ReportingToId == employee.Id
                , q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }

        public IEnumerable<PaymentRequest> GetSupportingPendingToBeReceived(Employee employee)
        {
            return _context.Get<PaymentRequest>(
                p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                p.SupportingDocumentsReceivedDate == null && p.EmployeeId != employee.Id && p.PaymentRequestCreatedById == employee.Id, q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }

        public IEnumerable<PaymentRequest> GetReimbursementClaims(Employee employee)
        {
            return _context.Get<PaymentRequest>(
                p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                p.Status == PaymentRequestStatus.PENDING.ToString() && p.EmployeeId != employee.Id, q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }

        public IEnumerable<PaymentRequest> GetNoAdvanceClaims(Employee employee)
        {
            return _context.Get<PaymentRequest>(
                p => p.Type == PaymentRequestType.ADVANCE.ToString() &&
                p.Status == PaymentRequestStatus.PENDING.ToString() && p.EmployeeId != employee.Id, q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }

        public IEnumerable<PaymentRequest> GetReimbursementClaimed(Employee employee)
        {
            return _context.Get<PaymentRequest>(
                p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                p.Status == PaymentRequestStatus.APPROVED.ToString() && p.EmployeeId != employee.Id && p.PaymentRequestCreatedById == employee.Id, q => q.OrderByDescending(s => s.PaymentRequestCreatedDate));
        }

        public IEnumerable<PaymentRequestLineItems> FindExpenseByDate(Employee employee, DateTime fromDate, DateTime toDate)
        {
            return _context.Get<PaymentRequestLineItems>(p => p.PaymentRequest.EmployeeId == employee.Id
                        && (p.PaymentRequest.Status == PaymentRequestStatus.APPROVED.ToString()||
                        p.PaymentRequest.Status == PaymentRequestStatus.POSTED.ToString() ||
                        p.PaymentRequest.Status == PaymentRequestStatus.PAID.ToString())
                        && p.ExpenseDate>=fromDate && p.ExpenseDate <= toDate);
        }

        public AdvanceViewModel GetAdvanceDraft(ClaimsPrincipal user, int id)
        {
            var leave_draft = _context.Get<PaymentRequestDraft>(d => d.UserIdentity == user.Identity.Name && d.Id == id).FirstOrDefault();
            return JsonConvert.DeserializeObject<AdvanceViewModel>(leave_draft.FormData);
        }
        public ExpenseViewModel GetExpenseDraft(ClaimsPrincipal user, int id)
        {
            var leave_draft = _context.Get<PaymentRequestDraft>(d => d.UserIdentity == user.Identity.Name && d.Id == id).FirstOrDefault();
            return JsonConvert.DeserializeObject<ExpenseViewModel>(leave_draft.FormData);
        }

        public IEnumerable<PaymentRequestDraft> GetDraftAdvanceList(String user)
        {
            var query = _context.Get<PaymentRequestDraft>(t => t.UserIdentity == user && t.Type=="ADVANCE", 
                q => q.OrderByDescending(s => s.LastUpdatedOn));
            return query;
        }
        public IEnumerable<PaymentRequestDraft> GetDraftExpenseList(String user)
        {
            var query = _context.Get<PaymentRequestDraft>(t => t.UserIdentity == user && t.Type == "EXPENSE",
                q => q.OrderByDescending(s => s.LastUpdatedOn));
            return query;
        }

        public void DeleteDraft(String user, int id)
        {
            var record = _context.Get<PaymentRequestDraft>(t => t.UserIdentity == user && t.Id == id).FirstOrDefault();
            if (record != null)
            {
                _context.Delete<PaymentRequestDraft>(record.Id);
                _context.Save();
            }
        }


        private IEnumerable<PaymentRequest> GetUnsettledAdvances(int employeeId, DateTime paidOnOrBefore)
        {
            var query = _dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.ADVANCE.ToString()
                && p.EmployeeId == employeeId
                && p.Status == PaymentRequestStatus.PAID.ToString()
                && p.Settled == false
                && p.CurrencyId == 1
                && p.PaidDate <= paidOnOrBefore).OrderBy(p => p.PaidDate).ThenBy(p => p.Id);
            return query;
        }

        private IEnumerable<PaymentRequest> GetUnsettledExpenses()
        {
            var query = _dbContext.PaymentRequests.Where(p => p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                && p.Status == PaymentRequestStatus.PAID.ToString()
                && p.CurrencyId == 1
                && p.Settled == false);
            return query;
        }

        public async Task MapAdvanceExpenses()
        {
            var unsettledExpense = GetUnsettledExpenses().ToList();
            foreach (var expense in unsettledExpense)
            {
                var unsettledAdvances = GetUnsettledAdvances(expense.EmployeeId, expense.PaidDate.Value).ToList();
                foreach (var advance in unsettledAdvances)
                {
                    var expenseAmountToAdjust = expense.Amount - expense.PaidAmount - expense.ClaimedAmount;
                    if (expenseAmountToAdjust < 1)
                    {
                        continue;
                    }
                    var advanceAmountAvailableToAdjust = advance.Amount - advance.ClaimedAmount;

                    if (advanceAmountAvailableToAdjust <= expenseAmountToAdjust)
                    {
                        advance.ClaimedAmount += advanceAmountAvailableToAdjust;
                        expense.ClaimedAmount += advanceAmountAvailableToAdjust;
                        if (advance.ClaimedAmount >= advance.Amount) advance.Settled = true;
                        if (expense.ClaimedAmount >= expense.Amount)
                        {
                            expense.Settled = true;
                        }
                        var adjustmentEntry = new AdvanceExpenseAdjustment
                        {
                            Advance = advance,
                            Expense = expense,
                            AdjustedAdvanceAmount = advanceAmountAvailableToAdjust
                        };
                        _dbContext.Entry(adjustmentEntry).State = EntityState.Added;
                    }
                    else
                    {
                        advance.ClaimedAmount += expenseAmountToAdjust;
                        expense.ClaimedAmount += expenseAmountToAdjust;
                        if (advance.ClaimedAmount >= advance.Amount) advance.Settled = true;
                        if (expense.ClaimedAmount >= expense.Amount)
                        {
                            expense.Settled = true;
                        }
                        var adjustmentEntry = new AdvanceExpenseAdjustment
                        {
                            Advance = advance,
                            Expense = expense,
                            AdjustedAdvanceAmount = expenseAmountToAdjust
                        };
                        _dbContext.Entry(adjustmentEntry).State = EntityState.Added;

                    }
                    _dbContext.Entry(advance).State = EntityState.Modified;
                    _dbContext.Entry(expense).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    if (expense.Settled) break;
                }
                expense.Settled = true;
                _dbContext.Entry(expense).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }

        }


        public async Task MapReimursementEntry(PaymentRequest paymentRequest, Employee employee)
        {

            //  var employee = await repository.GetOneAsync<Employee>(e => e.Id == paymentRequest.EmployeeId);

            // var employee = await _context.GetAsync<Employee>(e => e.Id == paymentRequest.EmployeeId  
            //    , o => o.OrderBy(b => b.Name));

           // AdvanceViewModel paymentRequestVm = new AdvanceViewModel();

         //   var paymentRequest = mapper.Map<AdvanceViewModel, PaymentRequest>(paymentRequestVm);

            paymentRequest.PaymentRequestCreatedById = paymentRequest.PaymentRequestCreatedById;
            paymentRequest.Type = PaymentRequestType.REIMBURSEMENT.ToString();
            //paymentRequest.Status = PaymentRequestStatus.PENDING.ToString();
            paymentRequest.PaymentRequestActionedById = employee.ReportingToId;
            paymentRequest.LocationId = employee.LocationId.Value;

            //paymentRequest.BalanceAmount = paymentRequestVm.Amount.Value;
            var financialYear = GetFinancialYear(DateTime.Today);
            paymentRequest.FinancialYearId = financialYear.Id;

            var requestNumber = "";
          
          //requestNumber = paymentRequestSequenceService.GetNextSequence(
          //PaymentRequestType.ADVANCE.ToString(),
          //employee.Location.Code,
          //employee.EmployeeCode,
          //financialYear.Code
          //);

                var sequence = dbContext.PaymentRequestSeriesSequence.FirstOrDefault(s =>
                string.Equals(s.AdvanceExpense, PaymentRequestType.ADVANCE.ToString()) &&
                string.Equals(s.FinancialYear, financialYear.Code) &&
                string.Equals(s.LocationCode, employee.Location.Code));


                if (sequence == null)
                {
                    sequence = new PaymentRequestSeriesSequence
                    {
                        AdvanceExpense = PaymentRequestType.ADVANCE.ToString(),
                        FinancialYear = financialYear.Code,
                        LocationCode = employee.Location.Code,
                        SequenceNumber = 1
                    };
                    dbContext.PaymentRequestSeriesSequence.Add(sequence);
                    dbContext.SaveChanges();
                }
                else
                {
                    sequence.SequenceNumber = sequence.SequenceNumber + 1;
                    dbContext.Entry(sequence).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }

                requestNumber = string.Join("/", new String[] { "ADV",
                 employee.Location.Code,
                employee.Id.ToString(),
                sequence.SequenceNumber.ToString(),
                sequence.FinancialYear}
                );




        
        
            paymentRequest.RequestNumber = requestNumber;
            paymentRequest.VersionNumber = 1;

            PaymentRequestApprovalAction approvalActions = new PaymentRequestApprovalAction
            {
                ActionById = employee.Id,
                PaymentRequest = paymentRequest,
                Timestamp = DateTime.Now,
                Action = PaymentRequestActions.SUBMITTED.ToString(),
                Type = PaymentRequestType.ADVANCE.ToString()
            };

            dbContext.Currencies.ToList();
            dbContext.PaymentRequests.Add(paymentRequest);
            dbContext.PaymentRequestApprovalActions.Add(approvalActions);
          //  dbContext.SaveChanges();

            await dbContext.SaveChangesAsync();




        }






        }
}

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;
using TechParvaLEAO.Notification;
using TechParvaLEAO.Service;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
   * Handler class for New Expense.
   */
    public class NewExpenseHandler : IRequestHandler<ExpenseViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestSequenceService _paymentRequestSequenceService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IsharepointEnhance sharepointSender;
        private readonly SharePointOptions sharepointOptions;
        private readonly SharePoint_service _SharePointservice;

   

        public NewExpenseHandler(IApplicationRepository repository, IMapper mapper,
            PaymentRequestSequenceService paymentRequestSequenceService,
            PaymentRequestService paymentRequestService,
            ApplicationDbContext dbContext,
            IMediator mediator, IsharepointEnhance sharepointsender, SharePointOptions sharepointOptions, SharePoint_service sharePointservice)
        {
            _repository = repository;
            _mapper = mapper;
            _paymentRequestService = paymentRequestService;
            _paymentRequestSequenceService = paymentRequestSequenceService;
            _dbContext = dbContext;
            _mediator = mediator;
            sharepointSender = sharepointsender;
            this.sharepointOptions = sharepointOptions;
            _SharePointservice = sharePointservice;
        }

        public async Task<bool> Handle(ExpenseViewModel paymentRequestVm, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetOneAsync<Employee>(e => e.Id == Int32.Parse(paymentRequestVm.EmployeeId));
            PaymentRequest expenseRequestRecord = null;
            var financialYear = _paymentRequestService.GetFinancialYear(DateTime.Today);
            var requestNumber = _paymentRequestSequenceService.GetNextSequence(
                PaymentRequestType.REIMBURSEMENT.ToString(),
                employee.Location.Code,
                employee.EmployeeCode,
                financialYear.Code
                );
            //FIXME-VOC -highest bu
            expenseRequestRecord = new PaymentRequest
            {
                FromDate = paymentRequestVm.FromDate,
                ToDate = paymentRequestVm.ToDate,
                CurrencyId = Int32.Parse(paymentRequestVm.CurrencyId),
                Employee = employee,
                Comment = paymentRequestVm.Comment,
                PaymentRequestCreatedById = paymentRequestVm.CreatedByEmployeeId,
                Status = PaymentRequestStatus.PENDING.ToString(),
                Type = PaymentRequestType.REIMBURSEMENT.ToString(),
                LocationId = employee.LocationId.Value,
                PaymentRequestActionedById = employee.ReportingToId,
                AdvancePaymentRequestId = paymentRequestVm.AdvancePaymentRequestId,
                FinancialYearId = financialYear.Id,
                RequestNumber = requestNumber,
                VersionNumber = 1,
                CreditCard = paymentRequestVm.CreditCard,
                PaidAmount = 0,
                operationtype=paymentRequestVm.operationtype,
                operationno=paymentRequestVm.operationno
                
            };
            _dbContext.Entry(expenseRequestRecord).State = EntityState.Added;
            var lineItemsEntries = new List<PaymentRequestLineItems>();
            var totalAmount = 0.0;
            var advancePaymentAmount = 0.0;
            var balance = 0.0;
            var advancePaymentRequest = null as PaymentRequest;
            
            if (expenseRequestRecord.AdvancePaymentRequestId!=null)
            {
                advancePaymentRequest = _dbContext.PaymentRequests.
                    FirstOrDefault(p => p.Id == expenseRequestRecord.AdvancePaymentRequestId);
                if (advancePaymentRequest != null)
                {
                    advancePaymentAmount = advancePaymentRequest.Amount;
                }
            }


            foreach (var dataEntry in paymentRequestVm.ExpenseLineItems)
            {
                PaymentRequestLineItems lineItem = new PaymentRequestLineItems
                {
                  operationtype= dataEntry.operationtype,
                  operationno= dataEntry.operationno,
                    PaymentRequest = expenseRequestRecord,
                    ExpenseHeadId = Int32.Parse(dataEntry.ExpenseHead),
                    BusinessActivityId = Int32.Parse(dataEntry.BusinessActivity),
                    CustomerMarketId = Int32.Parse(dataEntry.CustomerMarket),
                    Amount = dataEntry.Amount.Value,
                    CurrencyId = Int32.Parse(paymentRequestVm.CurrencyId),
                    ExpenseVoucherReferenceNumber = dataEntry.ExpenseVoucherReferenceNo,
                    VoucherDescription = dataEntry.Description,
                    ExpenseDate = dataEntry.Date.Value
                };
                totalAmount = totalAmount + dataEntry.Amount.Value;
                lineItemsEntries.Add(lineItem);
                _dbContext.Entry(lineItem).State = EntityState.Added;
            }

            var paidAdvaces = _paymentRequestService.GetPaidAdvances(employee, null);
            var approvedReimbursements = _paymentRequestService.GetApprovedReimbursements(employee, null);
            var paidAmount = 0.0d;
            var reimbusedAmount = 0.0d;
            foreach (var advance in paidAdvaces)
            {
                paidAmount += advance.Amount;
            }
            foreach (var reimb in approvedReimbursements)
            {
                reimbusedAmount += reimb.Amount;
            }


            balance = paidAmount - reimbusedAmount - totalAmount;
            expenseRequestRecord.Amount = totalAmount;
            expenseRequestRecord.BalanceAmount = balance;
            if (advancePaymentRequest != null)
            {
                advancePaymentRequest.AdvancePaymentRequest = expenseRequestRecord;
                advancePaymentRequest.BalanceAmount = balance;
                _dbContext.Entry(advancePaymentRequest).State = EntityState.Modified;
            }

            expenseRequestRecord.LineItems = lineItemsEntries;

            if (paymentRequestVm.DraftId != null)
            {
                var draft = _dbContext.PaymentRequestDraft.
                    Where(p => p.DraftId == paymentRequestVm.DraftId).FirstOrDefault();
                if (draft != null)
                {
                    _dbContext.PaymentRequestDraft.Remove(draft);
                }
            }

           

            var isPaymentexist = _paymentRequestService.CheckTodaysPaymentRequest(employee, totalAmount);

            if(isPaymentexist == true)
            {
                return true;
            }
            _dbContext.SaveChanges();
            var notification = new NotificationEventModel
            {
                Type = EmailNotification.TYPE_EXPENSE,
                Event = EmailNotification.STATUS_EXPENSE_SUBMITTED,
                ModelType = typeof(PaymentRequest),
                ObjectId = expenseRequestRecord.Id
            };
            long size = paymentRequestVm.Supportings.Sum(f => f.Length);
            string filePathData = "";

            var SPO = _SharePointservice.get_sharepointOption();
            if (SPO.SwitchOn == "yes")
            {


                foreach (var formFile in paymentRequestVm.Supportings)
                {
                    if (formFile.Length > 0)
                    {
                        string filename = expenseRequestRecord.RequestNumber.Replace('/', '_') + formFile.FileName;

                        string filePath = Path.Combine("Uploads", expenseRequestRecord.RequestNumber.Replace('/', '_') + formFile.FileName);
                        filePathData = filePathData + filePath;
                        filePathData = filePathData + "*";
                        // full path to file in temp location
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            // stream.Name
                            await formFile.CopyToAsync(stream);
                            //await sps.Upload_file_sharepoint_Async("2022", stream.Name);                       
                        }

                        string m_exePath = string.Empty;
                        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


                        string file_of_sharepoint = "";
                        //  file_to_sharepoint = Path.Combine(paymentRequestVm.fake_path, "//", filePath);
                        file_of_sharepoint = m_exePath + "/Uploads/" + filename;
                        //SharePoint_service sps = new SharePoint_service();
                        //await sps.Upload_file_sharepoint_Async(filename, DateTime.Now.Year.ToString(), file_of_sharepoint);
                        file_of_sharepoint = file_of_sharepoint.Replace('\\', '/');

                        var SPUD = new SharePointUploadData
                        {
                            filename = filename,
                            foldername = DateTime.Now.Year.ToString(),
                            filePath = file_of_sharepoint
                        };



                        await sharepointSender.Upload_file_sharepoint_Async(SPUD);
                    }
                }
          

            if (paymentRequestVm.Supportings.Count > 0)
            {
                expenseRequestRecord.SupportingDocumentsPath = filePathData;
                _dbContext.SaveChanges();
            }
            }
            await _mediator.Publish<NotificationEventModel>(notification);
            return true;
        }
    }
}

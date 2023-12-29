using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * View Model for Advance Data Export
     */
    public class AdvanceDataExportViewModel
    {
        public int SerialNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AdvanceRequestNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string LocationCode { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public double AmountInINR { get; set; }
        public string Detail { get; set; }
        public string Status { get; set; }
    }

    /*
     * View Model for Expense Data Export
     */
    public class ExpenseDataExportViewModel
    {
        public string EmployeeCode { get; set; }

     
        public string PostingDate { get; set; }
    
 
        public string DocumentDate { get; set; }
      

        //  public DateTime PostingDate { get; set; }
        //  public DateTime DocumentDate { get; set; }




        public int DocumentNumber { get; set; }
        public string DocumentType { get; set; } = "INVOICE";
        public string AccountNumber { get; set; }
        public double ForeignAmount { get; set; }
        public string Currency { get; set; }
        public double FxRate { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; } = "G/L ACCOUNT";
        public string ExpenseHead { get; set; }
        public string TDSNatureOfDeduction { get; set; } = "";
        public string BillNumber { get; set; }
        public string LocationCode { get; set; }
        public string Quantity { get; set; } = "1";
        public string Structure { get; set; } = "";
        public string GSTCredit { get; set; } = "NON - AVAILMENT";
        public string GSTGroupCode { get; set; } = "";
        public string HSNSACCode { get; set; } = "";
        public string BusinessMarket { get; set; }
        public string CustomerMarket { get; set; }
        public string Dimension3 { get; set; } = "";
        public string Dimension4 { get; set; } = "";
        public string Dimension5 { get; set; } = "";
        public string Dimension6 { get; set; } = "";
        public string Dimension7 { get; set; } = "";
        public string Dimension8 { get; set; } = "";
        public string GeneralProductPostingGroup { get; set; } = "SERVICE";
        public string PostingDescription { get; set; }
        public string LineDescription { get; set; }
    }

    /*
    * View Model for Pending Document Export
    */
    public class PendingDocumentExportViewModel
    {
        public string EmployeeCode { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime DocumentDate { get; set; }
        public int DocumentNumber { get; set; }
        public string AccountNumber { get; set; }
        public double ForeignAmount { get; set; }
        public string Currency { get; set; }
        public double FxRate { get; set; }
        public double Amount { get; set; }
        public string ExpenseHead { get; set; }
        public string BillNumber { get; set; }
        public string LocationCode { get; set; }
        public string PostingDescription { get; set; }
        public string ApprovedBy { get; set; }
        public string PostedBy { get; set; }
    }

    /*
    * View Model for Finance Payment Data Import
    */
    public class FinancePaymentDataImportViewModel
    {
        [Name("Transaction Type")]
        public string TransactionType { get; set; }
        [Name("Employee Code")]
        public string EmployeeCode { get; set; }
        [Name("Benificiary Code")]
        public string BenificiaryCode { get; set; }
        [Name("Transaction Amount")]
        public string TransactionAmount { get; set; }
        [Name("Benificiary Name")]
        public string BenificiaryName { get; set; }
        [Name("Statement Narration")]
        public string StatementNarration { get; set; }
        [Name("Payment Detail 1")]
        public string PaymentDetail1 { get; set; }
        [Name("Payment Detail 2")]
        public string PaymentDetail2 { get; set; }
        [Name("Payment Detail 3")]
        public string PaymentDetail3 { get; set; }
        [Name("Payment Detail 4")]
        public string PaymentDetail4 { get; set; }
        [Name("Payment Detail 5")]
        public string PaymentDetail5 { get; set; }
        [Name("Payment Detail 6")]
        public string PaymentDetail6 { get; set; }
        [Name("Payment Detail 7")]
        public string PaymentDetail7 { get; set; }

        [Name("Cheque No")]
        public string ChequeNo { get; set; }
        [Name("Chq/Time")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string ChqTime { get; set; }
    }

    /*
    * View Model for Finance Payment Data Import Result
    */
    public class FinancePaymentDataImportResult
    {
        public FinancePaymentDataImportResult()
        {

        }
        [Display(Name ="Transaction Type")]
        public string TransactionType { get; set; }
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Benificiary Code")]
        public string BenificiaryCode { get; set; }
        [Display(Name = "Transaction Amount")]
        public string TransactionAmount { get; set; }
        [Display(Name = "Benificiary Name")]
        public string BenificiaryName { get; set; }
        [Display(Name = "Statement Narration")]
        public string StatementNarration { get; set; }
        [Display(Name = "Payment Detail 1")]
        public string PaymentDetail1 { get; set; }
        [Display(Name = "Cheque No")]
        public string ChequeNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Chq/ ime")]
        public string ChqTime { get; set; }
        public FinancePaymentDataImportResult(FinancePaymentDataImportViewModel record,string payment_detail_data)
        {
            this.TransactionType = record.TransactionType;
            this.EmployeeCode = record.EmployeeCode;
            this.BenificiaryCode = record.BenificiaryCode;
            this.TransactionAmount = record.TransactionAmount;
            this.BenificiaryName = record.BenificiaryName;
            this.StatementNarration = record.StatementNarration;
            this.PaymentDetail1 = payment_detail_data;
            this.ChequeNo = record.ChequeNo;
            this.ChqTime = record.ChqTime;
        }

        [Display(Name = "Record Found")]
        public bool IsPaymentRequestFound { get; set; }
        [Display(Name = "Proceesing Status")]
        public string IsRecordUpdatedSuccessfully { get; set; }
    }
}
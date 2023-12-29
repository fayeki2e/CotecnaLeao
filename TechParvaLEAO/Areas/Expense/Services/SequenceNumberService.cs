using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Expense.Services
{
    public class PaymentRequestSequenceService
    {
        private ApplicationDbContext _context;
        public PaymentRequestSequenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetNextSequence(string type, string locationCode, string employeeId, string financialYear)
        {
            var sequence = _context.PaymentRequestSeriesSequence.FirstOrDefault(s =>
                string.Equals(s.AdvanceExpense, type) &&
                string.Equals(s.FinancialYear, financialYear) &&
                string.Equals(s.LocationCode, locationCode));
            if (sequence == null)
            {
                sequence = new PaymentRequestSeriesSequence
                {
                    AdvanceExpense = type,
                    FinancialYear = financialYear,
                    LocationCode = locationCode,
                    SequenceNumber = 1
                };
                _context.PaymentRequestSeriesSequence.Add(sequence);
                _context.SaveChanges();
            }
            else
            {
                sequence.SequenceNumber = sequence.SequenceNumber + 1;
                _context.Entry(sequence).State= EntityState.Modified;
                _context.SaveChanges();
            }
            return string.Join("/", new String[] { GetAdvanceExpenseCode(sequence.AdvanceExpense),
                sequence.LocationCode,
                employeeId,
                sequence.SequenceNumber.ToString(),
                sequence.FinancialYear}
            );
        }

        private string GetAdvanceExpenseCode(String AdvanceExpense)
        {
            if ("ADVANCE".Equals(AdvanceExpense))
            {
                return "ADV";
            }
            else if ("REIMBURSEMENT".Equals(AdvanceExpense))
            {
                return "EXP";
            }
            return "NA";
        }

    }
}

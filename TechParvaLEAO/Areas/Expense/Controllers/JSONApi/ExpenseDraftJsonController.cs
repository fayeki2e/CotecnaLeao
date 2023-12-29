using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using Newtonsoft.Json;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Models;

namespace TechParvaLEAO.Areas.Leave.Controllers.JsonApi
{
    /*
    * Json Api Controller for Expense Draft
    */
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseDraftJsonController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ExpenseDraftJsonController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public ActionResult Post([FromForm]ExpenseDraftViewModel form_data)
        {
            if (form_data.EmployeeId == null) return Ok("");
            var user = User.Identity;
            var draft = null as PaymentRequestDraft;
            string data = JsonConvert.SerializeObject(form_data);
            if (form_data.DraftId == null)
            {
                draft = new PaymentRequestDraft();
                draft.DraftId = Guid.NewGuid().ToString();
                form_data.DraftId = draft.DraftId;
                draft.FormData = data;
                draft.EmployeeId = Int32.Parse(form_data.EmployeeId);
                draft.LastUpdatedOn = DateTime.Now;
                draft.Type = "EXPENSE";
                draft.UserIdentity = User.Identity.Name;
                context.Add(draft);
                context.SaveChanges();
            }
            else
            {
                draft = context.PaymentRequestDraft.Where(d => d.UserIdentity == User.Identity.Name && 
                        d.DraftId == form_data.DraftId).FirstOrDefault();
                if (draft!=null)
                {
                    draft.FormData = data;
                    draft.LastUpdatedOn = DateTime.Now;
                    draft.Type = "EXPENSE";
                    context.Update(draft);
                    context.SaveChanges();
                }
            }
            return Ok(draft.DraftId);
        }
    }
}

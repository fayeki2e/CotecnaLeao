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

namespace TechParvaLEAO.Areas.Leave.Controllers.JsonApi
{
    /*
    * Json Api Controller for Leave Draft
    */
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveDraftJsonController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LeaveDraftJsonController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public ActionResult Post([FromForm]LeaveDraftViewModel form_data)
        {
            if (form_data.EmployeeId == null) return Ok("");
            var user = User.Identity;
            var draft = null as LeaveDraft;
            string data = JsonConvert.SerializeObject(form_data);
            if (form_data.DraftId == null)
            {
                draft = new LeaveDraft();
                draft.UniqueId = Guid.NewGuid().ToString();
                form_data.DraftId = draft.UniqueId;
                draft.FormData = data;
                draft.EmployeeId = form_data.EmployeeId;
                draft.LastUpdatedOn = DateTime.Now;
                draft.Type = "LEAVE";
                draft.UserIdentity = User.Identity.Name;
                context.Add(draft);
                context.SaveChanges();
            }
            else
            {
                draft = context.LeaveDrafts.Where(d => d.UserIdentity == User.Identity.Name && 
                        d.UniqueId == form_data.DraftId).FirstOrDefault();
                if (draft!=null)
                {
                    draft.FormData = data;
                    draft.LastUpdatedOn = DateTime.Now;
                    draft.Type = "LEAVE";
                    context.Update(draft);
                    context.SaveChanges();
                }
            }
            return Ok(draft?.UniqueId);
        }
    }
}

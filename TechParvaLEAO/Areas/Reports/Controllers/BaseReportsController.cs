using ClosedXML.Report;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Reports.Services;
using X.PagedList;

namespace TechParvaLEAO.Areas.Reports.Controllers
{
    public abstract class BaseReportsController : Controller
    {
        private readonly int PAGE_SIZE = 10;

        private readonly Random _random = new Random();

        protected IConverter pdfConverter;
        protected IViewRenderService viewRenderService;

        public abstract string GetXsltTemplatePath();
        public abstract string GetPdfTemplatePath();
        public abstract Task UpdateSearchVmNames(object searchVm);


        protected async Task<IActionResult> GenerateReport<T>(string templateName, IEnumerable<T> model, object searchVm, int? id, string download)
        {
            if (string.Equals(download, "xlsx"))
            {
                await UpdateSearchVmNames(searchVm);
                var report = GenerateFileReport(templateName, model, searchVm);
                report.Position = 0;
                return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }else if(string.Equals(download, "pdf"))
            {
                var result = await this.RenderViewAsync(GetPdfTemplatePath()+templateName, model, searchVm);

                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = templateName,
                };
                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = result,                    
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "styles.css") },
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Cotecna Inspection India Pvt. Ltd.", Line = false },                    
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Right = "Page [page] of [toPage]"}                    
                };
                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };
                var file = pdfConverter.Convert(pdf);
                return File(file, "application/pdf", templateName + ".pdf");
            }
            return View(model.ToPagedList(id ?? 1, PAGE_SIZE));
        }

        protected Stream GenerateFileReport<T>(string templateName, IEnumerable<T> model, object searchVm)
        {
            var template = new XLTemplate(string.Concat(GetXsltTemplatePath(), templateName, ".xlsx"));
            //var outputPath = reportsOptions.OutputLocation;
            //var outputLocation = string.Concat(outputPath, GenerateReportOutputFileName(templateName));
            template.AddVariable("Model", model);
            template.AddVariable("Search", searchVm);
            template.Generate();
            Stream stream = new MemoryStream();
            template.SaveAs(stream);
            return stream;
        }

        private string GenerateReportOutputFileName(string templateName)
        {
            var datePart = DateTime.Now.ToString("yyyy_mm_dd");
            var random = _random.Next(1111, 9999);
            return string.Concat(datePart, "_", random, "_", templateName, ".xlsx");
        }

    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Services;

namespace SalesWebMvc.Controllers
{
    public class SalesRecordsController : Controller
    {
        private readonly SalesRecordService _salesRecordService;

        public SalesRecordsController(SalesRecordService salesRecordService)
        {
            _salesRecordService = salesRecordService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SimpleSearch(DateTime? initial, DateTime? final)
        {
            if (!initial.HasValue)
            {
                initial = new DateTime(DateTime.Now.Year, 1, 1);
            }

            if (!final.HasValue)
            {
                final = DateTime.Now.Date;
            }

            ViewData["initial"] = initial.Value.ToString("yyyy-MM-dd");
            ViewData["final"] = final.Value.ToString("yyyy-MM-dd");

            return View(await _salesRecordService.FindByDateAsync(initial.Value, final.Value));
        }

        public async Task<IActionResult> GroupingSearch(DateTime? initial, DateTime? final)
        {
            if (!initial.HasValue)
            {
                initial = new DateTime(DateTime.Now.Year, 1, 1);
            }

            if (!final.HasValue)
            {
                final = DateTime.Now.Date;
            }

            ViewData["initial"] = initial.Value.ToString("yyyy-MM-dd");
            ViewData["final"] = final.Value.ToString("yyyy-MM-dd");

            return View(await _salesRecordService.FindByDateGroupingAsync(initial, final));
        }
    }
}

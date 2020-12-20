using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers
{
    public class SalesRecordsController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly SalesRecordService _salesRecordService;

        public SalesRecordsController(SellerService sellerService, SalesRecordService salesRecordService)
        {
            _sellerService = sellerService;
            _salesRecordService = salesRecordService;
        }

        public async Task<IActionResult> Create()
        {
            return View(new SalesRecordFormViewModel
            {
                Sellers = await _sellerService.FindAllAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesRecord salesRecord)
        {
            if (!ModelState.IsValid)
            {
                return View(new SalesRecordFormViewModel
                {
                    SalesRecord = salesRecord,
                    Sellers = await _sellerService.FindAllAsync()
                });
            }

            await _salesRecordService.InsertAsync(salesRecord);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not provided" });
            }

            SalesRecord sale = await _salesRecordService.FindByIdAsync(id.Value);

            if (sale == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not found" });
            }

            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _salesRecordService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {
                return RedirectToAction(nameof(Error), new { Message = e.Message });
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not provided" });
            }

            SalesRecord sale = await _salesRecordService.FindByIdAsync(id.Value);

            if (sale == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not found" });
            }

            return View(sale);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not provided" });
            }

            SalesRecord saleRecord = await _salesRecordService.FindByIdAsync(id.Value);

            if (saleRecord == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not found" });
            }

            return View(new SalesRecordFormViewModel
            {
                SalesRecord = saleRecord,
                Sellers = await _sellerService.FindAllAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SalesRecord salesRecord)
        {
            if (id != salesRecord.Id)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return View(new SalesRecordFormViewModel
                {
                    SalesRecord = salesRecord,
                    Sellers = await _sellerService.FindAllAsync()
                });
            }

            try
            {
                await _salesRecordService.UpdateAsync(salesRecord);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { Message = e.Message });
            }
        }

        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
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
    }
}

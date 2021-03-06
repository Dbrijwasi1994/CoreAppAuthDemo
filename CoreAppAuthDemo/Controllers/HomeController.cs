using CoreAppAuthDemo.Models;
using CoreAppAuthDemo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.EJ2.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMasterDataServiceRepo _masterDataService;

        public HomeController(ILogger<HomeController> logger, IMasterDataServiceRepo masterDataService)
        {
            _logger = logger;
            _masterDataService = masterDataService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy";
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Employees()
        {
            try
            {
                ViewData["Title"] = "Employee List";
                var employees = await _masterDataService.GetEmployees();
                return View(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        [Authorize]
        public IActionResult ProcessParameters()
        {
            try
            {
                var processParamData = _masterDataService.GetProcessParamData();
                return View(processParamData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        [Authorize]
        public async Task<IActionResult> PumpMasterData()
        {
            try
            {
                var pumpMasterData = await _masterDataService.GetPumpMasterData();
                return View(pumpMasterData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public IActionResult GetMasterData([FromBody] DataManagerRequest dm)
        {
            var DataSource = _masterDataService.GetPumpMasterData().Result;
            DataOperations operation = new DataOperations();
            int count = DataSource.Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public async Task<ActionResult> Insert([FromBody] ICRUDModel<PumpMasterData> value)
        {
            if (ModelState.IsValid)
            {
                bool success = await _masterDataService.InsertPumpMasterData(value.value);
                return Json(value.value);
            }
            else
                return Json(false);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

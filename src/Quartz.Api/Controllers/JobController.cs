using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz.Api.Models;
using Quartz.Api.Models.ViewModels;
using Quartz.Api.Services;

namespace Quartz.Api.Controllers
{
    public class JobController : Controller
    {
        private IScheduleService _scheduleService;
        public JobController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddJob([FromBody]ScheduleInfo entity)
        {
            return Ok(await _scheduleService.AddScheduleJobAsync(entity));
        }

        [HttpPost]
        public async Task<IActionResult> StartSchedule()
        {
            return Ok(await _scheduleService.StartScheduleAsync());
        }


        [HttpPost]
        public async Task<IActionResult> StopScheduleJob([FromBody]ScheduleInfo entity)
        {
            return Ok(await _scheduleService.StopScheduleJobAsync(entity));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllJob()
        {
            return Ok(await _scheduleService.GetAllJobAsync());
        }



        [HttpGet]
        public async Task<IActionResult> PagesJob([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 1)
        {
            var totalItems = 0;
            var q = await _scheduleService.PagesJob(pageIndex, pageSize, out totalItems).ToListAsync(); ;
            var model = new PaginatedItemsViewModel<ScheduleInfo>(pageIndex, pageSize, totalItems, q);
            return Ok(model);
        }

    }
}
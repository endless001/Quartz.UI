using Quartz;
using Quartz.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quartz.Api.Services
{
    public interface IScheduleService
    {
        Task<bool> AddScheduleJobAsync(ScheduleInfo schedule);
        Task<bool> StartScheduleJobAsync(ScheduleInfo schedule);
        Task<bool> StopScheduleJobAsync(ScheduleInfo schedule);
        Task<bool> ResumeJobAsync(string jobGroup, string jobName);
        Task<ScheduleInfo> QueryJobAsync(string jobGroup, string jobName);
        IQueryable<ScheduleInfo> PagesJob(int pageIndex, int pageSize, out int count);
        Task<IQueryable<ScheduleInfo>> GetAllJobAsync();
        Task<bool> StartScheduleAsync();
        Task<bool> StopScheduleAsync();


    }
}

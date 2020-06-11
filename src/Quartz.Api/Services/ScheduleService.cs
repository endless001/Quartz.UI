using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Simpl;
using Quartz.Util;
using Quartz.Api.Data;
using Quartz.Api.Job;
using Quartz.Api.Models;


namespace Quartz.Api.Services
{
    public class ScheduleService : IScheduleService
    {
        readonly QuartzDbContext _context;
        readonly IScheduler _scheduler;

        public ScheduleService(QuartzDbContext context, IScheduler scheduler)
        {
            _context = context;
            _scheduler = scheduler;

        }
        public async Task<bool> AddScheduleJobAsync(ScheduleInfo scheduleInfo)
        {
            try
            {
                if (scheduleInfo != null)
                {
                    if (scheduleInfo.BeginTime == null)
                    {
                        scheduleInfo.BeginTime = DateTime.Now;
                    }
                    DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(scheduleInfo.BeginTime, 1);
                    if (scheduleInfo.EndTime == null)
                    {
                        scheduleInfo.EndTime = DateTime.MaxValue.AddDays(-1);
                    }
                    DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(scheduleInfo.EndTime, 1);
                    IJobDetail job = JobBuilder.Create<HttpJob>()
                      .WithIdentity(scheduleInfo.JobName, scheduleInfo.JobGroup)
                      .Build();
                    ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                 .StartAt(starRunTime)
                                                 .EndAt(endRunTime)
                                                 .WithIdentity(scheduleInfo.JobName, scheduleInfo.JobGroup)
                                                 .WithCronSchedule(scheduleInfo.Cron)
                                                 .Build();
                    
                    _context.ScheduleInfo.Add(scheduleInfo);
                    await _context.SaveChangesAsync();
                    await _scheduler.ScheduleJob(job, trigger);
                    await _scheduler.Start();
                    await StopScheduleJobAsync(scheduleInfo);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<IQueryable<ScheduleInfo>> GetAllJobAsync()
        {
              return _context.Set<ScheduleInfo>().AsQueryable();

        }

        public IQueryable<ScheduleInfo> PagesJob(int pageIndex, int pageSize, out int count)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 10;
            }
            count = _context.Set<ScheduleInfo>().Count();

            return _context.Set<ScheduleInfo>().Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public Task<ScheduleInfo> QueryJobAsync(string jobGroup, string jobName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResumeJobAsync(string jobGroup, string jobName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StartScheduleAsync()
        {
            //开启调度器
            if (_scheduler.InStandbyMode)
            {
                await _scheduler.Start();

            }
            return _scheduler.InStandbyMode;
        }

        public async Task<bool> StopScheduleAsync()
        {

            if (!_scheduler.InStandbyMode)
            {
                //等待任务运行完成
                await _scheduler.Standby(); 
            }
            return !_scheduler.InStandbyMode;
        }


        public async Task<bool> StopScheduleJobAsync(ScheduleInfo model)
        {
            try
            {
                await _scheduler.PauseJob(new JobKey(model.JobName, model.JobGroup));
                //更新数据库
                var entity = _context.Set<ScheduleInfo>().Where(a => a.Id == model.Id).FirstOrDefault();
                entity.Status= (int)JobStatus.Stopped;
                _context.Entry(entity).State = EntityState.Modified;
          
                _context.SaveChanges();
                return true;

                 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> StartScheduleJobAsync(ScheduleInfo scheduleInfo)
        {
            try
            {
                if (scheduleInfo != null)
                {
                    if (scheduleInfo.BeginTime == null)
                    {
                        scheduleInfo.BeginTime = DateTime.Now;
                    }
                    DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(scheduleInfo.BeginTime, 1);
                    if (scheduleInfo.EndTime == null)
                    {
                        scheduleInfo.EndTime = DateTime.MaxValue.AddDays(-1);
                    }
                    DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(scheduleInfo.EndTime, 1);
                    IJobDetail job = JobBuilder.Create<HttpJob>()
                      .WithIdentity(scheduleInfo.JobName, scheduleInfo.JobGroup)
                      .Build();
                    ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                 .StartAt(starRunTime)
                                                 .EndAt(endRunTime)
                                                 .WithIdentity(scheduleInfo.JobName, scheduleInfo.JobGroup)
                                                 .WithCronSchedule(scheduleInfo.Cron)
                                                 .Build();

                    await _scheduler.ScheduleJob(job, trigger);
                    await _scheduler.Start();
                  //  await StopScheduleJobAsync(scheduleInfo.JobGroup, scheduleInfo.JobName);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}

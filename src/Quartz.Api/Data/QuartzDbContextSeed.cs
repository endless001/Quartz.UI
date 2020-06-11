using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz.Impl;
using Quartz.Api.Job;
using Quartz.Api.Models;
using Quartz.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quartz.Api.Data
{
    public class QuartzDbContextSeed
    {
         IScheduler _scheduler;
     
        public async Task SeedAsync(IHost host, IConfiguration configuration)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<QuartzDbContext>();
                _scheduler = serviceScope.ServiceProvider.GetRequiredService<IScheduler>();
                var schedule = context.Set<ScheduleInfo>().Where(a=>a.Status== 1).ToList();
                foreach (var item in schedule)
                {

                    await StartScheduleJobAsync(item);

                }

            }
        }

        private async Task<bool> StartScheduleJobAsync(ScheduleInfo scheduleInfo)
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
                    var keys = new Dictionary<string, string>()
                    {   
                          { "RequestUrl",scheduleInfo.RequestUrl},
                          { "RequestType" ,scheduleInfo.RequestType.ToString()},
                          { "JobId",scheduleInfo.Id.ToString() }
                    };
                    DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(scheduleInfo.EndTime, 1);
                    IJobDetail job = JobBuilder.Create<HttpJob>()
                         .SetJobData(new JobDataMap(keys))
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

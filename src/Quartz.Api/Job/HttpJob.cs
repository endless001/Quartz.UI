using Newtonsoft.Json;
using Quartz.Api.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Quartz.Api.Models;
using Quartz.Api.Infrastructure;

namespace Quartz.Api.Job
{
    public class HttpJob : IJob
    {
        readonly QuartzDbContext _context;
        readonly StandardHttpClient _httpClient;
        public HttpJob(QuartzDbContext context, StandardHttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("111");
   
            var requestUrl = context.JobDetail.JobDataMap.GetString("RequestUrl");
            var requestParameters = context.JobDetail.JobDataMap.GetString("RequestParameters");
            var httpHeaders= context.JobDetail.JobDataMap.GetString("Headers");
            var jobId = context.JobDetail.JobDataMap.GetInt("JobId");
            var headers = httpHeaders != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(httpHeaders?.Trim()) : null;
            var requestType = (RequestTypeEnum)int.Parse(context.JobDetail.JobDataMap.GetString("RequestType"));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();

            var loginfo = new LogInfo();
            loginfo.Url = requestUrl;
            loginfo.BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            loginfo.RequestType = requestType.ToString();
            loginfo.Parameters = requestParameters;
            loginfo.JobName = $"{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}";
            loginfo.JobId = jobId;
            HttpResponseMessage response = new HttpResponseMessage();
            var http = new HttpClient();
            switch (requestType)
            {
                case RequestTypeEnum.Get:
                    response = await _httpClient.GetStringAsync(requestUrl, headers);
                    break;
                case RequestTypeEnum.Post:
                    response = await _httpClient.PostAsync(requestUrl, requestParameters, headers);
                    break;
                case RequestTypeEnum.Put:
                    response = await _httpClient.PostAsync(requestUrl, requestParameters, headers);
                    break;
                case RequestTypeEnum.Delete:
                    response = await _httpClient.DeleteAsync(requestUrl, headers);
                    break;
            }

            var result = await response.Content.ReadAsStringAsync();
            stopwatch.Stop(); //  停止监视            
            double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数                                
            loginfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            loginfo.Seconds = seconds;
            loginfo.Result = result;
            loginfo.StatusCode = (int)response.StatusCode;
            _context.LogInfo.Add(loginfo);
            await _context.SaveChangesAsync();


        }
    }
}

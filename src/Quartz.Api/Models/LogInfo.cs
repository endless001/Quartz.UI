using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quartz.Api.Models
{
    public class LogInfo
    {

        public int Id { get; set; }
        /// <summary>
        /// 开始执行时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 耗时（秒）
        /// </summary>
        public double Seconds { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        public int JobId { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public string RequestType { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// 请求结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 异常消息
        /// </summary>
        public int  StatusCode { get; set; }
    }
}

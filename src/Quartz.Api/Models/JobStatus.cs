using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Quartz.Api.Models
{
    public enum JobStatus
    {
        [Description("已启用")]
        Activated=1,
        [Description("运行中")]
        Waiting=2,
        [Description("执行中")]
        Running =3,
        [Description("执行完成")]
        Complete=4,
        [Description("已停止")]
        Stopped=5,
    }
}

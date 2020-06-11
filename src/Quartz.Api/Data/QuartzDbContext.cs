using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz.Api.Models;
namespace Quartz.Api.Data
{
    public  class QuartzDbContext: DbContext
    {
        public QuartzDbContext(DbContextOptions<QuartzDbContext> options)
           : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ScheduleInfo>()
           .HasKey(a => a.Id);
            builder.Entity<LogInfo>()
           .HasKey(a => a.Id);
        }
        public virtual DbSet<ScheduleInfo> ScheduleInfo { get; set; }
        public virtual DbSet<LogInfo> LogInfo { get; set; }
    }
}

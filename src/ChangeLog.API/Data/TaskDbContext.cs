using ChangeLog.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChangeLog.API.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }

        public DbSet<TaskData> Tasks;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskData>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();

                b.Property(x => x.Status).IsRequired();
                b.HasIndex(x => x.Status).HasName("IndexStatus");

                b.Property(x => x.CreatedBy).HasMaxLength(256);
                b.Property(x => x.CancelledBy).HasMaxLength(256);

                b.Property(x => x.Payload).HasConversion(
                    x => JsonConvert.SerializeObject(x), 
                    x => JsonConvert.DeserializeObject<TaskPayload>(x));
            });
        }
    }
}

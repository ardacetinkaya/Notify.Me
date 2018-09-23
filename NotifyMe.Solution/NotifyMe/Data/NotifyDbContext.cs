using Microsoft.EntityFrameworkCore;
using NotifyMe.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotifyMe.Data
{
    public class NotifyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public NotifyDbContext(DbContextOptions<NotifyDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<ApplicationFeature> ApplicationFeatures { get; set; }
    }
}

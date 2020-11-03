using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPM.Server.Services
{
    public class DataContext: DbContext
    {
        //Parameterless version for Mocking in Tests
        public DataContext()
        {
        }

        public DataContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<BusinessObjects.Entities.Package> Packages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}

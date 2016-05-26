using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Security;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextNavigationReferenceObject : SecurityDbContext {
        public DbSet<One> One { get; set; }
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            SecurityTestHelper.ConfigureOptionsBuilder(optionsBuilder);
        }
    }

    public class One {
        public int ID { get; set; }
        public string Name { get; set; }
        public One Reference { get; set; }
    }
}

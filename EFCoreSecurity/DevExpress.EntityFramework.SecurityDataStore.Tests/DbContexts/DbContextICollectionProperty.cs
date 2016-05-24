using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextICollectionProperty : SecurityDbContext {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            // optionsBuilder.UseInMemoryDatabase();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=efcoresecuritytests;Trusted_Connection=True;");
        }       
        public DbSet<OneToManyICollection_One> OneToManyICollection_One { get; set; }
        public DbSet<OneToManyICollection_Many> OneToManyICollection_Many { get; set; }
    }
    public class OneToManyICollection_One {
        public static int Count;
        public OneToManyICollection_One() {
            Count++;
        }
        public Guid ID { get; set; }
       
        public string Name { get; set; }
        public ICollection<OneToManyICollection_Many> Collection { get; set; } 
            = new List<OneToManyICollection_Many>();

    }
    public class OneToManyICollection_Many {
        public static int Count;
        public OneToManyICollection_Many() {
            Count++; 
        }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public OneToManyICollection_One One { get; set; }

    }
}

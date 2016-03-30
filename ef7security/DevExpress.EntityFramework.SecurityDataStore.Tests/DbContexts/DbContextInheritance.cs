using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// TODO: is it needed?
/*
namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextInheritance : SecurityDbContext {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Employe> Employes { get; set; }
   
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder options) {
            options.UseInMemoryDatabase();
        }
    }
    public class Person {
        public int ID { get; private set; }
        public string Name { get; set; }
    }
    public class Employe : Person {
        public int EmployeID { get; private set; }
        public string Description { get; set; }
    }
}
*/
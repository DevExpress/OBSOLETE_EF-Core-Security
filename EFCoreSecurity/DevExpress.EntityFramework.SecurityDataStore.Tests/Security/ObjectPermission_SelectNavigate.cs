using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    [TestFixture]
    public class ObjectPermission_SelectNavigate {
        [SetUp]
        public void SetUp() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Database.EnsureDeleted();
            }
        }
        [Test]
        public void Read_PolicyDeny_OneObjectAllow() {
            CreateThreeObjectDbContextConnectionClass();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission<DbContextConnectionClass, Company>(
                    SecurityOperation.Read, OperationState.Allow, (p, d) => d.CompanyName == "1");
                Assert.AreEqual(1, dbContextConnectionClass.Company.Count());
            }
        }
        [Test]
        public void Read_PolicyAllow_OneObjectDeny() {
            CreateThreeObjectDbContextConnectionClass();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission<DbContextConnectionClass, Company>(
                    SecurityOperation.Read, OperationState.Deny, (p, d) => d.CompanyName == "1");
                Assert.AreEqual(2, dbContextConnectionClass.Company.Count());
            }
        }

        [Test]
        public void Read_PolicyDeny_OneObjectAllow_IncludeNavigateObject() {
            CreateThreeObjectDbContextConnectionClass();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission<DbContextConnectionClass, Company>(
                 SecurityOperation.Read, OperationState.Allow, (p, d) => d.CompanyName == "1");
                 Assert.AreEqual(1, dbContextConnectionClass.Company.Include(p=>p.Persons).Count());
                 Assert.AreEqual(0, dbContextConnectionClass.Persons.Include(p => p.Company).Count());
                Company company = dbContextConnectionClass.Company.Include(p=>p.Persons).First();

                Assert.IsNull(company.Persons);                 
            }
        }

        private void CreateThreeObjectDbContextConnectionClass() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                for(int i = 1; i < 4; i++) {
                    Company company = new Company();
                    company.CompanyName = i.ToString();
                    company.Description = i.ToString();
                    Persons persons = new Persons();
                    persons.PersonName = i.ToString();
                    persons.Description = i.ToString();
                    company.Persons = persons;
                    persons.Company = company;
                    dbContextConnectionClass.Company.Add(company);
                    dbContextConnectionClass.Persons.Add(persons);
                }
                dbContextConnectionClass.SaveChanges();
            }
        }
    }
}

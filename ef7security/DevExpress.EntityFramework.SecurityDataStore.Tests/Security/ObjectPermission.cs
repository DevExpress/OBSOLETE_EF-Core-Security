using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    [TestFixture]
    public class ObjectPermission {      

        [TearDown]
        public void ClearDatabase() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureDeleted();
            }

        }
        [Test]
        public void ReadObjectAllowPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                
                DbContextObject1 obj1 = new DbContextObject1();
                obj1.Description = "Good description";
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, criteria);
                
                Assert.AreEqual(1, dbContextMultiClass.dbContextDbSet1.Count());
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                Assert.AreEqual("Good description", obj1.Description);
            }
        }
        [Test]
        public void ReadObjectAllowPermissionClearPermissions() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.Description = "Good description";
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, criteria);
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());
            }
        }   
        [Test]
        public void AnotherDbContextInNewSecurity() {
            // SecurityDbContext originalDbContext;
            // ISecurityStrategy originalSecurityStrategy;
            var securityDbContextFI = typeof(SecurityStrategy).GetRuntimeFields().First(p => p.Name == "securityDbContext");
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.Description = "aaa";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.SaveChanges();
                if(dbContextMultiClass.Security is SecurityStrategy) {
                    SecurityDbContext value = (SecurityDbContext)securityDbContextFI.GetValue(dbContextMultiClass.Security);
                    Assert.AreEqual(dbContextMultiClass.realDbContext, value.realDbContext);
                    // originalDbContext = dbContextMultiClass.realDbContext;
                    // originalSecurityStrategy = dbContextMultiClass.Security;
                }
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.Description = "bbb";

                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();

                // Assert.AreNotEqual(originalSecurityStrategy, dbContextMultiClass.Security);
                // Assert.AreNotEqual(originalDbContext, dbContextMultiClass.Security.GetDbContext());
                DbContext securityDbContext = dbContextMultiClass.realDbContext;
                if(dbContextMultiClass.Security is SecurityStrategy) {
                    SecurityDbContext value = (SecurityDbContext)securityDbContextFI.GetValue(dbContextMultiClass.Security);
                    Assert.AreEqual(securityDbContext, value.realDbContext);
                }
            }
        }
        [Test]
        public void ReadObjectAllowPermissionComplexCriteria() {
            // SecurityDbContext originalDbContext;
            // ISecurityStrategy originalSecurityStrategy;
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj11 = new DbContextObject1();
                obj11.Description = "aaa";
                DbContextObject1 obj12 = new DbContextObject1();
                obj12.Description = "bbb";

                DbContextObject2 obj2 = new DbContextObject2();
                obj2.Description = "a";

                dbContextMultiClass.Add(obj11);
                dbContextMultiClass.Add(obj12);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();

                // originalDbContext = dbContextMultiClass.realDbContext;
                // originalSecurityStrategy = dbContextMultiClass.Security;
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                // Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());

                // Assert.AreNotEqual(originalSecurityStrategy, dbContextMultiClass.Security);
                // Assert.AreEqual(dbContextMultiClass.Security, dbContextMultiClass.Security.GetDbContext());

                // Assert.AreNotEqual(originalDbContext, dbContextMultiClass.Security.GetDbContext());
                // Assert.AreEqual(dbContextMultiClass.realDbContext, dbContextMultiClass.Security.GetDbContext());

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description.Contains(db.dbContextDbSet2.First().Description);
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, criteria);



                Assert.AreEqual(1, dbContextMultiClass.dbContextDbSet1.Count());
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                Assert.AreEqual("aaa", obj1.Description);
            }
        }        
        [Test]
        public void ReadObjectDenyPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.Description = "Good description";
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(dbContextMultiClass.dbContextDbSet1.Count(), 2);

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, criteria);

                Assert.AreEqual(1, dbContextMultiClass.dbContextDbSet1.Count());
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                Assert.AreEqual("Good description", obj1.Description);
            }
        }
        [Test]
        public void ReadObjectMultiplePermissions() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.DecimalItem = 5;
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.DecimalItem = 8;
                DbContextObject1 obj3 = new DbContextObject1();
                obj3.DecimalItem = 10;

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.Add(obj3);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                var o = dbContextMultiClass.dbContextDbSet1.ToArray();
                Assert.AreEqual(dbContextMultiClass.dbContextDbSet1.Count(), 3);

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria = (db, obj) => obj.DecimalItem > 3;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, goodCriteria);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria2 = (db, obj) => obj.DecimalItem < 9;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, goodCriteria2);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.DecimalItem == 8;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, badCriteria);

                var to = dbContextMultiClass.dbContextDbSet1.ToArray();

                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());              
            }
        }
        [Test]
        public void ReadObjectCriteriaNavigationPropertyIsNull() {
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                dbContext.Database.EnsureCreated();

                Company company1 = new Company() { CompanyName = "DevExpress" };
                Person person1 = new Person() { PersonName = "John", One = company1 };
                Person person2 = new Person() { PersonName = "Jack", One = null };

                dbContext.Add(person1);
                dbContext.Add(person2);
                dbContext.SaveChanges();
            }
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                IQueryable<Person> persons = dbContext.Persons;
                Assert.AreEqual(persons.Count(), 2);
                dbContext.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                Expression<Func<DbContextConnectionClass, Person, bool>> criteria = (db, obj) => obj.One != null && obj.One.CompanyName == "DevExpress";
                dbContext.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, criteria);

                IQueryable<Person> securedPersons = dbContext.Persons.Where(p => p.ID == 2).Include(p => p.One);
                foreach(Person person in securedPersons) {
                    //Assert.AreEqual(person.PersonName, "Jack");
                }
                securedPersons = dbContext.Persons.Include(p => p.One);
                foreach(Person person in securedPersons) {
                    Assert.AreEqual(person.PersonName, "Jack");
                }
                Assert.AreEqual(securedPersons.First().PersonName, "Jack");
                Assert.AreEqual(securedPersons.Count(), 1);
            }
        }
        [Test]
        public void ReadObjectCriteriaNavigationPropertyIsNotNull() {
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                dbContext.Database.EnsureCreated();

                Company company1 = new Company() { CompanyName = "DevExpress" };
                Company company2 = new Company() { CompanyName = "Microsoft" };
                Person person1 = new Person() { PersonName = "John", One = company1 };
                Person person2 = new Person() { PersonName = "James", One = company2 };

                dbContext.Add(person1);
                dbContext.Add(person2);
                dbContext.SaveChanges();
            }
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                IQueryable<Person> persons = dbContext.Persons;
                Assert.AreEqual(persons.Count(), 2);
                dbContext.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                Expression<Func<DbContextConnectionClass, Person, bool>> criteria = (db, obj) => obj.One.CompanyName == "DevExpress";
                dbContext.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, criteria);

                IQueryable<Person> securedPersons = dbContext.Persons.Include(p => p.One);
                Assert.AreEqual(securedPersons.Count(), 1);
                Assert.AreEqual(securedPersons.First().PersonName, "James");
            }
        }
        [Test]
        public void ReadObjectAnyCriteriaNavigationCollectionIsEmpty() {
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                dbContext.Database.EnsureCreated();

                Company company1 = new Company() { CompanyName = "DevExpress" };
                Company company2 = new Company() { CompanyName = "Microsoft" };
                Person person1 = new Person() { PersonName = "John", One = company1 };
                Person person2 = new Person() { PersonName = "Jack", One = null };

                dbContext.Add(person1);
                dbContext.Add(person2);
                dbContext.Add(company2);
                dbContext.SaveChanges();
            }
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                IQueryable<Company> companies = dbContext.Company;
                Assert.AreEqual(companies.Count(), 2);
                dbContext.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                Expression<Func<DbContextConnectionClass, Company, bool>> criteria = (db, obj) => obj.Collection.Any(p => p.PersonName == "John");
                dbContext.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, criteria);

                IQueryable<Company> securedCompanies = dbContext.Company.Include(p => p.Collection);
                Assert.AreEqual(securedCompanies.Count(), 1);
                Assert.AreEqual(securedCompanies.First().CompanyName, "Microsoft");
            }
        }
        [Test]
        public void ReadObjectAnyCriteriaNavigationCollectionIsNotEmpty() {
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                dbContext.Database.EnsureCreated();

                Company company1 = new Company() { CompanyName = "DevExpress" };
                Company company2 = new Company() { CompanyName = "Microsoft" };
                Person person1 = new Person() { PersonName = "John", One = company1 };
                Person person2 = new Person() { PersonName = "Jack", One = company2 };

                dbContext.Add(person1);
                dbContext.Add(person2);
                dbContext.SaveChanges();
            }
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                IQueryable<Company> companies = dbContext.Company;
                Assert.AreEqual(companies.Count(), 2);
                dbContext.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                Expression<Func<DbContextConnectionClass, Company, bool>> criteria = (db, obj) => obj.Collection.Any(p => p.PersonName == "John");
                dbContext.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, criteria);

                IQueryable<Company> securedCompanies = dbContext.Company.Include(p => p.Collection);
                Assert.AreEqual(securedCompanies.Count(), 1);
                Assert.AreEqual(securedCompanies.First().CompanyName, "Microsoft");
            }
        }
        [Test]
        public void ReadObjectContainsCriteriaNavigationCollectionIsEmpty() {
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                dbContext.Database.EnsureCreated();

                Company company1 = new Company() { CompanyName = "DevExpress" };
                Company company2 = new Company() { CompanyName = "Microsoft" };
                Person person1 = new Person() { PersonName = "John", One = company1 };
                Person person2 = new Person() { PersonName = "Jack", One = null };

                dbContext.Add(person1);
                dbContext.Add(person2);
                dbContext.Add(company2);
                dbContext.SaveChanges();
            }
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                IQueryable<Company> companies = dbContext.Company;
                Assert.AreEqual(companies.Count(), 2);
                dbContext.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                Expression<Func<DbContextConnectionClass, Company, bool>> criteria = (db, obj) => obj.Collection.Contains(db.Persons.First(p => p.PersonName == "John"));
                dbContext.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, criteria);

                IQueryable<Company> securedCompanies = dbContext.Company.Include(p => p.Collection);
                Assert.AreEqual(securedCompanies.Count(), 1);
                Assert.AreEqual(securedCompanies.First().CompanyName, "Microsoft");
            }
        }
        [Test]
        public void ReadObjectContainsCriteriaNavigationCollectionIsNotEmpty() {
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                dbContext.Database.EnsureCreated();

                Company company1 = new Company() { CompanyName = "DevExpress" };
                Company company2 = new Company() { CompanyName = "Microsoft" };
                Person person1 = new Person() { PersonName = "John", One = company1 };
                Person person2 = new Person() { PersonName = "Jack", One = company2 };

                dbContext.Add(person1);
                dbContext.Add(person2);
                dbContext.SaveChanges();
            }
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                IQueryable<Company> companies = dbContext.Company;
                Assert.AreEqual(companies.Count(), 2);
                dbContext.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                Expression<Func<DbContextConnectionClass, Company, bool>> criteria = (db, obj) => obj.Collection.Contains(db.Persons.First(p => p.PersonName == "John"));
                dbContext.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, criteria);

                IQueryable<Company> securedCompanies = dbContext.Company.Include(p => p.Collection);
                Assert.AreEqual(securedCompanies.Count(), 1);
                Assert.AreEqual(securedCompanies.First().CompanyName, "Microsoft");
            }
        }
        [Test]
        public void WriteObjectAllowPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Write, OperationState.Allow, criteria);

                obj1.Description = "Not good description";

                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.Description = "Good description";
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void WriteObjectDenyPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Write, OperationState.Deny, criteria);

                obj1.Description = "Not good description";

                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.Description = "Good description";
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void WriteObjectMultiplePermissions() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria = (db, obj) => obj.DecimalItem > 3;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Write, OperationState.Allow, goodCriteria);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria2 = (db, obj) => obj.DecimalItem < 9;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Write, OperationState.Allow, goodCriteria2);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.DecimalItem == 8;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Write, OperationState.Deny, badCriteria);

                obj1.DecimalItem = 8;
                try {
                    dbContextMultiClass.SaveChanges();
                    Assert.Fail("Fail");
                }
                catch(Exception e) {
                    // Assert.AreNotEqual("Fail", e.Message);
                    //bool isSecurityException = e.Message.StartsWith("Deny ");
                    Assert.AreNotEqual("Fail", e.Message);
                   // Assert.AreNotEqual("Fail", e.Message);
                }

                obj1.DecimalItem = 6;
                dbContextMultiClass.SaveChanges();
            }
        }     
        [Test]
        public void CreateObjectDenyPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                DbContextObject1 obj1 = new DbContextObject1();
                dbContextMultiClass.Add(obj1);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Create, OperationState.Deny, criteria);

                obj1.Description = "Not good description";

                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.Description = "Good description";
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void CreateObjectMultiplePermissions() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                DbContextObject1 obj1 = new DbContextObject1();

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria = (db, obj) => obj.DecimalItem > 3;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Create, OperationState.Allow, goodCriteria);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria2 = (db, obj) => obj.DecimalItem < 9;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Create, OperationState.Allow, goodCriteria2);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.DecimalItem == 8;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Create, OperationState.Deny, badCriteria);       

                obj1.DecimalItem = 8;
                dbContextMultiClass.Add(obj1);

                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.DecimalItem = 6;
                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void DeleteObjectCurrentValuesAllowPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Allow, criteria);

                obj1.Description = "Not good description";

                dbContextMultiClass.Remove(obj1);
                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);
            }


            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                obj1.Description = "Good description";
               

                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Allow, criteria);

                dbContextMultiClass.Remove(obj1);

                dbContextMultiClass.SaveChanges();
            }
        }        
        [Test]
        public void DeleteObjectDatabaseValuesAllowPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> deleteCriteria = (db, obj) => obj.Description == "Good description";
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> writeCriteria = (db, obj) => true;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Allow, deleteCriteria);
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Allow, writeCriteria);

                obj1.Description = "Not good description";
                dbContextMultiClass.SaveChanges();

                dbContextMultiClass.Remove(obj1);

                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                obj1.Description = "Good description";
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> deleteCriteria = (db, obj) => obj.Description == "Good description";
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> writeCriteria = (db, obj) => true;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Allow, deleteCriteria);
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Allow, writeCriteria);
                
                dbContextMultiClass.Remove(obj1);
                dbContextMultiClass.SaveChanges();
            }
            }
        [Test]
        public void DeleteObjectCurrentValuesDenyPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();     
                obj1.Description = "Not good description";
                dbContextMultiClass.SaveChanges(); //must save real value

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Deny, criteria);
                dbContextMultiClass.Remove(obj1);
                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.Description = "Good description";
                dbContextMultiClass.SaveChanges();

                dbContextMultiClass.Remove(obj1);
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void DeleteObjectDatabaseValuesDenyPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Deny, criteria);

                obj1.Description = "Not good description";
                dbContextMultiClass.SaveChanges();

                dbContextMultiClass.Remove(obj1);

                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.Description = "Good description";
                dbContextMultiClass.SaveChanges();
                dbContextMultiClass.Remove(obj1);
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void DeleteObjectMultiplePermissions() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();          
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj3 = new DbContextObject1();
                obj3.DecimalItem = 5;
                dbContextMultiClass.Add(obj3);
                dbContextMultiClass.SaveChanges();

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria = (db, obj) => obj.DecimalItem > 3;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Allow, goodCriteria);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria2 = (db, obj) => obj.DecimalItem < 9;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Allow, goodCriteria2);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.DecimalItem == 8;
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Deny, badCriteria);
                
                obj1.DecimalItem = 8;
                dbContextMultiClass.SaveChanges();

                dbContextMultiClass.Remove(obj1);

                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.DecimalItem = 5;
                dbContextMultiClass.SaveChanges();
                dbContextMultiClass.Remove(obj1);
                dbContextMultiClass.SaveChanges();
            }
        }
        // bad scenario
        [Test]
        public void DeleteObjectWithoutPermission() {
            int objectID = 0;
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                var obj = dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                objectID = obj1.ID;
                obj1.Description = "Not good description";
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = new DbContextObject1( ) { ID = objectID };
                dbContextMultiClass.Entry(obj1).State = EntityState.Deleted;

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.AddObjectPermission(SecurityOperation.Delete, OperationState.Deny, criteria);

                // obj1.Description = "Not good description";
                // dbContextMultiClass.SaveChanges();
    
                dbContextMultiClass.Remove(obj1);

                // FailSaveChanges(dbContextMultiClass);
                try {
                    dbContextMultiClass.SaveChanges();
                    Assert.Fail("Fail");
                }
                catch(Exception e) {
                    // good, not normal
                    Assert.AreEqual("Delete Deny DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1", e.Message);
                }

                // obj1.Description = "Good description";
                // dbContextMultiClass.Remove(obj1);
                // dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void IsGrantedAllowPermission() {
            foreach(SecurityOperation securityOperation in new SecurityOperation[] {SecurityOperation.Read, SecurityOperation.Write, SecurityOperation.Delete, SecurityOperation.Create }) {
                using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                    DbContextObject1 obj1 = new DbContextObject1();
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, null));

                    dbContextMultiClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                    dbContextMultiClass.Security.AddObjectPermission(securityOperation, OperationState.Allow, criteria);

                    obj1.Description = "Not good description";
                    Assert.IsFalse(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, null));

                    obj1.Description = "Good description";
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, null));
                }
            }
        }
        [Test]
        public void IsGrantedDenyPermission() {
            foreach(SecurityOperation securityOperation in new SecurityOperation[] { SecurityOperation.Read, SecurityOperation.Write, SecurityOperation.Delete, SecurityOperation.Create }) {
                using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                    DbContextObject1 obj1 = new DbContextObject1();
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, null));

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Not good description";
                    dbContextMultiClass.Security.AddObjectPermission(securityOperation, OperationState.Deny, criteria);

                    obj1.Description = "Not good description";
                    Assert.IsFalse(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, null));

                    obj1.Description = "Good description";
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, null));
                }
            }
        }
        [Test]
        public void IsGrantedMultiplePermissions() {
            foreach(SecurityOperation securityOperation in new SecurityOperation[] { SecurityOperation.Read, SecurityOperation.Write, SecurityOperation.Delete, SecurityOperation.Create }) {
                using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                    DbContextObject1 obj1 = new DbContextObject1();
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), SecurityOperation.Write, obj1, null));

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria = (db, obj) => obj.DecimalItem > 3;
                    dbContextMultiClass.Security.AddObjectPermission(securityOperation, OperationState.Allow, goodCriteria);

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria2 = (db, obj) => obj.DecimalItem < 9;
                    dbContextMultiClass.Security.AddObjectPermission(securityOperation, OperationState.Allow, goodCriteria2);

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.DecimalItem == 8;
                    dbContextMultiClass.Security.AddObjectPermission(securityOperation, OperationState.Deny, badCriteria);

                    obj1.DecimalItem = 8;
                    Assert.IsFalse(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, null));

                    obj1.DecimalItem = 5;
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, null));
                }
            }
        }

    }
}

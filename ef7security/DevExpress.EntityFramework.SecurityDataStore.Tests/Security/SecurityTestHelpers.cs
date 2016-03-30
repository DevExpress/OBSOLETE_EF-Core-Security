using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    class SecurityTestHelper {
        public static void FailSaveChanges(SecurityDbContext dbContextMultiClass) {
            bool withSecurityException = false;
            try {
                dbContextMultiClass.SaveChanges();
            }
            catch {
                withSecurityException = true;
            }
            //catch(Exception e) {
            //    Assert.Fail(e.Message);
            //}
            Assert.IsTrue(withSecurityException);
        }
        public static void InitializeContextWithNavigationProperties() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                Company companyFirst = null;               
                for(int i = 1; i < 4; i++) {
                    string indexString = i.ToString();
                    
                    Company company = new Company();
                    company.CompanyName = indexString;
                    company.Description = indexString;

                    Person person = new Person();
                    person.PersonName = indexString;
                    person.Description = indexString;
                    company.Person = person;
                    person.Company = company;
    
                    if(companyFirst == null) {
                        companyFirst = company;
                    }
    
                    companyFirst.Collection.Add(person);
                    dbContextConnectionClass.Company.Add(company);
                    dbContextConnectionClass.Persons.Add(person);
                }
                dbContextConnectionClass.SaveChanges();
            }
        }
        public static void InitializeContextWithNavigationPropertiesAndCollections() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                Company companyFirst = null;
                Company companySecond = null;
                for(int i = 1; i < 4; i++) {
                    string indexString = i.ToString();
                    
                    Company company = new Company();
                    company.CompanyName = indexString;
                    company.Description = indexString;

                    Person person = new Person();
                    person.PersonName = indexString;
                    person.Description = indexString;

                    // company.Person = person;
                    // person.Company = company;

                    if(companySecond == null && companyFirst != null) {
                        companySecond = company;
                    }
                    if(companyFirst == null) {
                        companyFirst = company;
                    }

                    companyFirst.Collection.Add(person);
                    if(companySecond != null)
                        companySecond.Collection.Add(person);

                    dbContextConnectionClass.Company.Add(company);
                    dbContextConnectionClass.Persons.Add(person);
                }
                dbContextConnectionClass.SaveChanges();
            }
        }
        public static Expression<Func<DbContextConnectionClass, Company, bool>> CompanyTrue {
            get {
                return (db, company) => true;
            }
        }
        public static Expression<Func<DbContextConnectionClass, Company, bool>> CompanyNameEqualsOne {
            get {
                return (db, company) => company.CompanyName == "1";
            }
        }
        public static Expression<Func<DbContextConnectionClass, Company, bool>> CompanyNameEqualsTwo {
            get {
                return (db, company) => company.CompanyName == "2";
            }
        }
        public static Expression<Func<DbContextConnectionClass, Person, bool>> PersonTrue {
            get {
                return (db, person) => true;
            }
        }
        public static Expression<Func<DbContextConnectionClass, Person, bool>> PersonNameEqualsOne {
            get {
                return (db, person) => person.PersonName == "1";
            }
        }
        public static Expression<Func<DbContextConnectionClass, Person, bool>> PersonNameEqualsTwo {
            get {
                return (db, person) => person.PersonName == "2";
            }
        }
    }
}

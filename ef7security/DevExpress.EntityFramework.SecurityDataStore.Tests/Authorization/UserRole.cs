using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Authorization {
    [TestFixture]
    public class UserRoleTests {
        [SetUp]
        public void SetUp() {
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                context.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                context.Database.EnsureCreated();
            }
        }
        // TODO: where is assert?
        [Test]
        public void SaveObjectCriteria() {
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                UserRole userRole = new UserRole();
                context.Add(userRole);
                SecurityUser user = new SecurityUser();
                userRole.User = user;             
                SecurityRole role = new SecurityRole();
                userRole.Role = role;
                SecurityMemberPermission securityMemberPermission = new SecurityMemberPermission();
                securityMemberPermission.MemberName = "Name";
                securityMemberPermission.Type = typeof(SecurityUser);
                Expression<Func<TestDbContextWithUsers, SecurityUser, bool>> criteria = (s, t) => t.Name == "1";
                securityMemberPermission.Criteria = criteria;
                Expression exp = securityMemberPermission.Criteria;
                securityMemberPermission.Operations = SecurityOperation.Read;
                securityMemberPermission.OperationState = OperationState.Deny;
                role.MemberPermissions.Add(securityMemberPermission);
                context.SaveChanges();
            }
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                var first = context.Roles.Include(p=>p.MemberPermissions).First();
            }
        }
    }
}

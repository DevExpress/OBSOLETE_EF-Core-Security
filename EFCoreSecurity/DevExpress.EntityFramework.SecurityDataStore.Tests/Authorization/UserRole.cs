using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Authorization {
    [TestFixture]
    public abstract class UserRoleTests {
        [SetUp]
        public void ClearDatabase() {
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                context.ResetDatabase();
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

    [TestFixture]
    public class InMemoryBaseSecurityObjectsTests : UserRoleTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
            base.ClearDatabase();
        }
    }

    [TestFixture]
    public class LocalDb2012UserRoleTests : UserRoleTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
            base.ClearDatabase();
        }
    }
}

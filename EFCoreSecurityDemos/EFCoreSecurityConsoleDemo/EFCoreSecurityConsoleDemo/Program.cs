using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore;
// using ExpressionSerialization;
namespace EFCoreSecurityConsoleDemo {
    class Program {
        static int GetSomeNumber() {
            return 3;
        }
        static void Main(string[] args) {
            EFSimpleDB efSimpleDB = CreateDataBase();

            Role role1 = new Role { RoleName = "a" };
            // User role2 = new User { Age = 2 };
            // User role3 = new User { Age = 3 };

            User user1 = new User { Age = 1, UserName = "aaa" };
            User user2 = new User { Age = 2, UserName = "bbb" };
            User user3 = new User { Age = 3, UserName = "ccc" };
            efSimpleDB.Add(user1);
            efSimpleDB.Add(user2);
            efSimpleDB.Add(user3);

            efSimpleDB.Add(role1);
            efSimpleDB.SaveChanges();

            // Console.WriteLine("Before permissions {0}" , efSimpleDB.Users.Where(p=> p.Age < efSimpleDB.Users.Count()).Count());

            Expression<Func<EFSimpleDB, User, bool>> criteria = (db, obj) => obj.Age < db.Users.Count();
            // Expression<Func<User, bool>> criteria = (obj) => obj.Age < GetSomeNumber();

            // Expression<Func<User, bool>> criteria = (obj) => obj.Age < efSimpleDB.Users.Count();

            // ExpressionSerializer expressionSerializer = new ExpressionSerializer();
            // var xml = expressionSerializer.Serialize(criteria);
            // var deserializedCriteria = expressionSerializer.Deserialize(xml);

            // efSimpleDB.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            // efSimpleDB.Security.AddObjectPermission<User>(SecurityOperation.Read, OperationState.Allow, obj => obj.Age < 3);
            // efSimpleDB.Security.AddObjectPermission<User>(SecurityOperation.Read, OperationState.Deny, obj => obj.Age == 1);
            // efSimpleDB.Security.AddObjectPermission<User>(SecurityOperation.Read, OperationState.Deny, obj => obj.Age < efSimpleDB.Users.Count());

            // efSimpleDB.Security.AddObjectPermission<EFSimpleDB, User>(SecurityOperation.Read, OperationState.Allow, (db, obj) => obj.Age < db.Users.Count());
            efSimpleDB.Security.AddObjectPermission<EFSimpleDB, User>(SecurityOperation.Read, OperationState.Allow, (db, obj) => obj.UserName.Contains(db.Roles.First().RoleName));
            // efSimpleDB.Security.AddObjectPermission<EFSimpleDB, User>(SecurityOperation.Read, OperationState.Allow, (db, obj) => obj.Age < 3);

            bool granted1 = efSimpleDB.Security.IsGranted(typeof(User), SecurityOperation.Read, user1);
            bool granted2 = efSimpleDB.Security.IsGranted(typeof(User), SecurityOperation.Read, user2);
            bool granted3 = efSimpleDB.Security.IsGranted(typeof(User), SecurityOperation.Read, user3);

            var blockedMembers = efSimpleDB.Entry(user1).GetBlockedMembers();

            Console.WriteLine("After permissions {0}", efSimpleDB.Users.Count());
            Console.WriteLine("Not granted: {0}", efSimpleDB.Security.IsGranted(typeof(User), SecurityOperation.Read, user1));
            Console.WriteLine("Granted: {0}", efSimpleDB.Security.IsGranted(typeof(User), SecurityOperation.Read, user2));

            Console.ReadLine();

            #region type checks
            /*
            //try {
            User u1 = new User { Age = 1 };
            ef7_SimpleDB.Add(u1);
            ef7_SimpleDB.SaveChanges();

            EF7_SimpleDB ef7_SimpleDB2 = CreateDataBase();
            User u2 = ef7_SimpleDB2.Users.First();
            u2.Age = 2;
            ef7_SimpleDB2.SaveChanges();

            EF7_SimpleDB ef7_SimpleDB3 = CreateDataBase();
            User u3 = ef7_SimpleDB2.Users.First();
            ef7_SimpleDB2.SaveChanges();

           

            Console.WriteLine("Allow type");
            ef7_SimpleDB.Security.SetTypePermission<User>(SecurityOperation.Create, OperationState.Allow);

            CreateUser(ef7_SimpleDB);
            Console.WriteLine("Object saved"); // No error

            var ss = ef7_SimpleDB.Users.First().UserName;

            Console.WriteLine("Deny type");
            ef7_SimpleDB.Security.SetTypePermission<User>(SecurityOperation.Create, OperationState.Deny);

            CreateUser(ef7_SimpleDB);
            Console.WriteLine("Object saved"); // Error!!!

            Console.ReadLine();

            ////}
            ////catch(Exception e) {
            ////    Console.WriteLine("Error: {0}", e.Message);
            ////}

            */
            #endregion
        }

        private static void CreateUser(EFSimpleDB ef7_SimpleDB) {
            User grantedUser = new User();
            ef7_SimpleDB.Add(grantedUser);
            ef7_SimpleDB.SaveChanges();
        }

        private static EFSimpleDB CreateDataBase() {
            EFSimpleDB ef7_SimpleDB = new EFSimpleDB();
            Console.WriteLine("Create DataBase");
            ef7_SimpleDB.Database.EnsureCreated();
            return ef7_SimpleDB;
        }
    }
}

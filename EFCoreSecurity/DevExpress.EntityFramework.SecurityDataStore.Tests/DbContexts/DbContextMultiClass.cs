using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public interface IDbContextMultiClass {
        DbSet<DbContextObject1> dbContextDbSet1 { get; set; }
        DbSet<DbContextObject2> dbContextDbSet2 { get; set; }
        DbSet<DbContextObject3> dbContextDbSet3 { get; set; }
        DbSet<DbContextObject4> dbContextDbSet4 { get; set; }
        DbSet<DbContextBaseSecurityObject> dbContextBaseSecurityObjectDbSet { get; set; }
        DbSet<DbContextISecurityEntityObject> dbContextISecurityEntityDbSet { get; set; }
    }

    public class DbContextMultiClass : SecurityDbContext, IDbContextMultiClass {
        public DbSet<DbContextObject1> dbContextDbSet1 { get; set; }
        public DbSet<DbContextObject2> dbContextDbSet2 { get; set; }
        public DbSet<DbContextObject3> dbContextDbSet3 { get; set; }
        public DbSet<DbContextObject4> dbContextDbSet4 { get; set; }
        public DbSet<DbContextBaseSecurityObject> dbContextBaseSecurityObjectDbSet { get; set; }
        public DbSet<DbContextISecurityEntityObject> dbContextISecurityEntityDbSet { get; set; }

        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            SecurityTestHelper.ConfigureOptionsBuilder(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DbContextObject1>(p => {
                p.Property(b => b.ID).ValueGeneratedOnAdd();
            });
           
        }
    }
    public class NativeDbContextMultiClass : DbContext, IDbContextMultiClass {
        public DbSet<DbContextObject1> dbContextDbSet1 { get; set; }
        public DbSet<DbContextObject2> dbContextDbSet2 { get; set; }
        public DbSet<DbContextObject3> dbContextDbSet3 { get; set; }
        public DbSet<DbContextObject4> dbContextDbSet4 { get; set; }
        public DbSet<DbContextBaseSecurityObject> dbContextBaseSecurityObjectDbSet { get; set; }
        public DbSet<DbContextISecurityEntityObject> dbContextISecurityEntityDbSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            SecurityTestHelper.ConfigureOptionsBuilder(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DbContextObject1>(p => {
                p.Property(b => b.ID).ValueGeneratedOnAdd();
            });

        }
    }
    public class DbContextDbSetKeyIsGuid : SecurityDbContext {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            SecurityTestHelper.ConfigureOptionsBuilder(optionsBuilder);
        }    
        public DbSet<DbContextObjectKeyIsGuid> DbSet1 { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder) {
        //    modelBuilder.Entity<DbContextObject_KeyIsGuid>(p => {
        //        p.Property(e => e.ID).GenerateValueOnAdd(true);
        //    });
        //}
    }
    public class DbContextObjectKeyIsGuid {
        public Guid Id { get; set; }
    }

    public class DbContextObject1 : IDisposable {
        public DbContextObject1() {
            Count++;
        }
        public int ID { get; set; }
        public int ItemCount { get; set; }
        public int? ItemCountNull { get; set; }
        public decimal DecimalItem { get; set; }
        public decimal? DecimalItemNull { get; set; }
        public long LongItem { get; set; }
        public long? LongItemNull { get; set; }
        public double DoubleItem { get; set; }
        public double? DoubleItemNull { get; set; }
        public float FloatItem { get; set; }
        public float? FloatItemNull { get; set; }
        public bool Flag { get; set; }
        public string ItemName { get; set; }
        public int UseID { get; set; }
        public string Description { get; set; }
        public static int Count { get; set; }
        public List<string> Notes = new List<string>();

        public void Dispose() {
            // throw new NotImplementedException();
            Count--;
        }
    }
    public class DbContextObject2 {
        public DbContextObject2() {
            Count++;
        }
        public static int Count { get; set; }
        public Guid ID { get; set; }
        public int UserID { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public bool BoolProperty { get; set; }
    }
    public class DbContextObject3 {
        public int ID { get; private set; }
        public string Notes { get; set; }
    }
    public class DbContextObject4 {
        public int ID { get; set; }
        public int ItemCount { get; set; }
    }
    public class DbContextBaseSecurityObject : BaseSecurityEntity {
        public int ID { get; set; }
        public string Description { get; set; }
        public int ItemCount { get; set; }
        public decimal DecimalItem { get; set; }
    }
    public class DbContextISecurityEntityObject : ISecurityEntity {
        public int ID { get; set; }
        public string Description { get; set; }
        public int ItemCount { get; set; }
        public decimal DecimalItem { get; set; }
        [NotMapped]
        public IEnumerable<string> BlockedMembers { get; set; }
    }
}

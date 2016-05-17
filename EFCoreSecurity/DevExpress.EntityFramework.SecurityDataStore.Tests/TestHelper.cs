using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests {
    public static class TestHelper {
        public static TSource MakeRealDbContext<TSource>(this TSource securityDbContext) where TSource : SecurityDbContext {
            FieldInfo propertyInfo = securityDbContext.GetType().GetRuntimeFields().First(p => p.Name == "useRealProvider");
            propertyInfo.SetValue(securityDbContext, true);
            return securityDbContext;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Utility {
    public class ConverterHelper {
        public static Type TypeToIQueryableType(Type objectType) {
            return typeof(IQueryable<>).MakeGenericType(objectType);
        }
        public static Type TypeToIEnumerableType(Type objectType) {
            return typeof(IEnumerable<>).MakeGenericType(objectType);
        }
        public static IEnumerable<TResult> ResultToIEnumerableResult<TResult>(object resultQuery) {
            if(resultQuery is IEnumerable<TResult>) {
                IEnumerable<TResult> resultQueryIEnumerable = (IEnumerable<TResult>)resultQuery;
                foreach(var obj in resultQueryIEnumerable) {
                    yield return obj;
                }
            }
            else {
                yield return (TResult)resultQuery;
            }
        }
    }
}

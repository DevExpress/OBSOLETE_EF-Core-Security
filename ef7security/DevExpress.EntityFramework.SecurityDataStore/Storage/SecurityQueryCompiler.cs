using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DevExpress.EntityFramework.SecurityDataStore.Storage {
    public class SecurityQueryCompiler : IQueryCompiler {
        SecurityDbContext securityDbContext;
        public SecurityQueryCompiler(SecurityDbContext securityDbContext) {
            this.securityDbContext = securityDbContext;
        }
        public TResult Execute<TResult>(Expression query) {
            TResult result;
            MethodCallExpression methodCallExpression = query as MethodCallExpression;
            if(methodCallExpression != null && methodCallExpression.Arguments.Count == 1) {
                ConstantExpression constantExpression = methodCallExpression.Arguments[0] as ConstantExpression;
                Type typeDbSet = constantExpression.Type.GetGenericArguments()[0];
                ConstantExpression newArg = Expression.Constant(securityDbContext.Set(typeDbSet));
                methodCallExpression = methodCallExpression.Update(null, new[] { newArg });
                var expL = Expression.Lambda<Func<TResult>>(methodCallExpression);
                var r = expL.Compile();
                result = r.Invoke();
            }
            else {
                throw new NotSupportedException();
            }
            return result;
        }

        public System.Collections.Generic.IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression query) {
            throw new NotImplementedException();
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}

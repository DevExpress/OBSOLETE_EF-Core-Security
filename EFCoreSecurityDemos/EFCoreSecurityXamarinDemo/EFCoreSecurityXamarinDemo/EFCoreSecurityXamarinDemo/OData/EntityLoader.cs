using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Simple.OData.Client;

namespace EFCoreSecurityXamarinDemo.OData {
    public static class EntityLoader {
        public static async Task<IEnumerable<T>> LoadEntities<T>(string userName, string serviceName) where T : class {
            V4Adapter.Reference();

            string password = userName;

            var odataClient = new ODataClient(
                    new ODataClientSettings("http://efcoresecurityodataservicedemo.azurewebsites.net/",
                    new NetworkCredential(userName, password)));

            var command = odataClient.For<T>(serviceName);

            return await command.FindEntriesAsync();
        }
    }
}

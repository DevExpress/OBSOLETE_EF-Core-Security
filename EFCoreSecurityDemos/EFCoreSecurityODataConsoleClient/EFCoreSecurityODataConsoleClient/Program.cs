using EFCoreSecurityConsoleClient.Default;
using EFCoreSecurityConsoleClient.EFCoreSecurityODataService.DataModel;
using Microsoft.OData.Client;
using System;
using System.Linq;
using System.Net;

namespace EFCoreSecurityConsoleClient {
    class Program {
        static void Main(string[] args) {
            //Uri serviceUri = new Uri("http://efcoresecurityodataservicedemo.azurewebsites.net/");
            Uri serviceUri = new Uri("http://localhost:54343/");
            Container container = new Container(serviceUri);
            container.MergeOption = MergeOption.OverwriteChanges;
            string enterResult = "";
            while(enterResult != "Exit") {
                Console.WriteLine("Choose an action: ");
                Console.WriteLine("1. LogIn");
                Console.WriteLine("2. Exit");
                switch(Console.ReadLine()) {
                    case "1":
                        Logon(container);
                        break;
                    case "2":
                        enterResult = "Exit";
                        break;
                    default:
                        break;
                }
            }
        }
        private static void Logon(Container container) {
            Console.WriteLine("Username: ");
            string userName = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            string password = Console.ReadLine();
            container.Credentials = new NetworkCredential(userName, password);
            try {
                ViewAllContacts(container);
            }
            catch(DataServiceQueryException exception) {
                if(exception.Response.StatusCode == 401 && exception.InnerException.Message.Contains("401.3 - Logon failed")) {
                    Console.WriteLine("Logon failed.");
                }
                if(exception.Response.StatusCode == 401 && exception.InnerException.Message.Contains("401.1 - Please provide Authorization headers with your request")) {
                    Console.WriteLine("Please provide Authorization headers with your request");
                }
            }
        }
        private static void ViewAllContacts(Container container) {
            int i = 1;
            foreach(Contact contact in container.Contacts) {
                if(i == 1) {
                    Console.WriteLine("Contacts:");
                }
                Console.WriteLine("\n{0}. Name: {1}\nAddress: {2}", i, contact.Name, contact.Address);
                i++;
            }
        }
    }
}

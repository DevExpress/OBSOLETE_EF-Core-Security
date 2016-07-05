using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using EFCoreSecurityXamarinDemo.ListViewPages;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo {
    public class ListViewMasterDetailPage : MasterDetailPage {
        string userName;
        public ListViewMasterDetailPage(string userName) {
            this.userName = userName;
            var menuPage = new MenuPage();

            menuPage.Menu.ItemSelected += NavigateTo;

            Master = menuPage;


            //Master = new ContentPage {
            //    Title = "Entities:",
            //    Icon = Device.OS == TargetPlatform.iOS ? "menu.png" : null,
            //    Content = new StackLayout {
            //        Children = {
            //           new Button { Text = "Contacts" },
            //           new Button { Text = "Departments" },
            //           new Button { Text = "Tasks" },
            //        }
            //    }
            //};

            Detail = new ContactsListViewPage(userName);
        }

        private void NavigateTo(object sender, SelectedItemChangedEventArgs e) {
            MenuItem menuItem = e.SelectedItem as MenuItem;

            ContentPage listViewPage;

            switch(menuItem.Text) {
                case "Contacts":
                    listViewPage = new ContactsListViewPage(userName);
                    break;
                case "Departments":
                    listViewPage = new DepartmentsListViewPage(userName);
                    break;
                case "Tasks":
                    listViewPage = new TasksListViewPage(userName);
                    break;
                default:
                    listViewPage = new ContactsListViewPage(userName);
                    break;
            }

            Detail = listViewPage;
            IsPresented = false;
        }
    }
}

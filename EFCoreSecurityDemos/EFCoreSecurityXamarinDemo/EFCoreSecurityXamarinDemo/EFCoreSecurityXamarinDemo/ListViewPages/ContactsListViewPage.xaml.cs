using System;
using EFCoreSecurityXamarinDemo.Entities;
using EFCoreSecurityXamarinDemo.OData;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.ListViewPages {
    public partial class ContactsListViewPage : ContentPage {
        public ContactsListViewPage(String userName) {
            InitializeComponent();

            new Action(async () => {
                var contacts = await EntityLoader.LoadEntities<Contact>(userName, "Contacts");
                listView.ItemsSource = contacts;
            }).Invoke();
        }
    }
}

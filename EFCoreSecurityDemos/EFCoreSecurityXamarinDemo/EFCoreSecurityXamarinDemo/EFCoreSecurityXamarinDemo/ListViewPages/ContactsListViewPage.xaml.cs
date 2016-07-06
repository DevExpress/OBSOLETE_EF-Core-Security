using System;
using System.Collections;
using System.Collections.Generic;
using EFCoreSecurityXamarinDemo.DetailViewPages;
using EFCoreSecurityXamarinDemo.Entities;
using EFCoreSecurityXamarinDemo.OData;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.ListViewPages {
    public partial class ContactsListViewPage : ContentPage {
        public ContactsListViewPage(String userName) {
            InitializeComponent();

            listView.ItemSelected += ContactSelected;

            new Action(async () => {
                var contacts = await EntityLoader.LoadEntities<Contact>(userName, "Contacts");

                List<ContactViewModel> contactViewModels = new List<ContactViewModel>();
                foreach(Contact contact in contacts)
                    contactViewModels.Add(new ContactViewModel(contact));

                listView.ItemsSource = contactViewModels;
            }).Invoke();
        }

        private async void ContactSelected(object sender, SelectedItemChangedEventArgs e) {
            ContactViewModel contactViewModel = e.SelectedItem as ContactViewModel;
            if(contactViewModel == null)
                return;

            ((ListView)sender).SelectedItem = null;
            await Navigation.PushAsync(new ContactsDetailViewPage(contactViewModel));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCoreSecurityXamarinDemo.Entities;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.DetailViewPages {
    public partial class ContactsDetailViewPage : ContentPage {
        public ContactsDetailViewPage(ContactViewModel contact) {
            InitializeComponent();

            BindingContext = contact;
        }
    }
}

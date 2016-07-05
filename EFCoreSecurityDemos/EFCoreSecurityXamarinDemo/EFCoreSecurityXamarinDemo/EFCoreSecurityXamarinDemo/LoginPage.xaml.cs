using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo {
    public partial class LoginPage : ContentPage {
        public LoginPage() {
            InitializeComponent();
        }
        async void OnLoginButtonClicked(object sender, EventArgs args) {
            string userName = loginPicker.Items[loginPicker.SelectedIndex];
            await Navigation.PushAsync(new ListViewMasterDetailPage(userName));
        }
    }
}

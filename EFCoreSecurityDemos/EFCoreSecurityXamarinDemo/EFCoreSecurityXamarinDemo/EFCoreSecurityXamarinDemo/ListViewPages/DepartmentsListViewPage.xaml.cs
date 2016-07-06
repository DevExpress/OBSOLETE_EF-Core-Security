using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCoreSecurityXamarinDemo.DetailViewPages;
using EFCoreSecurityXamarinDemo.Entities;
using EFCoreSecurityXamarinDemo.OData;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.ListViewPages {
    public partial class DepartmentsListViewPage : ContentPage {
        public DepartmentsListViewPage(string userName) {
            InitializeComponent();

            listView.ItemSelected += DepartmentSelected;

            new Action(async () => {
                var departments = await EntityLoader.LoadEntities<Department>(userName, "Departments");

                List<DepartmentViewModel> departmentViewModels = new List<DepartmentViewModel>();
                foreach(Department department in departments)
                    departmentViewModels.Add(new DepartmentViewModel(department));

                listView.ItemsSource = departmentViewModels;
            }).Invoke();
        }

        private async void DepartmentSelected(object sender, SelectedItemChangedEventArgs e) {
            DepartmentViewModel departmentViewModel = e.SelectedItem as DepartmentViewModel;
            if(departmentViewModel == null)
                return;

            ((ListView)sender).SelectedItem = null;
            await Navigation.PushAsync(new DepartmentsDetailViewPage(departmentViewModel));
        }
    }
}

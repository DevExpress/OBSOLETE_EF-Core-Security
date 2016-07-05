using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCoreSecurityXamarinDemo.Entities;
using EFCoreSecurityXamarinDemo.OData;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.ListViewPages {
    public partial class DepartmentsListViewPage : ContentPage {
        public DepartmentsListViewPage(string userName) {
            InitializeComponent();

            new Action(async () => {
                var departments = await EntityLoader.LoadEntities<Department>(userName, "Departments");
                listView.ItemsSource = departments;
            }).Invoke();
        }
    }
}

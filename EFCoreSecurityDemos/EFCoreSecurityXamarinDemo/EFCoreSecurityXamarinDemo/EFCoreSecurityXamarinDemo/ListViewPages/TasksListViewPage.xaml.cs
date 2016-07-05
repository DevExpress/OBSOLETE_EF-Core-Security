using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCoreSecurityXamarinDemo.Entities;
using EFCoreSecurityXamarinDemo.OData;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.ListViewPages {
    public partial class TasksListViewPage : ContentPage {
        public TasksListViewPage(string userName) {
            InitializeComponent();

            new Action(async () => {
                var tasks = await EntityLoader.LoadEntities<EFCoreSecurityXamarinDemo.Entities.Task>(userName, "Tasks");
                listView.ItemsSource = tasks;
            }).Invoke();
        }
    }
}

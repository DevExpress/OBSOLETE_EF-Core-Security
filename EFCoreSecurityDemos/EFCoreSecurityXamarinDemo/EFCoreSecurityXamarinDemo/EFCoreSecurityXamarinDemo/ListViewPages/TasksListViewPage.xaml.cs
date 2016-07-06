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
    public partial class TasksListViewPage : ContentPage {
        public TasksListViewPage(string userName) {
            InitializeComponent();

            listView.ItemSelected += TaskSelected;

            new Action(async () => {
                var tasks = await EntityLoader.LoadEntities<Entities.Task>(userName, "Tasks");

                List<TaskViewModel> taskViewModels = new List<TaskViewModel>();
                foreach(Entities.Task task in tasks)
                    taskViewModels.Add(new TaskViewModel(task));

                listView.ItemsSource = taskViewModels;
            }).Invoke();
        }

        private async void TaskSelected(object sender, SelectedItemChangedEventArgs e) {
            TaskViewModel taskViewModel = e.SelectedItem as TaskViewModel;
            if(taskViewModel == null)
                return;

            ((ListView)sender).SelectedItem = null;
            await Navigation.PushAsync(new TasksDetailViewPage(taskViewModel));
        }
    }
}

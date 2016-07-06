using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.Entities {
    public class Task : BaseSecurityEntity {
        public string Description { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DateCompleted { get; set; }
        public int PercentCompleted { get; set; }
    }

    public class TaskViewModel : BaseViewModelEntity {
        Task task;
        public string Description {
            get {
                return GetPropertyText("Description", task.Description);
            }
        }
        public Color DescriptionTextColor {
            get {
                return GetPropertyTextColor("Description");
            }
        }
        public string Note {
            get {
                return GetPropertyText("Note", task.Note);
            }
        }
        public Color NoteTextColor {
            get {
                return GetPropertyTextColor("Note");
            }
        }
        public string StartDate {
            get {
                return GetPropertyText("StartDate", task.StartDate.ToString());
            }
        }
        public Color StartDateTextColor {
            get {
                return GetPropertyTextColor("StartDate");
            }
        }
        public string DateCompleted {
            get {
                return GetPropertyText("DateCompleted", task.DateCompleted.ToString());
            }
        }
        public Color DateCompletedTextColor {
            get {
                return GetPropertyTextColor("DateCompleted");
            }
        }
        public string PercentCompleted {
            get {
                return GetPropertyText("PercentCompleted", task.PercentCompleted.ToString());
            }
        }
        public Color PercentCompletedTextColor {
            get {
                return GetPropertyTextColor("PercentCompleted");
            }
        }

        public TaskViewModel(Task task) : base(task) {
            this.task = task;
        }
    }
}

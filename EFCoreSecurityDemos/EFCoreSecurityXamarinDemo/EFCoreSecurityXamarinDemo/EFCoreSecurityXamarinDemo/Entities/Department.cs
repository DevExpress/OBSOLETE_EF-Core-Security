using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.Entities {
    public class Department : BaseSecurityEntity {
        public string Title { get; set; }
        public string Office { get; set; }
    }

    public class DepartmentViewModel : BaseViewModelEntity {
        Department department;
        public string Title {
            get {
                return GetPropertyText("Title", department.Title);
            }
        }
        public Color TitleTextColor {
            get {
                return GetPropertyTextColor("Description");
            }
        }
        public string Office {
            get {
                return GetPropertyText("Office", department.Office);
            }
        }
        public Color OfficeTextColor {
            get {
                return GetPropertyTextColor("Office");
            }
        }

        public DepartmentViewModel(Department department) : base(department) {
            this.department = department;
        }
    }
}

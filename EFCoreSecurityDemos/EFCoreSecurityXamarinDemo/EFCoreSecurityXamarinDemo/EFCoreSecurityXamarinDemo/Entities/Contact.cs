using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.Entities {
    public class Contact : BaseSecurityEntity {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Department { get; set; }
    }

    public class ContactViewModel : BaseViewModelEntity {
        Contact contact;
        public string Name {
            get {
                return GetPropertyText("Name", contact.Name);
            }
        }
        public Color NameTextColor {
            get {
                return GetPropertyTextColor("Name");
            }
        }
        public string Address {
            get {
                return GetPropertyText("Address", contact.Address);
            }
        }
        public Color AddressTextColor {
            get {
                return GetPropertyTextColor("Address");
            }
        }
        public string Department {
            get {
                return GetPropertyText("Department", contact.Department);
            }
        }
        public Color DepartmentTextColor {
            get {
                return GetPropertyTextColor("Department");
            }
        }

        public ContactViewModel(Contact contact) : base(contact) {
            this.contact = contact;
        }        
    }
}

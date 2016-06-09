using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;
using System.Linq;

namespace EFCoreSecurityConsoleDemo.DataModel {
    public class Contact : BaseSecurityEntity {
        private string name;
        private string address;

        public int Id { get; set; }
        public string Name {
            get {
                return GetValue(name, "Name");
            }
            set {
                name = value;
            }
        }
        public string Address {
            get {
                return GetValue(address, "Address");
            }
            set {
                address = value;
            }
        }
        private string GetValue(string propertyValue, string propertyName) {
            if(propertyValue == null && BlockedMembers.Contains(propertyName)) {
                return "Protected Content";
            }
            else {
                return propertyValue;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;
using DevExpress.EntityFramework.SecurityDataStore;

namespace EFCoreSecurityODataService.DataModel {
    public class Contact : BaseSecurityEntity {
        private string name;
        private string address;

        public int Id { get; set; }
        public string Name {
            get {
                return this.GetValue(name, "Name");
            }
            set {
                name = value;
            }
        }
        public string Address {
            get {
                return this.GetValue(address, "Address");
            }
            set {
                address = value;
            }
        }
    }
}

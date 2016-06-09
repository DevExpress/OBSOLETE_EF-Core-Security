using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;

namespace EFCoreSecurityConsoleDemo.DataModel {
    public class Contact : BaseSecurityEntity {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}

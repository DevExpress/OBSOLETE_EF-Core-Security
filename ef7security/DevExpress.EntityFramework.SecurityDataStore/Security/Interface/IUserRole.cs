namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface IUserRole {
        ISecurityUser User { get; set; }       
        ISecurityRole Role { get; set; }
    }
}
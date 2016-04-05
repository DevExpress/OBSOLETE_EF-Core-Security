namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public interface ISecurityServicesProvider {
        ISecurityObjectsBuilder SecurityProcessLoadObjects { get; }
        ISecuritySaveObjects SecuritySaveObjects { get; }
        ISecurityObjectRepository SecurityObjectRepository { get; }
        IModificationСriterionService ModificationСriterionService { get; }
        IPermissionProcessor PermissionProcessor { get; }
    }
}
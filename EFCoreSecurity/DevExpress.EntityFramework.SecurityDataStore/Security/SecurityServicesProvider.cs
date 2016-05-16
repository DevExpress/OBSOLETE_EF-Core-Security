//using DevExpress.EntityFramework.SecurityDataStore.Security.Services;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DevExpress.EntityFramework.SecurityDataStore.Security {
//    public class SecurityServicesProvider : ISecurityServicesProvider {
//        public virtual IPermissionProcessor PermissionProcessor { get; }
//        public virtual IModificationСriterionService ModificationСriterionService { get; }
//        public virtual ISecurityObjectRepository SecurityObjectRepository { get; }
//        public virtual ISecurityObjectsBuilder SecurityProcessLoadObjects { get; }
//        public virtual ISecuritySaveObjects SecuritySaveObjects { get; }
        
//        public SecurityServicesProvider(SecurityDbContext securityDbContext, IEnumerable<IPermission> securityPremissions) {
//            PermissionProcessor = new PermissionProcessor(securityPremissions, securityDbContext);
//            ModificationСriterionService = new ModificationСriterionService(PermissionProcessor, securityPremissions, securityDbContext.realDbContext);
//            SecurityObjectRepository = new SecurityObjectRepository();
//            SecurityProcessLoadObjects = new SecurityProcessLoadObjects(securityDbContext, SecurityObjectRepository, PermissionProcessor);
//            SecuritySaveObjects = new SecuritySaveObjectsService(securityDbContext, SecurityObjectRepository);
//        }
//    }
//}

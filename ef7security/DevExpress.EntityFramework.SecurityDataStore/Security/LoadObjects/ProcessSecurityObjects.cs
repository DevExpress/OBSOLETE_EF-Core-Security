using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security.LoadObjects {
    public class FillSecurityObjects : ISecurityInformationFiller {
        protected IPermissionProcessor processor;
        protected IModel model;
        public virtual void FillSecurityInformation(IEnumerable<SecurityObjectBuilder> objectBuilders) {
            foreach(SecurityObjectBuilder objectBuilder in objectBuilders) {
                FillObjects(objectBuilder);
            }
        }
        private void FillObjects(SecurityObjectBuilder securityObjectBuilder) {
            ISecurityObject securityObject = securityObjectBuilder.SecurityObject as ISecurityObject;
            if(securityObject != null) {
                securityObject.BlockedMembers = securityObjectBuilder.GetBlockedMembers();
                securityObject.ReadOnlyMembers = processor.GetReadOnlyMembersString(securityObject.GetType());
                securityObject.ReadOnlyMembersOnLoad = GetReadOnlyMembersOnLoad(securityObjectBuilder);
            }
        }
        private string GetReadOnlyMembersOnLoad(SecurityObjectBuilder securityObjectBuilder) {
            List<string> denyMembers = new List<string>();
            IEntityType entityType = model.FindEntityType(securityObjectBuilder.SecurityObject.GetType());
            if(!entityType.Equals(null)) {
                var properties = entityType.GetProperties();
                var denyMembersProperty = GetDenyMembers(properties.Select(p => p.Name), entityType.ClrType, securityObjectBuilder.RealObject);
                var propertiesNavigation = entityType.GetNavigations();
                var denyMembersNavigationProperty = GetDenyMembers(propertiesNavigation.Select(p => p.Name), entityType.ClrType, securityObjectBuilder.RealObject);
                denyMembers.AddRange(denyMembersProperty);
                denyMembers.AddRange(denyMembersNavigationProperty);
            }
            return string.Join(";", denyMembers);
        }
        private IEnumerable<string> GetDenyMembers(IEnumerable<string> members, Type type, object realObject) {
            List<string> denyMembers = new List<string>();
            foreach(string member in members) {
                if(!IsMemberGranted(member, type, realObject)) {
                    denyMembers.Add(member);
                }
            }
            return denyMembers;
        }
        private bool IsMemberGranted(string member, Type type, object realObject) {
            return processor.IsGranted(type, SecurityOperation.Write, realObject, member);
        }
        public FillSecurityObjects(IPermissionProcessor processor, IModel model) {
            this.processor = processor;
            this.model = model;
        }
    }
}

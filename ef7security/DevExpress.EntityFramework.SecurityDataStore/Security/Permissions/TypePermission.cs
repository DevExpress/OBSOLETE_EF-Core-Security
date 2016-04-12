using JetBrains.Annotations;
using System;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class TypePermission : ITypePermission {
        public TypePermission([NotNull] Type type) {            
            Type = type;
        }
        public Type Type { get; set; }
        public OperationState OperationState { get; set; }
        public SecurityOperation Operations { get; set; }
        public override int GetHashCode() {
            return (Type.Name + Operations + Operations.ToString()).GetHashCode();
        }
    }
}
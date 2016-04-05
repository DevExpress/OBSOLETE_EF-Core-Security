using System;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface IPermissionProcessor {
        bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName);
    }
}
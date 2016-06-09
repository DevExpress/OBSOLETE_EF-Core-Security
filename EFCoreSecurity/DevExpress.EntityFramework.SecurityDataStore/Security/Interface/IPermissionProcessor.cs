using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface IPermissionProcessor {
        bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName);
        IEnumerable<string> GetReadOnlyMembers(Type type);
    }
}
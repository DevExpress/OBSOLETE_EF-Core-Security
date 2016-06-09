using System;

namespace DevExpress.EntityFramework.SecurityDataStore {
    [Flags]
    public enum SecurityOperation {
        NoAccess = 0, Read = 1, Write = 2, Create = 4, Delete = 8,
        FullAccess = 15, ReadWrite = 3
    }
}

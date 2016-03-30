using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    /*
    public static class SecurityOperation {
        public static string Read => "Read";
        public static string Write => "Write";
        public static string Create => "Create";
        public static string Delete  => "Delete";
        public static string FullAccess => "Read;Write;Create;Delete";
        public static string RadWrite => "Read;Write";
        public static string Delimiter => ";";
    }
    */

    [Flags]
    public enum SecurityOperation {
        NoAccess = 0, Read = 1, Write = 2, Create = 4, Delete = 8,
        FullAccess = 15, ReadWrite = 3
    }
}

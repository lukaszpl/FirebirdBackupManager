using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary.Models.Storage
{
    public class BackupStorageInfo
    {
        public int id;
        public BackupStorageInfo(int id, string name, string storageType, string? storagePath, string? user, string? password)
        {
            this.id = id;
            this.name = name;
            this.storagePath = storagePath;
            this.user = user;
            this.password = password;
            this.storageType = storageType;
        }
        public string storageType { get; set; }      
        public string? name {  get; set; }
        public string? storagePath { get; set; }
        public string? user {  get; set; }
        public string? password { get; set; }
    }
}

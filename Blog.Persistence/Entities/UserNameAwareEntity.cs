using System.Text;
using Force.Crc32;
using Microsoft.WindowsAzure.Storage.Table;

namespace Blog.Persistence.Entities
{
    public abstract class UserNameAwareEntity : TableEntity
    {
        private string _userName;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RowKey = _userName;
                PartitionKey = GetPartitionKey(_userName);
            }
        }

        public static string GetPartitionKey(string userName) =>
            (Crc32Algorithm.Compute(Encoding.UTF8.GetBytes(userName)) % 4).ToString();
    }
}
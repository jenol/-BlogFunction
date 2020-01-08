using System.Text;
using Force.Crc32;

namespace Blog.Persistence.Repositories
{
    internal static class PartitionHelper
    {
        public static string GetPartitionKey(string value, int partitionCount) =>
            (Crc32Algorithm.Compute(Encoding.UTF8.GetBytes(value)) % partitionCount).ToString();
    }
}
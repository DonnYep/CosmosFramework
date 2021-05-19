using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.CosmosEditor
{
    public class BuildInfo
    {
        public int Id;
        public string Type;
        public string ABName;
        public int ReferenceCount = 0;
        public string Hash;
        public List<string> DependList;
    }
}

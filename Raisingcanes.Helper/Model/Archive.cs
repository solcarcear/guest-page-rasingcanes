using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raisingcanes.Model
{
    public class Archive
    {
        public string EntityId { get; set; }

        public string Name { get; set; }

        public byte[] Content { get; set; }

        public int Size { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raisingcanes.Model
{
    public class Account
    {
        public string Id  { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string CategoryId { get; set; }

        public string CategoryOthers { get; set; }

        public string AboutThe { get; set; }

        public Address oAddress { get; set; }
    }
}

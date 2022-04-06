using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Raisingcanes.Model
{
          
      public class Contact
        {
            public string Id { get; set; }

            public string FullName { get; set; }

            public string PhoneNumber { get; set; }

            public string Email { get; set; }

            public string BestWayToContactId { get; set; }

            public Address oAddress { get; set; }

            public string AccountId { get; set; }
           public string PreferredRestaurant { get; set; }
    }
   
}


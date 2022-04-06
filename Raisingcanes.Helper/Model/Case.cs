using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raisingcanes.Model
{
    public class Case
    {
        public string Id { get; set; }

        public string AccountId { get; set; }

        public string ContactId { get; set; }

        public string Subject { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan TimeEvent { get; set; }

        public bool AnotherAddress { get; set; }

        public string EventAddressId { get; set; }

        public string PreferredRestaurant { get; set; }

        public bool SupportedThisEventInThePast { get; set; }

        public string SupportedThisEventInThePastHow { get; set; }

        public string PurposeOfTheEvent { get; set; }

        public string HowManyPeopletoAttendSupport { get; set; }

        public string IdeaRCToHelp { get; set; }

        public string AdditionalInfoRCShoudKnow { get; set; }

        public Address oAddress { get; set; }

        public Archive FileAdditional { get; set; }

        public Archive FileW9 { get; set; }

    }
}

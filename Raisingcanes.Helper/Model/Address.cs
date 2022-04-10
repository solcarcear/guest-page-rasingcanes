using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Raisingcanes.Model
{
    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string CityId { get; set; }

        public string StateId { get; set; }

        public string Zip { get; set; }

        public string RestaurantCode { get; set; }

        public string RestaurantNumber { get; set; }
        public string RestaurantName { get; set; }

        public string DisplayName => $"{RestaurantName} ({RestaurantNumber}), {StateId}, {CityId}, {Street}";


        public string Lat { get; set; }
        public string Lon { get; set; }
       



        private double distance(double lat1, double lon1, double lat2, double lon2)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist =
                    Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2))
                    + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                return dist;
            }
        }
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


    }
}

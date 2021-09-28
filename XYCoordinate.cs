using CoordinateSharp;
using System;

namespace RobotGPSTrajectory
{
    /*
     *  Coordinate representing polar coordinate and its approximation in the plane
     */

    class XYCoordinate : IPointXY
    {
        private Coordinate geoCoordinate;


        public XYCoordinate(double latitude, double longitude)
        {
            geoCoordinate = new Coordinate(latitude, longitude);
        }

        public XYCoordinate(Coordinate coordinate)
        {
            geoCoordinate = coordinate;
        }

        public Coordinate getGeoCoordinate()
        {
            return geoCoordinate;
        }

        public double getHaversianDistanceInMeters(XYCoordinate other)
        {
            var distance = new Distance(geoCoordinate, other.geoCoordinate).Meters;
            return geoCoordinate.Get_Distance_From_Coordinate(other.geoCoordinate).Meters;
        }

        public double GetX()
        {
            return geoCoordinate.UTM.Easting;
        }

        public double GetY()
        {
            return geoCoordinate.UTM.Northing;
        }

        public double GetTransformedX(IPointXY origin)
        {
            return GetX() - origin.GetX();
        }

        public double GetTransformedY(IPointXY origin)
        {
            return - GetY() + origin.GetY();
        }

        public void Print(XYCoordinate origin)
        { 
            Console.WriteLine("({1, 6}; {2, 6}) [{0, 20}]",
                getGeoCoordinate().ToString(),
                string.Format("{0:0.00}", GetTransformedX(origin)),
                string.Format("{0:0.00}", GetTransformedY(origin)));
        }
    }
}

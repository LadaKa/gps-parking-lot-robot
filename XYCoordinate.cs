using CoordinateSharp;
using System;
using System.Collections.Generic;

namespace RobotGPSTrajectory
{
    /*
     *  Coordinate representing polar coordinate and its approximation in the plane
     */

    class XYCoordinate : IPointXY
    {
        public enum Direction { N, E, S, W };
        private Coordinate geoCoordinate;
        private double probability = 0;

        public XYCoordinate(double latitude, double longitude)
        {
            geoCoordinate = new Coordinate(latitude, longitude);
        }

        public XYCoordinate(Coordinate coordinate)
        {
            geoCoordinate = coordinate;
        }

        public XYCoordinate shiftXYCoordinateByVerticalDistance(
            float distance,
            Direction direction)
        {
            var gridXYCoordinates = new List<XYCoordinate>();
            Coordinate shifted = null;
            double x_shifted = GetX();
            double y_shifted = GetY();
            string utm_shifted; 

            bool valid = false;

            switch (direction)
            {
                //  north coordinate 
                case Direction.N:
                    y_shifted = y_shifted + distance;
                    utm_shifted =               //  "34X 551586mE 8921410mN"
                        (geoCoordinate.UTM.LongZone
                        + geoCoordinate.UTM.LatZone
                        + " "
                        + ((int)GetX()) + "mE "
                        + ((int)y_shifted) + "mN").Replace(",", ".");
                    valid = Coordinate.TryParse(
                        utm_shifted, out shifted);  
                    break;

                //  east coordinate 
                case Direction.E:
                    x_shifted = x_shifted + distance;
                    utm_shifted =               
                        (geoCoordinate.UTM.LongZone
                        + geoCoordinate.UTM.LatZone
                        + " "
                        + ((int)x_shifted) + "mE "
                        + ((int)GetY()) + "mN").Replace(",", ".");
                    valid = Coordinate.TryParse(
                        utm_shifted, out shifted);
                    break;

                //  south coordinate
                case Direction.S:
                    y_shifted = y_shifted - distance;
                    utm_shifted =
                        (geoCoordinate.UTM.LongZone
                        + geoCoordinate.UTM.LatZone
                        + " "
                        + ((int)GetX()) + "mE "
                        + ((int)(y_shifted)) + "mN").Replace(",", ".");
                    valid = Coordinate.TryParse(
                        utm_shifted, out shifted);  
                    break;

                //  west coordinate 
                case Direction.W:
                    x_shifted = x_shifted - distance;
                    utm_shifted =
                        (geoCoordinate.UTM.LongZone
                        + geoCoordinate.UTM.LatZone
                        + " "
                        + ((int)x_shifted) + "mE "
                        + ((int)GetY()) + "mN").Replace(",", ".");
                    valid = Coordinate.TryParse(
                        utm_shifted, out shifted);
                    break;

                //  invalid input
                default:
                    break;
            }

            if (!valid)
                throw new Exception(); //!

            return new XYCoordinate(shifted);

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

        public double getProbability()
        {
            return probability;
        }

        public void setProbability(double value)
        {
            probability = value;
        }

    }
}

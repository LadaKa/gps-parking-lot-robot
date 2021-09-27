using CoordinateSharp;
using System.Collections.Generic;

namespace RobotGPSTrajectory
{
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

        public static List<XYCoordinate> getHaversianAverages(
            List<XYCoordinate> xyCoordinates, int setSize)
        {
            var xyAvgCoordinates = new List<XYCoordinate>();
            var xyAvgCoordinate = xyCoordinates[0];
            Coordinate avgCoordinate = null;

            xyAvgCoordinates.Add(xyAvgCoordinate);
            for (int i = 1; i < setSize; i++)
            {
                avgCoordinate = getHaversianAverage(
                        new List<Coordinate>() {
                            xyCoordinates[i - 1].geoCoordinate,
                            xyCoordinates[i].geoCoordinate,
                            xyCoordinates[i + 1].geoCoordinate});
                if (i < setSize/2)
                    xyAvgCoordinates.Add(new XYCoordinate(avgCoordinate));
            }
            for (int i = setSize; i < xyCoordinates.Count; i++)
            {
                avgCoordinate = getHaversianAverage(
                    avgCoordinate,
                    xyCoordinates[i - setSize].geoCoordinate,
                    xyCoordinates[i].geoCoordinate,
                    setSize);
                xyAvgCoordinates.Add(new XYCoordinate(avgCoordinate));
            }
            //  TODO: add last
            return xyAvgCoordinates; 
        }

        private static Coordinate getHaversianAverage(
            Coordinate oldAvgCoord, Coordinate oldCoord, Coordinate newCoord, int count )
        {
            double lat = 
                ((count * oldAvgCoord.Latitude.ToDouble()) 
                    - oldAvgCoord.Latitude.ToDouble() 
                    + newCoord.Latitude.ToDouble())
                /count;
            double lon =
                ((count * oldAvgCoord.Longitude.ToDouble())
                    - oldAvgCoord.Longitude.ToDouble()
                    + newCoord.Longitude.ToDouble())
                / count;
            return new Coordinate(lat, lon);
        }

        private static Coordinate getHaversianAverage(List<Coordinate> coordinates) 
        {
            double lat = 0;
            double lon = 0;
            int count = coordinates.Count;

            if (count == 0)
                return new Coordinate(lat, lon);

            coordinates.ForEach(
                coord =>
                {
                    lat = lat + coord.Latitude.ToDouble();
                    lon = lon + coord.Longitude.ToDouble();
                });
            return new Coordinate(lat/count, lon/count);
        }

        public double GetX()
        {
            return geoCoordinate.UTM.Easting;
        }

        public double GetY()
        {
            return geoCoordinate.UTM.Northing;
        }
    }
}

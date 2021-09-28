using CoordinateSharp;
using System.Collections.Generic;
using System.Linq;

namespace RobotGPSTrajectory
{
    /*
     *  Moving average of polar coordinates (with given size of moving set).
     */
    class HaversianAverage
    {
        public static List<XYCoordinate> getHaversianAverages(
            List<XYCoordinate> xyCoordinates, int setSize)
        {
            var xyAvgCoordinates = new List<XYCoordinate>();
            Coordinate avgCoordinate = null;

            for (int i = 0; i <= setSize / 2; i++)
            {
                avgCoordinate = getHaversianAverage(
                        xyCoordinates.GetRange(i / 2, 2 * i + 1)
                            .Select(xyCoord => xyCoord.getGeoCoordinate())
                            .ToList());
                xyAvgCoordinates.Add(new XYCoordinate(avgCoordinate));
            }

            for (int i = setSize / 2 + 1; i < xyCoordinates.Count - setSize / 2; i++)
            {
                avgCoordinate = getHaversianAverage(
                    avgCoordinate,
                    xyCoordinates[i - setSize / 2 - 1].getGeoCoordinate(),
                    xyCoordinates[i + setSize / 2].getGeoCoordinate(),
                    setSize);
                xyAvgCoordinates.Add(new XYCoordinate(avgCoordinate));
            }

            for (int i = xyCoordinates.Count - setSize / 2; i < xyCoordinates.Count; i++)
            {
                int countToEnd = xyCoordinates.Count - i;
                int dd = i - countToEnd;
                avgCoordinate = getHaversianAverage(
                        xyCoordinates.GetRange(i - countToEnd, 2 * countToEnd)
                            .Select(xyCoord => xyCoord.getGeoCoordinate())
                            .ToList());
                xyAvgCoordinates.Add(new XYCoordinate(avgCoordinate));
            }
            return xyAvgCoordinates;
        }

        private static Coordinate getHaversianAverage(
            Coordinate oldAvgCoord, Coordinate oldCoord, Coordinate newCoord, int count)
        {
            double lat =
                ((count * oldAvgCoord.Latitude.ToDouble())
                    - oldAvgCoord.Latitude.ToDouble()
                    + newCoord.Latitude.ToDouble())
                / count;
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
            return new Coordinate(lat / count, lon / count);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static RobotGPSTrajectory.SvgImage;

namespace RobotGPSTrajectory
{
    /*
     *  Creation of svg image 
     *  of robot trajectory on parking lot 
     *  with given PLANE_ORIGIN and DOORS coordinates
     */

    class ParkingLot
    {
        public static void CreateSvgImageWithDoors(
            string fileName,
            List<XYCoordinate> xyCoordinates,
            List<XYCoordinate> xyAvgCoordinates,
            string doorsCoordinate,
            string originCoordinate,
            int outputStep,
            int avgSetSize)
        {
            // parse PLANE_ORIGIN and DOORS coordinates
            if (!(CoordinateSharp.Coordinate.TryParse(
                    doorsCoordinate,
                    out CoordinateSharp.Coordinate doors)
                    &&
                    (CoordinateSharp.Coordinate.TryParse(
                    originCoordinate,
                    out CoordinateSharp.Coordinate origin))))
            {
                Console.WriteLine("Invalid coordinates.");
                return;
            }

            // count distancies to DOORS
            var gpsDistance = doors.Get_Distance_From_Coordinate(
                xyCoordinates.Last().getGeoCoordinate());
            var avgDistance = doors.Get_Distance_From_Coordinate(
                xyAvgCoordinates.Last().getGeoCoordinate());

            // create svg image
            WriteElements(
                fileName,
                "Robot on parking lot",
                "Distance to doors from end position:  "
                    + avgDistance.Meters.ToString().Substring(0, 4) + " m (AVG " + avgSetSize +"); "
                    + gpsDistance.Meters.ToString().Substring(0, 5) + " m (GPS)",
                xyCoordinates.Cast<IPointXY>().ToList(),
                xyAvgCoordinates.Cast<IPointXY>().ToList(),
                new XYCoordinate(doors),
                new XYCoordinate(origin),
                outputStep);
        }


        private static void WriteElements(
            string fileName,
            string title,
            string info,
            List<IPointXY> black_points,
            List<IPointXY> blue_points,
            IPointXY doors,
            IPointXY origin,
            int step)
        {
            if (!fileName.EndsWith(SVG))
                fileName = fileName + SVG;
            try
            {
                var writer = new StreamWriter(fileName);
                WriteHeader(writer, WIDTH, HEIGHT, title, info);
                var allPoints = new List<IPointXY>(black_points);
                allPoints.Add(origin);
                allPoints.Add(doors);
                double[] difference = GetMaxDifference(allPoints);

                // avg start-point
                var start = blue_points[0];
                WritePoint(
                    writer,
                    Align(start.GetX(), difference[0]),
                    Align(start.GetY(), difference[1]),
                    4,
                    GREEN);

                // avg end-point
                var end = blue_points[blue_points.Count - 1];
                WritePoint(
                    writer,
                    Align(end.GetX(), difference[0]),
                    Align(end.GetY(), difference[1]),
                    4,
                    RED);

                // gps end-point
                end = black_points[black_points.Count - 1];
                WritePoint(
                    writer,
                    Align(end.GetX(), difference[0]),
                    Align(end.GetY(), difference[1]),
                    4,
                    RED);

                // doors
                WritePoint(
                    writer,
                    Align(doors.GetX(), difference[0]),
                    Align(doors.GetY(), difference[1]),
                    6,
                    BLACK);

                // origin axes
                WriteXYAxes(writer, difference, origin);

                // all avg points
                blue_points.ForEach(
                    point => WritePoint(
                        writer,
                        Align(point.GetX(), difference[0]),
                        Align(point.GetY(), difference[1]),
                        1,
                        BLUE));

                // all edges between gps points
                WriteEdges(writer, black_points, 1, BLACK, 0.2, difference);

                // all edges between gps points
                WriteEdges(writer, blue_points, 1, BLUE, 2, difference);

                WriteLastTag(writer);
                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Cannot create svg image \\\"{0}\\\": {1}.", fileName, ex.Message);
            }
        }

        private static void WriteXYAxes(StreamWriter writer, double[] difference, IPointXY origin)
        {
            WriteEdge(
                       writer,
                       0,
                       Align(origin.GetY(), difference[1]),
                       WIDTH,
                       Align(origin.GetY(), difference[1]),
                       1,
                       BLACK);
            WriteEdge(
                   writer,
                   Align(origin.GetX(), difference[0]),
                   0,
                   Align(origin.GetX(), difference[0]),
                   HEIGHT,
                   1,
                   BLACK);
        }
    }
}

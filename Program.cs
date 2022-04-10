using System;
using System.Collections.Generic;

namespace RobotGPSTrajectory
{
    /*
     *  Estimation of robot trajectory by given GPS coordinates
     *  
     *  args[0]     input fileName (coordinate format example: "50.0884745107823974 14.4043757008512703")
     *  args[1]     output svg fileName with trajectory image)
     */

    class Program
    {
        private static readonly string PLANE_ORIGIN = "50°5'18.404 N 14°24'15.307 E";
        private static readonly string DOORS = "50°5'18.475 N 14°24'13.495 E";

        private static readonly float GRID_CELL_SIZE = 1f;
        private static readonly int OUTPUT_STEP = 10;

        //  GPS measurment error
        private static readonly float START_RADIUS = 0.5f;
        private static readonly float GPS_RADIUS = 8;

        static void Main(string[] args)
        {

            if (!TryParseInput(
                args, out XYCoordinate startPosition, out List<XYCoordinate> xyCoordinates))
            {
                return;
            }

            //  estimate the trajectory coordinates
            var estimatedCoordinates =
                Trajectory.getTrajectoryCoordinates(
                    startPosition, xyCoordinates, START_RADIUS, GPS_RADIUS, GRID_CELL_SIZE);

            //  each (i * OUTPUT_STEP) trajectory coordinate is selected
            var selectedEstimatedCoordinates =
                SelectCoordinates(estimatedCoordinates, OUTPUT_STEP);

            //  create svg image of selected estimated coordinates
            ParkingLot.CreateSvgImageWithDoors(
                args[1], xyCoordinates, selectedEstimatedCoordinates,
                DOORS, PLANE_ORIGIN);

            // print selected estimated coordinates
            IOUtils.PrintCoordinates(
                 selectedEstimatedCoordinates, PLANE_ORIGIN);

            Console.ReadKey();
        }


        private static bool TryParseInput(
            string[] args,
            out XYCoordinate startPosition,
            out List<XYCoordinate> xyCoordinates
            )
        {
            startPosition = null;
            xyCoordinates = null;

            if (args.Length != 2)
            {
                Console.WriteLine("Incorrect args length.");
                return false;
            }

            if (!CoordinateSharp.Coordinate.TryParse(
                    PLANE_ORIGIN,
                    out CoordinateSharp.Coordinate origin))
            {

                Console.WriteLine("Invalid PLANE_ORIGIN coordinate.");
                return false;
            }

            startPosition = new XYCoordinate(origin);
            if (!IOUtils.TryParse(args[0], out xyCoordinates))
            {
                Console.WriteLine("Invalid coordinates.");
                return false;
            }

            return true;
        }


        private static List<XYCoordinate> SelectCoordinates(
            List<XYCoordinate> xyCoordinates,
            int output_step)
        {
            var selectedCoordinates = new List<XYCoordinate>();

            for (int i = 0; i < xyCoordinates.Count; i = i + OUTPUT_STEP)
            {
                selectedCoordinates.Add(xyCoordinates[i]);
            }

            return selectedCoordinates;
        }
    }
}

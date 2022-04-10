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
        private static readonly int OUTPUT_STEP = 10;   // each (i * OUTPUT_STEP) trajectory coordinate is printed

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Incorrect args length.");
                return;
            }
            if (!CoordinateSharp.Coordinate.TryParse(
                    PLANE_ORIGIN,
                    out CoordinateSharp.Coordinate origin))
            {

                Console.WriteLine("Invalid PLANE_ORIGIN coordinate.");
                return;
            }

            var startPosition = new XYCoordinate(origin);
            if (IOUtils.TryParse(args[0], out List<XYCoordinate> xyCoordinates))
            {
                var estimatedCoordinates =
                    Trajectory.getTrajectoryCoordinates(startPosition, xyCoordinates, 8);
                
                var selectedEstimatedCoordinates = new List<XYCoordinate>();

                for (int i = 0; i < estimatedCoordinates.Count; i = i + OUTPUT_STEP)
                {
                    Console.WriteLine(i);
                    selectedEstimatedCoordinates.Add(estimatedCoordinates[i]);
                }

                ParkingLot.CreateSvgImageWithDoors(
                    args[1], xyCoordinates, selectedEstimatedCoordinates, 
                    DOORS, PLANE_ORIGIN);

                IOUtils.PrintCoordinates(
                     selectedEstimatedCoordinates, PLANE_ORIGIN);
            }
            Console.ReadKey();
        }
    } 
}

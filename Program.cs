using System;
using System.Collections.Generic;

namespace RobotGPSTrajectory
{
    class Program
    {
        private static readonly string DOORS = "50°5'18.475 N 14°24'13.495 E";
        private static readonly int AVG_SET_SIZE = 20;
        private static readonly int OUTPUT_STEP = 10;

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Incorrect args length.");
                return;
            }

            if (IOUtils.TryParse(args[0], out List<XYCoordinate> xyCoordinates))
            {
                var xyAvgCoordinates = 
                    XYCoordinate.getHaversianAverages(xyCoordinates, AVG_SET_SIZE-1);

                IOUtils.PrintAvgCoordinates(
                    xyAvgCoordinates, OUTPUT_STEP);

                SvgImage.CreateSvgImageWithDoors(
                    args[1], xyCoordinates, xyAvgCoordinates, DOORS, OUTPUT_STEP);
            }
            Console.ReadKey();
        }
    }
}

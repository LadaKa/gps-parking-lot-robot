using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace RobotGPSTrajectory
{
    /*
     *  Parsing and print of coordinates 
     */

    class IOUtils
    {
        public static bool TryParseInput(
    string[] args,
    string plane_origin,
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
                    plane_origin,
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


        public static List<XYCoordinate> SelectCoordinates(
            List<XYCoordinate> xyCoordinates,
            int output_step)
        {
            var selectedCoordinates = new List<XYCoordinate>();

            for (int i = 0; i < xyCoordinates.Count; i = i + output_step)
            {
                selectedCoordinates.Add(xyCoordinates[i]);
            }

            return selectedCoordinates;
        }


        public static bool TryParse(string fileName, out List<XYCoordinate> xyCoordinates)
        {
            xyCoordinates = new List<XYCoordinate>();
            int lineNr = 0;
            try
            {
                var reader = new StreamReader(fileName);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    lineNr++;
                    if (TryParseLatLonCoordinate(line, out XYCoordinate xyCoordinate))
                    {
                        xyCoordinates.Add(xyCoordinate);
                    }
                    else
                    {
                        Console.WriteLine("Invalid coordinate format at line {0}: {1}", lineNr, line);
                        return false;
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Invalid input file \\\"{0}\\\": {1}", fileName, ex.Message);
                return false;
            }
            return true;
        }


        private static bool TryParseLatLonCoordinate(string line, out XYCoordinate xyCoordinate)
        {
            var lineWords = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (lineWords.Length == 2
                && double.TryParse(
                    lineWords[0], System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out double lat)
                && double.TryParse(
                    lineWords[1], System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out double lon))
            {
                xyCoordinate = new XYCoordinate(lat, lon);
                return true;
            }

            xyCoordinate = null;
            return false;
        }

        public static void PrintCoordinates(
            List<XYCoordinate> xyCoordinates,
            string originString)
        {
            CoordinateSharp.Coordinate.TryParse(
                    originString,
                    out CoordinateSharp.Coordinate origin);
            var xyOrigin = new XYCoordinate(origin);
            for (int i = 0; i < xyCoordinates.Count; i = i + 1)
            {
                xyCoordinates[i].Print(xyOrigin);
            }
            xyCoordinates[xyCoordinates.Count - 1].Print(xyOrigin);
        }
    }
}

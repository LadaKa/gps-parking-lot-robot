﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace RobotGPSTrajectory
{
    class IOUtils
    {
        
        public static bool TryParse(string fileName, out List<XYCoordinate> xyCoordinates)
        {
            xyCoordinates = new List<XYCoordinate>();
            int lineNr = 0;
            try
            {
                var reader = new StreamReader(fileName);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();   //  "50.0884745107823974 14.4043757008512703"
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

        public static void PrintAvgCoordinates(
            List<XYCoordinate> xyAvgCoordinates,
            int step)
        {

            for (int i = 0; i < xyAvgCoordinates.Count; i = i + step)
            {
                var coord = xyAvgCoordinates[i];
                Console.WriteLine(
                    coord.getGeoCoordinate().UTM.Easting.ToString()
                    + " "
                    + coord.getGeoCoordinate().UTM.Northing);
            }
            Console.WriteLine(
                xyAvgCoordinates[xyAvgCoordinates.Count - 1].getGeoCoordinate().UTM.Easting.ToString()
                + " "
                + xyAvgCoordinates[xyAvgCoordinates.Count - 1].getGeoCoordinate().UTM.Northing);
        }
    }
}
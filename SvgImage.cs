using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotGPSTrajectory
{
    class SvgImage
    {
        private static readonly string SVG = ".svg";
        private static readonly string BLACK = "#000000";
        private static readonly string BLUE  = "#0000FF";
        private static readonly string GREEN = "#008000";
        private static readonly string RED   = "#FF0000";
        private static readonly int WIDTH = 700;
        private static readonly int HEIGHT = 700;

        public static void CreateSvgImageWithDoors(
            string fileName,
            List<XYCoordinate> xyCoordinates,
            List<XYCoordinate> xyAvgCoordinates,
            string doorsCoordinate,
            int outputStep)
        {
            CoordinateSharp.Coordinate.TryParse(
                    doorsCoordinate,
                    out CoordinateSharp.Coordinate doors);
            var gpsDistance = doors.Get_Distance_From_Coordinate(
                xyCoordinates.Last().getGeoCoordinate());
            var avgDistance = doors.Get_Distance_From_Coordinate(
                xyAvgCoordinates.Last().getGeoCoordinate());

            SvgImage.CreatePointSvgImage(
                fileName,
                "Doors:                       " + doorsCoordinate,
                "Distance to doors:  "
                    + avgDistance.Meters.ToString().Substring(0, 4) + " m (AVG); "
                    + gpsDistance.Meters.ToString().Substring(0, 5) + " m (GPS)",
                xyCoordinates.Cast<IPointXY>().ToList(),
                xyAvgCoordinates.Cast<IPointXY>().ToList(),
                new XYCoordinate(doors),
                outputStep);
        }

        private static void CreatePointSvgImage(
            string fileName, 
            string title, 
            string info,
            List<IPointXY> black_points,
            List<IPointXY> blue_points,
            IPointXY doors,
            int step)
        {
            if (!fileName.EndsWith(SVG))
                fileName = fileName + SVG;
            try
            {
                var writer = new StreamWriter(fileName);
                WriteHeader(writer, WIDTH, HEIGHT, title, info);
                double[] difference = GetMaxDifference(black_points);

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

                // doors
                WritePoint(                                     
                    writer,
                    Align(doors.GetX(), difference[0]),
                    Align(doors.GetY(), difference[1]),
                    6,
                    BLACK);

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

                // edges between gps points [ n * step, n * (step + 1)]
                WriteEdges(writer, blue_points, step, BLUE, 2, difference);
            
                WriteLastTag(writer);
                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Cannot create svg image \\\"{0}\\\": {1}.", fileName, ex.Message);
            }
        }

        private static void WriteHeader(
            StreamWriter writer, int width, int height, string title, string info)
        {
            var headerLines = GetHeaderLines(width, height);
            var header = string.Join(Environment.NewLine, headerLines);
            writer.WriteLine(header);

            writer.WriteLine("<text font-weight=\"bold\" stroke=\"#426373\" xml:space=\"preserve\"" 
                + " text-anchor=\"start\" font-family=\"Helvetica\" font-size=\"11\" id=\"svg_1\" "
                + " y=\"20\" x=\"80\" fill-opacity=\"null\" stroke-opacity=\"null\" stroke-width=\"0\" "
                + "fill=\"#000000\">" + title +"</text>");
            writer.WriteLine("<text font-weight=\"bold\" stroke=\"#426373\" xml:space=\"preserve\"" 
                + " text-anchor=\"start\" font-family=\"Helvetica\" font-size=\"11\" id=\"svg_1\" y=\"30\" x=\"80\" " 
                + "fill-opacity=\"null\" stroke-opacity=\"null\" stroke-width=\"0\" fill=\"#000000\">{0}</text>", info);
        }

        private static void WritePoint(
            StreamWriter writer, double x, double y, double size, string color)
        {

            string Xs = x.ToString().Replace(',', '.');
            string Ys = y.ToString().Replace(',', '.');
            string pointSize = size.ToString().Replace(",", ".");

            writer.WriteLine
                ("<ellipse opacity=\"100\" stroke=\"1\" style=\"vector-effect: non-scaling-stroke;\""
                    + " ry=\"" + pointSize + "\" rx=\"" + pointSize + "\" id=\"svg_1\" cy=\"" + Ys + "\" cx=\"" + Xs
                    + "\" stroke-width=\"1\" fill=\"" + color + "\"/>");

        }

        private static void WriteEdges(
            StreamWriter writer,
            List<IPointXY> points, 
            int step, 
            string color, 
            double stroke, 
            double[] difference)
        {
            int i = 0;
            while (i < points.Count - step)
            {
                WriteEdge(
                    writer,
                    Align(points[i].GetX(), difference[0]),
                    Align(points[i].GetY(), difference[1]),
                    Align(points[i + step].GetX(), difference[0]),
                    Align(points[i + step].GetY(), difference[1]),
                    stroke,
                    color);
                i = i + step;
            }
            WriteEdge(
                    writer,
                    Align(points[i].GetX(), difference[0]),
                    Align(points[i].GetY(), difference[1]),
                    Align(points[points.Count - 1].GetX(), difference[0]),
                    Align(points[points.Count - 1].GetY(), difference[1]),
                    stroke,
                    color);
        }

        private static void WriteEdge(
            StreamWriter writer, 
            double x1, double y1, 
            double x2, double y2, 
            double stroke,
            string color)
        {
            string X1s = x1.ToString().Replace(',', '.');
            string Y1s = y1.ToString().Replace(',', '.');
            string X2s = x2.ToString().Replace(',', '.');
            string Y2s = y2.ToString().Replace(',', '.');
    
            writer.WriteLine
                ("<line stroke=\"{5}\" "
                + " y2=\"{0}\" x2=\"{1}\" y1=\"{2}\" x1=\"{3}\" "
                + "style=\"stroke: {4}; stroke-width:{5}\"/>",
                Y2s, X2s, Y1s, X1s, color, stroke.ToString().Replace(",","."));
        }

        private static void WriteLastTag(StreamWriter writer)
        {
            writer.WriteLine(
                " </g>" + Environment.NewLine
                + "</g>" + Environment.NewLine 
                + "</svg>");
        }

        private static List<string> GetHeaderLines(int width, int height)
        {
            return new List<string>() {
                "<svg width=\"" + width + "\" height=\"" + height
                    + "\" xmlns=\"http://www.w3.org/2000/svg\" "
                    + "style=\"vector-effect: non-scaling-stroke;\" stroke=\"null\">",
                "<g stroke = \"null\" >",
                "<title stroke = \"null\" > background </title >",
                "<rect stroke = \"#000000\" fill = \"#f9f9f9\" id = \"canvas_background\" "
                    + "width=\"" + width + "\" height=\"" + height + "\" y = \"0\" x = \"0\" />",
                "<g stroke = \"null\" >",
                "<title stroke = \"null\" > Layer 1 </title > "
            };
        }

        private static double[] GetMaxDifference(List<IPointXY> points)
        {
            var minX = double.MaxValue;     // TODO:    not needed max with fixed resize param
            var maxX = double.MinValue;
            var minY = double.MaxValue;
            var maxY = double.MinValue;
            points.ForEach(
                point =>
                {
                    var x = point.GetX();
                    var y = point.GetY();
                    if (x < minX)
                        minX = x;
                    if (x > maxX)
                        maxX = x;
                    if (y < minY)
                        minY = y;
                    if (y > maxY)
                        maxY = y;
                });
            return new[] { minX, minY };
        }

        private static double Align(double number, double difference)
        {
            return ((number - difference) * 10) + 100;  // TODO:    frame + resize param
        }
    }
}

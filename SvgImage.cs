using System;
using System.Collections.Generic;
using System.IO;

namespace RobotGPSTrajectory
{
   /*
    *  Utils for general svg image creation.
    */

    class SvgImage
    {
        public static readonly string SVG = ".svg";
        public static readonly string BLACK = "#000000";
        public static readonly string BLUE  = "#0000FF";
        public static readonly string GREEN = "#008000";
        public static readonly string RED   = "#FF0000";
        public static readonly int WIDTH = 700;
        public static readonly int HEIGHT = 700;

        public static void WriteHeader(
            StreamWriter writer, int width, int height, string title, string info)
        {
            var headerLines = GetHeaderLines(width, height);
            var header = string.Join(Environment.NewLine, headerLines);
            writer.WriteLine(header);

            writer.WriteLine("<text font-weight=\"bold\" stroke=\"#426373\" xml:space=\"preserve\"" 
                + " text-anchor=\"start\" font-family=\"Helvetica\" font-size=\"11\" id=\"svg_1\" "
                + " y=\"20\" x=\"300\" fill-opacity=\"null\" stroke-opacity=\"null\" stroke-width=\"0\" "
                + "fill=\"#000000\">" + title +"</text>");
            writer.WriteLine("<text font-weight=\"bold\" stroke=\"#426373\" xml:space=\"preserve\"" 
                + " text-anchor=\"start\" font-family=\"Helvetica\" font-size=\"11\" id=\"svg_1\" y=\"45\" x=\"80\" " 
                + "fill-opacity=\"null\" stroke-opacity=\"null\" stroke-width=\"0\" fill=\"#000000\">{0}</text>", info);
        }

        public static void WritePoint(
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

        public static void WriteEdges(
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

        public static void WriteEdge(
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

        public static void WriteLastTag(StreamWriter writer)
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

        public static double[] GetMaxDifference(List<IPointXY> points)
        {
            var minX = double.MaxValue;     
            var minY = double.MaxValue;
            points.ForEach(
                point =>
                {
                    var x = point.GetX();
                    var y = point.GetY();
                    if (x < minX)
                        minX = x;
                    if (y < minY)
                        minY = y;
                });
            return new[] { minX, minY };
        }

        public static double Align(double number, double difference)
        {
            return ((number - difference) * 10) + 100;  // TODO:    frame + resize param
        }
    }
}

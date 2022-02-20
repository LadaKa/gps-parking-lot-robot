using System;
using System.Collections.Generic;

namespace RobotGPSTrajectory
{
    class Trajectory
    {
        public static List<XYCoordinate> getTrajectoryCoordinates(
            List<XYCoordinate> xyCoordinates,
            int radius)
        {
            var trajectoryCoordinates = getGridCoordinates(xyCoordinates[0], radius);

            return trajectoryCoordinates; 
        }


        private static List<XYCoordinate> getGridCoordinates(
            XYCoordinate xy,
            int radius)
        {
            var gridCoordinates = new List<XYCoordinate>();

            int radius_pow = radius * radius;
            for (int i = 0; i < radius; i++) 
            {
                var east_middle_coord = xy.shiftXYCoordinateByVerticalDistance(i, XYCoordinate.Direction.E);
                var west_middle_coord = xy.shiftXYCoordinateByVerticalDistance(i, XYCoordinate.Direction.W);
                int columns = (int)Math.Sqrt(radius_pow - (i * i));
                for (int j = 0; j < columns; j++)
                {
                    var east_north_coord = east_middle_coord.shiftXYCoordinateByVerticalDistance(j, XYCoordinate.Direction.N);
                    gridCoordinates.Add(east_north_coord);

                    var east_south_coord = east_middle_coord.shiftXYCoordinateByVerticalDistance(j, XYCoordinate.Direction.S);
                    gridCoordinates.Add(east_south_coord);

                    var west_north_coord = west_middle_coord.shiftXYCoordinateByVerticalDistance(j, XYCoordinate.Direction.N);
                    gridCoordinates.Add(west_north_coord);

                    var west_south_coord = west_middle_coord.shiftXYCoordinateByVerticalDistance(j, XYCoordinate.Direction.S);
                    gridCoordinates.Add(west_south_coord);

                }
            }         
            return gridCoordinates;
        }
    }
}

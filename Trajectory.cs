using System;
using System.Collections.Generic;

namespace RobotGPSTrajectory
{
    class Trajectory
    {
        public static List<XYCoordinate> getTrajectoryCoordinates(
            List<XYCoordinate> measuredCoordinates,
            int radius)
        {
            var trajectoryLenght = measuredCoordinates.Count;
            var states = new List<XYCoordinate>[trajectoryLenght];
            states[0] = getXYCoordinatesGrid(measuredCoordinates[0], 0.5f);

            for (int i = 1; i < trajectoryLenght; i++)
            {
                var measuredCoordinate = measuredCoordinates[i];
                states[i] = getXYCoordinatesGrid(measuredCoordinate, radius);
                foreach (XYCoordinate coordinate in states[i])
                {
                    setProbability(coordinate, states[i - 1]);
                }
            }

            List<XYCoordinate> trajectoryCoordinates = new List<XYCoordinate>();
            foreach (var state in states)
            {
                var maxProbCoordinate = state[0];
                foreach (XYCoordinate coordinate in state)
                {
                    if (coordinate.getProbability() > maxProbCoordinate.getProbability())
                        maxProbCoordinate = coordinate;
                }
                trajectoryCoordinates.Add(maxProbCoordinate);
            }

            return trajectoryCoordinates; 
        }


        private static List<XYCoordinate> getXYCoordinatesGrid(
            XYCoordinate xy,
            float radius)
        {
            var gridCoordinates = new List<XYCoordinate>();

            float radius_pow = radius * radius;
            for (float i = 0; i < radius; i = i + 0.5f) 
            {
                var east_middle_coord = xy.shiftXYCoordinateByVerticalDistance(i, XYCoordinate.Direction.E);
                var west_middle_coord = xy.shiftXYCoordinateByVerticalDistance(i, XYCoordinate.Direction.W);
                int columns = (int)Math.Sqrt(radius_pow - (i * i));
                for (float j = 0; j < columns; j = j + 0.5f)
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

        private static void setProbability(XYCoordinate coordinate, List<XYCoordinate> previousGrid)
        {
            double sum = 0;
            foreach (XYCoordinate previousCoordinate in previousGrid)
            {
                if (coordinate.getHaversianDistanceInMeters(previousCoordinate) >= 1)
                {
                    sum = sum + previousCoordinate.getProbability();
                }
            }
            coordinate.setProbability(sum/previousGrid.Count);
        }
    }
}

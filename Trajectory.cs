using System;
using System.Collections.Generic;

namespace RobotGPSTrajectory
{
    class Trajectory
    {
        public static List<XYCoordinate> getTrajectoryCoordinates(
            XYCoordinate startPosition,
            List<XYCoordinate> measuredCoordinates,
            int radius)
        {
            measuredCoordinates.Insert(0, startPosition);
            var trajectoryLenght = measuredCoordinates.Count;

            for (int i = 1; i < measuredCoordinates.Count;i++)
            {
                Console.WriteLine(measuredCoordinates[i].getHaversianDistanceInMeters(measuredCoordinates[i - 1]));
            }
            var states = new List<XYCoordinate>[trajectoryLenght];
            states[0] = getXYCoordinatesGrid(measuredCoordinates[0], 0.5f, 0.5f);
            float p = 1f / states[0].Count;
            foreach (XYCoordinate coordinate in states[0])
            {
                coordinate.setProbability(p);
            }
            states[1] = getXYCoordinatesGrid(measuredCoordinates[1], radius, 1f);
            foreach (XYCoordinate coordinate in states[1])
            {
                setProbability(coordinate, states[0], 0.5f, radius);
            }

            for (int i = 2; i < trajectoryLenght; i++)
            {
                var measuredCoordinate = measuredCoordinates[i];
                states[i] = getXYCoordinatesGrid(measuredCoordinate, radius, 1f);
                foreach (XYCoordinate coordinate in states[i])
                {
                    setProbability(coordinate, states[i - 1], radius, radius);
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
            float radius,
            float cellSize)
        {
            var gridCoordinates = new List<XYCoordinate>();
            gridCoordinates.Add(xy);

            float radius_pow = radius * radius;
            for (float i = 0; i <= radius; i = i + cellSize) 
            {
                var east_middle_coord = xy.shiftXYCoordinateByVerticalDistance(i, XYCoordinate.Direction.E);
                var west_middle_coord = xy.shiftXYCoordinateByVerticalDistance(i, XYCoordinate.Direction.W);
                int columns = Math.Max(1, (int)Math.Sqrt(radius_pow - (i * i)));
                for (float j = 0; j < columns; j = j + cellSize)
                {
                    if ((i == 0) && (j == 0))
                        continue;

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

        private static void setProbability(
            XYCoordinate coordinate, 
            List<XYCoordinate> previousGrid,
            float distance1,
            float distance2
            )
        {
            double sum = 0;
            var distance = distance1 + distance2;
            foreach (XYCoordinate previousCoordinate in previousGrid)
            {
                if (coordinate.getHaversianDistanceInMeters(previousCoordinate) <= distance)
                {
                    sum = sum + previousCoordinate.getProbability();
                }
                //else
                //    Console.WriteLine(coordinate.getHaversianDistanceInMeters(previousCoordinate));
            }
            //Console.WriteLine("p: "+sum/previousGrid.Count);
            coordinate.setProbability(sum);
        }
    }
}

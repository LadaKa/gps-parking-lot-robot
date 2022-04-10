using System;
using System.Collections.Generic;

namespace RobotGPSTrajectory
{
    class Trajectory
    {
        public static List<XYCoordinate> getTrajectoryCoordinates(
            XYCoordinate startPosition,
            List<XYCoordinate> measuredCoordinates,
            float startRadius,
            float radius,
            float gridCellSize)
        {
            measuredCoordinates.Insert(0, startPosition);

            //  get coordinate grids around measured positions and its weights
            var states = GetStatesWeight(measuredCoordinates, startRadius, radius, gridCellSize);

            //  select coordinate with maximal weight from each grid
            return SelectCoordinatesByWeight(states);
        }


        private static List<XYCoordinate>[] GetStatesWeight(
            List<XYCoordinate> measuredCoordinates,
            float startRadius,
            float radius,
            float gridCellSize)
        {
            var trajectoryLenght = measuredCoordinates.Count;
            var states = new List<XYCoordinate>[trajectoryLenght];

            //  initialize grid around start position
            states[0] = getXYCoordinatesGrid(measuredCoordinates[0], startRadius, startRadius);
            float startWeight = 1f / states[0].Count;
            foreach (XYCoordinate coordinate in states[0])
            {
                coordinate.setWeight(startWeight);
            }

            //  initialize grid around first measured position
            states[1] = getXYCoordinatesGrid(measuredCoordinates[1], radius, gridCellSize);
            foreach (XYCoordinate coordinate in states[1])
            {
                //  set probability by distance to coordinates in start grid
                setWeight(coordinate, states[0], startRadius, radius);
            }

            //  initialize grids around other measured position
            for (int i = 2; i < trajectoryLenght; i++)
            {
                states[i] = getXYCoordinatesGrid(measuredCoordinates[i], radius, gridCellSize);
                foreach (XYCoordinate coordinate in states[i])
                {
                    //  set probability by distance to coordinates in previous grid
                    setWeight(coordinate, states[i - 1], radius, radius);
                }
            }

            return states;
        }


        //  get circle grid with centre in xy
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


        //  set probability by distance to coordinates in previous grid
        private static void setWeight(
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
                    sum = sum + previousCoordinate.getWeight();
                }
            }
            coordinate.setWeight(sum);
        }


        //  select coordinate with maximal weight from each grid
        private static List<XYCoordinate> SelectCoordinatesByWeight(List<XYCoordinate>[] states)
        {
            List<XYCoordinate> trajectoryCoordinates = new List<XYCoordinate>();
            foreach (var state in states)
            {
                var maxWeightCoordinate = state[0];
                foreach (XYCoordinate coordinate in state)
                {
                    if (coordinate.getWeight() > maxWeightCoordinate.getWeight())
                        maxWeightCoordinate = coordinate;
                }
                trajectoryCoordinates.Add(maxWeightCoordinate);
            }

            return trajectoryCoordinates;
        }

    }
}

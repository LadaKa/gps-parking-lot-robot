namespace RobotGPSTrajectory
{
    interface IPointXY
    {
        double GetX();
        double GetY();

        double GetTransformedX(IPointXY origin);
        double GetTransformedY(IPointXY origin);
    }
}

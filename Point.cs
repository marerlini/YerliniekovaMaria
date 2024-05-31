using System;

namespace WpfApp1
{
    public class Point
    {
        public double x; 
        public double y;

        public Point(double X, double Y)
        {
            this.x = X;
            this.y = Y;
        }

        public static double GenerateRandom(double min, double max)
        {
            double random = min + (max - min) * new Random().NextDouble();
            return random;
        }
    }
}

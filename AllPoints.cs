using System.Collections.Generic;

namespace WpfApp1
{
    public class AllPoints
    {
        private List<Point> points = new List<Point>();

        public IEnumerable<Point> Points
        {
            get { return points; }
        }

        public void AddPoint(double x, double y)
        {
            points.Add(new Point(x, y));
        }

        public void RemovePoint(int index)
        {
            points.Remove(points[index]);
        }

        public void SortByX()
        {
            points.Sort((p1, p2) => p1.x.CompareTo(p2.x));
        }

        public void ClearPoints()
        {
            points.Clear();
        }

        public (double, double) Linear(Point point0, Point point1)
        {
            double x0 = point0.x;
            double y0 = point0.y;
            double x1 = point1.x;
            double y1 = point1.y;
            double k = (y1 - y0) / (x1 - x0);
            double l = -k * x0 + y0;
            return (k, l);
        }

        public double NewtonPolynomial(double x, double[] dividedDifferences)
        {
            double result = dividedDifferences[0];
            double term = 1;
            for (int i = 1; i < dividedDifferences.Length; i++)
            {
                term *= (x - points[i - 1].x);
                result += term * dividedDifferences[i];
            }
            return result;
        }

        public double[] CalculateDividedDifferences()
        {
            int n = points.Count;
            double[] coefficients = new double[n];

            for (int i = 0; i < n; i++)
            {
                coefficients[i] = points[i].y;
            }

            for (int j = 1; j < n; j++)
            {
                for (int i = n - 1; i >= j; i--)
                {
                    coefficients[i] = (coefficients[i] - coefficients[i - 1]) / (points[i].x - points[i - j].x);
                }
            }

            return coefficients;
        }

        public bool NoDuplicateXValues()
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i].x == points[i + 1].x)
                {
                    return false;
                }
            }
            return true;
        }
    }
}


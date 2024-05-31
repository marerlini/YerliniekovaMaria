using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;

namespace WpfApp1
{
    public class MainViewModel
    {
        public PlotModel MyModel { get; private set; }
        private AllPoints pointCollection;

        public MainViewModel(AllPoints points)
        {
            pointCollection = points;
            MyModel = new PlotModel { Title = "Interpolation" };
            UpdatePlotModel();
        }

        public void UpdatePlotModel()
        {
            MyModel.Series.Clear();

            var scatterSeries = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerFill = OxyColor.FromRgb(49, 84, 44)
            };

            foreach (var point in pointCollection.Points)
            {
                scatterSeries.Points.Add(new ScatterPoint(point.x, point.y));
            }

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                ExtraGridlines = new double[] { 0 },
                ExtraGridlineColor = OxyColor.FromRgb(38, 64, 34),
                ExtraGridlineThickness = 3,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                MajorGridlineColor = OxyColor.FromRgb(153, 191, 126),
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                ExtraGridlines = new double[] { 0 },
                ExtraGridlineColor = OxyColor.FromRgb(38, 64, 34),
                ExtraGridlineThickness = 3,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                MajorGridlineColor = OxyColor.FromRgb(153, 191, 126),
            };

            MyModel.Axes.Clear();
            MyModel.Axes.Add(xAxis);
            MyModel.Axes.Add(yAxis);

            MyModel.Series.Add(scatterSeries);
            MyModel.InvalidatePlot(true);
        }
    }
}

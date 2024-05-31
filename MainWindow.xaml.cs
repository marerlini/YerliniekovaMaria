using OxyPlot;
using OxyPlot.Series;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private AllPoints pointCollection = new AllPoints();

        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel(pointCollection);
            DataContext = viewModel;
        }

        private const double MinValue = -50000.0;
        private const double MaxValue = 50000.0;
        private bool interpolate = false;
        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            Result.Text = "";
            HideGraph.Visibility = Visibility.Collapsed;
            bool valid = true;
            Result.Foreground = Brushes.DarkRed;

            XCoordinate.Text = XCoordinate.Text.Replace('.', ',');
            YCoordinate.Text = YCoordinate.Text.Replace('.', ',');

            if (double.TryParse(XCoordinate.Text, out double x) && double.TryParse(YCoordinate.Text, out double y))
            {
                if (x > MaxValue || x < MinValue)
                {
                    Result.Text += $"Значення х повинно бути в межах від {MinValue} до {MaxValue}!\n";
                    valid = false;
                }
                if (y > MaxValue || y < MinValue)
                {
                    Result.Text += $"Значення y повинно бути в межах від {MinValue} до {MaxValue}!";
                    valid = false;

                }
                if(valid)
                {
                    pointCollection.AddPoint(x, y);
                    PointListBox.Items.Add($"({x}; {y})");
                    var viewModel = (MainViewModel)DataContext;
                    viewModel.UpdatePlotModel();

                    XCoordinate.Clear();
                    YCoordinate.Clear();
                }
            }
            else
            {
                Result.Text = "Некоректно введені координати точки яку ви хочете додати!";
            }
        }

        private void DelateButtonClick(object sender, RoutedEventArgs e)
        {
            if (PointListBox.SelectedItem != null)
            {
                pointCollection.RemovePoint(PointListBox.SelectedIndex);
                PointListBox.Items.Remove(PointListBox.SelectedItem);
                HideGraph.Visibility = Visibility.Collapsed;
                var viewModel = (MainViewModel)DataContext;
                viewModel.UpdatePlotModel();
            }
        }

        private void DelateAllButtonClick(object sender, RoutedEventArgs e) 
        {
            pointCollection.ClearPoints(); 
            PointListBox.Items.Clear(); 
            var viewModel = (MainViewModel)DataContext;
            viewModel.UpdatePlotModel();
            Result.Text = "";
            HideGraph.Visibility = Visibility.Collapsed;
            interpolate = false;
            Complexity.Text = "Практична складність алгоритму: ";
        }

        private void RandomButtonClick(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            double minX = 0, maxX = 0, minY = 0, maxY = 0;
            int num = 0;

            Result.Text = "";
            HideGraph.Visibility = Visibility.Collapsed;
            Result.Foreground = Brushes.DarkRed;

            MinRandomX.Text = MinRandomX.Text.Replace('.', ',');
            MaxRandomX.Text = MaxRandomX.Text.Replace('.', ',');
            MinRandomY.Text = MinRandomY.Text.Replace('.', ',');
            MaxRandomY.Text = MaxRandomY.Text.Replace('.', ',');
            NumberOfRandom.Text = NumberOfRandom.Text.Replace('.', ',');



            if (double.TryParse(MinRandomX.Text, out minX) && double.TryParse(MaxRandomX.Text, out maxX))
            {
                if (minX > MaxValue || minX < MinValue)
                {
                    Result.Text += $"Нижня межа x повинна бути в межах від {MinValue} до {MaxValue}!\n";
                    valid = false;
                }
                if (maxX > MaxValue || maxX < MinValue)
                {
                    Result.Text += $"Верхня межа x повинна бути в межах від {MinValue} до {MaxValue}!\n";
                    valid = false;
                }
            }
            else
            {
                Result.Text += "Некоректно задано межі генерації х!\n";
                valid = false;
            }


            if (double.TryParse(MinRandomY.Text, out minY) && double.TryParse(MaxRandomY.Text, out maxY))
            {
                if (minY > MaxValue || minY < MinValue)
                {
                    Result.Text += $"Нижня межа y повинна бути в межах від {MinValue} до {MaxValue}!\n";
                    valid = false;
                }
                if (maxY > MaxValue || maxY < MinValue)
                {
                    Result.Text += $"Верхня межа y повинна бути в межах від {MinValue} до {MaxValue}!\n";
                    valid = false;
                }
            }
            else
            {
                Result.Text += "Некоректно введені межі генерації у!\n";
                valid = false;
            }

            if (int.TryParse(NumberOfRandom.Text, out num))
            {
                if (num < 0)
                {
                    Result.Text += $"Кількість повинна бути додатнім числом!\n";
                    valid = false;
                }
                if (num > MaxValue)
                {
                    Result.Text += $"Максимальна кількість елементів {MaxValue}!\n";
                    valid = false;
                }
            }
            else
            {
                Result.Text += "Некоректно введено кількість генерованих елементів!\n";
                valid = false;
            }

            if (valid)
            {
                for (int i = 0; i < num; i++)
                {
                    double x = Point.GenerateRandom(minX, maxX);
                    double y = Point.GenerateRandom(minY, maxY);
                    pointCollection.AddPoint(x, y);
                    PointListBox.Items.Add($"({x}; {y})");
                    var viewModel = (MainViewModel)DataContext;
                    viewModel.UpdatePlotModel();
                }
            }
        }

        private void InterpolateButtonClick(object sender, RoutedEventArgs e)
        {
            pointCollection.SortByX();
            PointListBox.Items.Clear();
            Complexity.Text = "Практична складність алгоритму: ";
            HideGraph.Visibility = Visibility.Collapsed;

            foreach (var point in pointCollection.Points)
            {
                PointListBox.Items.Add($"({point.x}; {point.y})");
            }
            string selectedValue = MethodSelection.SelectedItem.ToString();
            var viewModel = (MainViewModel)DataContext;

            var scatterSeriesOnly = viewModel.MyModel.Series.OfType<ScatterSeries>().ToList();
            viewModel.MyModel.Series.Clear();
            foreach (var scatterSeries in scatterSeriesOnly)
            {
                viewModel.MyModel.Series.Add(scatterSeries);
            }
            //перевірка чи в наборі є 2 точки
            if (pointCollection.Points.Any() && pointCollection.Points.Skip(1).Any())
            {
                Result.Foreground = Brushes.DarkGreen;
                Stopwatch stopwatch = new Stopwatch();
                switch (selectedValue)
                {
                    case "System.Windows.Controls.ComboBoxItem: Лінійний":


                        Result.Text = "Інтерполяційні відрізки:";
                        interpolate = true;
                        
                        stopwatch.Start();

                        for (int i = 0; i < pointCollection.Points.Count() - 1; i++)
                        {
                            Point firstPoint = pointCollection.Points.ElementAt(i);
                            Point secondPoint = pointCollection.Points.ElementAt(i + 1);

                            //Вивід інтерполяційних відрізків
                            if (firstPoint.x == secondPoint.x)
                            {
                                Result.Text += $"\n{i + 1}. x = {firstPoint.x}; у є [{firstPoint.y}; {secondPoint.y}]";
                            }
                            else
                            {
                                (double k, double l) = pointCollection.Linear(firstPoint, secondPoint);

                                if (k == 0)
                                {
                                Result.Text += $"\n{i + 1}. y = {l}; x є [{firstPoint.x}; {secondPoint.x}]";
                                }
                                else
                                { 
                                    if (l < 0)
                                    {
                                        Result.Text += $"\n{i + 1}. y = {k}x - {-l}; x є [{firstPoint.x}; {secondPoint.x}]";
                                    }
                                    else if (l == 0)
                                    { 
                                        Result.Text += $"\n{i + 1}. y = {k}x; x є [{firstPoint.x}; {secondPoint.x}]";
                                    }
                                    else
                                    {
                                        Result.Text += $"\n{i + 1}. y = {k}x + {l}; x є [{firstPoint.x}; {secondPoint.x}]";
                                    }
                                }
                            }

                            //генерація прямої між 2 точками
                            var lineSeries = new LineSeries
                            {
                                Title = $"Пряма {i + 1}",
                                Color = OxyColor.Parse("#3b6435"),
                                StrokeThickness = 2
                            };

                            lineSeries.Points.Add(new DataPoint(firstPoint.x, firstPoint.y));
                            lineSeries.Points.Add(new DataPoint(secondPoint.x, secondPoint.y));

                            viewModel.MyModel.Series.Add(lineSeries);
                        }

                        stopwatch.Stop();
                        Complexity.Text += $"Алгоритм є лінійним відповідно його часова складність O(n), додаткову память алгоритм не використовує. Час виконання алгоритму для масиву з {pointCollection.Points.Count()} точок - {stopwatch.ElapsedMilliseconds} мілісекунд.";

                        viewModel.MyModel.InvalidatePlot(true);
                    break;

                    case "System.Windows.Controls.ComboBoxItem: Ньютона":

                        if (pointCollection.NoDuplicateXValues())
                        {
                            interpolate = true;

                            stopwatch.Start();
                            double[] coefs = pointCollection.CalculateDividedDifferences();

                            //будова графіка

                            Result.Text = $"Інтерполяційний поліном Ньютона:\nP(x) = {coefs[0]}";
                            if(pointCollection.Points.Count() <= 100 && Math.Abs(Math.Abs(pointCollection.Points.First().x) - Math.Abs(pointCollection.Points.Last().x)) <= 3000){
                                var series = new LineSeries();

                                for (int i = 0; i < pointCollection.Points.Count() - 1; i++)
                                {
                                    double xStart = pointCollection.Points.ElementAt(i).x;
                                    double xEnd = pointCollection.Points.ElementAt(i + 1).x;

                                    series.Points.Add(new DataPoint(xStart, pointCollection.Points.ElementAt(i).y));

                                    for (double x = xStart; x < xEnd; x += 0.1)
                                    {
                                        series.Points.Add(new DataPoint(x, pointCollection.NewtonPolynomial(x, coefs)));
                                    }

                                }
                                series.Points.Add(new DataPoint(pointCollection.Points.Last().x, pointCollection.Points.Last().y));

                                viewModel.MyModel.Series.Add(series);
                                viewModel.MyModel.InvalidatePlot(true);
                            }
                            else
                            {
                                HideGraph.Text = "Нажаль, через специфіку методу та роботи програми графік для такої кількості точок в таких межах не може бути побудований цією програмою. Для побудови інтерполяційного графіку методом Ньютона масив повинен мати не більше 100 точок з максимальною відстаннню між мінімальним та максимальним х - 3000. Відкорегуйте масив даних або оберіть інший метод.";
                                HideGraph.Visibility = Visibility.Visible;
                            }

                            //вивід поліному
                            for (int i = 1; i < coefs.Length; i++)
                            {
                                if (coefs[i] > 0)
                                {
                                    Result.Text += "+";
                                }

                                Result.Text += $"{coefs[i]}";

                                for (int j = 0; j < i; j++)
                                {
                                    if (pointCollection.Points.ElementAt(j).x > 0)
                                    {
                                        Result.Text += $"(x-{pointCollection.Points.ElementAt(j).x})";
                                    }
                                    else if (pointCollection.Points.ElementAt(j).x == 0)
                                    {
                                        Result.Text += $"(x)";
                                    }
                                    else
                                    {
                                        Result.Text += $"(x{pointCollection.Points.ElementAt(j).x})";
                                    }
                                }
                            }

                            stopwatch.Stop();
                            Complexity.Text += $"Алгоритм має часову складність O(n^2), алгоритм використовує O(n) пам'яті. Час виконання алгоритму для масиву з {pointCollection.Points.Count()} точок - {stopwatch.ElapsedMilliseconds} мілісекунд.";

                        }
                        else
                        {
                            Result.Foreground = Brushes.DarkRed;
                            Result.Text = "Знайдено дві точки з однаковими значення х, через обмеження методу неможливо порахувати інтерполяційний поліном. Використайте інший метод.";
                        }
                        break;
                }
            }
            else
            {
                Result.Foreground = Brushes.DarkRed;
                Result.Text = "Повинно бути задано мінімум 2 точки для інтерполювання!";
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (interpolate)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    string textToSave = Result.Text;

                    File.WriteAllText(filePath, textToSave);
                }
            }
            else
            {
                Result.Foreground = Brushes.DarkRed;
                Result.Text = "Отримайте результат інтерполяції щоб зберегти його.";
            }
        }

    }
}

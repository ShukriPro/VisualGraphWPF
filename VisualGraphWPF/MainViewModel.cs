using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Annotations;
using MathNet.Numerics.Distributions;

namespace VisualGraphWPF
{
    public class MainViewModel
    {
        public PlotModel PlotModel { get; private set; }

        public MainViewModel()
        {
            // Initialize the plot model
            PlotModel = new PlotModel { Title = "Patient Health Status on Normal Distribution", TitleFontSize=15 };

            // Configure axes
            var yAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Probability Density" };
            var xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Health Metric" };
            PlotModel.Axes.Add(xAxis);
            PlotModel.Axes.Add(yAxis);

            // Parameters for the Gaussian distribution
            double mu = 0;  // mean
            double sigma = 1;  // standard deviation
            double patientValue = mu - 1.5 * sigma;  // Example patient value set to below average

            // Create normal distribution series and areas
            var (belowAverageSeries, averageSeries, aboveAverageSeries, normalDistributionSeries) = CreateNormalDistributionSeries(mu, sigma);

            // Create patient line series
            var patientLineSeries = new LineSeries
            {
                Color = OxyColors.Magenta,
                StrokeThickness = 2
            };
            patientLineSeries.Points.Add(new DataPoint(patientValue, 0));
            patientLineSeries.Points.Add(new DataPoint(patientValue, normalDistributionSeries.Points.Last().Y));

            // Add the annotations for the patient's value
            var patientAnnotation = new LineAnnotation
            {
                X = patientValue,
                Color = OxyColors.Magenta,
                Type = LineAnnotationType.Vertical,
                LineStyle = LineStyle.Dash
            };

            double patientPercentile = Normal.CDF(mu, sigma, patientValue) * 100;

            var patientTextAnnotation = new TextAnnotation
            {
                TextPosition = new DataPoint(patientValue, yAxis.Transform(0.9 * yAxis.ActualMaximum)),
                Text = $"Patient\n{patientPercentile:0.00}%",
                Stroke = OxyColors.Transparent,
                TextHorizontalAlignment = HorizontalAlignment.Center,
                Background = OxyColors.White
            };

            // Add series and annotations to the plot model

            PlotModel.Series.Add(belowAverageSeries);
            PlotModel.Series.Add(averageSeries);
            PlotModel.Series.Add(aboveAverageSeries);
            PlotModel.Series.Add(normalDistributionSeries);
            PlotModel.Series.Add(patientLineSeries);
            PlotModel.Annotations.Add(patientAnnotation);
            PlotModel.Annotations.Add(patientTextAnnotation);
        }

        private (AreaSeries, AreaSeries, AreaSeries, LineSeries) CreateNormalDistributionSeries(double mu, double sigma)
        {
            var normalDistributionSeries = new LineSeries
            {
                Color = OxyColors.Black,
                StrokeThickness = 2
            };

            var belowAverageSeries = new AreaSeries { Color = OxyColors.Blue, Fill = OxyColor.FromAColor(25, OxyColors.Blue) };
            var averageSeries = new AreaSeries { Color = OxyColors.Green, Fill = OxyColor.FromAColor(75, OxyColors.Green) };
            var aboveAverageSeries = new AreaSeries { Color = OxyColors.Red, Fill = OxyColor.FromAColor(25, OxyColors.Red) };

            for (int i = 0; i < 1000; i++)
            {
                double x = mu - 4 * sigma + i * 8 * sigma / 999;
                double y = Math.Exp(-0.5 * Math.Pow((x - mu) / sigma, 2)) / (sigma * Math.Sqrt(2 * Math.PI));

                normalDistributionSeries.Points.Add(new DataPoint(x, y));

                if (x < mu - sigma)
                {
                    belowAverageSeries.Points.Add(new DataPoint(x, y));
                }
                else if (x > mu + sigma)
                {
                    aboveAverageSeries.Points.Add(new DataPoint(x, y));
                }
                else
                {
                    averageSeries.Points.Add(new DataPoint(x, y));
                }
            }

            return (belowAverageSeries, averageSeries, aboveAverageSeries, normalDistributionSeries);
        }
    }
}

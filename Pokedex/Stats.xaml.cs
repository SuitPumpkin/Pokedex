using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pokedex
{
    public partial class Stats : UserControl
    {
        private double _HP = 0;
        public double HP
        { 
            get { return _HP; } 
            set
            {
                if (_HP != value)
                {
                    _HP = value;
                    OnPropertyChanged(nameof(HP));
                }
            }
        }
        private double _Speed = 0;
        public double Speed
        {
            get { return _Speed; }
            set
            {
                if (_Speed != value)
                {
                    _Speed = value;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }
        private double _SpecialDefense = 0;
        public double SpecialDefense
        {
            get { return _SpecialDefense; }
            set
            {
                if (_SpecialDefense != value)
                {
                    _SpecialDefense = value;
                    OnPropertyChanged(nameof(SpecialDefense));
                }
            }
        }
        private double _Defense = 0;
        public double Defense
        {
            get { return _Defense; }
            set
            {
                if (_Defense != value)
                {
                    _Defense = value;
                    OnPropertyChanged(nameof(Defense));
                }
            }
        }
        private double _SpecialAttack = 0;
        public double SpecialAttack
        {
            get { return _SpecialAttack; }
            set
            {
                if (_SpecialAttack != value)
                {
                    _SpecialAttack = value;
                    OnPropertyChanged(nameof(SpecialAttack));
                }
            }
        }
        private double _Attack = 0;
        public double Attack
        {
            get { return _Attack; }
            set
            {
                if (_Attack != value)
                {
                    _Attack = value;
                    OnPropertyChanged(nameof(Attack));
                }
            }
        }
        public Stats()
        {
            InitializeComponent();
            DibujarEscala();
        }
        protected void OnPropertyChanged(string propertyName)
        {
            Console.WriteLine(propertyName);
            DibujarEstadisticas();
        }
        private void DibujarEscala()
        {
            double centerX = 150;
            double centerY = 120;
            double radius = 100;
            double angleIncrement = 2 * Math.PI / 6;

            PointCollection points1 = [];
            PointCollection points2 = [];
            PointCollection points3 = [];
            PointCollection points4 = [];
            PointCollection points5 = [];
            PointCollection points6 = [];
            PointCollection points7 = [];

            for (int i = 0; i < 6; i++)
            {
                double angle = i * angleIncrement;

                double x = centerX + (radius / 7) * Math.Cos(angle);
                double y = centerY - (radius / 7) * Math.Sin(angle);
                points1.Add(new Point(x, y));

                x = centerX + ((radius / 7) * 2) * Math.Cos(angle);
                y = centerY - ((radius / 7) * 2) * Math.Sin(angle);
                points2.Add(new Point(x, y));

                x = centerX + ((radius / 7) * 3) * Math.Cos(angle);
                y = centerY - ((radius / 7) * 3) * Math.Sin(angle);
                points3.Add(new Point(x, y));

                x = centerX + ((radius / 7) * 4) * Math.Cos(angle);
                y = centerY - ((radius / 7) * 4) * Math.Sin(angle);
                points4.Add(new Point(x, y));

                x = centerX + ((radius / 7) * 5) * Math.Cos(angle);
                y = centerY - ((radius / 7) * 5) * Math.Sin(angle);
                points5.Add(new Point(x, y));

                x = centerX + ((radius / 7) * 6) * Math.Cos(angle);
                y = centerY - ((radius / 7) * 6) * Math.Sin(angle);
                points6.Add(new Point(x, y));

                x = centerX + radius * Math.Cos(angle);
                y = centerY - radius * Math.Sin(angle);
                points7.Add(new Point(x, y));
            }

            Medidas.Children.Add(new Polygon
            {
                Stroke = Brushes.White,
                StrokeThickness = 0.5,
                Opacity = 0.6,
                Points = points7
            }); //7
            Medidas.Children.Add(new Polygon
            {
                Stroke = Brushes.White,
                StrokeThickness = 0.5,
                Opacity = 0.6,
                Points = points1
            }); //1
            Medidas.Children.Add(new Polygon
            {
                Stroke = Brushes.White,
                StrokeThickness = 0.5,
                Opacity = 0.6,
                Points = points2
            }); //2
            Medidas.Children.Add(new Polygon
            {
                Stroke = Brushes.White,
                StrokeThickness = 0.5,
                Opacity = 0.6,
                Points = points3
            }); //3
            Medidas.Children.Add(new Polygon
            {
                Stroke = Brushes.White,
                StrokeThickness = 0.5,
                Opacity = 0.6,
                Points = points4
            }); //4
            Medidas.Children.Add(new Polygon
            {
                Stroke = Brushes.White,
                StrokeThickness = 0.5,
                Opacity = 0.6,
                Points = points5
            }); //5
            Medidas.Children.Add(new Polygon
            {
                Stroke = Brushes.White,
                StrokeThickness = 0.5,
                Opacity = 0.6,
                Points = points6
            }); //6
        }
        private void DibujarEstadisticas()
        {
            double valormax = 250.0;
            double[] values =
            [
                SpecialDefense / valormax,
                Speed / valormax,
                HP / valormax,
                SpecialAttack / valormax,
                Attack / valormax,
                Defense / valormax
            ];
            double centerX = 150;
            double centerY = 120;
            double radius = 100;
            double highervalue = 0;
            double angleIncrement = 2 * Math.PI / 6;
            PointCollection dataPoints = [];
            Stadisticas.Children.Clear();
            for (int f = 0; f < 6; f++)
            {
                if (values[f] > highervalue)
                {
                    highervalue = values[f];
                }
            }
            for (int i = 0; i < 6; i++)
            {
                double angle = i * angleIncrement;
                double dataX = centerX + radius * values[i] * Math.Cos(angle);
                double dataY = centerY - radius * values[i] * Math.Sin(angle);
                dataPoints.Add(new Point(dataX, dataY));

                // Crea un elipse en cada punto coloreandolo dependiendo de si es el stat resaltante o no
                Ellipse pointMarker;
                if (values[i] == highervalue)
                {
                    pointMarker = new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.Yellow,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Cursor = Cursors.Help,
                    };
                }
                else
                {
                    pointMarker = new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.White,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Cursor = Cursors.Help,
                    };
                }
                Canvas.SetLeft(pointMarker, dataX - 5);
                Canvas.SetTop(pointMarker, dataY - 5);
                // Asigna un ToolTip al elipse
                ToolTip tooltip = new()
                {
                    Content = $"{Math.Round(values[i] * valormax)}pts"
                };
                pointMarker.ToolTip = tooltip;
                // Añade el elipse al canvas
                Stadisticas.Children.Add(pointMarker);
            }
            Stadisticas.Children.Add(new Polygon
            {
                Fill = Brushes.Orange,
                Stroke = Brushes.Orange,
                StrokeThickness = 1,
                Opacity = 0.5,
                Points = dataPoints
            }); //Datos
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Graph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int pointSize = 20;
        private Brush pointFill = Brushes.Green;
        private Brush selectedPointFill = Brushes.Blue;

        private Ellipse selectedEllipse = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CanvasClick(object sender, MouseButtonEventArgs e)
        {
            if (Instruments.currentTool == Instruments.Tool.PointAdd)
            {
                Ellipse ellipse = new Ellipse() { Height = pointSize, Width = pointSize, Fill = pointFill };
                ellipse.MouseLeftButtonDown += PointClick;

                MainCanvas.Children.Add(ellipse);
                double x = Mouse.GetPosition(MainCanvas).X;
                double y = Mouse.GetPosition(MainCanvas).Y;
                Canvas.SetTop(ellipse, y - pointSize / 2);
                Canvas.SetLeft(ellipse, x - pointSize / 2);
                Canvas.SetZIndex(ellipse, 10);

                GraphData.AddPoint(x, y, ellipse);
            }
        }

        private void PointClick(object sender, MouseButtonEventArgs e)
        {
            switch (Instruments.currentTool)
            {
                case Instruments.Tool.PointRemove:
                    foreach (GraphData.GraphLine l in GraphData.GetLines(sender as Ellipse))
                    {
                        MainCanvas.Children.Remove(l.line);
                    }
                    GraphData.RemovePoint(sender as Ellipse);
                    MainCanvas.Children.Remove(sender as Ellipse);
                    break;

                case Instruments.Tool.LineAdd:
                    AddLine(sender as Ellipse);
                    break;

                case Instruments.Tool.LineRemove:
                    RemoveLine(sender as Ellipse);
                    break;
            }
        }

        private void AddLine(Ellipse e)
        {
            if (selectedEllipse == null)
            {
                selectedEllipse = e;
                selectedEllipse.Fill = selectedPointFill;
                return;
            }
            else if(selectedEllipse == e)
            {
                DeselectEllipse();
                return;
            }

            GraphData.GraphPoint first = GraphData.GetPoint(selectedEllipse);
            GraphData.GraphPoint second = GraphData.GetPoint(e);

            Line line = new Line() { Stroke = Brushes.Red, StrokeThickness = 2, X1 = first.X, Y1 = first.Y, X2 = second.X, Y2 = second.Y };
            MainCanvas.Children.Add(line);
            Canvas.SetZIndex(line, 2);

            GraphData.AddLine(selectedEllipse, e, line);
            DeselectEllipse();
        }

        private void RemoveLine(Ellipse e)
        {
            if (selectedEllipse == null)
            {
                selectedEllipse = e;
                selectedEllipse.Fill = selectedPointFill;
                return;
            }
            else if(selectedEllipse == e)
            {
                DeselectEllipse();
                return;
            }

            GraphData.GraphLine l = GraphData.GetLine(selectedEllipse, e);
            MainCanvas.Children.Remove(l.line);
            GraphData.RemoveLine(l.line);

            DeselectEllipse();
        }

        private void DeselectEllipse()
        {
            if (selectedEllipse != null)
            {
                selectedEllipse.Fill = pointFill;
                selectedEllipse = null;
            }
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {

        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {

        }

        private void ToolChanged(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            switch (radioButton.Content)
            {
                case "Точка +":
                    Instruments.currentTool = Instruments.Tool.PointAdd;
                    break;

                case "Точка -":
                    Instruments.currentTool = Instruments.Tool.PointRemove;
                    break;

                case "Линия +":
                    Instruments.currentTool = Instruments.Tool.LineAdd;
                    break;

                case "Линия -":
                    Instruments.currentTool = Instruments.Tool.LineRemove;
                    break;

                default:
                    MessageBox.Show("Нет такого инструмента", "Ошибка");
                    return;
            }
            DeselectEllipse();
        }
    }
}

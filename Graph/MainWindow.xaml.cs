using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.Threading;

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
            else if(selectedEllipse != null && Instruments.currentTool == Instruments.Tool.PointMove && Mouse.DirectlyOver == MainCanvas)
            {
                GraphData.GraphPoint point = GraphData.GetPoint(selectedEllipse);

                double y = Mouse.GetPosition(MainCanvas).Y;
                double x = Mouse.GetPosition(MainCanvas).X;

                GraphData.EditPoint(selectedEllipse, x, y); 

                selectedEllipse.Fill = pointFill;
                selectedEllipse = null;

                InitializeGraph(GraphData.data);                
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

                case Instruments.Tool.PointMove:
                    MovePoint(sender as Ellipse);
                    break;
            }
        }

        private void MovePoint(Ellipse e)
        {
            if(selectedEllipse == null)
            {
                selectedEllipse = e;
                selectedEllipse.Fill = selectedPointFill;
            }
            else if(selectedEllipse == e)
            {
                DeselectEllipse();
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
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Graph file|*.graph|All files|*.*";
            dialog.FilterIndex = 1;
            dialog.FileName = DateTime.Now.ToString("d-M-yy_HH-mm-ss");
            
            if(dialog.ShowDialog() == true)
            {
                string json = GraphData.DataToJson();
                File.WriteAllText(dialog.FileName, json);
            }
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
            GraphData.Clear();
        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Graph file|*.graph|All files|*.*";
            dialog.FilterIndex = 1;
            dialog.Multiselect = false;

            if(dialog.ShowDialog() == true)
            {
                string fileContent = File.ReadAllText(dialog.FileName);                
                InitializeGraph(GraphData.DataFromJson(fileContent));
            }
        }

        private void InitializeGraph(GraphData.Data data)
        {
            MainCanvas.Children.Clear();
            GraphData.Clear();

            foreach(GraphData.GraphPoint point in data.Points)
            {
                Ellipse ellipse = new Ellipse() { Height = pointSize, Width = pointSize, Fill = pointFill };
                ellipse.MouseLeftButtonDown += PointClick;

                MainCanvas.Children.Add(ellipse);
                Canvas.SetTop(ellipse, point.Y - pointSize / 2);
                Canvas.SetLeft(ellipse, point.X - pointSize / 2);
                Canvas.SetZIndex(ellipse, 10);

                GraphData.AddPoint(point.Id, point.X, point.Y, ellipse);
            }

            foreach(GraphData.GraphLine line in data.Lines)
            {
                GraphData.GraphPoint first = GraphData.GetPoint(line.FirstId);
                GraphData.GraphPoint second = GraphData.GetPoint(line.SecondId);

                Line l = new Line() { Stroke = Brushes.Red, StrokeThickness = 2, X1 = first.X, Y1 = first.Y, X2 = second.X, Y2 = second.Y };
                MainCanvas.Children.Add(l);
                Canvas.SetZIndex(l, 2);

                GraphData.AddLine(first, second, l);
            }
        }

        private void ToolChanged(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            var radioButtons = LogicalTreeHelper.GetChildren(stack).OfType<RadioButton>().ToList();
            var selected = radioButtons.FirstOrDefault(x => (bool)x.IsChecked);
            int id = radioButtons.IndexOf(selected);

            Instruments.currentTool = (Instruments.Tool)id + 1;

            DeselectEllipse();
        }
    }
}

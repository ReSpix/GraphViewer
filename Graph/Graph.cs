using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    class GraphData
    {
        public struct GraphPoint
        {
            public int Id { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public Ellipse ellipse;
        }

        public struct GraphLine
        {
            public int FirstId { get; set; }
            public int SecondId { get; set; }
            public Line line;
        }

        private static int pointId = 0;

        public static List<GraphLine> Lines { get; set; } = new List<GraphLine>();
        public static List<GraphPoint> Points { get; set; } = new List<GraphPoint>();

        public static GraphPoint GetPoint(Ellipse e)
        {
            return Points.Find((x) => x.ellipse.Equals(e));
        }

        public static GraphLine GetLine(Ellipse first, Ellipse second)
        {
            return Lines.Find((x) => x.FirstId == GetPoint(first).Id && x.SecondId == GetPoint(second).Id || x.FirstId == GetPoint(second).Id && x.SecondId == GetPoint(first).Id);
        }

        public static GraphLine[] GetLines(Ellipse ellipse)
        {
            return Lines.FindAll((x) => x.FirstId == GetPoint(ellipse).Id || x.SecondId == GetPoint(ellipse).Id).ToArray();
        }

        public static void AddPoint(double x, double y, Ellipse uiElement)
        {
            GraphPoint point = new GraphPoint() { Id = pointId++, X = x, Y = y, ellipse = uiElement};
            Points.Add(point);
        }

        public static void AddLine(Ellipse first, Ellipse second, Line line)
        {
            int firstIndex = Points.Find((x) => x.ellipse.Equals(first)).Id;
            int secondIndex = Points.Find((x) => x.ellipse.Equals(second)).Id;

            GraphLine l = new GraphLine() { FirstId = firstIndex, SecondId = secondIndex, line = line };
            Lines.Add(l);
        }

        public static void RemovePoint(Ellipse uiElement)
        {
            GraphPoint point = Points.Find((x) => x.ellipse.Equals(uiElement));
            Points.Remove(point);

            int i = 0;
            while(i < Lines.Count)
            {
                if (Lines[i].FirstId == point.Id || Lines[i].SecondId == point.Id)
                {
                    Lines.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        public static void RemoveLine(Line line)
        {
            foreach(GraphLine l in Lines)
            {
                if (l.Equals(line))
                {
                    Lines.Remove(l);
                    return;
                }
            }
        }
    }
}

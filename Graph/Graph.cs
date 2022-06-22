using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;

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

        public struct Data
        {
            public List<GraphLine> Lines;
            public List<GraphPoint> Points;

            public Data()
            {
                Lines = new List<GraphLine>();
                Points = new List<GraphPoint>();
            }
        }

        public static Data data = new Data();
        private static int pointId = 0;

        public static GraphPoint GetPoint(Ellipse e)
        {
            return data.Points.Find((x) => x.ellipse.Equals(e));
        }
        
        public static GraphPoint GetPoint(int id)
        {
            return data.Points.Find((x) => x.Id == id);
        }

        public static GraphLine GetLine(Ellipse first, Ellipse second)
        {
            return data.Lines.Find((x) => x.FirstId == GetPoint(first).Id && x.SecondId == GetPoint(second).Id || x.FirstId == GetPoint(second).Id && x.SecondId == GetPoint(first).Id);
        }

        public static GraphLine[] GetLines(Ellipse ellipse)
        {
            return data.Lines.FindAll((x) => x.FirstId == GetPoint(ellipse).Id || x.SecondId == GetPoint(ellipse).Id).ToArray();
        }

        public static void AddPoint(double x, double y, Ellipse uiElement)
        {
            GraphPoint point = new GraphPoint() { Id = pointId++, X = x, Y = y, ellipse = uiElement};
            data.Points.Add(point);
        }

        public static void AddPoint(int id, double x, double y, Ellipse uiElement)
        {
            GraphPoint point = new GraphPoint() { Id = id, X = x, Y = y, ellipse = uiElement };
            data.Points.Add(point);
            pointId = id;
        }

        public static void EditPoint(Ellipse e, double x, double y)
        {
            GraphPoint point = data.Points.Find((p) => p.ellipse.Equals(e));
            data.Points.Remove(point);

            AddPoint(point.Id, x, y, e);
        }

        public static void AddLine(Ellipse first, Ellipse second, Line line)
        {
            int firstIndex = data.Points.Find((x) => x.ellipse.Equals(first)).Id;
            int secondIndex = data.Points.Find((x) => x.ellipse.Equals(second)).Id;

            GraphLine l = new GraphLine() { FirstId = firstIndex, SecondId = secondIndex, line = line };
            data.Lines.Add(l);
        }

        public static void AddLine(GraphPoint first, GraphPoint second, Line line)
        {
            GraphLine l = new GraphLine() { FirstId = first.Id, SecondId = second.Id, line = line };
            data.Lines.Add(l);
        }

        public static void RemovePoint(Ellipse uiElement)
        {
            GraphPoint point = data.Points.Find((x) => x.ellipse.Equals(uiElement));
            data.Points.Remove(point);

            int i = 0;
            while(i < data.Lines.Count)
            {
                if (data.Lines[i].FirstId == point.Id || data.Lines[i].SecondId == point.Id)
                {
                    data.Lines.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        public static void RemoveLine(Line line)
        {
            foreach(GraphLine l in data.Lines)
            {
                if (l.line.Equals(line))
                {
                    data.Lines.Remove(l);
                    return;
                }
            }
        }

        public static void Clear()
        {
            data = new Data();
            pointId = 0;
        }

        public static string DataToJson()
        {
            return JsonSerializer.Serialize(data.Points) + "\n" + JsonSerializer.Serialize(data.Lines);
        }

        public static Data DataFromJson(string json)
        {
            string[] jsons = json.Split("\n");

            Data d = new Data();
            d.Points = JsonSerializer.Deserialize<List<GraphPoint>>(jsons[0]);
            d.Lines = JsonSerializer.Deserialize<List<GraphLine>>(jsons[1]);

            return d;
        }
    }
}

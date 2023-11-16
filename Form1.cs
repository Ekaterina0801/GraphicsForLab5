using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace indiv1
{
    public partial class Form1 : Form
    {
        private List<PointF> points = new List<PointF>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            points.Add(e.Location);
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Рисуем точки
            foreach (var point in points)
            {
                e.Graphics.FillEllipse(Brushes.Red, point.X - 3, point.Y - 3, 6, 6);
            }

            if (points.Count >= 3)
            {
                // Находим выпуклую оболочку
                var convexHull = FindConvexHull(points);

                // Рисуем выпуклую оболочку
                var pen = new Pen(Color.Blue);
                for (int i = 0; i < convexHull.Count; i++)
                {
                    var p1 = convexHull[i];
                    var p2 = convexHull[(i + 1) % convexHull.Count];
                    e.Graphics.DrawLine(pen, p1, p2);
                }
            }
        }

        private List<PointF> FindConvexHull(List<PointF> points)
        {
            // Находим самую левую точку
            var startPoint = points.OrderBy(p => p.X).First();

            var convexHull = new List<PointF> { startPoint };
            var currentPoint = startPoint;

            do
            {
                var nextPoint = points[0];

                foreach (var point in points)
                {
                    if (point == currentPoint)
                        continue;

                    var orientation = GetOrientation(currentPoint, nextPoint, point);

                    if (orientation == Orientation.CounterClockwise || orientation == Orientation.Collinear && Distance(currentPoint, point) > Distance(currentPoint, nextPoint))
                    {
                        nextPoint = point;
                    }
                }

                currentPoint = nextPoint;
                convexHull.Add(currentPoint);
            } while (currentPoint != startPoint);

            return convexHull;
        }

        private static double Distance(PointF p1, PointF p2)
        {
            var dx = p1.X - p2.X;
            var dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private enum Orientation
        {
            Clockwise,
            CounterClockwise,
            Collinear
        }

        private static Orientation GetOrientation(PointF p1, PointF p2, PointF p3)
        {
            var crossProduct = (p2.Y - p1.Y) * (p3.X - p2.X) - (p2.X - p1.X) * (p3.Y - p2.Y);

            if (crossProduct > 0)
                return Orientation.CounterClockwise;
            if (crossProduct < 0)
                return Orientation.Clockwise;
            return Orientation.Collinear;
        }
    }
}
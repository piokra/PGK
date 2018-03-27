using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KupaKupaKupa
{
    class Convex2D
    {
        public Vector2[] points { get; }
        public Plane[] planes { get; }

        private Vector2 _origin;
        public Vector2 origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
                for (int i = 0; i < planes.Length; i++)
                {
                    var start = points[i];
                    var end = points[(i + 1) % points.Length];

                    planes[i].origin = (start + end) / 2 + value;
                }
            }
        }
        public Convex2D(Vector2[] points)
        {
            this.points = points;
            List<Plane> planesList = new List<Plane>();
            for (int i = 0; i < points.Length; i++)
            {
                var start = points[i];
                var end = points[(i + 1) % points.Length];
                planesList.Add(new Plane(start, end));
            }
            planes = planesList.ToArray();
        }


        public bool collides(Convex2D other)
        {
            foreach (Vector2 point in points)
            {
                bool failed = false;
                foreach (Plane plane in other.planes)
                {
                    if (plane.penetration(point+origin) > 0)
                    {
                        failed = true;
                        break;
                    }
                }
                if (!failed)
                {
                    return true;
                }
            }
            return false;
            
        }

        public Plane maxPenPlane(Convex2D other)
        {
            foreach (Vector2 point in points)
            {
                bool failed = false;
                float maxPen = 0;
                Plane maxPlane = null;
                foreach (Plane plane in other.planes)
                {
                    float penetration = plane.penetration(point + origin);
                    if (penetration > 0)
                    {
                        failed = true;
                        break;
                    }
                    if (maxPen > penetration)
                    {
                        maxPen = penetration;
                        maxPlane = plane;
                    }
                }
                if (!failed)
                {
                    return maxPlane;
                }
            }
            return null;
        }

        public static Convex2D ApproxCircle(int pointsCount, float radius)
        {
            List<Vector2> pointsList = new List<Vector2>();
            for (int i = 0; i < pointsCount; i++)
            {
                Vector2 point = new Vector2
                {
                    X = radius
                };
                point = point.rotate((float)(i * 2 * Math.PI / pointsCount));
                pointsList.Add(point);
            }

            return new Convex2D(pointsList.ToArray());
        }

        public void DrawMe(Graphics g)
        {
            for (int i = 0; i < points.Length; i++)
            {
                var start = points[i] + origin;
                var end = points[(i + 1) % points.Length] + origin;
                g.DrawLine(Pens.Red, start.X, start.Y, end.X, end.Y);
            }
            
            foreach (Plane plane in planes)
            {
                plane.DrawMe(g);
            }
        }

    }
}

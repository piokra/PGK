using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KupaKupaKupa
{
    class Plane
    {
        public Vector2 normal { get; set; }
        public Vector2 origin { get; set; }

        public Plane (Vector2 start, Vector2 end)
        {
            origin = new Vector2(start);
            normal = new Vector2(end - start);
            normal = normal.rotate(-(float)Math.PI / 2);
            normal /= normal.length();
        }

        public int whichSide(Vector2 point)
        {
            float castedOrigin = normal * origin;
            float castedPoint = normal * point;

            if (castedOrigin + float.Epsilon > castedPoint)
            {
                return -1;
            }

            if (castedPoint + float.Epsilon > castedOrigin)
            {
                return 1;
            }

            return 0;
        }

        public float penetration(Vector2 point)
        {
            float castedOrigin = origin * normal;
            float castedPoint = normal * point;
            return castedPoint - castedOrigin;
        }

        public void DrawMe(Graphics g)
        {
            var other = origin + 30*normal;
            g.DrawLine(Pens.Green, origin.X, origin.Y, other.X, other.Y);
        }

        
    }
}

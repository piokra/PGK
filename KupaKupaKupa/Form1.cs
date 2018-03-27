using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KupaKupaKupa
{
    public class BG
    {
        Bitmap bitmap = new Bitmap("shrek.jpg");
        int dx = 0, dy = 0;
        Rectangle rc;
        public BG(Rectangle clientRectangle)
        {
            rc = clientRectangle;
        }

        public void DrawMe(Graphics g)
        {
            g.DrawImage(bitmap, 0, 0, new Rectangle(dx, dy, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
            g.DrawImage(bitmap, bitmap.Width-dx, 0, new Rectangle(0, dy, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
        }
        public void MoveMe(int dx, int dy)
        {
            this.dx += dx + bitmap.Width;
            this.dx %= bitmap.Width;

            this.dy += dy + bitmap.Height;
            this.dy %= bitmap.Height;
        }

    }

    public class Duszek
    {
        
        public Duszek(int x, int y)
        {
            bb = new Rectangle(x, y, 50, 50);

            convex2D = Convex2D.ApproxCircle(7, 25);
            convex2D.origin = new Vector2
            {
                X = x - 25,
                Y = y - 25,
            };

            var random = new Random();

            vX = random.Next(-9, 9);
            vX = Math.Abs(vX) == 0 ? 1 : vX;


            vY = random.Next(-9, 9);
            vY = Math.Abs(vY) == 0 ? 1 : vY;

            v = new Vector2(vX, vY);
        }

        public void DrawMe(Graphics g)
        {
            
            g.DrawPie(Pens.Black, bb, deg+(vX < 0 ? -180: 0), -deg*2+360);
            g.DrawLine(Pens.Purple, bb.Location.X + 25, bb.Location.Y + 25, bb.Location.X + 25 + 10*v.X, bb.Location.Y + 25 + 10*v.Y);
            convex2D.DrawMe(g);
        }

        public void ProcessMe()
        {
            deg += 5f;
            if (deg > 60)
                deg = 0;
            bb.Location = new Point(bb.Location.X + vX, bb.Location.Y + vY);
            convex2D.origin = new Vector2(bb.Location.X + 25, bb.Location.Y + 25);
        }

        public void CollideWith(Duszek innyDuszek)
        {
            if (innyDuszek == this)
                return;

            Plane plane = innyDuszek.convex2D.maxPenPlane(convex2D);
            
            if (plane == null)
                return;

            Vector2 normal = plane.normal;
            Vector2 mnormal = -plane.normal;
            if (normal * v > 0 && normal * innyDuszek.v < 0)
                return;            
            float dvn = v * normal;
            float dovn = innyDuszek.v * normal;

            v = v - 2 * dvn * normal;
            innyDuszek.v = innyDuszek.v - 2 * dovn * normal; 
            
            vX = (int)v.X;
            vY = (int)v.Y;

            innyDuszek.vX = (int)innyDuszek.v.X;
            innyDuszek.vY = (int)innyDuszek.v.Y;

            bb.Location = new Point(bb.X + (int)(10*normal.X), bb.Y + (int)(10*normal.Y));
            ProcessMe();
            innyDuszek.ProcessMe();
        }

        public float CEnergy()
        {
            return v.length() * v.length();
        }

        public void CollideWith(Rectangle boundaries)
        {
            if (boundaries.Right < bb.Right)
            {
                vX = -1 * Math.Abs(vX);
                v.X = -1 * Math.Abs(v.X);
            }
            if (boundaries.Left > bb.Left)
            {
                vX = Math.Abs(vX);
                v.X = Math.Abs(v.X);
            }
            if (boundaries.Top > bb.Top)
            {
                vY = Math.Abs(vY);
                v.Y = Math.Abs(v.Y);
            }
            if (boundaries.Bottom < bb.Bottom)
            {
                vY = -1 * Math.Abs(vY);
                v.Y = -1 * Math.Abs(v.Y);
            }
       }

        Rectangle bb;
        Convex2D convex2D;

        public Vector2 V { get { return v; } }
        Vector2 v;
        int vX;
        int vY;
        int r = 25;
        float deg = 0;
    }



    public partial class Form1 : Form
    {
        //????
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        Bitmap bitmap;
        List<Duszek> duszki;

        Bitmap skewXBitmap(Bitmap bitmap, double skew)
        {
            
            double width = bitmap.Width + skew * bitmap.Height;
            int iWidth = (int)width;
            var retBitmap = new Bitmap(iWidth, bitmap.Height);


            for (int i = 0; i < bitmap.Height; ++i)
            {
                int j0 = (int)((bitmap.Height - i) * skew);
                for (int j = j0; j < j0 + bitmap.Width; ++j)
                {
                    int nJ = j - (int)((bitmap.Height - i) * skew);
                    nJ = Math.Min(nJ, bitmap.Width - 1);
                    retBitmap.SetPixel(j, i, bitmap.GetPixel(nJ, i));
                }
            }
            return retBitmap;
        }

        Bitmap skewYBitmap(Bitmap bitmap, double skew)
        {

            var rotatedBitmap = new Bitmap(bitmap);
            rotatedBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var ret = skewXBitmap(rotatedBitmap, skew);
            ret.RotateFlip(RotateFlipType.Rotate270FlipNone);
            return ret;
        }
        BG bg;
        public Form1()
        {
            bitmap = new Bitmap(1000, 1000);
            bg = new KupaKupaKupa.BG(ClientRectangle);
            MouseMove += Form1_MouseMove;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            duszki = new List<Duszek>();
            duszki.Add(new KupaKupaKupa.Duszek(100, 100));
            duszki.Add(new KupaKupaKupa.Duszek(500, 500));
            duszki.Add(new KupaKupaKupa.Duszek(300, 300));
            duszki.Add(new KupaKupaKupa.Duszek(500, 300));
            duszki.Add(new KupaKupaKupa.Duszek(300, 500));



            //var image = Image.FromFile("shrek.jpg");
            //var imageBitmap = new Bitmap(image);
            //bitmap = skewYBitmap(imageBitmap, 1.5);
            //bitmap = skewXBitmap(bitmap, 2.0);
            InitializeComponent();
        }

        private Point previous = new Point(0, 0);
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Point current = new Point(e.X, e.Y);
            int dx = previous.X - current.X, dy = previous.Y - current.Y;
            bg.MoveMe(-dx, -dy);
            previous = current;
            
        }

        int dX = 15;
        int dY = 11;
        private void timer1_Tick(object sender, EventArgs e)
        {
            float totalEnergy = 0;
            Vector2 momentum = new Vector2();
            foreach (var duszek in duszki)
            {
                duszek.ProcessMe();
                totalEnergy += duszek.CEnergy();
                momentum += duszek.V;
                foreach (var innyDuszek in duszki)
                {
                    duszek.CollideWith(innyDuszek);
                }
                duszek.CollideWith(ClientRectangle);
            }

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                bg.DrawMe(g);

                foreach (var duszek in duszki)
                {
                    duszek.DrawMe(g);
                }
                var font = new Font("Times New Roman", 12.0f);
                g.DrawString(totalEnergy.ToString(), font, Brushes.Red, new PointF(10, 10));

                g.DrawString(momentum.ToString(), font, Brushes.Red, new PointF(10, 30));
            }

            using (Graphics g = CreateGraphics())
            {
                g.DrawImage(bitmap, 0, 0);
            }
            SuspendLayout();
            
        }



        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            
            //e.Graphics.DrawEllipse(Pens.Black, x, y, elipseWidth, elipseHeight);
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
   
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}

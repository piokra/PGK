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
            g.DrawImage(bitmap, 0, 0, new Rectangle(dx, dy, 500, 500), GraphicsUnit.Pixel);
        }
        public void MoveMe(int dx, int dy)
        {
            this.dx += dx;
            dx %= bitmap.Width;

            this.dy += dy;
            dy %= bitmap.Height;
        }

    }

    public class Duszek
    {
        
        public Duszek(int x, int y)
        {
            bb = new Rectangle(x, y, 50, 50);

            var random = new Random();

            vX = random.Next(-5, 5);
            vX = Math.Abs(vX) == 0 ? 1 : vX;


            vY = random.Next(-5, 5);
            vY = Math.Abs(vY) == 0 ? 1 : vY;
        }

        public void DrawMe(Graphics g)
        {
            
            g.DrawPie(Pens.Black, bb, deg+(vX < 0 ? -180: 0), -deg*2+360);
        }

        public void ProcessMe()
        {
            deg += 5f;
            if (deg > 60)
                deg = 0;
            bb.Location = new Point(bb.Location.X + vX, bb.Location.Y + vY);
        }

        public void CollideWith(Duszek innyDuszek)
        {
            if (innyDuszek == this)
                return;

            var ovX = innyDuszek.vX;
            var ovY = innyDuszek.vY;

            var boundaries = innyDuszek.bb;

            var diffLocation = new Point(bb.X - innyDuszek.bb.X, bb.Y - innyDuszek.bb.Y);

            var distanceSquared = diffLocation.X * diffLocation.X + diffLocation.Y * diffLocation.Y;

            var r2 = innyDuszek.r + r;
            if (distanceSquared > r2*r2)
                return;

            vX *= -1;
            vY *= -1;

            innyDuszek.vX *= -1;
            innyDuszek.vY *= -1;

            ProcessMe();
            innyDuszek.ProcessMe();
        }

        public void CollideWith(Rectangle boundaries)
        {
            if (boundaries.Right < bb.Right)
                vX = -1 * Math.Abs(vX);
            if (boundaries.Left > bb.Left)
                vX = Math.Abs(vX);
            if (boundaries.Top > bb.Top)
                vY = Math.Abs(vY);
            if (boundaries.Bottom < bb.Bottom)
                vY = -1 * Math.Abs(vY);
        }

        Rectangle bb;
        
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
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
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
            bg = new KupaKupaKupa.BG(ClientRectangle);
            MouseMove += Form1_MouseMove;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            duszki = new List<Duszek>();
            duszki.Add(new KupaKupaKupa.Duszek(100, 100));
            duszki.Add(new KupaKupaKupa.Duszek(500, 500));
            duszki.Add(new KupaKupaKupa.Duszek(300, 300));

            

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

            foreach (var duszek in duszki)
            {
                duszek.ProcessMe();
                foreach (var innyDuszek in duszki)
                {
                    duszek.CollideWith(innyDuszek);
                }
                duszek.CollideWith(ClientRectangle);
            }
            SuspendLayout();
            Invalidate();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            var g = e.Graphics;
            bg.DrawMe(e.Graphics);
            {
                foreach (var duszek in duszki)
                {
                    duszek.DrawMe(g);
                }
            }
            ResumeLayout();
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

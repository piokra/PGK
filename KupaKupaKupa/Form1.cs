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
        private bool toKupaOrNotToKupa = false;
        Bitmap bitmap;
        public Form1()
        {
            bitmap = new Bitmap(elipseWidth+10, elipseHeight+10);
            InitializeComponent();
        }

        int dX = 15;
        int dY = 11;
        int x = 0;
        int y = 0;
        int elipseWidth = 60;
        int elipseHeight = 12;
        private void timer1_Tick(object sender, EventArgs e)
        {
            x += dX;
            y += dY;

            if (dX > 0)
            {
                if (ClientRectangle.Width - elipseWidth < x+dX/2)
                {
                    dX = -dX;
                }
            }
            else
            {
                if (x < dX/2)
                {
                    dX = -dX;
                }
            }

            if (dY > 0)
            {
                if (ClientRectangle.Height - elipseHeight < y+dY/2)
                {
                    dY = -dY;
                }
            }
            else
            {
                if (y < dY/2)
                {
                    dY = -dY;
                }
            }

            Invalidate();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawEllipse(Pens.Black, 0, 0, elipseWidth, elipseHeight);
            }
            
            
        }



        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawEllipse(Pens.Black, x, y, elipseWidth, elipseHeight);
            e.Graphics.DrawImage(bitmap, x, y);
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    e.IsInputKey = true;
                    break;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down: 
                    e.IsInputKey = true;
                    break;
            }
   
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    dX -= 1;
                    break;
                case Keys.Right:
                    dX += 1;
                    break;
                case Keys.Up:
                    dY -= 1;
                    break;
                case Keys.Down:
                    dY += 1;
                    break;
            }
        }
    }
}

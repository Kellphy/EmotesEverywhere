using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EmotesEverywhere
{
    public partial class Window : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        public string baselink = "http://kellphy.com/emotes/";
        public string temp_path = Path.Combine(Path.GetTempPath(), "EmotesEverywhere");

        Pen pen, penBorder;

        public void Borderless()
        {
            FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }
        public void RefreshWindow()
        {
            ColorProfiles();
            SettingsRefresh();
        }
        public virtual void SettingsRefresh()
        {
            TopMost = (bool)Properties.Settings.Default["AOT"];
        }
        public virtual void ColorProfiles()
        {
            BackColor = (Color)Properties.Settings.Default["Color_BG"];
            for (int ix = Controls.Count - 1; ix >= 0; ix--)
            {
                if (Controls[ix] is Button)
                {
                    Controls[ix].BackColor = (Color)Properties.Settings.Default["Button_BG"];
                    Controls[ix].ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                }
                else if (Controls[ix] is Label)
                {
                    Controls[ix].ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                }
            }
        }
        public virtual void SaveColors()
        {
            Properties.Settings.Default.Save();
            ColorProfiles();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            pen = new Pen(new SolidBrush((Color)Properties.Settings.Default["Outline"]), 4);
            for (int ix = Controls.Count - 1; ix >= 0; ix--)
            {
                if (Controls[ix] is Button)
                {
                    e.Graphics.DrawRectangle(pen, Controls[ix].Location.X, Controls[ix].Location.Y, Controls[ix].Width, Controls[ix].Height);
                }
            }

            penBorder = new Pen(new SolidBrush((Color)Properties.Settings.Default["Button_BG"]), 4);
            Rectangle border = ClientRectangle;
            e.Graphics.DrawRectangle(penBorder, 2,2,Width-5,Height-5);


        }

        Point titleStart;
        public bool titleDrag = false;
        public void title_MouseDown(object sender, MouseEventArgs e)
        {
            titleStart = e.Location;
            titleDrag = true;
        }

        public void title_MouseUp(object sender, MouseEventArgs e)
        {
            titleDrag = false;
        }

        public void title_MouseMove(object sender, MouseEventArgs e)
        {
            if (titleDrag)
            {
                Point p1 = new Point(e.X, e.Y);
                Point p2 = PointToScreen(p1);
                Point p3 = new Point(p2.X - titleStart.X, p2.Y - titleStart.Y);
                Location = p3;
            }
        }
    }
}
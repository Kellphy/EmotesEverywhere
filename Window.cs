using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KEE
{
    public partial class Window : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        public void Border(bool border)
        {
            if (!border)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            }
        }
        public void RefreshWindow()
        {
            ColorProfiles();
            SettingsRefresh();
        }
        private void SettingsRefresh()
        {
            string curFile = $"{Environment.CurrentDirectory}\\KEE.exe.config";
            if (File.Exists(curFile))
            {
                this.TopMost = (bool)Properties.Settings.Default["AOT"];
            }
            else
            {
                this.TopMost = false;
            }
        }
        public virtual void ColorProfiles()
        {
            Color color_bg, color_fg, button_bg;
            string curFile = $"{Environment.CurrentDirectory}\\KEE.exe.config";
            if (File.Exists(curFile))
            {
                color_bg = (Color)Properties.Settings.Default["Color_BG"];
                color_fg = (Color)Properties.Settings.Default["Color_FG"];
                button_bg = (Color)Properties.Settings.Default["Button_BG"];
            }
            else
            {
                color_bg = new Form1().color_bg;
                color_fg = new Form1().color_fg;
                button_bg = new Form1().button_bg;
            }

            this.BackColor = color_bg;
            for (int ix = this.Controls.Count - 1; ix >= 0; ix--)
            {
                if (this.Controls[ix] is Button)
                {
                    this.Controls[ix].BackColor = button_bg;
                    this.Controls[ix].ForeColor = color_fg;
                }
                else if (this.Controls[ix] is Label)
                {
                    this.Controls[ix].ForeColor = color_fg;
                }
            }
        }
    }
}
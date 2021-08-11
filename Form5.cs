using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EmotesEverywhere
{
    public partial class Form5 : Window
    {
        public event ClosePanelHandler ClosePanel;
        public delegate void ClosePanelHandler(object sender, EventArgs e);
        public Form5()
        {
            InitializeComponent();
        }
        private Form1 mainForm = null;
        public Form5(Form callingForm)
        {
            mainForm = callingForm as Form1;
            InitializeComponent();
        }
        private void Form5_Load(object sender, EventArgs e) { }
        public void Start()
        {
            label1.MouseDown += title_MouseDown;
            label1.MouseUp += title_MouseUp;
            label1.MouseMove += title_MouseMove;
            pictureBox2.MouseDown += title_MouseDown;
            pictureBox2.MouseUp += title_MouseUp;
            pictureBox2.MouseMove += title_MouseMove;
            Borderless();
            RefreshWindow();
            StartPosition = FormStartPosition.Manual;
            Location = new Point(mainForm.Location.X + mainForm.Width / 2, mainForm.Location.Y + mainForm.Height / 2 - Height / 2);
            label1.Text = "Edit the selected image.";
            MergeButton_Update(mainForm.MergeOnOff);
            Show();
        }
        public override void SaveColors()
        {
            base.SaveColors();
            Invalidate();
            ClosePanel(this, new EventArgs());
        }
        public override void ColorProfiles()
        {
            base.ColorProfiles();

            pictureBox2.BackColor = (Color)Properties.Settings.Default["Button_BG"];
            label1.BackColor = (Color)Properties.Settings.Default["Button_BG"];
        }
        private void button16_Click(object sender, EventArgs e)
        {
            Dispose();
        }
        void Image_Edit(int option)
        {
            if (mainForm.PictureBoxName.ToLower() != "picturebox1")
            {
                SixLabors.ImageSharp.Image baseImage = SixLabors.ImageSharp.Image.Load(Path.Combine(temp_path, mainForm.PictureBoxName));
                SixLabors.ImageSharp.Image newImage = baseImage.Clone(ipc =>
                {
                    switch (option)
                    {
                        case 1: ipc.Grayscale(); break;
                        case 2: ipc.Brightness(1.1f); break;
                        case 3: ipc.Brightness(0.9f); break;
                        case 4: ipc.Contrast(1.1f); break;
                        case 5: ipc.Contrast(0.9f); break;
                        case 6: ipc.Flip(FlipMode.Horizontal); break;
                        case 7: ipc.Flip(FlipMode.Vertical); break;
                        case 8: ipc.GaussianSharpen(); break;
                        case 9: ipc.EntropyCrop(); break;
                        case 10: ipc.Dither(); break;
                        case 11: ipc.AdaptiveThreshold(); break;
                        case 12: ipc.HistogramEqualization(); break;
                        case 13: ipc.Invert(); break;
                        case 14: ipc.Kodachrome(); break;
                        case 15: ipc.Lomograph(); break;
                        case 16: ipc.OilPaint(); break;
                        case 17: ipc.Pixelate(); break;
                        case 18: ipc.Polaroid(); break;
                        case 19: ipc.Sepia(); break;
                        case 20: ipc.Vignette(); break;
                        case 21: ipc.Glow(SixLabors.ImageSharp.Color.White); break;
                        case 22: ipc.Glow(SixLabors.ImageSharp.Color.Black); break;
                        case 23: ipc.Hue(20); break;
                        case 24: ipc.Hue(-20); break;
                        case 25: ipc.Saturate(1.4f); break;
                        case 26: ipc.Saturate(0.6f); break;
                        case 27: ipc.Skew(5, 0); break;
                        case 28: ipc.Skew(-5, 0); break;
                        case 29: ipc.Skew(0, 5); break;
                        case 30: ipc.Skew(0, -5); break;
                        case 31: ipc.ColorBlindness(ColorBlindnessMode.Achromatomaly); break;
                        case 32: ipc.ColorBlindness(ColorBlindnessMode.Achromatopsia); break;
                        case 33: ipc.ColorBlindness(ColorBlindnessMode.Deuteranomaly); break;
                        case 34: ipc.ColorBlindness(ColorBlindnessMode.Deuteranopia); break;
                        case 35: ipc.ColorBlindness(ColorBlindnessMode.Protanomaly); break;
                        case 36: ipc.ColorBlindness(ColorBlindnessMode.Protanopia); break;
                        case 37: ipc.ColorBlindness(ColorBlindnessMode.Tritanomaly); break;
                        case 38: ipc.ColorBlindness(ColorBlindnessMode.Tritanopia); break;
                    }
                });
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    SixLabors.ImageSharp.Formats.IImageEncoder imageEncoder = newImage.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);

                    newImage.Save(memoryStream, imageEncoder);

                    (new Bitmap(memoryStream)).Save(Path.Combine(temp_path, "generated.png"));
                    mainForm.Merge_Execution(new Bitmap(memoryStream));
                }
            }
        }
        private void button1_Click(object sender, EventArgs e) { Image_Edit(1); }
        private void button2_Click(object sender, EventArgs e) { Image_Edit(2); }
        private void button3_Click(object sender, EventArgs e) { Image_Edit(3); }
        private void button4_Click(object sender, EventArgs e) { Image_Edit(4); }
        private void button5_Click(object sender, EventArgs e) { Image_Edit(5); }
        private void button6_Click(object sender, EventArgs e) { Image_Edit(6); }
        private void button7_Click(object sender, EventArgs e) { Image_Edit(7); }
        private void button8_Click(object sender, EventArgs e) { Image_Edit(8); }
        private void button9_Click(object sender, EventArgs e) { Image_Edit(9); }
        private void button10_Click(object sender, EventArgs e) { Image_Edit(10); }
        private void button11_Click(object sender, EventArgs e) { Image_Edit(11); }
        private void button12_Click(object sender, EventArgs e) { Image_Edit(12); }
        private void button13_Click(object sender, EventArgs e) { Image_Edit(13); }
        private void button14_Click(object sender, EventArgs e) { Image_Edit(14); }
        private void button15_Click(object sender, EventArgs e) { Image_Edit(15); }
        private void button17_Click(object sender, EventArgs e) { Image_Edit(16); }
        private void button18_Click(object sender, EventArgs e) { Image_Edit(17); }
        private void button19_Click(object sender, EventArgs e) { Image_Edit(18); }
        private void button20_Click(object sender, EventArgs e) { Image_Edit(19); }
        private void button21_Click(object sender, EventArgs e) { Image_Edit(20); }
        private void button22_Click(object sender, EventArgs e) { Image_Edit(21); }
        private void button23_Click(object sender, EventArgs e) { Image_Edit(22); }
        private void button24_Click(object sender, EventArgs e) { Image_Edit(23); }
        private void button25_Click(object sender, EventArgs e) { Image_Edit(24); }
        private void button26_Click(object sender, EventArgs e) { Image_Edit(25); }
        private void button27_Click(object sender, EventArgs e) { Image_Edit(26); }
        private void button28_Click(object sender, EventArgs e) { Image_Edit(27); }
        private void button29_Click(object sender, EventArgs e) { Image_Edit(28); }
        private void button30_Click(object sender, EventArgs e) { Image_Edit(29); }
        private void button31_Click(object sender, EventArgs e) { Image_Edit(30); }
        private void button32_Click(object sender, EventArgs e) { Image_Edit(31); }
        private void button33_Click(object sender, EventArgs e) { Image_Edit(32); }
        private void button34_Click(object sender, EventArgs e) { Image_Edit(33); }
        private void button35_Click(object sender, EventArgs e) { Image_Edit(34); }
        private void button36_Click(object sender, EventArgs e) { Image_Edit(35); }
        private void button37_Click(object sender, EventArgs e) { Image_Edit(36); }
        private void button38_Click(object sender, EventArgs e) { Image_Edit(37); }
        private void button39_Click(object sender, EventArgs e) { Image_Edit(38); }
        private void button40_Click(object sender, EventArgs e)
        {
            mainForm.MergeOnOff = !mainForm.MergeOnOff;
            MergeButton_Update(mainForm.MergeOnOff);
        }
        void MergeButton_Update(bool merge)
        {
            if (merge)
            {
                button40.BackColor = (Color)Properties.Settings.Default["Copy"];
                button40.Text = "Merge ON";
            }
            else
            {
                button40.BackColor = (Color)Properties.Settings.Default["Error"];
                button40.Text = "Merge OFF";
            }
        }
    }
}
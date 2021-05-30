using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KEE
{
    public partial class Form1 : Window
    {
        #region variables
        public int option, integer, division, page, paging;
        public string searchEmotes, firstLabel;
        public static bool processStop;

        public List<PictureBox> pictureList = new List<PictureBox>();
        public List<string> emoteString;
        public FlowLayoutPanel flowPanel = new FlowLayoutPanel();

        string temp_path = Path.Combine(Path.GetTempPath(), "KEE");

        MatchCollection matches;
        SemaphoreSlim semaphore = new SemaphoreSlim(1);
        #endregion
        #region pre
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        #endregion
        #region Start
        public Form1()
        {
            FindOrCreate();
            InitializeComponent();
        }
        public void Form1_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory(temp_path);
            Temp_Clean();
            RefreshWindow();

            flowPanel = new FlowLayoutPanel();
            Controls.Add(flowPanel);

            option = 1;
            division = 10;
            page = 0;
            paging = division * 100;
            searchEmotes = "Emote to Search";
            firstLabel = "Bottom left corner for info. Drag and Drop with RMB for best results.";
            label1.Text = firstLabel;
            textBox2.ForeColor = (Color)Properties.Settings.Default["Color_FG"];

            processStop = false;

            ImageFirstGetting();
            NewSearch();
        }
        #endregion
        #region AppData
        public void Temp_Clean()
        {
            DirectoryInfo di = new DirectoryInfo(temp_path);
            bool empty = true;
            foreach (FileInfo file in di.GetFiles())
            {
                empty = false;
                break;
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                empty = false;
                break;
            }
            if (!empty)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }
        #endregion
        #region First Get
        public void ImageFirstGetting()
        {
            emoteString = new List<string>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baselink);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();
                    Regex regex = new Regex(GetDirectoryListingRegexForUrl(baselink));
                    matches = regex.Matches(html);
                }
            }
        }
        public string GetDirectoryListingRegexForUrl(string url)
        {
            if (url.Equals(baselink))
            {
                return "alt=\"\\[IMG\\]\"></td><td><a href=\".*\">(?<name>.*?)</a>";
            }
            throw new NotSupportedException();
        }
        #endregion
        #region Search
        public async void NewSearch()
        {
            try
            {
                processStop = true;

                await semaphore.WaitAsync();

                textBox2.Text = textBox2.Text.Trim(' ');

                if (textBox2.Text == searchEmotes || textBox2.Text.Length < 1)
                {
                    label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                    label1.Text = firstLabel;
                    await ImageFilter();
                }
                else
                {
                    label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                    label1.Text = $"Searching for {textBox2.Text.ToLower()} ...";
                    await ImageFilter(textBox2.Text.ToLower());
                }

                semaphore.Release();
            }
            catch (Exception ex) { SendErrorMessage("1 " + ex.Message.ToString()); }
        }
        public async Task ImageFilter(string keyword = "")
        {
            processStop = false;

            flowPanel.Dispose();
            flowPanel = new FlowLayoutPanel
            {
                Size = new Size(558, 268),
                Location = new Point(14, 103),
                //BorderStyle = BorderStyle.None,
                AutoScroll = true,
            };
            Controls.Add(flowPanel);

            //GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            integer = 0;
            emoteString = new List<string>();
            pictureList = new List<PictureBox>();

            ImageGetting(keyword);
            label2.Text = $"{emoteString.Count} Search Results";
            label3.Text = $"{page + 1} / {emoteString.Count / paging + 1}";

            int emotesOnPage = Math.Min(paging, emoteString.Count - page * paging);
            for (int x = 0; x < emotesOnPage; x++)
            {
                if (processStop) break;
                await Task.Run(() => ImageLoading(x));
                flowPanel.Controls.Add(pictureList[x]);
                label4.Text = $"{integer} / {emotesOnPage} Loaded";
            }
        }
        public void ImageLoading(int y)
        {
            PictureBox picture = new PictureBox()
            {
                Name = emoteString[y + page * paging],
                TabStop = false,
                //FlatStyle = FlatStyle.Flat,
                Size = new Size(48, 48),
                Image = GetImage(emoteString[y + page * paging]),
            };
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            //if (emoteString[y + page * paging] .Substring(emoteString[y + page * paging].Length - 4, 4) == ".gif")
            //{
            //    button.FlatAppearance.BorderColor = Color.FromArgb(200, 50, 100, 150);
            //    button.FlatAppearance.BorderSize = 3;
            //}
            //button.FlatAppearance.BorderSize = 0;
            picture.Click += buttonGenerated_Click;
            picture.MouseMove += buttonGenerated_MouseMove;

            pictureList.Add(picture);
            integer++;
        }
        public Image GetImage(string filename)
        {
            string path = Path.Combine(temp_path, filename);
            try
            {
                if (!File.Exists(path))
                {
                    DownloadImage(filename, path);
                }
                return Image.FromFile(path);
            }
            catch
            {
                DownloadImage(filename, path);
                return Image.FromFile(path);
            }
        }
        public Image DownloadImage(string filename, string path)
        {
            using (Stream stream2 = WebRequest.Create($"{baselink}{filename}").GetResponse().GetResponseStream())
            using (FileStream fs = File.Create(path))
            {
                stream2.CopyTo(fs);
            }
            return Image.FromFile(path);
        }
        public void ImageGetting(string keyword)
        {
            if (matches.Count > 0)
            {
                string temp;
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        temp = match.Groups["name"].ToString();
                        if (temp.Substring(0, temp.Length - 4) == keyword)
                        {
                            Execution(temp);
                        }
                        if (temp.Contains(keyword))
                        {
                            emoteString.Add(temp);
                        }
                    }
                }
            }
        }
        public void Execution(string emoteToSearch)
        {
            try
            {
                string link = $"{baselink}{emoteToSearch}";

                Stream stream;
                using (WebClient webClient = new WebClient())
                {
                    stream = webClient.OpenRead($"{baselink}{emoteToSearch}");
                }

                Image i = Image.FromStream(stream);
                Bitmap image = new Bitmap(i);

                switch (option)
                {
                    case 1:
                        Bitmap blank = new Bitmap(Convert.ToInt32(i.Width), Convert.ToInt32(i.Height));
                        Graphics g = Graphics.FromImage(blank);
                        g.Clear(Color.FromArgb(54, 57, 63));
                        g.DrawImage(i, 0, 0, Convert.ToInt32(i.Width), Convert.ToInt32(i.Height));

                        Bitmap tempImage = new Bitmap(blank);
                        blank.Dispose();

                        Clipboard.SetImage(new Bitmap(tempImage));
                        tempImage.Dispose();
                        break;
                    case 2:
                        new Option2().Start(image);
                        //new Transparency().CopyTransparentImageToClipboard(i);
                        //IDataObject clipboard = Clipboard.GetDataObject();
                        //string datas = new Transparency().GetFormatList(clipboard);
                        //MessageBox.Show(datas);
                        break;
                    case 3:
                        Clipboard.SetText(link);
                        break;
                    default:
                        break;
                }

                i.Dispose();
                image.Dispose();

                label1.ForeColor = (Color)Properties.Settings.Default["Copy"];
                label1.Text = $"{emoteToSearch} - Copied to clipboard!";
                textBox2.Focus();
            }
            catch (Exception ex) { SendErrorMessage("3 " + ex.Message.ToString()); }
        }
        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyValue >= 0x30 && e.KeyValue <= 0x39) // numbers
                || (e.KeyValue >= 0x41 && e.KeyValue <= 0x5A) // letters
                || (e.KeyValue >= 0x60 && e.KeyValue <= 0x69) // numpad
                || (e.KeyValue == 0x08) || (e.KeyValue == 0x2E)) // backspace + delete
            {
                page = 0;
                NewSearch();
            }
        }
        #endregion
        #region Generated Events
        private void buttonGenerated_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    string filename = (sender as PictureBox).Name;

                    pictureBox1.Image = GetImage(filename);

                    string path = Path.Combine(temp_path, filename);
                    string[] paths = new[] { path };
                    DoDragDrop(new DataObject(DataFormats.FileDrop, paths), DragDropEffects.Copy);
                }
                catch (Exception ex) { SendErrorMessage("4 " + ex.Message.ToString()); }
            }
        }
        private void buttonGenerated_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = (sender as PictureBox).Name;
                pictureBox1.Image = GetImage(filename);
                Execution(filename);
            }
            catch (Exception ex) { SendErrorMessage("5 " + ex.Message.ToString()); }
        }
        #endregion
        #region Options
        private void button2_Click(object sender, EventArgs e)
        {
            option = 1;
            label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
            label1.Text = "Now using the 1st Option.";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            option = 2;
            label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
            label1.Text = "Now using the 2nd Option.";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            option = 3;
            label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
            label1.Text = "Now using the 3rd Option.";
        }
        #endregion
        #region Links
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel1, "http://kellphy.com/");
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel2, "https://kellphy.com/discord");
        }
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel4, "https://github.com/Kellphy/KEE/releases/");
        }
        public void VisitLink(LinkLabel label, string link)
        {
            try
            {
                label.LinkVisited = true;
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception ex) { SendErrorMessage("0 " + ex.Message.ToString()); }
        }
        #endregion
        #region Pages & Reset
        private void button6_Click(object sender, EventArgs e)
        {
            if (page > 0)
            {
                page--;
                NewSearch();
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (page < emoteString.Count / paging)
            {
                page++;
                NewSearch();
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            page = 0;
            TextColor(textBox2, searchEmotes, "", (Color)Properties.Settings.Default["Color_NonText"], true);
            NewSearch();
        }
        #endregion
        #region Textbox Test
        private void textBox2_Enter(object sender, EventArgs e)
        {
            TextColor(textBox2, "", searchEmotes, (Color)Properties.Settings.Default["Color_FG"]);
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            TextColor(textBox2, searchEmotes, "", (Color)Properties.Settings.Default["Color_NonText"]);
        }
        public void TextColor(TextBox textBox, string text, string reqText, Color color, bool forcedReplace = false)
        {
            if (textBox.Text == reqText || forcedReplace)
            {
                textBox.ForeColor = color;
                textBox.Text = text;
            }
        }
        #endregion
        #region Errors
        public void SendErrorMessage(string error)
        {
            label1.ForeColor = (Color)Properties.Settings.Default["Error"];
            label1.Text = error;
        }
        #endregion
        #region Info, Profiles, Settings
        //Info
        private void button1_Click(object sender, EventArgs e)
        {
            new Form2().Start();
        }
        //Profiles
        private void button5_Click(object sender, EventArgs e)
        {
            Form3 Frm = new Form3();
            Frm.ClosePanel += HandleCloseRequest;
            Frm.Start();
        }
        //Settings
        private void button9_Click(object sender, EventArgs e)
        {
            Form4 Frm = new Form4();
            Frm.ClosePanel += HandleCloseRequest;
            Frm.Start();
        }
        private void HandleCloseRequest(object sender, EventArgs e)
        {
            RefreshWindow();
        }
        #endregion
        #region Color Overrite
        public override void ColorProfiles()
        {
            FindOrCreate();

            BackColor = (Color)Properties.Settings.Default["Color_BG"];
            for (int ix = Controls.Count - 1; ix >= 0; ix--)
            {
                if (Controls[ix] is Button)
                {
                    Controls[ix].BackColor = (Color)Properties.Settings.Default["Button_BG"];
                    Controls[ix].ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                }
                else if (Controls[ix] is LinkLabel)
                {
                    ((LinkLabel)Controls[ix]).LinkColor = (Color)Properties.Settings.Default["Color_Link"];
                    ((LinkLabel)Controls[ix]).VisitedLinkColor = (Color)Properties.Settings.Default["Color_VLink"];
                }
                else if (Controls[ix] is Label)
                {
                    Controls[ix].ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                }
                else if (Controls[ix] is TextBox)
                {
                    Controls[ix].ForeColor = (Color)Properties.Settings.Default["Color_NonText"];
                    Controls[ix].BackColor = (Color)Properties.Settings.Default["TextBox_BG"];
                }
            }

            Invalidate();
        }
        #endregion
        #region Borders for textbox, picturebox, flowpanel
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen pen = new Pen(new SolidBrush((Color)Properties.Settings.Default["Color_FG"]), 4);
            e.Graphics.DrawRectangle(pen, flowPanel.Location.X, flowPanel.Location.Y, flowPanel.Width, flowPanel.Height);
            e.Graphics.DrawRectangle(pen, pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height);
            e.Graphics.DrawRectangle(pen, textBox2.Location.X, textBox2.Location.Y, textBox2.Width, textBox2.Height);
        }
        #endregion
    }
}
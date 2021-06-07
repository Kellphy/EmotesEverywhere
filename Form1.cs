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
        // Variables
        public int
            option = 1,
            integer,
            division = 10,
            page = 0,
            paging,
            paging_multiplier,
            search_length;
        public string
            searchEmotes = "Emote to Search",
            firstLabel = "Bottom left corner for info. Drag and Drop with RMB for best results.";
        public bool processStop = false;

        public List<PictureBox> pictureList = new List<PictureBox>();
        public List<string> emoteString;
        public FlowLayoutPanel flowPanel = new FlowLayoutPanel();
        public FlowLayoutPanel flowPanelFav = new FlowLayoutPanel();

        public string temp_path = Path.Combine(Path.GetTempPath(), "KEE");

        MatchCollection matches;
        SemaphoreSlim semaphore = new SemaphoreSlim(1);
        // PreStart
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        // Start
        public Form1()
        {
            FindOrCreate();
            InitializeComponent();
        }
        public void Form1_Load(object sender, EventArgs e)
        {
            flowPanel = new FlowLayoutPanel();
            flowPanelFav = new FlowLayoutPanel();
            Controls.Add(flowPanel);
            Controls.Add(flowPanelFav);

            label1.Text = firstLabel;
            ImageFirstGetting();

            Directory.CreateDirectory(temp_path);
            Temp_Clean();
            RefreshWindow();

            textBox2.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
        }
        // AppData
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
        // Window Overrites
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
        public override void SettingsRefresh()
        {
            base.SettingsRefresh();

            int temp_paging = (int)Properties.Settings.Default["Paging"];
            if (paging_multiplier != temp_paging)
            {
                paging_multiplier = temp_paging;
                paging = division * paging_multiplier;
                NewSearch();
                FlowFavorite();
            }
        }
        // First Get
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
        // Search
        public async void NewSearch()
        {
            try
            {
                processStop = true;
                await semaphore.WaitAsync();
                processStop = false;

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
            if (processStop) return;
            flowPanel.Dispose();
            flowPanel = new FlowLayoutPanel
            {
                Size = new Size(558, 272),
                Location = new Point(14, 103),
                AutoScroll = true,
            };
            Controls.Add(flowPanel);

            FlowFavorite();

            integer = 0;
            emoteString = new List<string>();
            pictureList = new List<PictureBox>();

            if (processStop) return;
            ImageGetting(keyword);
            label2.Text = $"{emoteString.Count} Search Results";
            label3.Text = $"{page + 1} / {emoteString.Count / paging + 1}";
            int emotesOnPage = Math.Min(paging, emoteString.Count - page * paging);

            for (int x = 0; x < emotesOnPage; x++)
            {
                if (processStop) break;
                await Task.Run(() => ImageLoading(x));
                if (processStop) break;
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
                Size = new Size(48, 48),
                Image = GetImage(emoteString[y + page * paging]),
            };
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.Click += buttonGenerated_Click;
            picture.MouseMove += buttonGenerated_MouseMove;

            pictureList.Add(picture);
            integer++;
        }
        public Image GetImage(string filename,string prefix = "")
        {
            string path = Path.Combine(temp_path, prefix+filename);
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
                string labelText = $"{emoteToSearch} - Copied to clipboard";

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
                        labelText = $"{emoteToSearch} - Copied to clipboard as RGB!";
                        break;
                    case 2:
                        new Option2().Start(new Bitmap(i));
                        labelText = $"{emoteToSearch} - Copied to clipboard as DiB!";
                        break;
                    case 3:
                        Clipboard.SetText(link);
                        labelText = $"{emoteToSearch} - Copied to clipboard as link!";
                        break;
                    default:
                        break;
                }

                i.Dispose();

                label1.ForeColor = (Color)Properties.Settings.Default["Copy"];
                label1.Text = labelText;
                pictureBox1.Image = GetImage(emoteToSearch);

                textBox2.Focus();
            }
            catch (Exception ex) { SendErrorMessage("3 " + ex.Message.ToString()); }
        }
        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox2.Text.Length!=search_length)
            {
                search_length = textBox2.Text.Length;
                page = 0;
                NewSearch();
            }
        }
        // Generated Events
        private void buttonGenerated_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                string filename = (sender as PictureBox).Name;
                if (e.Button == MouseButtons.Right)
                {

                    label1.ForeColor = (Color)Properties.Settings.Default["Copy"];
                    label1.Text = $"{filename} - Drag and Drop!";
                    pictureBox1.Image = GetImage(filename);

                    string path = Path.Combine(temp_path, filename);
                    string[] paths = new[] { path };
                    DoDragDrop(new DataObject(DataFormats.FileDrop, paths), DragDropEffects.Copy);

                }
            }
            catch (Exception ex) { SendErrorMessage("4 " + ex.Message.ToString()); }
        }
        private void FlowFavorite()
        {
            System.Collections.Specialized.StringCollection favorites = (System.Collections.Specialized.StringCollection)Properties.Settings.Default["Favorite"];
            if (favorites == null) favorites = new System.Collections.Specialized.StringCollection();
            if (favorites.Count < 1)
            {
                flowPanelFav.Size = new Size(558,0);
                flowPanel.Size = new Size(558,272);
            }
            else
            {
                flowPanel.Size = new Size(558,216);
                flowPanelFav.Dispose();
                flowPanelFav = new FlowLayoutPanel
                {
                    Size = new Size(558, 54),
                    Location = new Point(14, 321),
                    AutoScroll = true,
                };
                Controls.Add(flowPanelFav);

                foreach (string favorite in favorites)
                {
                    PictureBox picture = new PictureBox()
                    {
                        Name = favorite,
                        TabStop = false,
                        Size = new Size(48, 48),
                        Image = GetImage(favorite, "f_"),
                    };
                    picture.SizeMode = PictureBoxSizeMode.Zoom;
                    picture.Click += buttonGenerated_Click;
                    picture.MouseMove += buttonGenerated_MouseMove;
                    flowPanelFav.Controls.Add(picture);
                }
            }
        }
        private void buttonGenerated_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = (sender as PictureBox).Name;
                if (ModifierKeys == Keys.Shift)
                {
                    System.Collections.Specialized.StringCollection favorites = (System.Collections.Specialized.StringCollection)Properties.Settings.Default["Favorite"];
                    if (favorites == null) favorites = new System.Collections.Specialized.StringCollection();
                    if (favorites.Contains(filename))
                    {
                        favorites.Remove(filename);
                        Properties.Settings.Default["Favorite"] = favorites;
                        Properties.Settings.Default.Save();
                        label1.ForeColor = (Color)Properties.Settings.Default["Copy"];
                        label1.Text = $"{filename} - Removed from Favorites! - {favorites.Count}/20";
                    }
                    else if (favorites.Count < 20)
                    {
                        favorites.Add(filename);
                        Properties.Settings.Default["Favorite"] = favorites;
                        Properties.Settings.Default.Save();
                        label1.ForeColor = (Color)Properties.Settings.Default["Copy"];
                        label1.Text = $"{filename} - Added to Favorites! - {favorites.Count}/20";
                    }
                    else
                    {
                        label1.ForeColor = (Color)Properties.Settings.Default["Error"];
                        label1.Text = $"{filename} - Favorites Full - {favorites.Count}/20";
                    }
                    FlowFavorite();
                }
                else
                {
                    Execution(filename);
                }
            }
            catch (Exception ex) { SendErrorMessage("5 " + ex.Message.ToString()); }
        }
        // Options
        private void button2_Click(object sender, EventArgs e)
        {
            option = 1;
            label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
            label1.Text = "Saved! Now click on emotes to copy them as RGB.";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            option = 2;
            label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
            label1.Text = "Saved! Now click on emotes to copy them as Device independent Bitmap.";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            option = 3;
            label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
            label1.Text = "Saved! Now click on emotes to copy them as Links.";
        }
        // Links
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
        // Pages & Reset
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
        // Textbox Test
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
        // Errors
        public void SendErrorMessage(string error)
        {
            label1.ForeColor = (Color)Properties.Settings.Default["Error"];
            label1.Text = error;
        }
        // Info
        private void button1_Click(object sender, EventArgs e)
        {
            new Form2().Start();
        }
        // Profiles
        private void button5_Click(object sender, EventArgs e)
        {
            Form3 Frm = new Form3();
            Frm.ClosePanel += HandleCloseRequest;
            Frm.Start();
        }
        // Settings
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
        // Borders for textbox, picturebox, flowpanel
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen pen = new Pen(new SolidBrush((Color)Properties.Settings.Default["Color_FG"]), 4);
            e.Graphics.DrawRectangle(pen, flowPanel.Location.X, flowPanel.Location.Y, flowPanel.Width, flowPanel.Height);
            e.Graphics.DrawRectangle(pen, flowPanelFav.Location.X, flowPanelFav.Location.Y, flowPanelFav.Width, flowPanelFav.Height);
            e.Graphics.DrawRectangle(pen, pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height);
            e.Graphics.DrawRectangle(pen, textBox2.Location.X, textBox2.Location.Y, textBox2.Width, textBox2.Height);
        }
    }
}
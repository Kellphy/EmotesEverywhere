using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmotesEverywhere
{
    public partial class Form1 : Window
    {
        // Variables
        public int
            integer,
            division = 10,
            page = 0,
            paging,
            paging_multiplier,
            search_length;
        public string
            searchEmotes = "Search Emotes",
            firstLabel = " Drag and Drop with RMB gives best results.";
        public bool processStop = false;
        public bool merge = false;

        public List<PictureBox> pictureList = new List<PictureBox>();
        public List<string> emoteString;
        public FlowLayoutPanel flowPanel = new FlowLayoutPanel();
        public FlowLayoutPanel flowPanelFav = new FlowLayoutPanel();

        System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();

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
            if (Process.GetProcessesByName("EmotesEverywhere").Length> 1)
            {
                MessageBox.Show("Emotes Everywhere is already open!");
                Application.Exit();
            }
            InitializeComponent();
        }
        public void Form1_Load(object sender, EventArgs e)
        {
            SetVisibleCore(false);
            Borderless();
            flowPanel = new FlowLayoutPanel();
            flowPanelFav = new FlowLayoutPanel();
            Controls.Add(flowPanel);
            Controls.Add(flowPanelFav);
            linkLabel1.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabel2.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabel3.LinkBehavior = LinkBehavior.NeverUnderline;
            label1.Text = firstLabel;
            label5.Text = Text;
            label5.MouseDown += title_MouseDown;
            label5.MouseUp += title_MouseUp;
            label5.MouseMove += title_MouseMove;
            pictureBox2.MouseDown += title_MouseDown;
            pictureBox2.MouseUp += title_MouseUp;
            pictureBox2.MouseMove += title_MouseMove;
            pictureBox1.MouseMove += buttonGenerated_MouseMove;
            pictureBox1.Click += buttonGenerated_Click;
            toolTip_Settings();

            Cache_Check();

            ImageFirstGetting();

            RefreshWindow();

            SetVisibleCore(true);

            textBox2.Focus();

            CheckForUpdates();
        }

        private void Cache_Check()
        {
            //try
            {
                using (Stream stream2 = WebRequest.Create($"{baseLink}downloads/EmotesEverywhere/cache").GetResponse().GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream2))
                    {
                        string cached = reader.ReadToEnd();
                        if (cached != (string)Properties.Settings.Default["Cache"])
                        {
                            Properties.Settings.Default["Cache"] = cached;
                            Properties.Settings.Default.Save();
                            Temp_Clean();
                        }
                    }
                }
            }
            //catch
            //{
            //    Temp_Clean();
            //}
        }

        // AppData
        public void Temp_Clean()
        {
            Directory.CreateDirectory(temp_path);
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
                    ((LinkLabel)Controls[ix]).ActiveLinkColor = (Color)Properties.Settings.Default["Color_Link"];
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

            textBox2.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
            label5.BackColor = (Color)Properties.Settings.Default["Button_BG"];
            pictureBox2.BackColor = (Color)Properties.Settings.Default["Button_BG"];
            button14.ForeColor = (Color)Properties.Settings.Default["Copy"];
            ToolTip1.BackColor = (Color)Properties.Settings.Default["Button_BG"];

            Option_Button_BG();
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(emotesLink);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();
                    Regex regex = new Regex(GetDirectoryListingRegexForUrl(emotesLink));
                    matches = regex.Matches(html);
                }
            }
        }
        public string GetDirectoryListingRegexForUrl(string url)
        {
            if (url.Equals(emotesLink))
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
            catch (Exception ex) { SendErrorMessage("1 " +ex.ToString()); }
        }
        public async Task ImageFilter(string keyword = "")
        {
            if (processStop) return;
            flowPanel.Dispose();
            flowPanel = new FlowLayoutPanel
            {
                Size = new Size(562, 272),
                Location = new Point(14, 126),
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
            ToolTip1.SetToolTip(picture, picture.Name.Substring(0,picture.Name.Length-4));

            pictureList.Add(picture);
            integer++;
        }
        public Image GetImage(string filename, string prefix = "")
        {
            string path = Path.Combine(temp_path, prefix + filename);
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
            using (Stream stream2 = WebRequest.Create($"{emotesLink}{filename}").GetResponse().GetResponseStream())
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
                        if (temp.Substring(0, temp.Length - 4) == keyword && !merge)
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
            pictureBox1.Image = GetImage(emoteToSearch);
            pictureBox1.Name = emoteToSearch;
            textBox2.Focus();

            GetFocused();
        }

        public void GetFocused()
        {
            try
            {
                if (pictureBox1.Name.ToLower() == "picturebox1")
            {
                label1.ForeColor = (Color)Properties.Settings.Default["Color_FG"];
                switch (Properties.Settings.Default["Option"])
                {
                    case 1:
                        label1.Text = "Now click on emotes to copy them as RGB.";
                        break;
                    case 2:
                        label1.Text = "Now click on emotes to copy them as Device independent Bitmap.";
                        break;
                    case 3:
                        label1.Text = "Now click on emotes to copy them as Links.";
                        break;
                    case 4:
                        label1.Text = "Now click on emotes to copy them as Files.";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                string labelText = $"{pictureBox1.Name} - Copied to clipboard";

                switch (Properties.Settings.Default["Option"])
                {
                    case 1:
                        Bitmap blank = new Bitmap(Convert.ToInt32(pictureBox1.Image.Width), Convert.ToInt32(pictureBox1.Image.Height));
                        Graphics g = Graphics.FromImage(blank);
                        g.Clear(Color.FromArgb(54, 57, 63));
                        g.DrawImage(pictureBox1.Image, 0, 0, Convert.ToInt32(pictureBox1.Image.Width), Convert.ToInt32(pictureBox1.Image.Height));

                        Bitmap tempImage = new Bitmap(blank);
                        blank.Dispose();

                        Clipboard.SetImage(new Bitmap(tempImage));
                        tempImage.Dispose();
                        labelText = $"{pictureBox1.Name} - Copied to clipboard as RGB!";
                        break;
                    case 2:
                        new Option2().Start(new Bitmap(pictureBox1.Image));
                        labelText = $"{pictureBox1.Name} - Copied to clipboard as DiB!";
                        break;
                    case 3:
                        if (!merge)
                        {
                            Clipboard.SetText($"{emotesLink}{pictureBox1.Name}");
                            labelText = $"{pictureBox1.Name} - Copied to clipboard as link!";
                        }
                        else
                        {
                            labelText = "You can't copy the link of a merged image.";
                        }
                        break;
                    case 4:
                        //string prefix = "t_";
                        //GetImage(pictureBox1.Name, prefix);
                        //Clipboard.SetData(DataFormats.FileDrop, new string[] { Path.Combine(temp_path, prefix + pictureBox1.Name) });
                        Clipboard.SetData(DataFormats.FileDrop, new string[] { Path.Combine(temp_path, pictureBox1.Name) });
                        labelText = $"{pictureBox1.Name} - Copied to clipboard as file!";
                        break;
                    default:
                        break;
                }

                label1.ForeColor = (Color)Properties.Settings.Default["Copy"];
                label1.Text = labelText;
            }
            }
            catch (Exception ex) { SendErrorMessage("3 " + ex.ToString()); }
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox2.Text.Length != search_length)
            {
                search_length = textBox2.Text.Length;
                page = 0;
                NewSearch();
            }
        }
        // Generated Events
        private void buttonGenerated_MouseMove(object sender, MouseEventArgs e)
        {
            if ((sender as PictureBox).Name.ToLower() == "picturebox1") return;
            try
            {
                string filename = (sender as PictureBox).Name;
                if (e.Button == MouseButtons.Right)
                {

                    label1.ForeColor = (Color)Properties.Settings.Default["Copy"];
                    label1.Text = $"{filename} - Drag and Drop!";
                    pictureBox1.Image = GetImage(filename);
                    pictureBox1.Name = filename;

                    string path = Path.Combine(temp_path, filename);
                    string[] paths = new[] { path };
                    DoDragDrop(new DataObject(DataFormats.FileDrop, paths), DragDropEffects.Copy);

                }
            }
            catch (Exception ex) { SendErrorMessage("4 " +ex.ToString()); }
        }
        private void FlowFavorite()
        {
            System.Collections.Specialized.StringCollection favorites = (System.Collections.Specialized.StringCollection)Properties.Settings.Default["Favorite"];
            if (favorites == null) favorites = new System.Collections.Specialized.StringCollection();
            if (favorites.Count < 1)
            {
                flowPanelFav.Size = new Size(562, 0);
                flowPanel.Size = new Size(562, 272);
            }
            else
            {
                flowPanel.Size = new Size(562, 216);
                flowPanelFav.Dispose();
                flowPanelFav = new FlowLayoutPanel
                {
                    Size = new Size(562, 54),
                    Location = new Point(14, 344),
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
            if ((sender as PictureBox).Name.ToLower() == "picturebox1") return;
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
                else if (!merge || pictureBox1.Name.ToLower() == "picturebox1")
                {
                    Execution(filename);
                }
                else if (merge)
                {
                    Merge(filename);
                }
            }
            catch (Exception ex) { SendErrorMessage("5 " +ex.ToString()); }
        }
        // Options
        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Option"] = 1;
            Properties.Settings.Default.Save();
            Option_Button_BG();
            GetFocused();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Option"] = 2;
            Properties.Settings.Default.Save();
            Option_Button_BG();
            GetFocused();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Option"] = 3;
            Properties.Settings.Default.Save();
            Option_Button_BG();
            GetFocused();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Option"] = 4;
            Properties.Settings.Default.Save();
            Option_Button_BG();
            GetFocused();
        }
        private void Option_Button_BG()
        {
            int option = (int)Properties.Settings.Default["Option"];
            Button[] buttons = new Button[] { button2, button3, button4, button10 };
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == (option - 1))
                {
                    buttons[i].BackColor = (Color)Properties.Settings.Default["Button_BG"];
                }
                else
                {
                    buttons[i].BackColor = (Color)Properties.Settings.Default["Color_BG"];
                }
            }
        }
        // Links
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel1, baseLink);
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel2, $"{baseLink}discord");
        }
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel3, "https://github.com/Kellphy/KEE/releases/");
        }
        public void VisitLink(LinkLabel label, string link)
        {
            try
            {
                label.LinkVisited = true;
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception ex) { SendErrorMessage("0 " +ex.ToString()); }
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
            MessageBox.Show(error);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
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

        private void button26_Click(object sender, EventArgs e)
        {
            Form5 Frm = new Form5(this);
            Frm.ClosePanel += HandleCloseRequest;
            Frm.Start();
        }

        private void HandleCloseRequest(object sender, EventArgs e)
        {
            RefreshWindow();
        }

        // Save
        private void button15_Click(object sender, EventArgs e)
        {
            SaveFocused(true);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            SaveFocused(false);
        }
        void SaveFocused(bool withDialog)
        {
            label1.Text = "Click an emote to access its save option";
            if (pictureBox1.Name.ToLower() != "picturebox1")
            {
                label1.Text = "Quick Saving";
                if (((string)Properties.Settings.Default["Quick_Save"]).Length < 1 || withDialog)
                {
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.Filter = "All files (*.*)|*.*";
                        dialog.FilterIndex = 1;
                        dialog.RestoreDirectory = true;

                        dialog.FileName = pictureBox1.Name;
                        dialog.Title = pictureBox1.Name;

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            if (!File.Exists(dialog.FileName))
                            {
                                pictureBox1.Image.Save(dialog.FileName);
                            }

                            label1.ForeColor = (Color)Properties.Settings.Default["Copy"];

                            string quick = withDialog ? string.Empty : "Quick ";
                            label1.Text = $"{quick}Saved: " + dialog.FileName;
                            Properties.Settings.Default["Quick_Save"] = Path.GetDirectoryName(dialog.FileName);
                            Properties.Settings.Default.Save();
                        }
                    }
                }
                else
                {
                    string fullpath = Path.Combine((string)Properties.Settings.Default["Quick_Save"], pictureBox1.Name);
                    label1.Text = "Quick Saved: " + fullpath;

                    if (!File.Exists(fullpath))
                    {
                        pictureBox1.Image.Save(fullpath);
                    }
                }
            }
        }

        //Update
        void CheckForUpdates()
        {
            string name = "EmotesEverywhere";
            string webfolder = $"{baseLink}downloads/{name}";

            using (Stream stream2 = WebRequest.Create($"{webfolder}/version").GetResponse().GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream2))
                {
                    string toVer = reader.ReadToEnd();
                    if (toVer.Length > 0 && toVer != linkLabel3.Text)
                    {
                        button14.Visible = true;
                        button14.Text = $"Update to {toVer}";
                    }
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
                if (!principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                {
                    MessageBox.Show("Please run Emotes Everywhere as an Administrator!");
                    Process.Start("explorer.exe", $"{Environment.CurrentDirectory}");
                }
                else
                {
                    string name = "Updater.exe";
                    string updaterPath = Path.Combine(Environment.CurrentDirectory, $"{name}");
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFile($"{baseLink}downloads/EmotesEverywhere/{name}", updaterPath);
                    }
                    Process.Start(updaterPath);
                }
                Application.Exit();
            }
            catch (Exception ex) { SendErrorMessage("1 " + ex.ToString()); }
        }

        // Borders for textbox, picturebox, flowpanel
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen pen = new Pen(new SolidBrush((Color)Properties.Settings.Default["Outline"]), 4);
            e.Graphics.DrawRectangle(pen, flowPanel.Location.X, flowPanel.Location.Y, flowPanel.Width, flowPanel.Height);
            e.Graphics.DrawRectangle(pen, flowPanelFav.Location.X, flowPanelFav.Location.Y, flowPanelFav.Width, flowPanelFav.Height);
            e.Graphics.DrawRectangle(pen, pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height);
            e.Graphics.DrawRectangle(pen, textBox2.Location.X, textBox2.Location.Y, textBox2.Width, textBox2.Height);
        }

        void Image_Edit(int option)
        {
            if (pictureBox1.Name.ToLower() != "picturebox1")
            {
                SixLabors.ImageSharp.Image baseImage = SixLabors.ImageSharp.Image.Load(Path.Combine(temp_path, pictureBox1.Name));
                SixLabors.ImageSharp.Image newImage = baseImage.Clone(ipc =>
                {
                    switch (option)
                    {
                        case 1:
                            ipc.Grayscale();
                            break;
                        case 2:
                            ipc.Brightness(0.9f);
                            break;
                        case 3:
                            ipc.Brightness(1.1f);
                            break;
                        case 4:
                            ipc.Contrast(1.1f);
                            //ipc.ColorBlindness(ColorBlindnessMode.Achromatomaly);
                            //ipc.ColorBlindness(ColorBlindnessMode.Achromatopsia);
                            //ipc.ColorBlindness(ColorBlindnessMode.Deuteranomaly);
                            //ipc.ColorBlindness(ColorBlindnessMode.Deuteranopia);
                            //ipc.ColorBlindness(ColorBlindnessMode.Protanomaly);
                            //ipc.ColorBlindness(ColorBlindnessMode.Protanopia);
                            //ipc.ColorBlindness(ColorBlindnessMode.Tritanomaly);
                            //ipc.ColorBlindness(ColorBlindnessMode.Tritanopia);
                            break;
                        case 5:
                            ipc.Contrast(0.9f);
                            break;
                        case 6:
                            ipc.Flip(FlipMode.Horizontal);
                            break;
                        case 7:
                            ipc.Flip(FlipMode.Vertical);
                            break;
                        case 8:
                            ipc.GaussianSharpen();
                            break;
                        case 9:
                            ipc.EntropyCrop();
                            break;
                        case 10:
                            ipc.Dither();
                            break;
                        case 11:
                            ipc.AdaptiveThreshold();
                            break;
                    }
                });
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    SixLabors.ImageSharp.Formats.IImageEncoder imageEncoder = newImage.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);

                    newImage.Save(memoryStream, imageEncoder);

                    (new Bitmap(memoryStream)).Save(Path.Combine(temp_path, "generated.png"));
                    Merge_Execution(new Bitmap(memoryStream));
                }
            }
        }
        void Merge(string addImageName)
        {
            SixLabors.ImageSharp.Image baseImage = SixLabors.ImageSharp.Image.Load(Path.Combine(temp_path, pictureBox1.Name));
            SixLabors.ImageSharp.Image addImage = SixLabors.ImageSharp.Image.Load(Path.Combine(temp_path, addImageName));

            SixLabors.ImageSharp.Point toRight = new SixLabors.ImageSharp.Point(x: baseImage.Width, y: 0);
            SixLabors.ImageSharp.Point topLeft = new SixLabors.ImageSharp.Point(x: 0, y: 0);

            SixLabors.ImageSharp.Image newImage = new SixLabors.ImageSharp.Image<Rgba32>(baseImage.Width + addImage.Width, baseImage.Height);

            newImage = newImage.Clone(ipc =>
            {
                ipc.DrawImage(baseImage, topLeft, 1);
                ipc.DrawImage(addImage, toRight, 1);
            });
            using (MemoryStream memoryStream = new MemoryStream())
            {
                SixLabors.ImageSharp.Formats.IImageEncoder imageEncoder = newImage.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);

                newImage.Save(memoryStream, imageEncoder);

                (new Bitmap(memoryStream)).Save(Path.Combine(temp_path, "generated.png"));
                Merge_Execution(new Bitmap(memoryStream));
            }
        }

        public void Merge_Execution(Bitmap bmp)
        {
            pictureBox1.Image = bmp;
            pictureBox1.Name = "generated.png";
            textBox2.Focus();

            GetFocused();
        }

        public string PictureBoxName
        {
            get { return pictureBox1.Name; }
        }
        public bool MergeOnOff
        {
            get { return merge; }
            set { merge = value; }
        }


        private void toolTip_Settings()
        {
            ToolTip1.ShowAlways = true;
            ToolTip1.AutomaticDelay = 0;
            ToolTip1.AutoPopDelay = 0;
            ToolTip1.OwnerDraw = true;
            ToolTip1.InitialDelay = 0;
            ToolTip1.ReshowDelay = 0;
            ToolTip1.UseFading = true;
            ToolTip1.UseAnimation = false;
            ToolTip1.Draw += toolTip_Draw;
            ToolTip1.Popup += toolTip_Popup;
        }
        private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            //e.DrawBorder();
            //e.Graphics.DrawLines(new Pen(new SolidBrush((Color)Properties.Settings.Default["Outline"]), 4), new Point[] {
            //        new Point (0, e.Bounds.Height - 1),
            //        new Point (0, 0),
            //        new Point (e.Bounds.Width - 1, 0)
            //    });
            //e.Graphics.DrawLines(new Pen(new SolidBrush((Color)Properties.Settings.Default["Outline"]), 3), new Point[] {
            //        new Point (0, e.Bounds.Height - 1),
            //        new Point (e.Bounds.Width - 1, e.Bounds.Height - 1),
            //        new Point (e.Bounds.Width - 1, 0)
            //    });

            //e.DrawText();
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                using (Font f = new Font("Segoe", 10f, FontStyle.Bold))
                {
                    e.Graphics.DrawString(e.ToolTipText, f,
                        new SolidBrush((Color)Properties.Settings.Default["Color_FG"]), e.Bounds, sf);
                }
            }
        }
        private void toolTip_Popup(object sender, PopupEventArgs e)
        {
            using (Font f = new Font("Segoe", 10f, FontStyle.Bold))
            {
                e.ToolTipSize = TextRenderer.MeasureText(
                    ToolTip1.GetToolTip(e.AssociatedControl), f) + new Size(0, 8);
            }
        }
    }
}
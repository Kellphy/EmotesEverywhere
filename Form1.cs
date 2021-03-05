using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordCopy
{
    public partial class Form1 : Form
    {
        public int option = 1;
        public int integer;
        public int division = 8;
        public string searchEmotes = "Search For Emotes";
        public string getEmotes = "Get Emotes By Name";

        public Color textColor = Color.FromArgb(0, 48, 102);
        public Color nonTextColor = Color.FromArgb(92, 103, 125);

        public List<Image> imagesList;
        public List<Button> buttonList;
        public List<string> emoteString;

        public bool processStop = false;
        public bool processStopped = true;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Select();
            textBox1.ForeColor = nonTextColor;
            textBox2.ForeColor = nonTextColor;
            textBox1.Text = getEmotes;
            textBox2.Text = searchEmotes;
            label1.ForeColor = textColor;
            ImageFilter();
        }
        //Options
        private void button2_Click(object sender, EventArgs e)
        {
            option = 1;
            label1.ForeColor = textColor;
            label1.Text = "Now using the 1st Option";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            option = 2;
            label1.ForeColor = textColor;
            label1.Text = "Now using the 2nd Option";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            option = 3;
            label1.ForeColor = textColor;
            label1.Text = "Now using the 3rd Option";
        }
        //Links
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel1, "http://kellphy.com/");
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink(linkLabel2, "https://discord.gg/ycYmMmP");
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
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        //Get Emote
        private void button1_Click(object sender, EventArgs e)
        {
            Execution(true);
            TextColor(textBox1, getEmotes, "", nonTextColor);
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                Execution(false);
            }
        }
        //Execution
        public void Execution(bool button, string emoteToSearch = "")
        {
            try
            {
                if (textBox1.Text == getEmotes) return;
                //string outputFile = @"kee_temp.png";
                string image_name;
                if (emoteToSearch.Length > 0)
                {
                    image_name = emoteToSearch;
                }
                else
                {
                    image_name = textBox1.Text.ToLower();
                }
                string link = $"http://kellphy.com/emotes/{image_name}.png";

                //using (WebClient client = new WebClient())
                //{
                //    client.DownloadFile(new Uri(link), outputFile);
                //    client.Dispose();
                //}
                Image i = DownloadImage(link);
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
                        break;
                    case 3:
                        Clipboard.SetText(link);
                        break;
                    default:
                        break;
                }
                textBox1.Text = "";
                i.Dispose();
                image.Dispose();
                //File.Delete(outputFile);
                label1.ForeColor = Color.LightGreen;
                label1.Text = $"{image_name} - Copied to clipboard!";
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        //Search
        private void button5_Click(object sender, EventArgs e)
        {
            NewSearch(true);
        }
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                NewSearch(false);
            }
        }
        public async void NewSearch(bool button)
        {
            try
            {
                processStop = true;
                while (!processStopped) await Task.Delay(100);
                if (textBox2.Text == searchEmotes)
                {
                    ImageFilter("");
                }
                else
                {
                    ImageFilter(textBox2.Text);
                }
                textBox2.Text = "";
                if (button) TextColor(textBox2, searchEmotes, "", nonTextColor);
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        public async void ImageFilter(string keyword = "")
        {
            try
            {
                processStop = false;
                processStopped = false;

                integer = 0;
                imagesList = new List<Image>();
                emoteString = new List<string>();
                buttonList = new List<Button>();

                for (int ix = flowLayoutPanel1.Controls.Count - 1; ix >= 0; ix--)
                {
                    if (flowLayoutPanel1.Controls[ix] is Button) flowLayoutPanel1.Controls[ix].Dispose();
                }

                ImageGetting(keyword);
                for (int x = 0; x <= emoteString.Count / division; x++)
                {
                    int max = Math.Min(x * division + division, emoteString.Count);
                    await Task.Run(() => ImageLoading(x, division, max));
                    if (processStop) break;
                    ShowImages(x, division, max);
                }
                processStopped = true;
                processStop = false;
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        public void ImageGetting(string keyword = "")
        {
            emoteString = new List<string>();
            string url = "http://kellphy.com/emotes/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();
                    Regex regex = new Regex(GetDirectoryListingRegexForUrl(url));
                    MatchCollection matches = regex.Matches(html);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Success && match.Groups["name"].ToString().Contains(keyword))
                            {
                                emoteString.Add(match.Groups["name"].ToString());
                            }
                        }
                    }
                }
            }
        }
        public static string GetDirectoryListingRegexForUrl(string url)
        {
            if (url.Equals("http://kellphy.com/emotes/"))
            {
                return "<a href=\".*.png\">(?<name>.*).png</a>";
            }
            throw new NotSupportedException();
        }
        public async Task ImageLoading(int x, int division, int max)
        {
            //int top = 10;
            //int left = 400;
            for (int y = x * division; y < max; y++)
            {
                if (processStop) break;
                string link = $"http://kellphy.com/emotes/{emoteString[y]}.png";

                imagesList.Add(DownloadImage(link));

                Button button = new Button();
                button.Name = emoteString[integer];
                //button.Left = left;
                //button.Top = top;
                button.TabStop = false;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Size = new Size(48, 48);
                button.Click += this.buttonGenerated_Click;
                button.BackgroundImageLayout = ImageLayout.Zoom;
                button.BackgroundImage = imagesList.ElementAt(integer);
                //button.Image = new Bitmap(imagesList.ElementAt(integer), new Size(button.Width, button.Width));
                buttonList.Add(button);
                //top += button.Height + 2;
                integer++;
            }
            await Task.CompletedTask;
        }
        public void ShowImages(int x, int division, int max)
        {
            label2.Text = $"{integer} / {emoteString.Count} Emotes";
            for (int y = x * division; y < max; y++)
            {
                flowLayoutPanel1.Controls.Add(buttonList[y]);
            }
        }
        private void buttonGenerated_Click(object sender, EventArgs e)
        {
            try
            {
                Execution(false, (sender as Button).Name);
            }
            catch (Exception ex) { SendErrorMessage(ex.Message.ToString()); }
        }
        //Get & Search
        public Image DownloadImage(string fromUrl)
        {
            using (WebClient webClient = new WebClient())
            {
                using (Stream stream = webClient.OpenRead(fromUrl))
                {
                    return Image.FromStream(stream);
                }
            }
        }
        //TextBox placeholder text
        private void textBox1_Enter(object sender, EventArgs e)
        {
            TextColor(textBox1, "", getEmotes, textColor);
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            TextColor(textBox1, getEmotes, "", nonTextColor);
        }
        private void textBox2_Enter(object sender, EventArgs e)
        {
            TextColor(textBox2, "", searchEmotes, textColor);
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            TextColor(textBox2, searchEmotes, "", nonTextColor);
        }
        public void TextColor(TextBox textBox, string text, string reqText, Color color)
        {
            if (textBox.Text == reqText)
            {
                textBox.ForeColor = color;
                textBox.Text = text;
            }
        }
        //Errors
        public void SendErrorMessage(string error)
        {
            label1.ForeColor = Color.DarkRed;
            label1.Text = error;
        }
    }
}
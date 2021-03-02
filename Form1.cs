using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordCopy
{
    public partial class Form1 : Form
    {
        public int option = 1;
        public int integer = 0;
        public Color textColor= Color.FromArgb(0,40,85);
        public List<Image> imagesList = new List<Image>();
        public List<Button> buttonList = new List<Button>();
        public List<string> emoteString = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            label1.ForeColor = textColor;
            try
            {
                ImageGetting();
                await Task.Run(() => ImageLoading());
                ShowImages();
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.DarkRed;
                label1.Text = ex.Message.ToString();
            }

        }
        public void ImageGetting()
        {
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
                            if (match.Success)
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
        public async Task ImageLoading()
        {
            int top = 10;
            int left = 400;
            foreach (string emoteName in emoteString)
            {
                string link = $"http://kellphy.com/emotes/{emoteName}.png";

                imagesList.Add(DownloadImage(link));

                Button button = new Button();
                button.Name = emoteString[integer];
                button.Left = left;
                button.Top = top;
                button.Image = imagesList.ElementAt(integer);
                button.TabStop = false;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Size = new Size(48, 48);
                button.Click += this.buttonGenerated_Click;

                buttonList.Add(button);

                top += button.Height + 2;
                integer++;
            }
            await Task.CompletedTask;
        }
        public void ShowImages()
        {
            label2.Text = $"{integer} Emotes";
            foreach (Button button in buttonList)
            {
                flowLayoutPanel1.Controls.Add(button);
            }
        }
        private void buttonGenerated_Click(object sender, EventArgs e)
        {
            try
            {
                Execution((sender as Button).Name);
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.DarkRed;
                label1.Text = ex.Message.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Execution();
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.DarkRed;
                label1.Text = ex.Message.ToString();
            }
        }
        public void Execution(string emoteToSearch="")
        {
            //string outputFile = @"kee_temp.png";
            string image_name;
            if (emoteToSearch.Length > 1)
            {
                image_name = emoteToSearch;
            }
            else
            {
                image_name = textBox1.Text.ToLower();
            }

            string link = $"http://kellphy.com/emotes/{image_name}.png";

            try
            {
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
                        //stackoverflow.com/questions/44177115/copying-from-and-to-clipboard-loses-image-transparency
                        SetClipboardImage(image, image, null);

                        DataObject retrievedData = Clipboard.GetDataObject() as DataObject;
                        GetClipboardImage(retrievedData);
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
            catch (Exception ex)
            {
                label1.ForeColor = Color.DarkRed;
                label1.Text = ex.Message.ToString();
            }
        }
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
        public static Bitmap GetClipboardImage(DataObject retrievedData)
        {
            Bitmap clipboardimage = null;
            // Order: try PNG, move on to try 32-bit ARGB DIB, then try the normal Bitmap and Image types.
            if (retrievedData.GetDataPresent("PNG"))
            {
                MemoryStream png_stream = retrievedData.GetData("PNG") as MemoryStream;
                if (png_stream != null)
                    using (Bitmap bm = new Bitmap(png_stream))
                        clipboardimage = ImageUtils.CloneImage(bm);
            }
            if (clipboardimage == null && retrievedData.GetDataPresent(DataFormats.Dib))
            {
                MemoryStream dib = retrievedData.GetData(DataFormats.Dib) as MemoryStream;
                if (dib != null)
                    clipboardimage = ImageFromClipboardDib(dib.ToArray());
            }
            if (clipboardimage == null && retrievedData.GetDataPresent(DataFormats.Bitmap))
                clipboardimage = new Bitmap(retrievedData.GetData(DataFormats.Bitmap) as Image);
            if (clipboardimage == null && retrievedData.GetDataPresent(typeof(Image)))
                clipboardimage = new Bitmap(retrievedData.GetData(typeof(Image)) as Image);
            return clipboardimage;
        }
        public static Bitmap ImageFromClipboardDib(Byte[] dibBytes)
        {
            if (dibBytes == null || dibBytes.Length < 4)
                return null;
            try
            {
                Int32 headerSize = (Int32)ArrayUtils.ReadIntFromByteArray(dibBytes, 0, 4, true);
                // Only supporting 40-byte DIB from clipboard
                if (headerSize != 40)
                    return null;
                Byte[] header = new Byte[40];
                Array.Copy(dibBytes, header, 40);
                Int32 imageIndex = headerSize;
                Int32 width = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x04, 4, true);
                Int32 height = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x08, 4, true);
                Int16 planes = (Int16)ArrayUtils.ReadIntFromByteArray(header, 0x0C, 2, true);
                Int16 bitCount = (Int16)ArrayUtils.ReadIntFromByteArray(header, 0x0E, 2, true);
                //Compression: 0 = RGB; 3 = BITFIELDS.
                Int32 compression = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x10, 4, true);
                // Not dealing with non-standard formats.
                if (planes != 1 || (compression != 0 && compression != 3))
                    return null;
                PixelFormat fmt;
                switch (bitCount)
                {
                    case 32:
                        fmt = PixelFormat.Format32bppRgb;
                        break;
                    case 24:
                        fmt = PixelFormat.Format24bppRgb;
                        break;
                    case 16:
                        fmt = PixelFormat.Format16bppRgb555;
                        break;
                    default:
                        return null;
                }
                if (compression == 3)
                    imageIndex += 12;
                if (dibBytes.Length < imageIndex)
                    return null;
                Byte[] image = new Byte[dibBytes.Length - imageIndex];
                Array.Copy(dibBytes, imageIndex, image, 0, image.Length);
                // Classic stride: fit within blocks of 4 bytes.
                Int32 stride = (((((bitCount * width) + 7) / 8) + 3) / 4) * 4;
                if (compression == 3)
                {
                    UInt32 redMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 0, 4, true);
                    UInt32 greenMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 4, 4, true);
                    UInt32 blueMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 8, 4, true);
                    // Fix for the undocumented use of 32bppARGB disguised as BITFIELDS. Despite lacking an alpha bit field,
                    // the alpha bytes are still filled in, without any header indication of alpha usage.
                    // Pure 32-bit RGB: check if a switch to ARGB can be made by checking for non-zero alpha.
                    // Admitted, this may give a mess if the alpha bits simply aren't cleared, but why the hell wouldn't it use 24bpp then?
                    if (bitCount == 32 && redMask == 0xFF0000 && greenMask == 0x00FF00 && blueMask == 0x0000FF)
                    {
                        // Stride is always a multiple of 4; no need to take it into account for 32bpp.
                        for (Int32 pix = 3; pix < image.Length; pix += 4)
                        {
                            // 0 can mean transparent, but can also mean the alpha isn't filled in, so only check for non-zero alpha,
                            // which would indicate there is actual data in the alpha bytes.
                            if (image[pix] == 0)
                                continue;
                            fmt = PixelFormat.Format32bppPArgb;
                            break;
                        }
                    }
                    else
                        // Could be supported with a system that parses the colour masks,
                        // but I don't think the clipboard ever uses these anyway.
                        return null;
                }
                Bitmap bitmap = ImageUtils.BuildImage(image, width, height, stride, fmt, null, null);
                // This is bmp; reverse image lines.
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
        public static void SetClipboardImage(Bitmap image, Bitmap imageNoTr, DataObject data)
        {
            Clipboard.Clear();
            if (data == null)
                data = new DataObject();
            if (imageNoTr == null)
                imageNoTr = image;
            using (MemoryStream pngMemStream = new MemoryStream())
            using (MemoryStream dibMemStream = new MemoryStream())
            {
                // As standard bitmap, without transparency support
                data.SetData(DataFormats.Bitmap, true, imageNoTr);
                // As PNG. Gimp will prefer this over the other two.
                image.Save(pngMemStream, ImageFormat.Png);
                data.SetData("PNG", false, pngMemStream);
                // As DIB. This is (wrongly) accepted as ARGB by many applications.
                Byte[] dibData = ConvertToDib(image);
                dibMemStream.Write(dibData, 0, dibData.Length);
                data.SetData(DataFormats.Dib, false, dibMemStream);
                // The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
                Clipboard.SetDataObject(data, true);
            }
        }
        public static Byte[] ConvertToDib(Image image)
        {
            Byte[] bm32bData;
            Int32 width = image.Width;
            Int32 height = image.Height;
            // Ensure image is 32bppARGB by painting it on a new 32bppARGB image.
            using (Bitmap bm32b = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics gr = Graphics.FromImage(bm32b))
                    gr.DrawImage(image, new Rectangle(0, 0, bm32b.Width, bm32b.Height));
                // Bitmap format has its lines reversed.
                bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);
                Int32 stride;
                bm32bData = ImageUtils.GetImageData(bm32b, out stride);
            }
            // BITMAPINFOHEADER struct for DIB.
            Int32 hdrSize = 0x28;
            Byte[] fullImage = new Byte[hdrSize + 12 + bm32bData.Length];
            //Int32 biSize;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x00, 4, true, (UInt32)hdrSize);
            //Int32 biWidth;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x04, 4, true, (UInt32)width);
            //Int32 biHeight;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x08, 4, true, (UInt32)height);
            //Int16 biPlanes;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x0C, 2, true, 1);
            //Int16 biBitCount;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x0E, 2, true, 32);
            //BITMAPCOMPRESSION biCompression = BITMAPCOMPRESSION.BITFIELDS;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x10, 4, true, 3);
            //Int32 biSizeImage;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x14, 4, true, (UInt32)bm32bData.Length);
            // These are all 0. Since .net clears new arrays, don't bother writing them.
            //Int32 biXPelsPerMeter = 0;
            //Int32 biYPelsPerMeter = 0;
            //Int32 biClrUsed = 0;
            //Int32 biClrImportant = 0;

            // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
            Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
            return fullImage;
        }
        public byte[] ImageToByteArray(Image imageIn)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            option = 1;
            label1.ForeColor = textColor;
            label1.Text ="Now using the 1st Option";
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
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel1.LinkVisited = true;
                System.Diagnostics.Process.Start("http://kellphy.com/");
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.Red;
                label1.Text = ex.Message.ToString();
            }
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel2.LinkVisited = true;
                System.Diagnostics.Process.Start("https://discord.gg/ycYmMmP");
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.Red;
                label1.Text = ex.Message.ToString();
            }
        }
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel3.LinkVisited = true;
                System.Diagnostics.Process.Start("http://kellphy.com/projects/apps/kee-list.php");
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.Red;
                label1.Text = ex.Message.ToString();
            }
        }
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel3.LinkVisited = true;
                System.Diagnostics.Process.Start("https://github.com/Kellphy/KEE/releases/");
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.Red;
                label1.Text = ex.Message.ToString();
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Execution();
            }
        }
    }
    public class ArrayUtils
    {
        public static void WriteIntToByteArray(Byte[] data, Int32 startIndex, Int32 bytes, Boolean littleEndian, UInt32 value)
        {
            Int32 lastByte = bytes - 1;
            if (data.Length < startIndex + bytes)
                throw new ArgumentOutOfRangeException("startIndex", "Data array is too small to write a " + bytes + "-byte value at offset " + startIndex + ".");
            for (Int32 index = 0; index < bytes; index++)
            {
                Int32 offs = startIndex + (littleEndian ? index : lastByte - index);
                data[offs] = (Byte)(value >> (8 * index) & 0xFF);
            }
        }

        public static UInt32 ReadIntFromByteArray(Byte[] data, Int32 startIndex, Int32 bytes, Boolean littleEndian)
        {
            Int32 lastByte = bytes - 1;
            if (data.Length < startIndex + bytes)
                throw new ArgumentOutOfRangeException("startIndex", "Data array is too small to read a " + bytes + "-byte value at offset " + startIndex + ".");
            UInt32 value = 0;
            for (Int32 index = 0; index < bytes; index++)
            {
                Int32 offs = startIndex + (littleEndian ? index : lastByte - index);
                value += (UInt32)(data[offs] << (8 * index));
            }
            return value;
        }
    }
    public class ImageUtils
    {
        public static Bitmap CloneImage(Bitmap bm) { int stride; return BuildImage(GetImageData(bm, out stride), bm.Width, bm.Height, stride, bm.PixelFormat, null, null); }
        public static Bitmap BuildImage(Byte[] sourceData, Int32 width, Int32 height, Int32 stride, PixelFormat pixelFormat, Color[] palette, Color? defaultColor)
        {
            Bitmap newImage = new Bitmap(width, height, pixelFormat);
            BitmapData targetData = newImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, newImage.PixelFormat);
            Int32 newDataWidth = ((Image.GetPixelFormatSize(pixelFormat) * width) + 7) / 8;
            // Compensate for possible negative stride on BMP format.
            Boolean isFlipped = stride < 0;
            stride = Math.Abs(stride);
            // Cache these to avoid unnecessary getter calls.
            Int32 targetStride = targetData.Stride;
            Int64 scan0 = targetData.Scan0.ToInt64();
            for (Int32 y = 0; y < height; y++)
                Marshal.Copy(sourceData, y * stride, new IntPtr(scan0 + y * targetStride), newDataWidth);
            newImage.UnlockBits(targetData);
            // Fix negative stride on BMP format.
            if (isFlipped)
                newImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            // For indexed images, set the palette.
            if ((pixelFormat & PixelFormat.Indexed) != 0 && palette != null)
            {
                ColorPalette pal = newImage.Palette;
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                {
                    if (i < palette.Length)
                        pal.Entries[i] = palette[i];
                    else if (defaultColor.HasValue)
                        pal.Entries[i] = defaultColor.Value;
                    else
                        break;
                }
                newImage.Palette = pal;
            }
            return newImage;
        }
        public static Byte[] GetImageData(Bitmap sourceImage, out Int32 stride)
        {
            BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            stride = sourceData.Stride;
            Byte[] data = new Byte[stride * sourceImage.Height];
            Marshal.Copy(sourceData.Scan0, data, 0, data.Length);
            sourceImage.UnlockBits(sourceData);
            return data;
        }
    }
}



using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace KEE
{
    public class Transparency
    {
        //[STAThread]
        //static void Main(string[] args)
        //{
        //    IDataObject clipboardData;
        //    clipboardData = Clipboard.GetDataObject();

        //    // print out a list of the formats currently on the clipboard
        //    string formatList = GetFormatList(clipboardData);
        //    Console.WriteLine(formatList);

        //    // Parse in the image from the clipboard
        //    Image img = GetImageFromClipboard();
        //    if (img == null)
        //        return;

        //    img.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX); // gotta rotate it for some reason


        //    bool success = CopyTransparentImageToClipboard(img); // main part of script
        //    if (success)
        //        Environment.Exit(0);
        //    else
        //        Environment.Exit(1);
        //}
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        // returns true/false if successful/unsuccessful
        public bool CopyTransparentImageToClipboard(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);

                IDataObject dataObj = new DataObject();
                //dataObj.SetData("Format17", ms);

                dataObj.SetData("PNG", ms);
                //dataObj.SetData(DataFormats.Dib, ConvertToDib(img));
                dataObj.SetData(DataFormats.Bitmap, new Bitmap(img));
                //dataObj.SetData(DataFormats.Html, html);
                System.Windows.Forms.Clipboard.SetDataObject(dataObj, true);
            }
            DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Dib);
            MessageBox.Show(Clipboard.GetData(DataFormats.Html).ToString());
            return true;
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

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource BitmapToBitmapSource(Bitmap bmp)
        {
            IntPtr hBitmap = bmp.GetHbitmap();
            BitmapSource imageSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hBitmap);
            return imageSource;
        }

        //winapi structs
        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER header;
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;


            public void init(int w, int h)
            {
                header.init(w, h);
            }


        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            //winapi constant
            private const uint BI_RGB = 0;
            public uint size;
            public int width;
            public int height;
            public ushort planes;
            public ushort bitCount;
            public uint compression;
            public uint sizeImage;
            public int xPelsPerMeter;
            public int yPelsPerMeter;
            public uint clrUsed;
            public uint clrImportant;

            public void init(int w, int h)
            {
                size = (uint)Marshal.SizeOf(this);
                width = w;
                height = -h;//negative so we are a top-down image
                planes = 1;//uneeded (always 1)
                bitCount = 32;//bits per pixel
                compression = BI_RGB;
            }

        }


        public string GetFormatList(IDataObject clipboardData)
        {
            string separator = "---------";
            String[] allFormats = clipboardData.GetFormats();
            string theResult;

            if (allFormats.Length == 0)
            {
                theResult = "Nothing in Clipboard\n";
            }
            else
            {
                theResult = "Clipboard Format(s):\n" + separator + "\n";
                for (int i = 0; i < allFormats.Length; i++)
                    theResult += allFormats[i] + "\n";
                theResult += separator + "\n";
            }

            return theResult;
        }

        public Image GetImageFromClipboard(Image img)
        {

            byte[] dib = ((System.IO.MemoryStream)Clipboard.GetData(DataFormats.Dib)).ToArray();
            int width = BitConverter.ToInt32(dib, 4);
            int height = BitConverter.ToInt32(dib, 8);
            short bpp = BitConverter.ToInt16(dib, 14);
            if (bpp == 32)
            {
                GCHandle gch = GCHandle.Alloc(dib, GCHandleType.Pinned);
                Bitmap bmp = null;
                try
                {
                    IntPtr ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 40);
                    bmp = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
                    return new Bitmap(bmp);
                }
                finally
                {
                    gch.Free();
                    if (bmp != null) bmp.Dispose();
                }
            }
            return Clipboard.ContainsImage() ? Clipboard.GetImage() : null;
        }

        public struct BITMAPV5HEADER
        {
            public uint bV5Size;
            public int bV5Width;
            public int bV5Height;
            public UInt16 bV5Planes;
            public UInt16 bV5BitCount;
            public uint bV5Compression;
            public uint bV5SizeImage;
            public int bV5XPelsPerMeter;
            public int bV5YPelsPerMeter;
            public UInt16 bV5ClrUsed;
            public UInt16 bV5ClrImportant;
            public UInt16 bV5RedMask;
            public UInt16 bV5GreenMask;
            public UInt16 bV5BlueMask;
            public UInt16 bV5AlphaMask;
            public UInt16 bV5CSType;
            public IntPtr bV5Endpoints;
            public UInt16 bV5GammaRed;
            public UInt16 bV5GammaGreen;
            public UInt16 bV5GammaBlue;
            public UInt16 bV5Intent;
            public UInt16 bV5ProfileData;
            public UInt16 bV5ProfileSize;
            public UInt16 bV5Reserved;
        }
        public Bitmap CF_DIBV5ToBitmap(byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            BITMAPV5HEADER bmi = (BITMAPV5HEADER)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(BITMAPV5HEADER));
            Bitmap bitmap = new Bitmap((int)bmi.bV5Width, (int)bmi.bV5Height, -
                                       (int)(bmi.bV5SizeImage / bmi.bV5Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                                       new IntPtr(handle.AddrOfPinnedObject().ToInt32()
                                       + bmi.bV5Size + (bmi.bV5Height - 1)
                                       * (int)(bmi.bV5SizeImage / bmi.bV5Height)));
            handle.Free();
            return bitmap;
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
                bm32bData = GetImageData(bm32b, out stride);
            }
            // BITMAPINFOHEADER struct for DIB.
            Int32 hdrSize = 0x28;
            Byte[] fullImage = new Byte[hdrSize + 12 + bm32bData.Length];
            //Int32 biSize;
            WriteIntToByteArray(fullImage, 0x00, 4, true, (UInt32)hdrSize);
            //Int32 biWidth;
            WriteIntToByteArray(fullImage, 0x04, 4, true, (UInt32)width);
            //Int32 biHeight;
            WriteIntToByteArray(fullImage, 0x08, 4, true, (UInt32)height);
            //Int16 biPlanes;
            WriteIntToByteArray(fullImage, 0x0C, 2, true, 1);
            //Int16 biBitCount;
            WriteIntToByteArray(fullImage, 0x0E, 2, true, 32);
            //BITMAPCOMPRESSION biCompression = BITMAPCOMPRESSION.BITFIELDS;
            WriteIntToByteArray(fullImage, 0x10, 4, true, 3);
            //Int32 biSizeImage;
            WriteIntToByteArray(fullImage, 0x14, 4, true, (UInt32)bm32bData.Length);
            // These are all 0. Since .net clears new arrays, don't bother writing them.
            //Int32 biXPelsPerMeter = 0;
            //Int32 biYPelsPerMeter = 0;
            //Int32 biClrUsed = 0;
            //Int32 biClrImportant = 0;

            // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
            WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
            WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
            WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
            Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
            return fullImage;
        }
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

    }

}

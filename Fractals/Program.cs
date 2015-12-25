using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Anvil.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
//using System.Numerics;

namespace Fractals
{
    public class Program
    {
        public static Form MainForm;

        public static int FormWidth = 800, FormHeight = 600;
        public static string Version = "1.02";
        public static string FormTitle = string.Format("Fractals (v {0})", Version);

        static Thread RenderingThread;
        static double MaxExtent = 100;
        static int MaxIterations = 400;

        static double GlobalScale = 1;

        static double Zoom = 1 / GlobalScale;
        static double OffsetX = 0;
        static double OffsetY = 0;

        static bool RecordMode = false;

        //

        static int ColorMode = 0;
        //

        [STAThread]
        static void Main()
        {
            Anvil.Net.Core.Init();

            //

            MainForm = UI.CreateForm(new Rect(50, 10, FormWidth, FormHeight), FormTitle);
            MainForm.MaximizeBox = false;
            MainForm.MaximumSize = new Size(FormWidth, FormHeight);
            MainForm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            MainForm.Load += new System.EventHandler(Render);
            CreateUI();

            //

            Application.Run(MainForm);
        }

        static void CreateUI()
        {
            UI.Style = CommonUI.CommonStyle;

            //

            UI.Append(UI.CreateButton(new Rect(5, MainForm.ClientSize.Height - 65, 30, 30), UI.DefaultAnchor, "Left", "3", delegate
            {
                OffsetX += (Zoom / 20) * GlobalScale;
                Render(null, null);
            }), MainForm);
            UI.Controls["Left"].Font = new Font("Webdings", 10f);

            UI.Append(UI.CreateButton(new Rect(65, MainForm.ClientSize.Height - 65, 30, 30), UI.DefaultAnchor, "Right", "4", delegate
            {
                OffsetX -= (Zoom / 20) * GlobalScale;
                Render(null, null);
            }), MainForm);
            UI.Controls["Right"].Font = new Font("Webdings", 10f);

            UI.Append(UI.CreateButton(new Rect(35, MainForm.ClientSize.Height - 95, 30, 30), UI.DefaultAnchor, "Up", "5", delegate
            {
                OffsetY += (Zoom / 20) * GlobalScale;
                Render(null, null);
            }), MainForm);
            UI.Controls["Up"].Font = new Font("Webdings", 10f);

            UI.Append(UI.CreateButton(new Rect(35, MainForm.ClientSize.Height - 35, 30, 30), UI.DefaultAnchor, "Down", "6", delegate
            {
                OffsetY -= (Zoom / 20) * GlobalScale;
                Render(null, null);
            }), MainForm);
            UI.Controls["Down"].Font = new Font("Webdings", 10f);

            UI.Append(UI.CreateButton(new Rect(100, MainForm.ClientSize.Height - 65, 30, 30), UI.DefaultAnchor, "ZoomIn", "+", delegate
            {
                ZoomIn();
                Render(null, null);
            }), MainForm);

            UI.Append(UI.CreateButton(new Rect(135, MainForm.ClientSize.Height - 65, 30, 30), UI.DefaultAnchor, "ZoomOut", "-", delegate
            {
                ZoomOut();
                Render(null, null);
            }), MainForm);

            UI.Append(UI.CreateButton(new Rect(MainForm.ClientSize.Width - 95, MainForm.ClientSize.Height - 35, 90, 30), UI.DefaultAnchor, "SaveBtn", "Save", delegate
            {
                Save();
            }), MainForm);

            UI.Append(UI.CreateButton(new Rect(MainForm.ClientSize.Width - 190, MainForm.ClientSize.Height - 35, 90, 30), UI.DefaultAnchor, "RecBtn", "Record", delegate
            {
                RecordMode = !RecordMode;
                //Save();
            }), MainForm);

            for (int i = 0; i < 5; i++ ) {
                int mode = i;

                UI.Append(UI.CreateButton(new Rect(200 + (i * 35), MainForm.ClientSize.Height - 35, 30, 30), UI.DefaultAnchor, "Color" + i, (i + 1).ToString(), delegate
                {
                    ColorMode = mode;
                }), MainForm);
            }
        }

        static void ZoomIn()
        {
            Zoom -= Zoom / 10;
        }

        static void ZoomOut()
        {
            Zoom += Zoom / 10;
        }

        static void MiniZoomOut()
        {
            Zoom += Zoom / 15;
        }

        static void Render(object sender, EventArgs e)
        {
            if (RenderingThread != null)
            {
                RenderingThread.Abort();
                RenderingThread = null;
            }

            RenderingThread = new Thread(RenderProc);
            RenderingThread.IsBackground = true;
            //
            int ImageScale = 16;

            RenderingThread.Start(new Size(MainForm.ClientSize.Width * ImageScale, MainForm.ClientSize.Height * ImageScale));//MainForm.ClientSize
        }

        static void RenderProc(object args)
        {
            Size CanvasSize = (Size)args;

            int Width = 0x20;

            while (Width * 2 < CanvasSize.Width)
            {
                int Height = Width * CanvasSize.Height / CanvasSize.Width;
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

                GenerateBitmap(bitmap);
                MainForm.BeginInvoke(new SetBitmapDelegate(SetBitmap), bitmap);

                Width *= 2;
                Thread.Sleep(100);
            }

            try
            {
                Bitmap FinalImg = new Bitmap(CanvasSize.Width, CanvasSize.Height, PixelFormat.Format24bppRgb);
                GenerateBitmap(FinalImg);
                MainForm.BeginInvoke(new SetBitmapDelegate(SetBitmap), FinalImg);
            }
            catch {
                //
            }
        }

        //

        static void GenerateBitmap(Bitmap bitmap)
        {
            double ImageZoom = (Zoom / MaxExtent) * (GlobalScale * 2);
            //
            double scale = (ImageZoom * MaxExtent) / Math.Min(bitmap.Width, bitmap.Height);
            for (int i = 0; i < bitmap.Height; i++)
            {
                double y = (bitmap.Height / 2 - i) * scale + OffsetY;
                for (int j = 0; j < bitmap.Width; j++)
                {
                    double x = (j - bitmap.Width / 2) * scale - OffsetX;
                    double ColorRate = Calc(new Complex(x, y));
                    bitmap.SetPixel(j, i, GetColor(ColorRate));
                }
            }
        }

        static Color GetColor(double rate)
        {
            double MaxColor = 255;
            
            //

            int r = 0, g = 0, b = 0;

            if (ColorMode == 0)
            {
                r = (int)(MaxColor * rate * rate * rate);
                g = (int)(MaxColor * rate);
                b = 0;
            }

            if(ColorMode == 1)
            {
                double Contrast = 0.01 / (rate * rate + (0.02 * Zoom));
                double color = Math.Pow(rate, Contrast);

                r = (int)(MaxColor * color);
                g = (int)(MaxColor * color * color);
                b = 0;
            }

            if (ColorMode == 2)
            {
                double Contrast = 0.01 / (rate * rate + (0.02 * Zoom));
                double color = Math.Pow(rate, Contrast);

                r = (int)(MaxColor * color * color * color);
                g = (int)(MaxColor * color * color);
                b = (int)(MaxColor * color);
            }

            if (ColorMode == 3)
            {
                double Contrast = 0.01 / (rate * rate + (0.02 * Zoom));
                double color = Math.Pow(rate, Contrast);

                color *= 2;

                double colorZero = color <= 1 ? color : 0;
                double colorUnit = color <= 1 ? color : 1;
                double colorDelta = color - colorUnit;

                r = (int)(MaxColor * Math.Pow((colorUnit - colorDelta) * (colorUnit + colorDelta), 1.5));
                g = (int)(MaxColor * Math.Pow((colorUnit - (colorDelta)) * (colorUnit + colorDelta), 2));
                b = (int)(MaxColor * Math.Pow((colorUnit - colorDelta) * (1 - colorDelta), 1));
            }

            if (ColorMode == 4)
            {
                double Contrast = 0.01 / (rate * rate + (0.02 * Zoom));
                double color = Math.Pow(rate, Contrast);

                double c = color * 10;

                r = (int)(MaxColor * color * Math.Abs(Math.Sin(c)));
                g = (int)(MaxColor * color * Math.Abs(Math.Cos(c)));
                b = (int)(MaxColor * color);
            }

            return Color.FromArgb(r, g, b);
        }

        public static Color Rainbow(double progress)
        {
            double div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
            int descending = 255 - ascending;

            switch ((int)div)
            {
                case 0:
                    return Color.FromArgb(255, 255, ascending, 0);
                case 1:
                    return Color.FromArgb(255, descending, 255, 0);
                case 2:
                    return Color.FromArgb(255, 0, 255, ascending);
                case 3:
                    return Color.FromArgb(255, 0, descending, 255);
                case 4:
                    return Color.FromArgb(255, ascending, 0, 255);
                default: // case 5:
                    return Color.FromArgb(255, 255, 0, descending);
            }
        }


        static double Calc(Complex C)
        {
            double MaxNorm = MaxExtent * MaxExtent;

            int Iteration = 0;
            Complex Z = new Complex(0, 0);
            //double Norm = Z.Real * Z.Real + Z.Imaginary * Z.Imaginary;
            
            while (Z.Norm() < MaxNorm && Iteration < MaxIterations)
            {
                Z = Z * Z + C - 0.7;
                //Z = (Z * Z + C) * (C / Z / Z / C / Z);
                //Z = (Z * Z + C) - (Z * Z / C);
                //Z = Z * Z * Z * Z + C/Z * C;//Сердца
                //Z = Z - ((Z * Z * Z * C - C) / (3 * (Z * Z)));//Круги
                //Z = Z * Z + C - 0.7;//Мандельброт
                //Z = Complex.Pow(Z, 2) + C;
                Iteration++;
            }

            if (Iteration < MaxIterations)return (double)Iteration / MaxIterations;
            else return 0;
        }

        static void SetBitmap(Bitmap img)
        {
            if (MainForm.BackgroundImage != null) MainForm.BackgroundImage.Dispose();
            MainForm.BackgroundImage = img;
            MainForm.Text = string.Format("{0} | zoom: {1} | {2}, {3}", FormTitle, (Zoom * GlobalScale), img.Width, img.Height);

            //

            if (img.Width >= 1024 && RecordMode)
            {
                Save();
                MiniZoomOut();
                Render(null, null);
            }
        }

        delegate void SetBitmapDelegate(Bitmap img);

        static int SavesCount = 0;
        static void Save() 
        {
            SavesCount++;
            int RndId = 0;
            Random Rnd = new Random();
            RndId = Rnd.Next(1, 100000);

            try
            {
                MainForm.BackgroundImage.Save(Application.StartupPath + @"\fractal_" + SavesCount + ".png");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "Saving error");
            }
        }
    }
}

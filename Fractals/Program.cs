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

namespace Fractals
{
    public class Program
    {
        public static Form MainForm;

        public static int FormWidth = 1024, FormHeight = 700;
        public static string Version = "1.0";
        public static string FormTitle = string.Format("Mandelbrot Set (v {0})", Version);

        static Thread RenderingThread;
        static double MaxExtent = 6;
        static int MaxIterations = 600;

        static double Zoom = 1;
        static double OffsetX = 0;
        static double OffsetY = 0;

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
                OffsetX += Zoom / 20;
                Render(null, null);
            }), MainForm);
            UI.Controls["Left"].Font = new Font("Webdings", 10f);

            UI.Append(UI.CreateButton(new Rect(65, MainForm.ClientSize.Height - 65, 30, 30), UI.DefaultAnchor, "Right", "4", delegate
            {
                OffsetX -= Zoom / 20;
                Render(null, null);
            }), MainForm);
            UI.Controls["Right"].Font = new Font("Webdings", 10f);

            UI.Append(UI.CreateButton(new Rect(35, MainForm.ClientSize.Height - 95, 30, 30), UI.DefaultAnchor, "Up", "5", delegate
            {
                OffsetY += Zoom / 20;
                Render(null, null);
            }), MainForm);
            UI.Controls["Up"].Font = new Font("Webdings", 10f);

            UI.Append(UI.CreateButton(new Rect(35, MainForm.ClientSize.Height - 35, 30, 30), UI.DefaultAnchor, "Down", "6", delegate
            {
                OffsetY -= Zoom / 20;
                Render(null, null);
            }), MainForm);
            UI.Controls["Down"].Font = new Font("Webdings", 10f);

            UI.Append(UI.CreateButton(new Rect(100, MainForm.ClientSize.Height - 65, 30, 30), UI.DefaultAnchor, "ZoomIn", "+", delegate
            {
                Zoom -= Zoom / 10;
                Render(null, null);
            }), MainForm);

            UI.Append(UI.CreateButton(new Rect(135, MainForm.ClientSize.Height - 65, 30, 30), UI.DefaultAnchor, "ZoomOut", "-", delegate
            {
                Zoom += Zoom / 10;
                Render(null, null);
            }), MainForm);

            UI.Append(UI.CreateButton(new Rect(MainForm.ClientSize.Width - 95, MainForm.ClientSize.Height - 35, 90, 30), UI.DefaultAnchor, "SaveBtn", "Save", delegate
            {
                Save();
            }), MainForm);
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
            int ImageScale = 4;

            RenderingThread.Start(new Size(MainForm.ClientSize.Width * ImageScale, MainForm.ClientSize.Height * ImageScale));//MainForm.ClientSize
        }

        static void RenderProc(object args)
        {
            Size CanvasSize = (Size)args;

            int Width = 0x20;

            while (Width * 2 < /*MainForm.ClientSize.Width*/CanvasSize.Width)
            {
                int Height = Width * CanvasSize.Height / CanvasSize.Width;
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

                GenerateBitmap(bitmap);
                MainForm.BeginInvoke(new SetBitmapDelegate(SetBitmap), bitmap);

                Width *= 2;
                Thread.Sleep(100);
            }

            Bitmap FinalImg = new Bitmap(CanvasSize.Width, CanvasSize.Height, PixelFormat.Format24bppRgb);
            GenerateBitmap(FinalImg);
            MainForm.BeginInvoke(new SetBitmapDelegate(SetBitmap), FinalImg);
        }

        //

        static void GenerateBitmap(Bitmap bitmap)
        {
            double ImageZoom = (Zoom / MaxExtent) * 2;
            //
            double scale = ImageZoom * MaxExtent / Math.Min(bitmap.Width, bitmap.Height);
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
            double MaxColor = 256;
            double Contrast = 0.005 / rate + 0.001;//0.15

            int color = (int)(MaxColor * Math.Pow(rate, Contrast));

<<<<<<< HEAD
            //return Color.FromArgb(0, (int)(MaxColor * rate), color);
            return Color.FromArgb(color, (int)(MaxColor * rate), 0);
=======
            return Color.FromArgb(color / 3, color / 3, color);
>>>>>>> origin/master
        }

        static double Calc(Complex C)
        {
            double MaxNorm = MaxExtent * MaxExtent;

            int Iteration = 0;
            Complex Z = new Complex(0, 0);

            while (Z.Norm() < MaxNorm && Iteration < MaxIterations)
            {
                Z = Z * Z + C - 0.7;
                //Z = Complex.Pow(Z, 10) + C;
                Iteration++;
            }

            if (Iteration < MaxIterations)return (double)Iteration / MaxIterations;
            else return 0;
        }

        static void SetBitmap(Bitmap img)
        {
            if (MainForm.BackgroundImage != null) MainForm.BackgroundImage.Dispose();
            MainForm.BackgroundImage = img;
            MainForm.Text = string.Format("{0} | {1}, {2}", FormTitle, img.Width, img.Height);
        }

        delegate void SetBitmapDelegate(Bitmap img);


        static void Save() 
        {
            try
            {
                MainForm.BackgroundImage.Save(Application.StartupPath + @"\output.png");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "Saving error");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        //順時鐘90度
        private void button1_Click(object sender, EventArgs e)
        {
            Image image = Image.FromFile(@"../../../Lena.jpg");
            Bitmap rotate = new Bitmap(image);
            rotate = RotateBitmap(rotate, 90);
            pictureBox2.Image = rotate;
        }

        //逆時鐘90度
        private void button2_Click(object sender, EventArgs e)
        {
            Image image = Image.FromFile(@"../../../Lena.jpg");
            Bitmap rotate = new Bitmap(image);
            rotate = RotateBitmap(rotate, 270);
            pictureBox2.Image = rotate;
        }

        //灰階
        private void button3_Click(object sender, EventArgs e)
        {
            Image image = Image.FromFile(@"../../../Lena.jpg");
            Bitmap gray = new Bitmap(image);
            gray = doGray(gray);
            pictureBox2.Image = gray;
        }

        //二值化
        private void button4_Click(object sender, EventArgs e)
        {
            Image image = Image.FromFile(@"../../../Lena.jpg");
            Bitmap bpp = new Bitmap(image);
            bpp = ConvertTo1Bpp1(bpp);
            pictureBox2.Image = bpp;
        }
        
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            
        }

        //重設
        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox2.Image= Image.FromFile(@"../../../Lena.jpg");
        }


        //-----------------------------------------以下函式-----------------------------------------------------
       
        //計算旋轉後影像Size
        public Size CalculateNewSize(int width, int height, double RotateAngle)
        {
            double r = Math.Sqrt(Math.Pow((double)width / 2d, 2d) + Math.Pow((double)height / 2d, 2d)); //半徑L
            double OriginalAngle = Math.Acos((width / 2d) / r) / Math.PI * 180d;  //對角線和X軸的角度θ
            double minW = 0d, maxW = 0d, minH = 0d, maxH = 0d; //最大和最小的 X、Y座標
            double[] drawPoint = new double[4];

            drawPoint[0] = (-OriginalAngle + RotateAngle) * Math.PI / 180d;
            drawPoint[1] = (OriginalAngle + RotateAngle) * Math.PI / 180d;
            drawPoint[2] = (180f - OriginalAngle + RotateAngle) * Math.PI / 180d;
            drawPoint[3] = (180f + OriginalAngle + RotateAngle) * Math.PI / 180d;

            foreach (double point in drawPoint) //由四個角的點算出X、Y的最大值及最小值
            {
                double x = r * Math.Cos(point);
                double y = r * Math.Sin(point);

                if (x < minW)
                    minW = x;
                if (x > maxW)
                    maxW = x;
                if (y < minH)
                    minH = y;
                if (y > maxH)
                    maxH = y;
            }

            return new Size((int)(maxW - minW), (int)(maxH - minH));
        }


        //灰階前置
        public int[,,] getRGBData(Bitmap abimage)
        {
            // Step 1: 利用 Bitmap 將 image 包起來
            int Height = abimage.Height;
            int Width = abimage.Width;
            int[,,] rgbData = new int[Width, Height, 3];

            // Step 2: 取得像點顏色資訊
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Color color = abimage.GetPixel(x, y);
                    rgbData[x, y, 0] = color.R;
                    rgbData[x, y, 1] = color.G;
                    rgbData[x, y, 2] = color.B;
                }
            }

            return rgbData;
        }

        //旋轉圖片之函式
        //參數    image：要旋轉的圖片  RotateAngle：旋轉角度
        public Bitmap RotateBitmap(Bitmap image, float RotateAngle)
        {
            Size newSize = CalculateNewSize(image.Width, image.Height, RotateAngle);
            Bitmap rotatedBmp = new Bitmap(newSize.Width, newSize.Height);
            PointF centerPoint = new PointF((float)rotatedBmp.Width / 2f, (float)rotatedBmp.Height / 2f);
            Graphics g = Graphics.FromImage(rotatedBmp);

            g.TranslateTransform(centerPoint.X, centerPoint.Y);
            g.RotateTransform(RotateAngle);
            g.TranslateTransform(-centerPoint.X, -centerPoint.Y);

            g.DrawImage(image, (float)(newSize.Width - image.Width) / 2f, (float)(newSize.Height - image.Height) / 2f, image.Width, image.Height);
            g.Dispose();

            return rotatedBmp;
        }

        //將圖片灰階之函式
        // 參數    image:要灰階的圖片
        public Bitmap doGray(Bitmap image)
        {
            // Step 1: 建立 Bitmap 元件
            int[,,] rgbData = getRGBData(image);
            int Height = image.Height;
            int Width = image.Width;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int gray = (rgbData[x, y, 0] + rgbData[x, y, 1] + rgbData[x, y, 2]) / 3;
                    image.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            return image;
        }

        //將圖片二值化之函式
        //參數 bmp:要二值化的圖片
        public static Bitmap ConvertTo1Bpp1(Bitmap bmp)
        {
            int average = 0;      
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = average / (bmp.Width * bmp.Height); 

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //獲取該點的像素的RGB的顏色  
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

    }
}
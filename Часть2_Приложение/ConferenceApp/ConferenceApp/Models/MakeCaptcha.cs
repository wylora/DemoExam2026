using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
namespace ConferenceApp.Models
{
    public class MakeCaptcha
    {
        /// <summary>
        /// Генерирует картинку, содержащую текст капчи
        /// </summary>
        /// <returns>возвращает картинку и текст капчи</returns>
        public static (BitmapImage image, string captcha) CreateImage(int width, int height, int symbolCount)
        {
            Random rnd = new Random();
            Bitmap result = new Bitmap(width, height);
            Brush[] colors = { Brushes.Black, Brushes.Red, Brushes.RoyalBlue, Brushes.Green };
            Graphics g = Graphics.FromImage(result);
            g.Clear(Color.Gray);
            string alphabet = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
            string captcha = "";
            int h = width / symbolCount; // поле для одного символа
            for (int i = 0; i < symbolCount; ++i)
            {
                int size = rnd.Next(35, Math.Min(h, height - 20)); // размер символа в пикселях
                char symbol = alphabet[rnd.Next(alphabet.Length)];
                captcha += symbol;
                int Ypos = rnd.Next(Math.Max(1, height - size - 20));
                int Xpos = rnd.Next(i * h, Math.Max(i * h + 1, (i + 1) * h - size));
                g.DrawString(symbol.ToString(), new Font("Arial", size, GraphicsUnit.Pixel),
                    colors[rnd.Next(colors.Length)], new PointF(Xpos, Ypos));
                // помехи - линии
                Pen pen = new Pen(colors[rnd.Next(colors.Length)]);
                pen.Width = rnd.Next(2, 5);
                g.DrawLine(pen, new Point(Xpos + 5, Ypos + 5), new Point(Xpos + size + 5, Ypos + size + 5));
            }
            // белые точки
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    if (rnd.Next() % 20 == 0)
                        result.SetPixel(i, j, Color.White);
            g.Dispose();
            return (BitmapToImageSource(result), captcha);
        }

        /// <summary>Конвертирует Bitmap в BitmapImage для Image</summary>
        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
    }
}

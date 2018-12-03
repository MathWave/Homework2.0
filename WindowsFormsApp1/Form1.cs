﻿using System;
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

        Bitmap bmp;
        Graphics graph;
        Pen pen;
        Color defaultBG;
        Color defaultPen;
        bool isEmpty;
        Point start;
        FractalData data;
        float resize;
        bool PossibleToOverDraw;

        public Form1()
		{
            InitializeComponent();
            bmp = new Bitmap(picture.Width, picture.Height);
            graph = Graphics.FromImage(bmp);
            pen = new Pen(Color.Black);
            defaultBG = Color.White;
            defaultPen = Color.Black;
            isEmpty = true;
            start = null;
            resize = 1;
            PossibleToOverDraw = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Draw();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            defaultBG = defaultBG == Color.White ? Color.Black : Color.White;
            graph.Clear(defaultBG);
            picture.Image = bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(690, 520);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            graph.Clear(defaultBG);
            picture.Image = bmp;
            isEmpty = true;
            data = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.ShowDialog();
            string filename = save.FileName + ".jpg";
            bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        void Draw()
        {
            Clear();
            string fractal_name = "";
            int depth = 0;
            bool flag = true;
            try
            {
                fractal_name = comboBox1.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("Выберите фрактал!", "Ошибка!");
                flag = false;
            }
            if (flag)
            {
                try
                {
                    int.TryParse(textBox1.Text, out depth);
                    if (depth < 1)
                        throw new Exception();
                }

                catch
                {
                    MessageBox.Show("Некорректный ввод глубины!", "Ошибка!");
                    flag = false;
                }
                if (flag)
                {
                    int argMin = Color.FromName(textBox2.Text).ToArgb();
                    if (argMin == 0)
                        argMin = defaultPen.ToArgb();
                    int argMax = Color.FromName(textBox3.Text).ToArgb();
                    if (argMax == 0)
                        argMax = defaultPen.ToArgb();
                    List<Color> colorList = new List<Color>();
                    for (int i = 0; i < depth; i++)
                    {
                        var colorAverage = argMin + (int)((argMax - argMin) * i / depth);
                        colorList.Add(Color.FromArgb(colorAverage));
                    }
                    Point center = new Point(picture.Width / 2, picture.Height / 2);
                    data = new FractalData(fractal_name, depth, colorList, center);
                    _Draw();
                }

            }
        }

        void _Draw()
        {
            string fractal_name = data.Name;
            int depth = data.Depth;
            List<Color> colorList = data.Colors;
            Point center = data.Center;
            if (fractal_name == "Н-фрактал")
            {
                if (depth < 8)
                    new H_Fractal(resize * 270 * (float)Math.Sqrt((float)picture.Width * (float)picture.Height / 1000 / 600), Color.Black, Color.Black, depth)
                        .Draw(center, ref graph, pen, colorList);
                else
                    MessageBox.Show("Слишком глубокая рекурсия, не получится!\nМаксимальная возможная глубина - 7.", "Упс!");
            }
            else if (fractal_name == "С-Кривая Леви")
            {
                if (depth < 19)
                    new C_Fractal(resize * 330 * (float)Math.Sqrt((float)picture.Width * (float)picture.Height / 1000 / 600), Color.Black, Color.Black, depth)
                        .Draw(new Point(center.x, center.y + 50), ref graph, pen, colorList);
                else
                    MessageBox.Show("Слишком глубокая рекурсия, не получится!\nМаксимальная возможная глубина - 18.", "Упс!");
            }
            else
            {
                if (depth < 8)
                    new T_Fractal(resize * 1200 * (float)Math.Sqrt((float)picture.Width * (float)picture.Height / 1000 / 600), Color.Black, Color.Black, depth)
                        .Draw(new Point(center.x, center.y + 40), ref graph, pen, colorList);
                else
                    MessageBox.Show("Слишком глубокая рекурсия, не получится.\nМаксимальная возможная глубина - 7.", "Упс!");
            }
            picture.Image = bmp;
            isEmpty = false;
            PossibleToOverDraw = true;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox2.SelectedItem.ToString())
            {
                case "1x":
                    resize = 1;
                    break;
                case "2x":
                    resize = 2;
                    break;
                case "3x":
                    resize = 3;
                    break;
                case "5x":
                    resize = 5;
                    break;
            }
            if (!isEmpty)
                Draw();
        }

        private void picture_MouseDown(object sender, MouseEventArgs e)
        {
            start = new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        private void picture_MouseUp(object sender, MouseEventArgs e)
        {
            if (start != null && !isEmpty)
            {
                Point end = new Point(Cursor.Position.X, Cursor.Position.Y);
                float dx = end.x - start.x;
                float dy = end.y - start.y;
                data = new FractalData
                    (data.Name, data.Depth, data.Colors, new Point(data.Center.x + dx, data.Center.y + dy));
                graph.Clear(defaultBG);
                _Draw();
                start = null;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            picture.Width = Width - 30;
            picture.Height = Height - 120;
            bmp = new Bitmap(picture.Width, picture.Height);
            graph = Graphics.FromImage(bmp);
            if (PossibleToOverDraw)
            {
                data.Center = new Point(picture.Width / 2, picture.Height / 2);
                _Draw();
            }
        }
    }
}
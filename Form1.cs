using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using static System.Math;
namespace CompGraf4
{
    public partial class Form1 : Form
    {
        Bitmap pic;
        Graphics graph;
        List<Point> points = new List<Point>();
        List<(Point, Point)> lines = new List<(Point, Point)>();
        List<List<Point>> polygons = new List<List<Point>>();
        Pen pen = new Pen(Color.Black, 1), rpen = new Pen(Color.Red, 5);
        bool first = false;
        bool cur_pol = false;
        (int, int) first_coord;
        int point_count = 0;
        int polygon_count = 0;
        int lines_count = 0;
        string curpol = "";
        string curpoint = "";
        string curline1 = "";
        string curline2 = "";
        public Form1()
        {
            InitializeComponent();
            pic = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = pic;
            graph = Graphics.FromImage(pictureBox1.Image);
            graph.Clear(Color.White);
            textBox1.Text = "0";
            textBox2.Text = "0";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            first = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked)
            {
                points.Add(new Point(e.X, e.Y));
                listView1.Items.Add("point" + ++point_count);
            }
            else if (radioButton2.Checked)
            {
                if (!first)
                {
                    first_coord = (e.X, e.Y);
                    first = true;
                }
                else
                {
                    lines.Add((new Point(first_coord.Item1, first_coord.Item2), new Point(e.X, e.Y)));
                    first = false;
                    listView1.Items.Add("line" + ++lines_count);
                }
            }
            else if (radioButton3.Checked)
            {
                polygons[polygons.Count-1].Add(new Point(e.X, e.Y));
                if (!cur_pol)
                {
                    listView1.Items.Add("polygon" + ++polygon_count);
                    cur_pol = true;
                }
            }
            DrawAll();
        }
        void DrawAll()
        {
            graph.Clear(Color.White);
            foreach (var x in points)
                //graph.DrawEllipse(pen, (float)(x.X - 0.5), (float)(x.Y - 0.5), 1, 1);
                pic.SetPixel(x.X, x.Y, Color.Black);
            pen.EndCap = LineCap.ArrowAnchor;
            foreach (var x in lines)
                graph.DrawLine(pen, x.Item1.X, x.Item1.Y, x.Item2.X, x.Item2.Y);
            pen.EndCap = LineCap.NoAnchor;
            foreach (var x in polygons)
            {
                graph.DrawLine(pen, x[0], x[x.Count-1]);
                for (int i = 0; i < x.Count-1; i++)
                    graph.DrawLine(pen, x[i], x[i+1]);
            }

            pictureBox1.Invalidate();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                polygons.Add(new List<Point>());
            else
                if (polygons[polygons.Count - 1].Count == 0)
            {
                polygons.RemoveAt(polygons.Count - 1);
            }
            else
                cur_pol = false;
        }
        void PicClear()
        {
            graph.Clear(Color.White);
            radioButton1.Checked = true;
            polygons.Clear();
            points.Clear();
            lines.Clear();
            polygon_count = 0; point_count = 0; lines_count = 0;
            listView1.Clear();
            pictureBox1.Invalidate();
            curpol = "";
            curpoint = "";
            curline1 = "";
            curline2 = "";
            textBox1.Text = "0";
            textBox2.Text = "0";
            trackBar1.Value = 0;
            label3.Text = "0";
            label4.Text = "100";
            label5.Text = "100";
            label6.Text = "точка";
            label7.Text = "ребро";
            label8.Text = "полигон";
            trackBar2.Value = 100;
            trackBar3.Value = 100;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            PicClear();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    if (listView1.SelectedItems[i].Text.StartsWith("polygon"))
                    {
                        curpol = listView1.SelectedItems[i].Text;
                        label8.Text = "полигон: " + curpol;
                    }
                    if (listView1.SelectedItems[i].Text.StartsWith("point"))
                    {
                        curpoint = listView1.SelectedItems[i].Text;
                        label6.Text = "точка: " + curpoint;
                    }
                    if (listView1.SelectedItems[i].Text.StartsWith("line"))
                    {
                        if (curline2.Length == 0)
                        {
                            curline2 = listView1.SelectedItems[i].Text;
                        }
                        else
                        {
                            curline1 = curline2;
                            curline2 = listView1.SelectedItems[i].Text;
                        }
                        label7.Text = "ребро: " + curline2;
                    }
                }
            }
        }
        double[,] multMatrix(double[,] a , double[,] b)
        {
            double[,] res = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
                for (int j = 0; j < b.GetLength(1); j++)
                    for (int z = 0; z < b.GetLength(0); z++)
                        res[i, j] += a[i, z] * b[z, j];
            return res;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                button2.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                
            }
            else
            {
                button2.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int dx, dy;
            if (curpol.Length == 0)
            {
                MessageBox.Show("Полигон не выбран");
                return;
            }
            try
            {
                dx = int.Parse(textBox1.Text);
                dy = int.Parse(textBox2.Text);
            }
            catch
            {
                MessageBox.Show("Смещение введено неверно");
                return;
            }
            if (!curpol.StartsWith("polygon"))
            {
                MessageBox.Show("Выбран НЕ полигон");
                return;
            }
            else
            {
                double[,] mat = { { 1, 0, 0 }, { 0, 1, 0 }, { dx, dy, 1 } };
                int ind = int.Parse(curpol.Substring(7));
                var curl = polygons[ind - 1];
                for (int i = 0; i < curl.Count; i++)
                {
                    Point x = curl[i];
                    double[,] tek = { { x.X, x.Y, 1 } };
                    double[,] newp = multMatrix(tek, mat);
                    x.X = (int)newp[0,0];
                    x.Y = (int)newp[0,1];
                    curl[i] = x;
                }
                polygons[ind - 1] = curl;
            }
            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                button3.Enabled = true;
                trackBar1.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
                trackBar1.Enabled = false;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton6.Checked)
            {
                button4.Enabled = true;
                trackBar2.Enabled = true;
                trackBar3.Enabled = true;
            }
            else
            {
                button4.Enabled = false;
                trackBar2.Enabled = false;
                trackBar3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (curpol.Length == 0)
            {
                MessageBox.Show("Полигон не выбран");
                return;
            }
            if (curpoint.Length == 0)
            {
                MessageBox.Show("Точка не выбрана");
                return;
            }
            int indpoint = int.Parse(curpoint.Substring(5));
            int dx = points[indpoint - 1].X;
            int dy = points[indpoint - 1].Y;
            double fi = int.Parse(label3.Text);
            double[,] turn = { {Cos(fi * PI/180),Sin(fi * PI / 180), 0 }, {-Sin(fi * PI / 180), Cos(fi * PI / 180), 0 },{0,0,1 } };
            double[,] movm = { { 1, 0, 0 }, { 0, 1, 0 }, { -dx, -dy, 1 } };
            double[,] movp = { { 1, 0, 0 }, { 0, 1, 0 }, { dx, dy, 1 } };
            int indpol = int.Parse(curpol.Substring(7));
            var curl = polygons[indpol - 1];
            for (int i = 0; i < curl.Count; i++)
            {

                Point x = curl[i];
                double[,] tek = { { x.X, x.Y, 1 } };
                double[,] newp = multMatrix(tek, movm);
                newp = multMatrix(newp, turn);
                newp = multMatrix(newp, movp);
                x.X = (int)newp[0, 0];
                x.Y = (int)newp[0, 1];
                curl[i] = x;
            }
            polygons[indpol - 1] = curl;
            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label3.Text = trackBar1.Value.ToString();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label4.Text = trackBar2.Value.ToString();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label5.Text = trackBar3.Value.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (curpol.Length == 0)
            {
                MessageBox.Show("Полигон не выбран");
                return;
            }
            if (curpoint.Length == 0)
            {
                MessageBox.Show("Точка не выбрана");
                return;
            }
            int indpoint = int.Parse(curpoint.Substring(5));
            int dx = points[indpoint - 1].X;
            int dy = points[indpoint - 1].Y;
            double a = int.Parse(label4.Text) / 100.0;
            double b = int.Parse(label5.Text) / 100.0;
            double[,] r = { { a, 0, 0 }, { 0, b, 0 }, { 0, 0, 1 } };
            double[,] movm = { { 1, 0, 0 }, { 0, 1, 0 }, { -dx, -dy, 1 } };
            double[,] movp = { { 1, 0, 0 }, { 0, 1, 0 }, { dx, dy, 1 } };
            int indpol = int.Parse(curpol.Substring(7));
            var curl = polygons[indpol - 1];
            for (int i = 0; i < curl.Count; i++)
            {

                Point x = curl[i];
                double[,] tek = { { x.X, x.Y, 1 } };
                double[,] newp = multMatrix(tek, movm);
                newp = multMatrix(newp, r);
                newp = multMatrix(newp, movp);
                x.X = (int)newp[0, 0];
                x.Y = (int)newp[0, 1];
                curl[i] = x;
            }
            polygons[indpol - 1] = curl;
            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                button5.Enabled = true;
            }
            else
            {
                button5.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (curline2.Length == 0)
            {
                MessageBox.Show("Ребро не выбрано");
                return;
            }
            int ind = int.Parse(curline2.Substring(4));
            var curl = lines[ind - 1];
            int dx = (curl.Item1.X + curl.Item2.X)/2;
            int dy = (curl.Item1.Y + curl.Item2.Y) / 2;
            int fi = 90;
            double[,] turn = { { Cos(fi * PI / 180), Sin(fi * PI / 180), 0 }, { -Sin(fi * PI / 180), Cos(fi * PI / 180), 0 }, { 0, 0, 1 } };
            double[,] movm = { { 1, 0, 0 }, { 0, 1, 0 }, { -dx, -dy, 1 } };
            double[,] movp = { { 1, 0, 0 }, { 0, 1, 0 }, { dx, dy, 1 } };
            Point x = curl.Item1;
            double[,] tek = { { x.X, x.Y, 1 } };
            double[,] newp = multMatrix(tek, movm);
            newp = multMatrix(newp, turn);
            newp = multMatrix(newp, movp);
            x.X = (int)newp[0, 0];
            x.Y = (int)newp[0, 1];
            curl.Item1 = x;
            x = curl.Item2;
            double[,] tek2 = { { x.X, x.Y, 1 } };
            newp = multMatrix(tek2, movm);
            newp = multMatrix(newp, turn);
            newp = multMatrix(newp, movp);
            x.X = (int)newp[0, 0];
            x.Y = (int)newp[0, 1];
            curl.Item2 = x;
            lines[ind - 1] = curl;
            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton8.Checked)
            {
                button6.Enabled = true;
            }
            else
            {
                button6.Enabled = false;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton9.Checked)
            {
                button7.Enabled = true;
            }
            else
            {
                button7.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (curline2.Length == 0 || curline1.Length == 0)
            {
                MessageBox.Show("Ребра не выбраны");
                return;
            }
            int ind1 = int.Parse(curline2.Substring(4));
            int ind2 = int.Parse(curline1.Substring(4));
            Point p1 = lines[ind1 - 1].Item1, p2 = lines[ind1 - 1].Item2, p3 = lines[ind2 - 1].Item1, p4 = lines[ind2 - 1].Item2;
            double x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y, x3 = p3.X, y3 = p3.Y, x4 = p4.X, y4 = p4.Y;
            (int, int)a = cross(x1, y1, x2, y2, x3, y3, x4, y4);
            int x = a.Item1,y=a.Item2;
            if (x1 > x2)
            {
                double t = x1;
                x1 = x2;
                x2 = t;
            }
            if (x3 > x4)
            {
                double t = x3;
                x3 = x4;
                x4 = t;
            }
            if(x == int.MaxValue)
            {
                MessageBox.Show("Отрезки не пересекаются");
                return;
            }
            if ((x >= x1 && x <= x2) && (x >= x3 && x <= x4))
            {
                graph.DrawEllipse(rpen, (float)(x - 2.5), (float)(y - 2.5), 5, 5);
                pictureBox1.Invalidate();
            }
            else
                MessageBox.Show("Отрезки не пересекаются");
        }
        public (int,int) cross(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            double n;
            if (y2 - y1 != 0)
            {
                double q = (x2 - x1) / (y1 - y2);
                double sn = (x3 - x4) + (y3 - y4) * q;
                if (sn == 0)
                {
                    MessageBox.Show("Отрезки не пересекаются");
                    return (int.MaxValue, int.MaxValue);
                }
                double fn = (x3 - x1) + (y3 - y1) * q;
                n = fn / sn;
            }
            else
            {
                if (y3 - y4 == 0)
                {
                    MessageBox.Show("Отрезки не пересекаются");
                    return (int.MaxValue, int.MaxValue);
                }
                n = (y3 - y1) / (y3 - y4);
            }
            double x = x3 + (x4 - x3) * n;
            double y = y3 + (y4 - y3) * n;
            return ((int)x, (int)y);
        }
        private void button7_Click(object sender, EventArgs e)
        {

            if (curpol.Length == 0)
            {
                MessageBox.Show("Полигон не выбран");
                return;
            }
            if (curpoint.Length == 0)
            {
                MessageBox.Show("Точка не выбрана");
                return;
            }

            int indpoint = int.Parse(curpoint.Substring(5));
            double x1 = points[indpoint - 1].X;
            double y1 = points[indpoint - 1].Y;
            int indpol = int.Parse(curpol.Substring(7));
            double x2 = 5000, y2 = 5000;
            int count = 0;
            List<Point> l = polygons[indpol - 1];
            double x3, y3, x4 = 0, y4 = 0;
            (int, int) a;
            for (int i = 0; i < l.Count-1; i++)
            {
                x3 = l[i].X; y3 = l[i].Y; x4 = l[i + 1].X; y4 = l[i + 1].Y;
                a = cross(x1, y1, x2, y2, x3, y3, x4, y4);
                //graph.DrawEllipse(rpen, (float)(a.Item1 - 2.5), (float)(a.Item2 - 2.5), 5, 5);
                if (x3 > x4)
                {
                    double t = x3;
                    x3 = x4;
                    x4 = t;
                }
                if (a.Item1 != int.MaxValue)
                    if ((a.Item1 >= x1 && a.Item1 <= x2) && (a.Item1 >= x3 && a.Item1 <= x4))
                    {
                        count++;
                        //graph.DrawEllipse(rpen, (float)(a.Item1 - 2.5), (float)(a.Item2 - 2.5), 5, 5);
                    }
            }
            x3 = l[0].X; y3 = l[0].Y; x4 = l[l.Count - 1].X; y4 = l[l.Count - 1].Y;
            a = cross(x1, y1, x2, y2, x3, y3, x4, y4);
            //graph.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
            if (x3 > x4)
            {
                double t = x3;
                x3 = x4;
                x4 = t;
            }
            if (a.Item1 != int.MaxValue)
                if ((a.Item1 >= x1 && a.Item1 <= x2) && (a.Item1 >= x3 && a.Item1 <= x4))
                {
                    count++;
                    graph.DrawEllipse(rpen, (float)(a.Item1 - 2.5), (float)(a.Item2 - 2.5), 5, 5);
                }
            //pictureBox1.Invalidate();
            if (count % 2 == 1)
                MessageBox.Show("Точка внутри полигона");
            else
                MessageBox.Show("Точка снаружи полигона");
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton10.Checked)
            {
                button8.Enabled = true;
            }
            else
            {
                button8.Enabled = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (curline2.Length == 0)
            {
                MessageBox.Show("Отрезок не выбран");
                return;
            }
            if (curpoint.Length == 0)
            {
                MessageBox.Show("Точка не выбрана");
                return;
            }
            int indl = int.Parse(curline2.Substring(4));
            int indp = int.Parse(curpoint.Substring(5));
            Point c = points[indp-1],b = lines[indl-1].Item2, a = lines[indl - 1].Item1;
            int xa = a.X, ya = a.Y, xb = b.X, yb = b.Y, xc = c.X, yc = c.Y;
            Console.WriteLine(yb * xa - xb * ya);
            if ((yc-ya)*(xb-xa) - (xc-xa)*(yb-ya) < 0)
            {
                MessageBox.Show("Точка слева");
            }
            if ((yc - ya) * (xb - xa) - (xc - xa) * (yb - ya) > 0)
            {
                MessageBox.Show("Точка справа");
            }
        }
    }
}

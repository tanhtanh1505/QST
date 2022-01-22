using KAutoHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QST
{
    public partial class Form1 : Form
    {
        private string dir = "";
        private int[] listTime;
        private Thread thread;
        private TextBox[] textBoxes;
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            thread = new Thread(RunTool);
            thread.Start();
        }
        private void RunTool()
        {
            for (int i = 0; i < 7; i++)
            {
                if (textBoxes[i].Text.Length > 0)
                {
                    getListTime(i);
                    for(int curSubject = 0; curSubject < listTime.Length; curSubject++)
                    {
                        if (listTime[curSubject] <= 0)
                            continue;
                        int cycle = listTime[curSubject] / 4 + 1;
                        Console.WriteLine(cycle);
                        for(int j = 0; j < cycle; j++)
                        {
                            Process.Start(textBox1.Text);
                            Thread.Sleep(20000);
                            selectSection(i, curSubject);
                            wheelMouse(4);
                            AutoControl.SendMultiKeysFocus(new KeyCode[] { KeyCode.CONTROL, KeyCode.KEY_W });
                        }
                    }
                }
            }
        }

        private void selectSection(int pos, int pos2)
        {
            Rectangle r = new Rectangle(0, 0, (int)(Screen.PrimaryScreen.Bounds.Width * 1.25),
                                                            (int)(Screen.PrimaryScreen.Bounds.Height * 1.25));
            var subBitmap = ImageScanOpenCV.GetImage(dir + pos.ToString() + ".png");

            Bitmap bmp = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(r.Left, r.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            Bitmap resBitmap = ImageScanOpenCV.Find(bmp, subBitmap);
            if (resBitmap != null)
            {
                Point resBitmap2 = (Point)ImageScanOpenCV.FindOutPoint(bmp, subBitmap);

                int x = (int)(resBitmap2.X / 1.25 + 10);
                int y = (int)(resBitmap2.Y / 1.25 + 8);
                AutoControl.MouseClick(x, y, EMouseKey.LEFT);
                for (int k = 0; k <= pos2; k++)
                {
                    Thread.Sleep(2000);
                    AutoControl.SendKeyFocus(KeyCode.TAB);
                }
                Thread.Sleep(2000);
                AutoControl.SendKeyFocus(KeyCode.ENTER);
            }
        }

        private void wheelMouse(int minute)
        {
            int cycle = minute * 20 + 1;
            for (int i = 0; i < cycle; i++)
            {
                AutoControl.MouseScroll(new Point(800, 400), -200, true);
                Thread.Sleep(3000);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (thread != null && thread.IsAlive)
                thread.Abort();
            thread = null;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(dir);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            creatDir();
            textBoxes = new TextBox[] { textBox2, textBox3, textBox4, textBox5,
                textBox6, textBox7, textBox8};
        }

        private void getListTime(int pos)
        {
            string[] authorsList = textBoxes[pos].Text.Split(',');
            listTime = new int[authorsList.Length];
            for(int i = 0; i < authorsList.Length; i++)
            {
                listTime[i] = Int32.Parse(authorsList[i]);
            }
        }

        void creatDir()
        {
            if (Directory.Exists("D:\\"))
            {
                dir = "D:\\QST\\Data\\";
            }
            else if (Directory.Exists("E:\\"))
            {
                dir = "E:\\QST\\Data\\";
            }
            else if (Directory.Exists("F:\\"))
            {
                dir = "F:\\QST\\Data\\";
            }
            else if (Directory.Exists("G:\\"))
            {
                dir = "G:\\QST\\Data\\";
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _931903.gorbatyuk.anastasiya.lab4
{
    public partial class Form1 : Form
    {
        int sizepanel_W, sizepanel_H; // размер панели
        int sizecell_W, sizecell_H;// размер ячейки

        const int cell_W = 30, cell_H = 30;//количество ячеек, далее номер 

        
        bool Proc; //процесс
        Bitmap Bmap; //объект рисования пикселями
        Graphics graphic; // поверхность рисования
        bool[,] Cell_Mat;//клеточная матрицы
        int toLive, live1, live2;
        
        public Form1()
        {
            InitializeComponent();
            One_Step();// создаем доску рисования и первое поколение
            PaintingOverCell(); //закрашиваем ячейки первого поколения
            DrawGridLines(); //Рисуем линии сетки
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toLive = (int)edToLive.Value;
            live1 = (int)edLive1.Value;
            live2 = (int)edLive2.Value;
            if (Proc)
            {
                timer1.Stop();
            }
            else
            {
                timer1.Start();
            }
            Proc = !Proc;
        }

        private void One_Step() // создаем доску рисования и первое поколение
        {
            sizepanel_W = panel1.Width;// считываем ширину панели
            sizepanel_H = panel1.Height;//считываем высоту панели
            sizecell_W = sizepanel_W / cell_W;//размер счейки равен размер панели по ширине/ количество ячеек
            sizecell_H = sizepanel_H / cell_H;//размер счейки равен размер панели по высоте/ количество ячеек 

            Bmap = new Bitmap(sizepanel_W, sizepanel_W);//задаем область рисования по размеру панели 
            graphic = Graphics.FromImage(Bmap);// создаем новый объект рисования из bm

            Cell_Mat = new bool[cell_H, cell_W];// создаем  тип ячейки с параметрами координат 


            //Создаем начальное поколение (можно с рандомом)
            Random rnd = new Random();
            int n = rnd.Next(100, 400);
          
            for (int i=0;i<=n;i++)
            {
                Cell_Mat[rnd.Next(0, 29), rnd.Next(0, 29)] = true;
            }
        }

        private void PaintingOverCell() //создаем кисть для заливки 
        {
            SolidBrush Brush = new SolidBrush(Color.Red); //создаем кисть заливки красного цвета

            //Закрашиваем ячейку,если она true
            for (int i = 0; i < cell_H; i++)
            {
                for (int j = 0; j <cell_W; j++)
                {
                    if (Cell_Mat[i, j])
                    {
                        graphic.FillRectangle(Brush, j * sizecell_W, i * sizecell_H, sizecell_W, sizecell_H);
                    }
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Bmap, Point.Empty);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            graphic.Clear(Color.White); //очищаем поверхность рисования
            Next_Step();  // создаем следующее поколение
            PaintingOverCell();//закрашиваем ячейки первого поколения
            DrawGridLines(); //Рисуем линии сетки
            panel1.Invalidate(); //перерисовываем панель
        }

        private void DrawGridLines() // Рисуем сетку
        {
            Pen pen = new Pen(Color.Black); //создаем кисть для рисования линий
            //вертикальные линии
            for (int i = 0; i < cell_W + 1; i++)
            {
                graphic.DrawLine(pen, i * sizecell_W, 0, i * sizecell_W, sizepanel_H);
            }
            // горизнтальные линии
            for (int i = 0; i < cell_H + 1; i++)
            {
                graphic.DrawLine(pen, 0, i * sizecell_H, sizepanel_W, i * sizecell_H);
            }
        }

        private void Next_Step() //создаем следующее поколение
        {
            bool[,] Next_Mat = new bool[cell_H, cell_W]; // создаем тип ячейки с параметрами координат следующего поколения

            for (int i = 0; i < cell_H; i++)
            {
                for (int j = 0; j < cell_W; j++)
                {
                    if (Cell_Mat[i, j]) // если ячейка живая
                    {
                        int alive = NumberLivingNeighbors(i, j); // число ее соседей
                        Next_Mat[i, j] = (alive == live2) || (alive == live1); // если 2 или 3 соседа, тогда жива
                    }
                    else //если ячейка мертвая
                    {
                        Next_Mat[i, j] = NumberLivingNeighbors(i, j) == toLive; // оживает, если 3 соседа рядом 
                    }
                }
            }
            Cell_Mat = Next_Mat; // переносим это поколение в массив старого поколения
        }

        private int NumberLivingNeighbors(int i, int j) // число живых пикселей вокруг  
        {
            int aliveNeighbours = 0; // число живых соседей
            for (int x = -1; x <= 1; x++) // -1 0 1
            {
                int m = i + x; // по высоте сверху и снизу

                if ((m >= 0) && (m < cell_H)) // если сосед в пределах сетки по высоте
                    for (int y = -1; y <= 1; y++) // -1 0 1
                    {
                        int a = j + y;

                        if ((a >= 0) && (a < cell_W)) // если сосед в пределах сетки по ширине
                            aliveNeighbours += Cell_Mat[m, a] ? 1 : 0; // если сосед существует, прибавляем его
                    }
            }

            aliveNeighbours -= Cell_Mat[i, j] ? 1 : 0; // вычитаем саму ячейку, если она есть

            return aliveNeighbours; // возвращаем число соседей
        }
    }
}

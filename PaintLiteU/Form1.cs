using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace PaintLiteU
{
    public partial class Paint : Form
    {
        bool drawing;
        GraphicsPath currentPath;
        Point oldLocation;
        Pen currentPen;
        int historyCounter=-1; //Счетчик истории  
        List<Image> History = new List<Image>(); //Список для истории  
        public Paint()
        {
            InitializeComponent();
            drawing = false; //Переменная, ответственная за рисование  
            currentPen = new Pen(Color.Black); //Инициализация пера с черным цветом  
            currentPen.Width = trackBar1.Value; //Инициализация толщины пера
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)//когда мы зажмём мышь мы начнем рисовать
        {

            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Сначала создайте новый файл!");
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                drawing = true;
                oldLocation = e.Location;
                currentPath = new GraphicsPath();
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //Очистка ненужной истории  
            History.RemoveRange(historyCounter + 1, History.Count - historyCounter - 1);
            History.Add(new Bitmap(pictureBox1.Image));
            if (historyCounter + 1 < 10) historyCounter++;
            if (History.Count - 1 == 10) History.RemoveAt(0);
            drawing = false;
            try
            {
                currentPath.Dispose();
            }
            catch { };
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                currentPath.AddLine(oldLocation, e.Location);
                g.DrawPath(currentPen, currentPath);
                oldLocation = e.Location;
                g.Dispose();
                pictureBox1.Invalidate();
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)//кнопка дополнительного цвета
            {
                currentPen.Color = colorDialog1.Color;
                ((Button)sender).BackColor = colorDialog1.Color;
            }
        }
        private void button1_Click(object sender, EventArgs e)//чистка холста
        {
            Bitmap pic = new Bitmap(1000, 1000);
            pictureBox1.Image = pic;
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)//изменение толщины линии
        {

        }
        void Save()
        {
            SaveFileDialog SaveDlg = new SaveFileDialog();
            SaveDlg.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image | *.png";
            SaveDlg.Title = "Save an Image File";
            SaveDlg.FilterIndex = 4; //По умолчанию будет выбрано последнее расширение *.png
            SaveDlg.ShowDialog();
            if (SaveDlg.FileName != "") //Если введено не пустое имя  
            {
                System.IO.FileStream fs =
                (System.IO.FileStream)SaveDlg.OpenFile();
                switch (SaveDlg.FilterIndex)
                {
                    case 1:
                        this.pictureBox1.Image.Save(fs, ImageFormat.Jpeg);
                        break;
                    case 2:
                        this.pictureBox1.Image.Save(fs, ImageFormat.Bmp);
                        break;
                    case 3:
                        this.pictureBox1.Image.Save(fs, ImageFormat.Gif);
                        break;
                    case 4:
                        this.pictureBox1.Image.Save(fs, ImageFormat.Png);
                        break;
                }
                fs.Close();
            }
        }
        void Open()
        {
            OpenFileDialog OP = new OpenFileDialog();
            OP.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image | *.png";
            OP.Title = "Open an Image File";
            OP.FilterIndex = 1; //По умолчанию будет выбрано первое расширение *.jpg И, когда пользователь укажет нужный путь к картинке, ее нужно будет загрузить в PictureBox:  
            if (OP.ShowDialog() != DialogResult.Cancel)
                pictureBox1.Load(OP.FileName);
            pictureBox1.AutoSize = true;
        }
        private void button2_Click(object sender, EventArgs e)//Кнопка сохранения файла
        {
            Save();
        }
        private void button11_Click(object sender, EventArgs e)//Открытие файла
        {
            Open();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            History.Clear();
            historyCounter = 0;
            Bitmap pic = new Bitmap(1000, 1000);
            pictureBox1.Image = pic;
            History.Add(new Bitmap(pictureBox1.Image));
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (History.Count != 0 && historyCounter != 0)
            {
                pictureBox1.Image = new Bitmap(History[--historyCounter]);
            }
            else MessageBox.Show("История пуста");
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyCounter < History.Count - 1)
            {
                pictureBox1.Image = new Bitmap(History[++historyCounter]);
            }
            else MessageBox.Show("История пуста");
        }
        private void styleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private void dashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Solid;
            styleToolStripMenuItem.Checked = true;
            dashToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = false;
            dashDotToolStripMenuItem.Checked = false;
        }
        private void dashDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDot;
        }
        private void dashDotDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDotDot;
        }
        private void dotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Dot;
        }
        
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Вы работаете с максимально простой версией данного приложения");
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
                currentPen.Width = trackBar1.Value;
        }

        private void panel3_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void colorToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            currentPen.Color = ((Button)sender).BackColor;
        }
    }
}


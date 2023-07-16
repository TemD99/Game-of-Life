using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GOLNEW
{
    public partial class Form1 : Form
    {
        int cellW = 30;
        int cellH = 30;

        bool[,] universe;
        bool[,] scracthPad;
        public bool showHud;
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Pink;
        Color numColor = Color.Red;
        CreateU createU = new CreateU();
        // The Timer class
        Timer timer = new Timer();

        // Generation count
       
        int generations = 0;
        int livingCells = 0;
        int random = 0;
        public Form1()
        {
            InitializeComponent();
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            universe = createU.CreateUniverse(cellW, cellH);
            scracthPad = createU.CreateUniverse(cellW, cellH);
            // Setup the timer
            timer.Interval = 20; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }
       
        // Calculate the next generation of cells
        private void NextGeneration()
        {

            livingCells = 0;
            int count = 0;
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    count = CountNeighbors(x, y);

                    if (universe[x, y] == true)
                    {
                        if ((count < 2) || (count > 3))
                        {
                            scracthPad[x, y] = false;

                        }
                        if ((count == 2) || (count == 3))
                        {
                            scracthPad[x, y] = true;
                            livingCells++;
                        }
                    }

                    if (universe[x, y] == false && !(count == 3))
                    {
                        scracthPad[x, y] = false;

                    }

                    if (universe[x, y] == false && count == 3)
                    {
                        scracthPad[x, y] = true;
                        livingCells++;
                    }



                }
            }
            bool[,] universe1 = universe;
            universe = scracthPad;
            scracthPad = universe1;
            generations++;



            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Living Cells = " + livingCells.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {

            if (showHud)
            {
                Brush brush2 = new SolidBrush(Color.FromArgb(200, Color.Red));
                //e.Graphics.DrawString(generations.ToString(), graphicsPanel1.Font, numBrush, new PointF(50,0));
                e.Graphics.DrawString("Generations: " + generations.ToString(), Font, brush2, new PointF(0, 0));
                e.Graphics.DrawString("Living Cells: " + livingCells.ToString(), Font, brush2, new PointF(0, 15));
                e.Graphics.DrawString("Boundary Type : Toroidal", Font, brush2, new PointF(0, 30));
                e.Graphics.DrawString("Universe Size : " + cellW.ToString() + "x" + cellH.ToString(), Font, brush2, new PointF(0, 45));

                brush2.Dispose();
            }
            
            
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Font font = new Font("Arial", cellWidth / 2.5f);
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels

                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    int neighborCount = CountNeighbors(x, y);
                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        if (customizeToolStripMenuItem.Checked)
                        {
                            if (neighborCount == 3)
                            {
                                e.Graphics.DrawString(neighborCount.ToString(), font, Brushes.Green, cellRect, format);
                            }
                            else if (neighborCount != 0)
                            {
                                e.Graphics.DrawString(neighborCount.ToString(), font, Brushes.Red, cellRect, format);
                            }
                        }
                           
                    }

                    else
                    {
                        if (customizeToolStripMenuItem.Checked)
                        {
                            if ((neighborCount == 2) || (neighborCount == 3))
                            {
                                e.Graphics.DrawString(neighborCount.ToString(), font, Brushes.Green, cellRect, format);
                            }
                            else if (neighborCount != 0)
                            {
                                e.Graphics.DrawString(neighborCount.ToString(), font, Brushes.Red, cellRect, format);
                            }
                        }

                    }
                    if (optionsToolStripMenuItem.Checked)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                   
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
           
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            Clear();
        }

        public void Clear()
        {
            generations = 0;
            livingCells = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Living Cells = " + livingCells.ToString();
            int x = universe.GetLength(0);
            int y = universe.GetLength(1);

            int n1 = 0;
            while (n1 < x)
            {
                int n2 = 0;
                while (true)
                {
                    if (n2 >= y)
                    {
                        n1++;
                        break;
                    }
                    universe[n1, n2] = false;
                    n2++;
                }
            }


            graphicsPanel1.Invalidate();
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private int CountNeighbors(int x, int y)
        {
            int n = 0;


            int upperBound = universe.GetUpperBound(0);
            int upperBound1 = universe.GetUpperBound(1);
            int n3 = (y > 0) ? (y - 1) : upperBound1;
            int n1 = (x > 0) ? (x - 1) : upperBound;


            if (universe[n1, n3] == true)
            {
                n++;
            }
            n1 = x;
            if (universe[n1, n3] == true)
            {
                n++;
            }
            n1 = (x < upperBound) ? (x + 1) : 0;
            if (universe[n1, n3] == true)
            {
                n++;
            }
            n1 = (x > 0) ? (x - 1) : upperBound;
            n3 = y;
            if (universe[n1, n3] == true)
            {
                n++;
            }
            n1 = (x < upperBound) ? (x + 1) : 0;
            if (universe[n1, n3] == true)
            {
                n++;
            }
            n1 = (x > 0) ? (x - 1) : upperBound;
            n3 = (y < upperBound1) ? (y + 1) : 0;
            if (universe[n1, n3] == true)
            {
                n++;
            }
            n1 = x;
            if (universe[n1, n3] == true)
            {
                n++;
            }
            n1 = (x < upperBound) ? (x + 1) : 0;
            if (universe[n1, n3] == true)
            {
                n++;
            }
            return n;
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;
            if(DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }

        }

        private void modalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModalDialog dlg = new ModalDialog();
            
            dlg.Miliseconds = timer.Interval;
            dlg.CellHeight = cellH;
            dlg.CellWidth = cellW;
            if (DialogResult.OK == dlg.ShowDialog())
            {
               
                timer.Interval = dlg.Miliseconds;
                cellH = dlg.CellHeight;
                cellW = dlg.CellWidth;
                CreateNewUniverse(cellW, cellH);
               
                graphicsPanel1.Invalidate();
            }
        }
        public void CreateNewUniverse(int x, int y)
        {
            bool[,] universe1 = new bool[x, y];
            universe = universe1;
            bool[,] universe2 = new bool[x, y];
            scracthPad = universe2;
        }
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
               cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
               gridColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        public void Randomize(int seed)
        {
            Clear();
            Random random = new Random(seed);
            int length = this.universe.GetLength(0);
            int num2 = this.universe.GetLength(1);
            livingCells = 0;
            generations = 0;
            int num3 = 0;
            while (num3 < length)
            {
                int num4 = 0;
                while (true)
                {
                    if (num4 >= num2)
                    {
                        num3++;
                        break;
                    }
                    if (random.Next(0, 3) == 0)
                    {
                        universe[num3, num4] = true;
                        livingCells++;
                    }
                    num4++;
                }
            }
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Living Cells = " + livingCells.ToString();
        }

        private void rToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomDialog dlg = new RandomDialog();
            dlg.RandomSeedNumber = random;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                random = dlg.RandomSeedNumber;
                if (random != 0)
                {
                    Randomize(random);
                }
                graphicsPanel1.Invalidate();

            }
        }

        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Randomize(random);
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customizeToolStripMenuItem.Checked = !customizeToolStripMenuItem.Checked;
            graphicsPanel1.Invalidate();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            optionsToolStripMenuItem.Checked = !optionsToolStripMenuItem.Checked;
            graphicsPanel1.Invalidate();
        }

       

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "All Files|*.*|Text|*.txt",
                FilterIndex = 2,
                DefaultExt = "cells"
            };
            if (DialogResult.OK == dialog.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dialog.FileName);
                writer.WriteLine($"!{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
                int num = 0;
                while (true)
                {
                    if ((num >= cellH))
                    {
                        writer.Close();
                        break;
                    }
                    StringBuilder builder = new StringBuilder();
                    int num2 = 0;
                    while (true)
                    {

                        if (num2 >= cellW)
                        {
                            writer.WriteLine(builder.ToString());
                            num++;
                            break;
                        }

                        builder.Append(universe[num2, num] ? 'O' : '.');
                        num2++;
                    }
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "All Files|*.*|Text|*.txt",
                FilterIndex = 2
            };
            if (DialogResult.OK == dialog.ShowDialog())
            {
                StreamReader reader = new StreamReader(dialog.FileName);
                int width = 0;
                int height = 0;
                while (true)
                {
                    if (reader.EndOfStream)
                    {
                        universe = createU.CreateUniverse(width, height);
                        reader.BaseStream.Seek(0L, SeekOrigin.Begin);
                        int num3 = 0;
                        while (true)
                        {
                            if (reader.EndOfStream)
                            {
                                reader.Close();
                                graphicsPanel1.Invalidate();
                                break;
                            }
                            string str2 = reader.ReadLine();
                            if (str2[0] != '!')
                            {
                                int num4 = 0;
                                while (true)
                                {
                                    if (num4 >= str2.Length)
                                    {
                                        num3++;
                                        break;
                                    }
                                    universe[num4, num3] = (str2[num4] == 'O') ? true: false;
                                    num4++;
                                }
                            }
                        }
                        break;
                    }
                    string str = reader.ReadLine();
                    if (str[0] != '!')
                    {
                        height++;
                        if (str.Length > width)
                        {
                            width = str.Length;
                        }
                    }
                }
            }
        }

        private void hudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hudToolStripMenuItem.Checked = !hudToolStripMenuItem.Checked;

            if (hudToolStripMenuItem.Checked)
            {
                hudToolStripMenuItem1.Checked = true;
                showHud = true;
            }
            if (!hudToolStripMenuItem.Checked)
            {
                hudToolStripMenuItem1.Checked = false;
                showHud = false;
            }

            graphicsPanel1.Invalidate();
        }

        private void hudToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            hudToolStripMenuItem1.Checked = !hudToolStripMenuItem1.Checked;

            if (hudToolStripMenuItem1.Checked)
            {
                hudToolStripMenuItem.Checked = true;
                showHud = true;
            }
            if (!hudToolStripMenuItem1.Checked)
            {
                hudToolStripMenuItem.Checked = false;
                showHud = false;
            }

            graphicsPanel1.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.PanelColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridColor = gridColor;

            Properties.Settings.Default.Save();
        }

        private void resetBackColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            graphicsPanel1.Invalidate();

        }

        private void resetCellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            cellColor = Properties.Settings.Default.CellColor;

            graphicsPanel1.Invalidate();

        }

        private void resetGridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            gridColor = Properties.Settings.Default.GridColor;
            graphicsPanel1.Invalidate();
        }

        private void reloadBackColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            graphicsPanel1.Invalidate();
        }

        private void reloadCellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            cellColor = Properties.Settings.Default.CellColor;
            graphicsPanel1.Invalidate();
        }

        private void reloadGridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            gridColor = Properties.Settings.Default.GridColor;
            graphicsPanel1.Invalidate();

        }
    }
}

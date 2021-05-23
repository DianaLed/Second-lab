using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2_Tomographycs
{
    public partial class Form1 : Form
    {
        Bin bin;
        View view;
        bool loaded;
        int currentLayer;
        int frameCount;
        DateTime NextFPSUpdate;
        bool needReload;
        protected void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Vizualizer (fps = {0})", frameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                frameCount = 0;
                trackBar2.Maximum = 2000;
                trackBar3.Maximum = 4000;
                trackBar3.Minimum = 1;
                trackBar2.Value = view.min;
                trackBar3.Value = view.width;
            }
            frameCount++;
        }
        public Form1()
        {
            InitializeComponent();
            bin = new Bin();
            view = new View();
            loaded = false;
            currentLayer = 0;
            frameCount = 0;
            NextFPSUpdate = DateTime.Now.AddSeconds(1);
            needReload = true;
            comboBox1.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            draw();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string name = dialog.FileName;
                Bin.readBIN(name);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
                trackBar1.Maximum = Bin.Z - 1;
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            needReload = true;
            view.width = trackBar3.Value;
            draw();
        }
        void draw()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        if (loaded)
                        {
                            if (needReload)
                            {
                                view.generateTextureImage(currentLayer);
                                view.Load2DTexture();
                                needReload = false;
                            }
                            view.DrawTexture();
                            glControl1.SwapBuffers();
                        }
                        break;
                    }
                case 1:
                    {
                        if (loaded)
                        {
                            view.DrawQuads(currentLayer);
                            glControl1.SwapBuffers();
                        }
                        break;
                    }
            }
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            needReload = true;
            view.min += trackBar2.Value - view.min;
            draw();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

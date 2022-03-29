using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OTTER
{
    public partial class izbornik : Form
    {
        public izbornik()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BGL level2 = new BGL(11);
            level2.frmIzbornik = this;
            level2.Igrac = textBox1.Text;
            level2.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BGL level1 = new BGL(7);
            level1.frmIzbornik = this;
            level1.Igrac = textBox1.Text;
            level1.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BGL level3 = new BGL(18);
            level3.frmIzbornik = this;
            level3.Igrac = textBox1.Text;
            level3.ShowDialog();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            BGL level4 = new BGL(22);
            level4.frmIzbornik = this;
            level4.Igrac = textBox1.Text;
            level4.ShowDialog();
        }


        private void prikazi_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedIndex == 0)
            {
                if (!File.Exists("dat.txt")) //ako ne postoji
                    return;

                using (StreamReader sr = File.OpenText("dat.txt")) //otvara postojeću tekstualnu
                {
                    string linija;
                    listBox1.Items.Clear();

                    while ((linija = sr.ReadLine()) != null)
                    {
                        if (linija.Contains("Level 1"))
                        {
                            listBox1.Items.Add(linija);
                        }
                    }
                }
            }

            else if (comboBox1.SelectedIndex == 1)
            {
                if (!File.Exists("dat.txt")) //ako ne postoji
                    return;

                using (StreamReader sr = File.OpenText("dat.txt")) //otvara postojeću tekstualnu
                {
                    string linija;
                    listBox1.Items.Clear();

                    while ((linija = sr.ReadLine()) != null)
                    {
                        if (linija.Contains("Level 2"))
                            listBox1.Items.Add(linija);
                    }
                }
            }

            if (comboBox1.SelectedIndex == 2)
            {
                if (!File.Exists("dat.txt")) //ako ne postoji
                    return;

                using (StreamReader sr = File.OpenText("dat.txt")) //otvara postojeću tekstualnu
                {
                    string linija;
                    listBox1.Items.Clear();

                    while ((linija = sr.ReadLine()) != null)
                    {
                        if (linija.Contains("Level 3"))
                            listBox1.Items.Add(linija);
                    }
                }
            }
            if (comboBox1.SelectedIndex == 3)
            {
                if (!File.Exists("dat.txt")) //ako ne postoji
                    return;

                using (StreamReader sr = File.OpenText("dat.txt")) //otvara postojeću tekstualnu
                {
                    string linija;
                    listBox1.Items.Clear();

                    while ((linija = sr.ReadLine()) != null)
                    {
                        if (linija.Contains("Level 4"))
                            listBox1.Items.Add(linija);
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

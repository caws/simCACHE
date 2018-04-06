using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simCACHE
{
    public partial class Form1 : Form
    {
        int tag = 0;
        int line = 0;
        int word = 4;
        int bloco = 16; //4 bits 
        int tamSet = 0;
        int set = 0;
        int tamCache = 1024;

        double nBlocosMemo = Math.Pow(2, 28); //2 elevado a 28
        Boolean deixaFuncionar = false;

        public static Form1 _Form1;

        public Form1()
        {
            InitializeComponent();
            _Form1 = this;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbTipoMapeamento.SelectedIndex = 0; 
            cmbPolSubstituicao.SelectedIndex = 0;
            cmbTamCache.SelectedIndex = 0;
            deixaFuncionar = true;
            CheckForIllegalCrossThreadCalls = false;
            for (int i = 0; i < 3; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView2.Rows.Add();
            }
            defineTipoMapeamento();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            funcoes teste = new funcoes();
            if (openFileDialog1.FileName != "openFileDialog1")
            {
                if (checkBox1.Checked == false)
                {
                    richTextBox1.Text = DateTime.Now.ToString();
                    defineTipoMapeamento();
                    dataGridView1.Enabled = false;
                    dataGridView2.Enabled = false;
                    if (cmbTipoMapeamento.SelectedIndex == 0) //mapeamento direto
                    {
                        Thread mpDireto = new Thread(() => teste.mapeamentoDireto(openFileDialog1.FileName, teste.potencia2(Convert.ToInt32(tamCache) / bloco), 32 - (teste.potencia2(Convert.ToInt32(tamCache) / bloco)) - word, word, 0));
                        mpDireto.Start();
                        Thread.Sleep(30);
                    }

                    if (cmbTipoMapeamento.SelectedIndex >= 1) //mapeamento associativo 2-way
                    {
                        if (cmbPolSubstituicao.SelectedIndex == 0)
                        {
                            Thread mpAssociativoLRU = new Thread(() => teste.mapeamentoAssociativo(openFileDialog1.FileName, 32 - (teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet)) - word, teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet), tamSet, true, false, word, 0, 0));
                            mpAssociativoLRU.Start();
                            Thread.Sleep(90);
                        }

                        if (cmbPolSubstituicao.SelectedIndex == 1)
                        {
                            Thread mpAssociativoFIFO = new Thread(() => teste.mapeamentoAssociativo(openFileDialog1.FileName, 32 - (teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet)) - word, teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet), tamSet, false, true, word, 0, 0));
                            mpAssociativoFIFO.Start();
                            Thread.Sleep(40);
                        }
                    }
                }
                else
                {
                    tamCache = 1024;
                    int a = 0;
                    while (a < 5){
                       
                        line = Convert.ToInt32(tamCache) / bloco;
                        line = teste.potencia2(line);

                        //direto

                        //teste.mapeamentoDireto(openFileDialog1.FileName, teste.potencia2(Convert.ToInt32(tamCache) / bloco), 32 - (teste.potencia2(Convert.ToInt32(tamCache) / bloco)) - word, word, a);

                        Thread mpDireto = new Thread(() => teste.mapeamentoDireto(openFileDialog1.FileName, teste.potencia2(Convert.ToInt32(tamCache) / bloco), 32 - (teste.potencia2(Convert.ToInt32(tamCache) / bloco)) - word, word, a));
                        mpDireto.Start();
                        Thread.Sleep(300);

                        //associativo
                        tamSet = 2;
                        for (int p = 1; p < 4; p++)
                        {
                            //teste.mapeamentoAssociativo(openFileDialog1.FileName, 32 - (teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet)) - word, teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet), tamSet, true, false, word, p, a);

                            Thread mpAssociativoLRU = new Thread(() => teste.mapeamentoAssociativo(openFileDialog1.FileName, 32 - (teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet)) - word, teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet), tamSet, true, false, word, p, a));
                             mpAssociativoLRU.Start();
                             Thread.Sleep(300);

                            //teste.mapeamentoAssociativo(openFileDialog1.FileName, 32 - (teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet)) - word, teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet), tamSet, false, true, word, p, a);

                            Thread mpAssociativoFIFO = new Thread(() => teste.mapeamentoAssociativo(openFileDialog1.FileName, 32 - (teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet)) - word, teste.potencia2(Convert.ToInt32(Math.Pow(2, line)) / tamSet), tamSet, false, true, word, p, a));
                            mpAssociativoFIFO.Start();
                            Thread.Sleep(300);
                            tamSet = tamSet * 2;
                        }
                        tamCache = tamCache * 2;
                        a++;
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um arquivo.");
            }

        }

        private void cmbTamCache_TextUpdate(object sender, EventArgs e)
        {
            
        }

        private void cmbTamCache_SelectedIndexChanged(object sender, EventArgs e)
        {
            defineTipoMapeamento();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        public string getNomeArquivo()
        {
            return openFileDialog1.FileName;
        }

        private void cmbTipoMapeamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            defineTipoMapeamento();
        }
        
        private void cmbPolSubstituicao_SelectedIndexChanged(object sender, EventArgs e)
        {
            defineTipoMapeamento();
        }

        public void defineTipoMapeamento()
        {
            if (deixaFuncionar == true)
            {
                funcoes teste = new funcoes();
                line = Convert.ToInt32(cmbTamCache.Text) / bloco;
                line = teste.potencia2(line);
                tamCache = Convert.ToInt32(cmbTamCache.Text);

                if (cmbTipoMapeamento.SelectedIndex == 0) //mapeamento direto
                {
                    tag = 32 - line - word;

                    label5.Visible = true;
                    labelSet.Visible = false;
                    labLine.Text = line.ToString();
                    labTag.Text = tag.ToString();
                    labWord.Text = word.ToString();
                }

                if (cmbTipoMapeamento.SelectedIndex == 1) //mapeamento associativo 2-way
                {
                    tamSet = 2;//pois é 2 way
                    set = Convert.ToInt32(Math.Pow(2, line)) / tamSet;
                    set = teste.potencia2(set);
                    tag = 32 - set - word;

                    label5.Visible = false;
                    labelSet.Visible = true;

                    labLine.Text = set.ToString();
                    labTag.Text = tag.ToString();
                    labWord.Text = word.ToString();
                }

                if (cmbTipoMapeamento.SelectedIndex == 2) //mapeamento associativo 4-way
                {
                    tamSet = 4;//pois é 4 way
                    set = Convert.ToInt32(Math.Pow(2, line)) / tamSet;
                    set = teste.potencia2(set);
                    tag = 32 - set - word;

                    label5.Visible = false;
                    labelSet.Visible = true;

                    labLine.Text = set.ToString();
                    labTag.Text = tag.ToString();
                    labWord.Text = word.ToString();
                }

                if (cmbTipoMapeamento.SelectedIndex == 3) //mapeamento associativo 8-way
                {
                    tamSet = 8;//pois é 8 way
                    set = Convert.ToInt32(Math.Pow(2, line)) / tamSet;
                    set = teste.potencia2(set);
                    tag = 32 - set - word;

                    label5.Visible = false;
                    labelSet.Visible = true;

                    labLine.Text = set.ToString();
                    labTag.Text = tag.ToString();
                    labWord.Text = word.ToString();
                }
            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        public void postaResultado(string texto)
        {
            lock (richTextBox1)
            {
                richTextBox1.AppendText("\n" + texto);
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        public void addcelulaGrid1(int indice, int cel, int valor)
        {
            lock (dataGridView1)
            {
                dataGridView1.Rows[indice].Cells[cel].Value = valor;
            }
        }

        public void addcelulaGrid2(int indice, int cel, int valor)
        {
            lock (dataGridView2)
            {
                dataGridView2.Rows[indice].Cells[cel].Value = valor;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }       

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                cmbTamCache.Enabled = false;
                cmbPolSubstituicao.Enabled = false;
                cmbTipoMapeamento.Enabled = false;
            }
            if (checkBox1.Checked == false)
            {
                cmbTamCache.Enabled = true;
                cmbPolSubstituicao.Enabled = true;
                cmbTipoMapeamento.Enabled = true;
            }

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear(); 
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            for (int i = 0; i < 3; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView2.Rows.Add();
            }
        }
    }
}

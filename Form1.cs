using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeHaming
{
    public partial class Form1 : Form
    {
        public int r = 4;
        public int m;
        public List<string> blocks;
        public List<string> newBlocks;
        public Dictionary<char, string> Codes = new Dictionary<char, string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m = (int)Math.Pow(2, r) - r - 1;
            blocks = new List<string>();
        }

        private void CreateByneryBlocks()
        {
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add("Symbol", "Символ");
            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns.Add("Code", "Код");
            dataGridView1.Columns[1].Width = 100;
            foreach (var ch in richTextBox1.Lines)
            {
                foreach (var c in ch)
                {
                    if (!Codes.Keys.Contains(c))
                    {
                        dataGridView1.Rows.Add(c, Convert.ToString(c, 2));
                        Codes.Add(c, Convert.ToString(c, 2));
                    }
                    richTextBox2.Text += Convert.ToString(c, 2);
                }
            }
            int k = 0;
            string param = "";
            foreach (var ch in richTextBox2.Lines)
            {
                foreach (var c in ch)
                {
                    param += c;
                    if (k == (m - 1)) { blocks.Add(param); k = -1; param = ""; }
                    k++;
                }
            }
            if (k % (m - 1) != 0) blocks.Add(param);
            if (blocks[blocks.Count - 1].Length != m)
                while(m - blocks[blocks.Count - 1].Length!=0)
                    blocks[blocks.Count - 1] += "0";
            for (int i = 0; i < blocks.Count; i++)
                richTextBox3.Text += blocks[i] + "\r\n";
        }
        private void SetControlBits()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = blocks[i].Insert(0, "0");
                blocks[i] = blocks[i].Insert(1, "0");
                blocks[i] = blocks[i].Insert(3, "0");
                blocks[i] = blocks[i].Insert(7, "0");
                int state = 0;
                for (int j = 0; j < blocks[i].Length; j++)
                    if (j % 2 == 0) state += Convert.ToInt32(blocks[i][j]);
                blocks[i] = blocks[i].Remove(0, 1).Insert(0, (state % 2).ToString());
                state = 0;
                for (int j = 1; j < blocks[i].Length - 1; j += 4)
                    state += Convert.ToInt32(blocks[i][j]) + Convert.ToInt32(blocks[i][j + 1]);
                blocks[i] = blocks[i].Remove(1, 1).Insert(1, (state % 2).ToString());
                state = 0;
                for (int j = 3; j < blocks[i].Length; j += 8)
                    state += Convert.ToInt32(blocks[i][j]) + Convert.ToInt32(blocks[i][j + 1])
                        + Convert.ToInt32(blocks[i][j + 2]) + Convert.ToInt32(blocks[i][j + 3]);
                blocks[i] = blocks[i].Remove(3, 1).Insert(3, (state % 2).ToString());
                state = 0;
                for (int j = 7; j < blocks[i].Length; j++)
                    state += Convert.ToInt32(blocks[i][j]);
                blocks[i] = blocks[i].Remove(7, 1).Insert(7, (state % 2).ToString());

                richTextBox4.Text += blocks[i] + "\r\n";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CreateByneryBlocks();
            SetControlBits();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox5.Text = "";
            newBlocks = new List<string>();
            foreach (var str in richTextBox4.Lines)
                richTextBox5.Text += str;

            string param = "";
            foreach (var ch in richTextBox5.Text)
            {
                param += ch;
                if (param.Length == (int)Math.Pow(2, r) - 1 && param != "")
                { newBlocks.Add(param); param = ""; }
            }
        }

        private void richTextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar != '1') && (e.KeyChar != '0'))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        private string invertor(string str, int numErr)
        {
            if (str[numErr] == '1') return str = str.Remove(numErr, 1).Insert(numErr, "0");
            else return str = str.Remove(numErr, 1).Insert(numErr, "1");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < newBlocks.Count; i++)
            {
                int NumError = 0;
                int newState = 0;
                string param = "";

                param = newBlocks[i];
                for (int j = 0; j < newBlocks[i].Length; j++)
                    if (j % 2 == 0&& j!=0) newState += Convert.ToInt32(newBlocks[i][j]) % 2;
                param = param.Remove(0, 1).Insert(0, (newState % 2).ToString());
                newState = 0;
                for (int j = 1; j < newBlocks[i].Length - 1; j += 4)
                {
                    if (j != 1)
                        newState += Convert.ToInt32(newBlocks[i][j]) % 2;
                    newState += Convert.ToInt32(newBlocks[i][j + 1]) % 2;
                }
                param = param.Remove(1, 1).Insert(1, (newState % 2).ToString());
                newState = 0;
                for (int j = 3; j < newBlocks[i].Length; j += 8)
                {
                    if (j != 3)
                        newState += Convert.ToInt32(newBlocks[i][j]) % 2;
                    newState += Convert.ToInt32(newBlocks[i][j + 1]) % 2
                        + Convert.ToInt32(newBlocks[i][j + 2]) % 2 + Convert.ToInt32(newBlocks[i][j + 3]) % 2;
                }
                param = param.Remove(3, 1).Insert(3, (newState % 2).ToString());
                newState = 0;
                for (int j = 8; j < newBlocks[i].Length; j++)
                    newState += Convert.ToInt32(newBlocks[i][j]) % 2;
                param = param.Remove(7, 1).Insert(7, (newState % 2).ToString());

                for (int j = 0; j < param.Length; j++)
                    if (param[j] != newBlocks[i][j]) NumError += j + 1;
                if(newBlocks[i]!=blocks[i])
                if (NumError != 0) newBlocks[i] = invertor(newBlocks[i], NumError - 1);

                richTextBox6.Text += newBlocks[i] + "\r\n";
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Dictionary<string,char> newDict = new Dictionary<string,char>();
            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Add("column1", "Код");
            dataGridView2.Columns.Add("column2", "Символ");
            foreach (var d in Codes)
            {
                newDict.Add(d.Value, d.Key);
                dataGridView2.Rows.Add(d.Value, d.Key);
            }
            string str = "";
            for (int i = 0; i < newBlocks.Count; i++)
            {
                newBlocks[i] = newBlocks[i].Remove(7, 1);
                newBlocks[i] = newBlocks[i].Remove(3, 1);
                newBlocks[i] = newBlocks[i].Remove(1, 1);
                newBlocks[i] = newBlocks[i].Remove(0, 1);
                str += newBlocks[i];
            }
            //for (int i = 0; i < blocks.Count; i++)
            //{
            //    blocks[i] = blocks[i].Remove(7, 1);
            //    blocks[i] = blocks[i].Remove(3, 1);
            //    blocks[i] = blocks[i].Remove(1, 1);
            //    blocks[i] = blocks[i].Remove(0, 1);
            //    str += blocks[i];
            //}
            string group = "";
            string result = "";
            foreach(var ch in str)
            {
                group+=ch;
                if (newDict.ContainsKey(group))
                {
                    result += newDict[group];
                    group = "";
                }
                if (group.Length > m) group = "";
            }
            richTextBox7.Text = result;
        }


        
    }
}

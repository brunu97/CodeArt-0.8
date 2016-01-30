using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeArt_BASIC
{
    public partial class InputBoxcs : Form
    {
        public string Exporta { get { return textBox1.Text; } }
        public InputBoxcs(string texto_pergunta,string nome_da_janela)
        {
            InitializeComponent();
            this.Text = nome_da_janela;
            label1.Text = texto_pergunta;
            if (label1.Width < 620)
            {
                this.Width = label1.Width + 30;
            }
            if (label1.Height > 13)
            {
                this.Height += label1.Height + 10;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

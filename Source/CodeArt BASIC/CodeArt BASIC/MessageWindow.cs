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
    public partial class MessageWindow : Form
    {
        public MessageWindow(string texto,string nome_janela)
        {            
            InitializeComponent();
            this.Text = nome_janela;
            label1.Text = texto;
            if (label1.Width < 620){
                this.Width = label1.Width + 30;
            }
            if (label1.Height > 13)
            {
                this.Height += label1.Height + 10;
            }
            

           
        }


        //Fecha a janela
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MessageWindow_Load(object sender, EventArgs e)
        {

        }
    }
}

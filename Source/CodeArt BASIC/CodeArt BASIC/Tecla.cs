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
    public partial class Tecla : Form
    {
        char tecla;
        bool marcador;
        string tecla2;
        public Tecla(char passado,bool tipo,string passado2)
        {
            tecla = passado;
            marcador = tipo;
            tecla2 = passado2;
            InitializeComponent();
        }

        private void Tecla_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (marcador == false)
            {
                if ((e.KeyChar == tecla) || (e.KeyChar == char.ToUpper(tecla)))
                {
                    this.Close();
                }
            }
          
        }

        private void Tecla_KeyDown(object sender, KeyEventArgs e)
        {
            if (marcador == true)
            {
                if ((e.KeyCode == Keys.Enter) && (tecla2 == "ENTER"))
                {
                    this.Close();
                }
                if ((e.KeyCode == Keys.Escape) && (tecla2 == "ESCAPE"))
                {
                    this.Close();
                }
                if ((e.KeyCode == Keys.Space) && (tecla2 == "SPACE"))
                {
                    this.Close();
                }
            }
        }
    }
}

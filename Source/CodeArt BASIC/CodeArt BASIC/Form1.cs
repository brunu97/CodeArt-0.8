using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;


namespace CodeArt_BASIC
{
     
    public partial class Form1 : Form
    {        
        int Caneta_Transparencia;
        int Caneta_Cor1;
        int Caneta_Cor2;
        int Caneta_Cor3;
        int Caneta_Tamamho;
        Pen Caneta_para_desenhar;
        Brush corfundo;

        public string ReturnValue1 { get; set; } 
        string local;
        public Form1(string portado)
        {
            InitializeComponent();
            local = portado;
           
            
        }     

        private void CodeArtBASIC()
        {
           
            {
                
                //Guarda valores
                var PenCodeArt = new Dictionary<string, string>();
                var BrushCodeArt = new Dictionary<string, string>();
                var TextVarCodeArt = new Dictionary<string, string>();
                var NumberVarCodeAr = new Dictionary<string, int>();
                var FontVarCodeArt = new Dictionary<string, Font>();
                var LoopNameCodeArt = new Dictionary<string, int>();
                var LoopvezesCodeArt = new Dictionary<string, int>();

                var PontosCodeArt = new Dictionary<string, Point>();
                var ListaPontos = new Dictionary<string, Point[]>();

                var TextureCodeArt = new Dictionary<string, string>();

                Regex itens_x = new Regex(@"^\w+$"); //Não permite simbolos nos nomes de um Number,Point e Text e outros
                Random aleatorio = new Random(); //Só um para todo o programa, senão os números repetem-se
                int linhas_lidas = 0;


                //Define campo de desenho
                Bitmap bmp = new Bitmap(2000, 2000); //Tamanho da Folha
                
                Graphics g = Graphics.FromImage(bmp);
                pictureBox1.Image = bmp;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                //Conta Todas as Linhas
                int linhas = richTextBox1.Lines.Count();


                for (int topo = 0; topo < linhas; topo++)
                {
                    string linha = richTextBox1.Lines[topo];
                    string escrito = linha.TrimStart();
                    int linha_marcada = topo + 1; //Serve só para mostrar a linha visual atual de leitura

                    //De vez em quando mostra o desenho e deixa a janela responder (Ciclos sem Pause)
                    linhas_lidas++;
                    if (linhas_lidas % 500 == 0)
                    {
                        pictureBox1.Refresh();
                        Application.DoEvents();
                        if (this.IsDisposed) { break; } //A janela foi fechada
                    }

                    if (string.IsNullOrWhiteSpace(linha)) { continue; }
                    else if (escrito.StartsWith("WindowIcon.Hide"))
                    {
                        escrito = escrito.Substring(15).Trim();
                        this.ShowIcon = false;
                        continue;
                    }
                    else if (escrito.StartsWith("WindowIcon.Show"))
                    {
                        escrito = escrito.Substring(15).Trim();
                        this.ShowIcon = true;
                        continue;

                    }
                    else if (escrito.StartsWith("WindowIcon.Set"))
                    {
                        Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                        escrito = regex.Replace(escrito, @" ");
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(")", "");
                        escrito = escrito.Replace(",", " ");
                        escrito = escrito.Substring(14).Trim();
                        if (TextureCodeArt.ContainsKey(escrito))
                        {
                            try
                            {
                            this.Icon = new Icon(TextureCodeArt[escrito]);
                            continue;
                            }
                            catch
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Icon is invalid", "CodeArt Error"); break; 
                            }
                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Texture does not exist", "CodeArt Error"); break; 
                        }
                    }
                    else if (escrito.StartsWith("Font")) //Font f = Tamanho Estilo Tipo
                    {
                        try
                        {
                            Font f;
                            int a2;
                            string a1;
                            string a3;
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace(",", " ");
                            escrito = escrito.Substring(4).Trim();
                            string valor_escrito = escrito.Substring(escrito.LastIndexOf('=') + 1).Trim();
                            int tamanho_do_valor_escrito = valor_escrito.Length;
                            string font_nome = escrito.Substring(0, escrito.LastIndexOf('=')).Trim();

                            if (font_nome.Contains(" ") || string.IsNullOrWhiteSpace(font_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Font can not be empty or contain spaces", "CodeArt Error"); break; } //Var não pode ter espaços
                            if (font_nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Font should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(font_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Font can not have symbols", "CodeArt Error"); break; }

                            var coisas = valor_escrito.Trim().Split(' ');
                            if (coisas.Length < 3)
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                            }
                            if (TextVarCodeArt.ContainsKey(coisas[1])) { a1 = TextVarCodeArt[coisas[1]]; }
                            else
                            {
                                 a1 = coisas[1];
                            }
                            if (NumberVarCodeAr.ContainsKey(coisas[0])) { a2 = NumberVarCodeAr[coisas[0]]; }
                            else
                            {
                               a2 = Convert.ToInt32(coisas[0]);
                            }
                            coisas = coisas.Skip(2).ToArray();
                            string x = string.Join(" ", coisas).Trim();
                            
                            if (TextVarCodeArt.ContainsKey(x)) { a3 = TextVarCodeArt[x]; }
                            else
                            {
                                a3 = x;
                            }
                           
                            if (FontVarCodeArt.ContainsKey(font_nome)) { FontVarCodeArt.Remove(font_nome); }
                            if (a1 == "Bold")
                            {
                                f = new Font(a3, a2, FontStyle.Bold);
                            } else
                            if (a1 == "Italic")
                            {
                                f = new Font(a3, a2, FontStyle.Italic);
                            } else
                            if (a1 == "Regular")
                            {
                                f = new Font(a3, a2, FontStyle.Regular);
                            } else
                            if (a1 == "Strikeout")
                            {
                                f = new Font(a3, a2, FontStyle.Strikeout);
                            } else
                            if (a1 == "Underline")
                            {
                                f = new Font(a3, a2, FontStyle.Underline);
                            }
                            else
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid Font Style", "CodeArt Error"); break;
                            }
                            FontVarCodeArt.Add(font_nome, f);
                            continue;
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }


                    }

                    else if (escrito.StartsWith("Draw.Text")) //draw.text Text Font Brush Ponto/Number Number
                    {
                        try
                        {
                            escrito = escrito.Substring(9).Trim();
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace(",", " ");
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");
                            var valores = escrito.Trim().Split(' ');

                            if (!TextVarCodeArt.ContainsKey(valores[0])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Text does not exist", "CodeArt Error"); break; }
                            if (!FontVarCodeArt.ContainsKey(valores[1])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Font does not exist", "CodeArt Error"); break; }
                            if (!BrushCodeArt.ContainsKey(valores[2])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Brush does not exist", "CodeArt Error"); break; }
                            if (valores.Length == 4)
                            {
                                if (!PontosCodeArt.ContainsKey(valores[3])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Point does not exist", "CodeArt Error"); break; }
                                Font k = FontVarCodeArt[valores[1]];
                                string texto = TextVarCodeArt[valores[0]];
                                Point p = PontosCodeArt[valores[3]];
                                if (BrushCodeArt.ContainsKey(valores[2]))
                                {
                                    string caneta_valor = BrushCodeArt[valores[2]];

                                    var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                    Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                    Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                    Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                    Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                    corfundo = new SolidBrush(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3));
                                }
                                else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, Brush does not exist", "CodeArt Error"); break; }
                                g.DrawString(texto, k, corfundo, p);

                            }

                            else if (valores.Length == 5)
                            {
                                int n1;
                                int n2;
                                if (NumberVarCodeAr.ContainsKey(valores[3]))
                                {
                                    n1 = NumberVarCodeAr[valores[3]];
                                }
                                else
                                {
                                    n1 = Convert.ToInt32(valores[3]);
                                }

                                if (NumberVarCodeAr.ContainsKey(valores[4]))
                                {
                                    n2 = NumberVarCodeAr[valores[4]];
                                }
                                else
                                {
                                    n2 = Convert.ToInt32(valores[4]);
                                }

                                Font k = FontVarCodeArt[valores[1]];
                                string texto = TextVarCodeArt[valores[0]];
                                Point p = new Point(n1, n2);
                                if (BrushCodeArt.ContainsKey(valores[2]))
                                {
                                    string caneta_valor = BrushCodeArt[valores[2]];

                                    var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                    Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                    Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                    Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                    Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                    corfundo = new SolidBrush(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3));
                                }
                                else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, Brush does not exist", "CodeArt Error"); break; }
                                g.DrawString(texto, k, corfundo, p);

                            }
                            else
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }
                        else
                    if (escrito.StartsWith("TextToNumber"))
                    {
                        try
                        {
                            string texto;
                            escrito = escrito.Substring(12).Trim();
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace(",", " ");
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");
                            var valores = escrito.Trim().Split(' ');

                            if (valores.Length != 2)
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                            }

                            if (!TextVarCodeArt.ContainsKey(valores[0]))
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Text does not exist", "CodeArt Error"); break;
                            }
                            else
                            {
                                texto = TextVarCodeArt[valores[0]];
                            }
                            if (valores[1].All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(valores[1])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                            if (NumberVarCodeAr.ContainsKey(valores[1])) { NumberVarCodeAr.Remove(valores[1]); }

                            NumberVarCodeAr.Add(valores[1], Convert.ToInt32(texto));
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }
                    else

                    if (escrito.StartsWith("NumberToText"))
                    {
                        try
                        {
                            int number;
                            escrito = escrito.Substring(12).Trim();
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace(",", " ");
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");
                            var valores = escrito.Trim().Split(' ');

                            if (valores.Length != 2)
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() +"\nInvalid function, invalid values", "CodeArt Error"); break;
                            }

                            if (!NumberVarCodeAr.ContainsKey(valores[0]))
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Number does not exist", "CodeArt Error"); break;
                            }
                            else
                            {
                                number = NumberVarCodeAr[valores[0]];
                            }
                            if (valores[1].All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(valores[1])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text can not have symbols", "CodeArt Error"); break; }
                            if (TextVarCodeArt.ContainsKey(valores[1])) { TextVarCodeArt.Remove(valores[1]); }

                            TextVarCodeArt.Add(valores[1], number.ToString());
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }                    
                    else
                    if (escrito.StartsWith("Draw.FilledRectangle"))
                    {
                        try
                        {
                            int primeiro;
                            int segundo;
                            int terceiro;
                            int quarto;
                            string quinto;

                            escrito = escrito.Substring(20).Trim();
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');

                            if (!PontosCodeArt.ContainsKey(valores[0]))
                            {
                                if (valores.Count() != 5)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }

                                if (NumberVarCodeAr.ContainsKey(valores[0])) { primeiro = NumberVarCodeAr[valores[0]]; }
                                else { primeiro = Convert.ToInt32(valores[0]); }

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { segundo = NumberVarCodeAr[valores[1]]; }
                                else { segundo = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { terceiro = NumberVarCodeAr[valores[2]]; }
                                else { terceiro = Convert.ToInt32(valores[2]); }

                                if (NumberVarCodeAr.ContainsKey(valores[3])) { quarto = NumberVarCodeAr[valores[3]]; }
                                else { quarto = Convert.ToInt32(valores[3]); }

                                quinto = valores[4];
                                quinto = quinto.Trim();
                            }
                            else
                            {
                                if (valores.Count() != 4)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }
                                primeiro = PontosCodeArt[valores[0]].X;
                                segundo = PontosCodeArt[valores[0]].Y;

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { terceiro = NumberVarCodeAr[valores[1]]; }
                                else { terceiro = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { quarto = NumberVarCodeAr[valores[2]]; }
                                else { quarto = Convert.ToInt32(valores[2]); }

                                quinto = valores[3];
                                quinto = quinto.Trim();
                            }

                            //Obtem a Brush
                            if (BrushCodeArt.ContainsKey(quinto))
                            {
                                string caneta_valor = BrushCodeArt[quinto];

                                var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                corfundo = new SolidBrush(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3));
                            }
                            else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, Brush does not exist", "CodeArt Error"); break; }


                            g.FillRectangle(corfundo, new Rectangle(primeiro, segundo, terceiro, quarto));
                            continue;
                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values or nonexistent", "CodeArt Error"); break; }
                    }
                    else
                    if (escrito.StartsWith("Texture"))
                    {

                        if (!escrito.Contains("=")) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                        escrito = escrito.Substring(7).Trim();
                        string valor_escrito = escrito.Substring(escrito.LastIndexOf('=') + 1).Trim(); //Obtem o Valor da Textura
                        int tamanho_do_valor_escrito = valor_escrito.Length;
                        string Textura_nome = escrito.Substring(0, escrito.LastIndexOf('=')).Trim(); //Obtem o nome da Textura

                        if (Textura_nome.Contains(" ") || string.IsNullOrWhiteSpace(Textura_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Texture can not be empty or contain spaces", "CodeArt Error"); break; } //Var não pode ter espaços
                        if (Textura_nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Texture should not only take numbers", "CodeArt Error"); break; }
                        if (!itens_x.IsMatch(Textura_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Texture can not have symbols", "CodeArt Error"); break; }
                        
                        if (TextureCodeArt.ContainsKey(Textura_nome))
                        {
                            TextureCodeArt.Remove(Textura_nome);
                        }
                        if (TextVarCodeArt.ContainsKey(valor_escrito)) { valor_escrito = TextVarCodeArt[valor_escrito]; }
                       TextureCodeArt.Add(Textura_nome, valor_escrito);
                        continue;
                    }

                    else
                    if (escrito.StartsWith("Draw.FilledTextureRectangle"))
                    {
                        try
                        {
                            int primeiro;
                            int segundo;
                            int terceiro;
                            int quarto;
                            string quinto;

                            escrito = escrito.Substring(27).Trim();
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');

                            if (!PontosCodeArt.ContainsKey(valores[0]))
                            {
                                if (valores.Count() != 5)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }

                                if (NumberVarCodeAr.ContainsKey(valores[0])) { primeiro = NumberVarCodeAr[valores[0]]; }
                                else { primeiro = Convert.ToInt32(valores[0]); }

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { segundo = NumberVarCodeAr[valores[1]]; }
                                else { segundo = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { terceiro = NumberVarCodeAr[valores[2]]; }
                                else { terceiro = Convert.ToInt32(valores[2]); }

                                if (NumberVarCodeAr.ContainsKey(valores[3])) { quarto = NumberVarCodeAr[valores[3]]; }
                                else { quarto = Convert.ToInt32(valores[3]); }

                                quinto = valores[4];
                                quinto = quinto.Trim();
                            }
                            else
                            {
                                if (valores.Count() != 4)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }
                                primeiro = PontosCodeArt[valores[0]].X;
                                segundo = PontosCodeArt[valores[0]].Y;

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { terceiro = NumberVarCodeAr[valores[1]]; }
                                else { terceiro = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { quarto = NumberVarCodeAr[valores[2]]; }
                                else { quarto = Convert.ToInt32(valores[2]); }

                                quinto = valores[3];
                                quinto = quinto.Trim();
                            }

                            //Obtem a Textura
                            if (TextureCodeArt.ContainsKey(quinto))
                            {
                                string caneta_valor = TextureCodeArt[quinto];
                                Image s = Image.FromFile(caneta_valor);
                                corfundo = new TextureBrush(s);
                            }
                            else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, Texture does not exist", "CodeArt Error"); break; }


                            g.FillRectangle(corfundo, new Rectangle(primeiro, segundo, terceiro, quarto));
                            continue;
                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values or nonexistent", "CodeArt Error"); break; }
                    }
                    else
                    if (escrito.StartsWith("AboutCodeArt"))
                    {
                        MessageBox.Show("CodeArt BASIC 0.85\nMade By Bruno Silva\nNovember 2026");
                        continue;
                    }
                    else
                    if (escrito.StartsWith("Brush")) 
                    {
                        int index_max;
                        int vt_opacidade;
                        int vt_red;
                        int vt_green;
                        int vt_blue;
                        Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                        escrito = regex.Replace(escrito, @" ");
                        if (!escrito.Contains("=")) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                        escrito = escrito.Substring(5);

                        int tamanho_total = escrito.Length;
                        
                        string valor_escrito = escrito.Substring(escrito.LastIndexOf('=') + 1).Trim(); //Obtem o Valor da Brush
                        int tamanho_do_valor_escrito = valor_escrito.Length;
                        string Brush_nome = escrito.Substring(0, escrito.LastIndexOf('=')).Trim(); //Obtem num string o nome da Brush

                        if (Brush_nome.Contains(" ") || string.IsNullOrWhiteSpace(Brush_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a brush can not be empty or contain spaces", "CodeArt Error"); break; } //Var não pode ter espaços
                        if (Brush_nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Brush should not only take numbers", "CodeArt Error"); break; }
                        if (!itens_x.IsMatch(Brush_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a brush can not have symbols", "CodeArt Error"); break; }
                        if (BrushCodeArt.ContainsKey(Brush_nome)) { BrushCodeArt.Remove(Brush_nome); }
             
                        valor_escrito = valor_escrito.Replace("(", "");
                        valor_escrito = valor_escrito.Replace(")", "");
                        valor_escrito = valor_escrito.Replace(",", " ");
                        string[] teste_valido = valor_escrito.Split(' ');
                        try
                        {
                            if (NumberVarCodeAr.ContainsKey(teste_valido[0])) { vt_opacidade = NumberVarCodeAr[teste_valido[0]]; }
                            else { vt_opacidade = Convert.ToInt32(teste_valido[0]); }

                            if (NumberVarCodeAr.ContainsKey(teste_valido[1])) { vt_red = NumberVarCodeAr[teste_valido[1]]; }
                            else { vt_red = Convert.ToInt32(teste_valido[1]); }

                            if (NumberVarCodeAr.ContainsKey(teste_valido[2])) { vt_green = NumberVarCodeAr[teste_valido[2]]; }
                            else { vt_green = Convert.ToInt32(teste_valido[2]); }

                            if (NumberVarCodeAr.ContainsKey(teste_valido[3])) { vt_blue = NumberVarCodeAr[teste_valido[3]]; }
                            else { vt_blue = Convert.ToInt32(teste_valido[3]); }                            
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }

                        try
                        {
                            //Determinar se os valores da Pen são possiveis
                            index_max = teste_valido.Count();

                            if (index_max > 4) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; } //Limite de valores
                            if (vt_opacidade > 255 || vt_opacidade < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nOpacity can not be greater than 255 or less than 0", "CodeArt Error"); break; }
                            if (vt_red > 255 || vt_red < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nRed can not be greater than 255 or less than 0", "CodeArt Error"); break; }
                            if (vt_green > 255 || vt_green < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nGreen can not be greater than 255 or less than 0", "CodeArt Error"); break; }
                            if (vt_blue > 255 || vt_blue < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nBlue can not be greater than 255 or less than 0", "CodeArt Error"); break; }

                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                        string Cores_da_brush = vt_opacidade.ToString() + " " + vt_red.ToString() + " " + vt_green.ToString() + " " + vt_blue.ToString();
                        BrushCodeArt.Add(Brush_nome, Cores_da_brush);
                        continue;
                    }
                    else
                    if (escrito.StartsWith("PointList"))
                    {
                        try
                        {
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");
                            string valor_escrito = escrito.Substring(escrito.LastIndexOf('=') + 1).Trim(); //Obtem o Valor da Var
                            escrito = escrito.Substring(9);
                            string var_nome = escrito.Substring(0, escrito.LastIndexOf('=')).Trim(); //Obtem num string o nome da Var

                            if (var_nome.Contains(" ") || string.IsNullOrWhiteSpace(var_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a PointList can not be empty or contain spaces", "CodeArt Error"); break; } //Var não pode ter espaços
                            if (var_nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a PointList should not only take numbers.", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(var_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a PointList can not have symbols", "CodeArt Error"); break; }
                            var valores = valor_escrito.Split(' ');
                            int compri = valores.Length;


                            string j = "";
                            bool lista_valida = true;
                            for (int x = 0; x < compri; x++)
                            {
                                if (valores[x].All(char.IsDigit))
                                {
                                    j += " " + valores[x];
                                    continue;
                                } else

                                if (NumberVarCodeAr.ContainsKey(valores[x]))
                                {
                                    j += " " + NumberVarCodeAr[valores[x]];
                                    continue;
                                } else

                                if (PontosCodeArt.ContainsKey(valores[x]))
                                {
                                    j += " " + PontosCodeArt[valores[x]].X.ToString() + " " + PontosCodeArt[valores[x]].Y.ToString();

                                    continue;
                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error");
                                    lista_valida = false;
                                    break;
                                }

                            }
                            if (lista_valida == false) { break; }
                            var pontos = j.Trim().Split(' ');
                            int quantos = pontos.Length / 2;
                            Point[] lista = new Point[quantos];
                            int y = 0;
                            for (int k = 0; k < quantos; k++)
                            {

                                lista[k] = new Point(Convert.ToInt32(pontos[y]), Convert.ToInt32(pontos[y + 1]));
                                y += 2;


                            }

                            if (ListaPontos.ContainsKey(var_nome)) { ListaPontos.Remove(var_nome); }
                            ListaPontos.Add(var_nome, lista);
                            continue;
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; 
                        }
                    }
                    else
                    if (escrito.StartsWith("WaitForKey"))
                    {
                        escrito = escrito.Substring(10).Trim();
                        
                        char tecla = ' ';
                        string outratecla = " ";
                        bool tipo = false;
                        string teclas_normais = "QWERTYUIOPASDFGHJKLÇZXCVBNM1234567890";

                        if (escrito == ".Enter") { outratecla = "ENTER"; tipo = true; } else
                        if (escrito == ".Space") { outratecla = "SPACE"; tipo = true; } else
                        if (escrito == ".Escape") { outratecla = "ESCAPE"; tipo = true; } else
                        if (escrito.Length == 2 && escrito[0] == '.' && teclas_normais.Contains(escrito[1]))
                        {
                            tecla = char.ToLowerInvariant(escrito[1]);
                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }

                        pictureBox1.Refresh(); //Mostra o desenho antes de esperar pela tecla
                        Tecla tyu = new Tecla(tecla,tipo,outratecla);
                        tyu.ShowDialog();
                        continue;
                    }
                    else
                    if (escrito.StartsWith("TakeScreenShot"))
                    {
                        escrito = escrito.Substring(14).Trim();
                        pictureBox1.Refresh(); //Mostra o desenho antes da imagem
                        if (escrito == ".All")
                        {
                            escrito = escrito.Substring(4);
                            using (Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
                            {
                                using (Graphics img = Graphics.FromImage(bitmap))
                                {
                                    img.CopyFromScreen(Point.Empty, Point.Empty, Screen.PrimaryScreen.Bounds.Size);
                                }
                                bitmap.Save("Image.Png", ImageFormat.Png);
                                continue;
                            }

                        }
                        else if (escrito == ".Window")
                        {
                            escrito = escrito.Substring(7);
                            using (Bitmap bitmap = new Bitmap(this.Width,this.Height))
                            {
                                using (Graphics img = Graphics.FromImage(bitmap))
                                {
                                    img.CopyFromScreen(this.Location, Point.Empty, this.Size);
                                }
                                bitmap.Save("Image.Png", ImageFormat.Png);
                                continue;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; 
                        }
                    }
                    else
                    if (escrito.StartsWith("StopDraw")) //Para o programa
                    {
                        escrito = escrito.Substring(8).Trim();
                        if (escrito.Length > 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                        break;
                    }
                    else
                    if (escrito.StartsWith("WindowBorder"))
                    {
                        escrito = escrito.Substring(12).TrimEnd();
                        if (escrito == ".Hide")
                        {
                            this.FormBorderStyle = FormBorderStyle.None;
                            continue;
                        }
                        else if (escrito == ".Show")
                        {
                            this.FormBorderStyle = FormBorderStyle.FixedDialog;
                            continue;
                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; 
                        }
                    }
                    else
                    if (escrito.StartsWith("CloseWindow"))
                    {
                        escrito = escrito.Substring(11).Trim();
                        if (escrito.Length > 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                        else
                        {
                            System.Windows.Forms.Application.Exit();
                            break;
                        }

                    }
                    else
                    if (escrito.StartsWith("WindowTitle"))
                    {
                        escrito = escrito.Substring(11).Trim();
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(")", "");
                        if (TextVarCodeArt.ContainsKey(escrito))
                        {
                            this.Text = TextVarCodeArt[escrito];
                            continue;
                        }
                        else
                        {
                            this.Text = escrito;
                            continue;
                        }

                    }
                    else
                    if (escrito.StartsWith("/<")) //Comentar multi linhas /< xxxxxxxxxxxxx >\
                    {
                        int linha_terminal = 0;
                        bool encontra = false;
                        bool erroA = false;
                        for (int x = linha_marcada - 1; x < linhas; x++)
                        {
                            linha = richTextBox1.Lines[x].Trim();
                            if (linha.Contains(@">\"))
                            {
                                if (linha.Length > 2) { linha = linha.Substring(linha.LastIndexOf(@">\")).Trim(); }
                                if (linha.Length > 2) { erroA = true; break; }
                                encontra = true;
                                linha_terminal = x;
                                break;
                            }
                            else { continue; }

                        }
                        if (erroA == true)
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid comments", "CodeArt Error"); break;
                        }
                        if (encontra == true)
                        {
                            topo = linha_terminal;
                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThe comments function for multiple lines is not closed", "CodeArt Error"); break;
                        }
                    }

                    else
                    if (escrito.StartsWith("CutText")) //CutText(text,posição incial,alcançe)
                    {
                        try
                        {
                            int primeiro;
                            int segundo;
                            string guarda;
                            string texto;
                            escrito = escrito.Substring(7).Trim();
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');
                            if ((valores.Count() > 4) || (valores.Count() < 3)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                            if (valores.Count() == 3)
                            {
                                if (!TextVarCodeArt.ContainsKey(valores[0])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Text does not exist", "CodeArt Error"); break; }
                                texto = TextVarCodeArt[valores[0]];
                                TextVarCodeArt.Remove(valores[0]);

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { primeiro = NumberVarCodeAr[valores[1]]; }
                                else { primeiro = Convert.ToInt32(valores[1]); }
                                if (NumberVarCodeAr.ContainsKey(valores[2])) { segundo = NumberVarCodeAr[valores[2]]; }
                                else { segundo = Convert.ToInt32(valores[2]); }

                                texto = texto.Substring(primeiro, segundo);
                                TextVarCodeArt.Add(valores[0], texto);
                                continue;
                            }
                            if (valores.Count() == 4)
                            {
                                if (!TextVarCodeArt.ContainsKey(valores[0])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Text does not exist", "CodeArt Error"); break; }
                                texto = TextVarCodeArt[valores[0]];

                                if (valores[3].All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text should not only take numbers", "CodeArt Error"); break; }
                                if (!itens_x.IsMatch(valores[3])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text can not have symbols", "CodeArt Error"); break; }
                                if (TextVarCodeArt.ContainsKey(valores[3])) { TextVarCodeArt.Remove(valores[3]); }

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { primeiro = NumberVarCodeAr[valores[1]]; }
                                else { primeiro = Convert.ToInt32(valores[1]); }
                                if (NumberVarCodeAr.ContainsKey(valores[2])) { segundo = NumberVarCodeAr[valores[2]]; }
                                else { segundo = Convert.ToInt32(valores[2]); }
                                
                                if (TextVarCodeArt.ContainsKey(valores[3])) { guarda = TextVarCodeArt[valores[3]]; }
                                else { guarda = valores[3]; }
                                TextVarCodeArt.Remove(valores[3]);
                                guarda = texto.Substring(primeiro, segundo);
                                TextVarCodeArt.Add(valores[3], guarda);
                                continue;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }

                    else
                    if (escrito.StartsWith("GetScreenResolution.Width"))
                    {
                        escrito = escrito.Substring(25).Trim();
                        Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                        escrito = regex.Replace(escrito, @" ");
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(")", "");
                        if (NumberVarCodeAr.ContainsKey(escrito)) { NumberVarCodeAr.Remove(escrito); }
                        if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                        if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                        if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                        NumberVarCodeAr.Add(escrito, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width);
                        continue;
                    }
                    else
                        if (escrito.StartsWith("GetScreenResolution.Height"))
                    {
                        escrito = escrito.Substring(26).Trim();
                        Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                        escrito = regex.Replace(escrito, @" ");
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(")", "");
                        if (NumberVarCodeAr.ContainsKey(escrito)) { NumberVarCodeAr.Remove(escrito); }
                        if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                        if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                        if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                        NumberVarCodeAr.Add(escrito, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                        continue;
                    }
                    else
                    //Comprimento do quadro
                    if (escrito.StartsWith("WindowSize.Width"))
                    {
                        escrito = escrito.Substring(16).Trim();
                        Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                        escrito = regex.Replace(escrito, @" ");
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(")", "");
                        if (NumberVarCodeAr.ContainsKey(escrito))
                        {
                            if (NumberVarCodeAr[escrito] > 1800) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nWidth maximum is 1800", "CodeArt Error"); break; }
                            pictureBox1.Width = NumberVarCodeAr[escrito];
                            this.Width = NumberVarCodeAr[escrito];
                            continue;
                        }
                        else
                        {
                            try
                            {
                                if (Convert.ToInt32(escrito) > 1800) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nWidth maximum is 1800", "CodeArt Error"); break; }
                                pictureBox1.Width = Convert.ToInt32(escrito);
                                this.Width = Convert.ToInt32(escrito);
                                continue;
                            }
                            catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                        }


                    }
                    //Altura do quadro
                    else
                    if (escrito.StartsWith("WindowSize.Height"))
                    {
                        escrito = escrito.Substring(17).Trim();
                        Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                        escrito = regex.Replace(escrito, @" ");
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(")", "");
                        if (NumberVarCodeAr.ContainsKey(escrito))
                        {
                            if (NumberVarCodeAr[escrito] > 1800) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nHeight maximum is 1800", "CodeArt Error"); break; }
                            pictureBox1.Height = NumberVarCodeAr[escrito];
                            this.Height = NumberVarCodeAr[escrito];
                            continue;
                        }
                        else
                        {
                            try
                            {
                                if (Convert.ToInt32(escrito) > 1800) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nHeight maximum is 1800", "CodeArt Error"); break; }
                                pictureBox1.Height = Convert.ToInt32(escrito);
                                this.Height = Convert.ToInt32(escrito);
                                continue;
                            }
                            catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                        }


                    }
                    else
                    if (escrito.StartsWith("GetTextLenght"))
                    {
                        try
                        {
                            escrito = escrito.Substring(13).Trim();
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace(",", " ");

                            var valores = escrito.Trim().Split(' ');

                            if (!TextVarCodeArt.ContainsKey(valores[0]))
                            {
                                 MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Text does not exist", "CodeArt Error"); break; 

                            }
                            if (!NumberVarCodeAr.ContainsKey(valores[1]))
                            {
                                if (valores[1].All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text should not only take numbers", "CodeArt Error"); break; }
                                if (!itens_x.IsMatch(valores[1])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text can not have symbols", "CodeArt Error"); break; }
                                int tamanho = TextVarCodeArt[valores[0]].Trim().Length;
                                NumberVarCodeAr.Add(valores[1], tamanho);
                                continue;

                            }
                            else
                            {
                                int tamanho = TextVarCodeArt[valores[0]].Trim().Length;
                                NumberVarCodeAr.Remove(valores[1]);
                                NumberVarCodeAr.Add(valores[1], tamanho);
                                continue;
                            }

                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }
                    else
                    if (escrito.StartsWith("InputWindow"))
                    {
                        try
                        {
                            escrito = escrito.Substring(11).Trim();

                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(")", "");
                            string Texo_da_janela = escrito.Substring(escrito.LastIndexOf(',') + 1).Trim();
                            string Text_Nome = escrito.Substring(0, escrito.LastIndexOf(',')).Trim();

                            if (Text_Nome.Contains(" ") || string.IsNullOrWhiteSpace(Text_Nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nErro ao criar um Text, um Text não pode ter espaços e não pode ser nulo.", "CodeArt Error"); break; }
                            if (Text_Nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nUm nome para um Text não pode ter só números.", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(Text_Nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text can not have symbols", "CodeArt Error"); break; }
                            if (TextVarCodeArt.ContainsKey(Texo_da_janela)) { Texo_da_janela = TextVarCodeArt[Texo_da_janela]; }
                            pictureBox1.Refresh(); //Mostra o desenho antes da janela
                            InputBoxcs input = new InputBoxcs(Texo_da_janela,this.Text);
                            input.ShowDialog();
                            string total = input.Exporta;
                            if (TextVarCodeArt.ContainsKey(Text_Nome)) { TextVarCodeArt.Remove(Text_Nome); }
                            TextVarCodeArt.Add(Text_Nome, total);
                            continue;
                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                    }


                    else
                    //Obtem a Data
                    if (escrito.StartsWith("GetCalender."))
                    {
                        escrito = escrito.Substring(12);
                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex(@"[ ]{2,}", options);
                        escrito = regex.Replace(escrito, @" ");
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(")", "");

                        if (escrito.StartsWith("Day"))
                        {
                            escrito = escrito.Substring(3).Trim();
                            if (NumberVarCodeAr.ContainsKey(escrito)) { NumberVarCodeAr.Remove(escrito); }
                            if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                            if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                            NumberVarCodeAr.Add(escrito, DateTime.Now.Day);
                            continue;
                        }
                        else if (escrito.StartsWith("Year"))
                        {
                            escrito = escrito.Substring(4).Trim();
                            if (NumberVarCodeAr.ContainsKey(escrito)) { NumberVarCodeAr.Remove(escrito); }
                            if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                            if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                            NumberVarCodeAr.Add(escrito, DateTime.Now.Year);
                            continue;
                        }
                        else if (escrito.StartsWith("Month"))
                        {
                            escrito = escrito.Substring(5).Trim();
                            if (NumberVarCodeAr.ContainsKey(escrito)) { NumberVarCodeAr.Remove(escrito); }
                            if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                            if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                            NumberVarCodeAr.Add(escrito, DateTime.Now.Month);
                            continue;

                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA Expresão é invalida", "CodeArt Error"); break;
                        }
                    }

                    else
                    //Obtem o tempo
                    if (escrito.StartsWith("GetTime."))
                    {
                        escrito = escrito.Substring(8);
                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex(@"[ ]{2,}", options);
                        escrito = regex.Replace(escrito, @" ");
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(")", "");

                        if (escrito.StartsWith("Second"))
                        {
                            escrito = escrito.Substring(6).Trim();
                            if (NumberVarCodeAr.ContainsKey(escrito)) { NumberVarCodeAr.Remove(escrito); }
                            if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                            if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                            NumberVarCodeAr.Add(escrito, DateTime.Now.Second);
                            continue;

                        }
                        else if (escrito.StartsWith("Minute"))
                        {
                            escrito = escrito.Substring(6).Trim();
                            if (NumberVarCodeAr.ContainsKey(escrito)) { NumberVarCodeAr.Remove(escrito); }
                            if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                            if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                            NumberVarCodeAr.Add(escrito, DateTime.Now.Minute);
                            continue;
                        }
                        else if (escrito.StartsWith("Hour"))
                        {
                            escrito = escrito.Substring(4).Trim();
                            if (NumberVarCodeAr.ContainsKey(escrito)) { NumberVarCodeAr.Remove(escrito); }
                            if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                            if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                            NumberVarCodeAr.Add(escrito, DateTime.Now.Hour);
                            continue;
                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }

                    else
                    //Obtem a cor de um pixel para 4 vares de números escolhidos
                    if (escrito.StartsWith("GetPixelColor"))
                    {
                        try
                        {
                            int primeiro;
                            int segundo;
                            int Opac;
                            int Red;
                            int Blue;
                            int Green;
                            escrito = escrito.Substring(13).Trim();
                            RegexOptions options = RegexOptions.None;
                            Color pixel_escolhido;
                            Regex regex = new Regex(@"[ ]{2,}", options);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');
                            if (PontosCodeArt.ContainsKey(valores[0]))
                            {
                                //coordenadas do ponto para ver a cor
                                primeiro = PontosCodeArt[valores[0]].X;
                                segundo = PontosCodeArt[valores[0]].Y;
                                try
                                {
                                    //Registar as cores nos 4 Numbers (Isto cria 4 Numbers)
                                    if (NumberVarCodeAr.ContainsKey(valores[1])) { NumberVarCodeAr.Remove(valores[1]); }
                                    if (NumberVarCodeAr.ContainsKey(valores[2])) { NumberVarCodeAr.Remove(valores[2]); }
                                    if (NumberVarCodeAr.ContainsKey(valores[3])) { NumberVarCodeAr.Remove(valores[3]); }
                                    if (NumberVarCodeAr.ContainsKey(valores[4])) { NumberVarCodeAr.Remove(valores[4]); }
                                    if (valores[1].Contains(" ") || valores[2].Contains(" ") || valores[3].Contains(" ") || valores[4].Contains(" ")) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nErro ao criar um Number, um Number não pode ter espaços", "CodeArt Error"); break; }

                                    if (valores[1].All(char.IsDigit) || valores[2].All(char.IsDigit) || valores[3].All(char.IsDigit) || valores[4].All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                                    if (!itens_x.IsMatch(valores[1]) || !itens_x.IsMatch(valores[2]) || !itens_x.IsMatch(valores[3]) || !itens_x.IsMatch(valores[4])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                                }
                                catch
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }
                                pixel_escolhido = bmp.GetPixel(primeiro, segundo);
                                Opac = pixel_escolhido.A;
                                Red = pixel_escolhido.R;
                                Blue = pixel_escolhido.B;
                                Green = pixel_escolhido.G;

                                NumberVarCodeAr.Add(valores[1], Opac);
                                NumberVarCodeAr.Add(valores[2], Red);
                                NumberVarCodeAr.Add(valores[3], Green);
                                NumberVarCodeAr.Add(valores[4], Blue);
                                continue;
                            }
                            else
                            {
                                //coordenadas do ponto para ver a cor
                                if (NumberVarCodeAr.ContainsKey(valores[0])) { primeiro = NumberVarCodeAr[valores[0]]; }
                                else { primeiro = Convert.ToInt32(valores[0]); }

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { segundo = NumberVarCodeAr[valores[1]]; }
                                else { segundo = Convert.ToInt32(valores[1]); }

                                try
                                {
                                    //Registar as cores nos 4 Numbers (Isto cria 4 Numbers)
                                    if (NumberVarCodeAr.ContainsKey(valores[2])) { NumberVarCodeAr.Remove(valores[2]); }
                                    if (NumberVarCodeAr.ContainsKey(valores[3])) { NumberVarCodeAr.Remove(valores[3]); }
                                    if (NumberVarCodeAr.ContainsKey(valores[4])) { NumberVarCodeAr.Remove(valores[4]); }
                                    if (NumberVarCodeAr.ContainsKey(valores[5])) { NumberVarCodeAr.Remove(valores[5]); }
                                    if (valores[2].Contains(" ") || valores[3].Contains(" ") || valores[4].Contains(" ") || valores[5].Contains(" ")) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nErro ao criar um Number, um Number não pode ter espaços", "CodeArt Error"); break; }
                                    if (valores[2].All(char.IsDigit) || valores[3].All(char.IsDigit) || valores[4].All(char.IsDigit) || valores[5].All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                                    if (!itens_x.IsMatch(valores[2]) || !itens_x.IsMatch(valores[3]) || !itens_x.IsMatch(valores[4]) || !itens_x.IsMatch(valores[5])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }
                                }
                                catch
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }

                                pixel_escolhido = bmp.GetPixel(primeiro, segundo);
                                Opac = pixel_escolhido.A;
                                Red = pixel_escolhido.R;
                                Blue = pixel_escolhido.B;
                                Green = pixel_escolhido.G;

                                NumberVarCodeAr.Add(valores[2], Opac);
                                NumberVarCodeAr.Add(valores[3], Red);
                                NumberVarCodeAr.Add(valores[4], Green);
                                NumberVarCodeAr.Add(valores[5], Blue);
                                continue;

                            }
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }
                    else

                    if (escrito.StartsWith("RandomNumber"))
                    {
                        try
                        {
                            escrito = escrito.Substring(12).Trim();

                            RegexOptions options = RegexOptions.None;
                            Regex regex = new Regex(@"[ ]{2,}", options);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');

                            if (valores.Count() > 3) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                            if (valores[0].Contains(" ") || string.IsNullOrWhiteSpace(valores[0])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                            if (valores[0].All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(valores[0])) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }

                            //Número minimo
                            int minValor;
                            if (NumberVarCodeAr.ContainsKey(valores[1])) { minValor = NumberVarCodeAr[valores[1]]; }
                            else { minValor = Convert.ToInt32(valores[1]); }
                            //Número Máximo
                            int maxValor;
                            if (NumberVarCodeAr.ContainsKey(valores[2])) { maxValor = NumberVarCodeAr[valores[2]]; }
                            else { maxValor = Convert.ToInt32(valores[2]); }
                            //Escolhe (O Next não inclui o máximo, por isso o + 1)
                            int Vescolhido = aleatorio.Next(minValor, maxValor + 1);

                            if (NumberVarCodeAr.ContainsKey(valores[0])) { NumberVarCodeAr.Remove(valores[0]); }
                            NumberVarCodeAr.Add(valores[0], Vescolhido);
                            continue;
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }
                    else
                    if (escrito.StartsWith("Point"))
                    {
                         try
                        {
                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex(@"[ ]{2,}", options);
                        escrito = regex.Replace(escrito, @" ");

                        escrito = escrito.Replace(")", "");
                        escrito = escrito.Replace("(", "");
                        escrito = escrito.Replace(",", " ");

                        string valor_escrito = escrito.Substring(escrito.LastIndexOf('=') + 1).Trim(); //Obtem as coordenadas
                        escrito = escrito.Substring(5);
                        string Pointo_nome = escrito.Substring(0, escrito.LastIndexOf('=')).Trim(); //Nome do Ponto

                        if (Pointo_nome.Contains(" ") ||(string.IsNullOrWhiteSpace(Pointo_nome)) ) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Point can not be empty or contain spaces", "CodeArt Error"); break; } 
                        if (Pointo_nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Point should not only take numbers", "CodeArt Error"); break; }
                        if (!itens_x.IsMatch(Pointo_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Point can not have symbols", "CodeArt Error"); break; }


                        //Se a Var já existe então remove para depois se adicionar de novo
                        if (PontosCodeArt.ContainsKey(Pointo_nome)) { PontosCodeArt.Remove(Pointo_nome); }
                        var coordendas = valor_escrito.Trim().Split(' ');
                        if (PontosCodeArt.ContainsKey(coordendas[0]))
                        {

                            if (coordendas.Count() >= 2) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nError creating a Point, a Point can only read two values, one X and one Y.", "CodeArt Error"); break; }
                            PontosCodeArt.Add(Pointo_nome, new Point(PontosCodeArt[coordendas[0]].X, PontosCodeArt[coordendas[0]].Y));
                            continue;
                        }
                        if (NumberVarCodeAr.ContainsKey(coordendas[0]) && NumberVarCodeAr.ContainsKey(coordendas[1]))
                        {
                            if (coordendas.Count() >= 3) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nError creating a Point, a Point can only read two values, one X and one Y", "CodeArt Error"); break; }
                            PontosCodeArt.Add(Pointo_nome, new Point(NumberVarCodeAr[coordendas[0]], NumberVarCodeAr[coordendas[1]]));
                            continue;
                        }
                        if (!NumberVarCodeAr.ContainsKey(coordendas[0]) && !NumberVarCodeAr.ContainsKey(coordendas[1]))
                        {
                            if (coordendas.Count() >= 3) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nError creating a Point, a Point can only read two values, one X and one Y", "CodeArt Error"); break; }
                            PontosCodeArt.Add(Pointo_nome, new Point(Convert.ToInt32(coordendas[0]), Convert.ToInt32(coordendas[1])));
                            continue;
                        }
                        if ((!NumberVarCodeAr.ContainsKey(coordendas[0])) && NumberVarCodeAr.ContainsKey(coordendas[1]))
                        {
                            if (coordendas.Count() >= 3) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nError creating a Point, a Point can only read two values, one X and one Y", "CodeArt Error"); break; }
                            PontosCodeArt.Add(Pointo_nome, new Point(Convert.ToInt32(coordendas[0]), NumberVarCodeAr[coordendas[1]]));
                            continue;
                        }
                        if (NumberVarCodeAr.ContainsKey(coordendas[0]) && (!NumberVarCodeAr.ContainsKey(coordendas[1])))
                        {
                            if (coordendas.Count() >= 3) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nError creating a Point, a Point can only read two values, one X and one Y", "CodeArt Error"); break; }
                            PontosCodeArt.Add(Pointo_nome, new Point(NumberVarCodeAr[coordendas[0]], Convert.ToInt32(coordendas[1])));
                            continue;
                        }

                        }
                         catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                    }

                    else if (escrito.StartsWith("EndIf")) { continue; }

                    else
                    //Estrutua lógica de If e EndIf --------------------------------------------------------------------------------
                    if (escrito.StartsWith("If"))
                    {
                        try
                        {
                            int linha_if = topo;
                            int endif_linha = 0;
                            bool endif = false;
                            escrito = escrito.Substring(2);
                            RegexOptions options = RegexOptions.None;
                            Regex regex = new Regex(@"[ ]{2,}", options);
                            escrito = regex.Replace(escrito, @" ");
                            var valores = escrito.Trim().Split(' ');

                            //Vê se o sinal de comparação é válido
                            List<string> If_comparadores = new List<string> { "==", "=/=", "<=", ">=", "<", ">", "Numeric", "NotNumeric" };
                            if (!If_comparadores.Contains(valores[1]))
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThe If structure have the wrong comparison symbol.", "CodeArt Error"); break;
                            }

                            //Obtem a linha onde termina a estrutura do If (Conta os If que estão dentro deste If)
                            int nivel_if = 0;
                            bool comentario_if = false;
                            for (int dentroif = topo; dentroif < linhas; dentroif++)
                            {

                                string linha_dentro = richTextBox1.Lines[dentroif].TrimStart();

                                //O que está dentro de /< >\ não conta
                                if (linha_dentro.StartsWith("/<")) { comentario_if = true; }
                                if (comentario_if == true)
                                {
                                    if (linha_dentro.Contains(@">\")) { comentario_if = false; }
                                    continue;
                                }

                                if (linha_dentro.StartsWith("EndIf"))
                                {
                                    nivel_if--;
                                    if (nivel_if == 0)
                                    {
                                        endif = true;
                                        endif_linha = dentroif;
                                        break;
                                    }
                                }
                                else if (linha_dentro.StartsWith("If"))
                                {
                                    nivel_if++;
                                }
                                else
                                {
                                    if (dentroif >= linhas) { break; } else { continue; }

                                }

                            }



                            //Se não exitir EndIf há erro
                            if (endif == false) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThe If structure is not closed"); break; }


                            //Inicio da comparação entre valores (== é Igual, =/= é diferente, <= é menor ou igual, >= maior ou igual, < menor e > maior)
                            if (valores[1] == "==")
                            {
                                if (NumberVarCodeAr.ContainsKey(valores[0]) && NumberVarCodeAr.ContainsKey(valores[2]))
                                {
                                    if (NumberVarCodeAr[valores[0]] == NumberVarCodeAr[valores[2]])
                                    {
                                        endif = false;
                                        topo = linha_if;
                                    }
                                    else { endif = false; topo = endif_linha; }


                                }
                                else if (NumberVarCodeAr.ContainsKey(valores[0]))
                                {
                                    try
                                    {
                                        if (NumberVarCodeAr[valores[0]] == Convert.ToInt32(valores[2]))
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else if (TextVarCodeArt.ContainsKey(valores[0]) && TextVarCodeArt.ContainsKey(valores[2])) //Compara Text com Text
                                {

                                    try
                                    {
                                        if (TextVarCodeArt[valores[0].Trim()].Trim() == TextVarCodeArt[valores[2].Trim()].Trim())
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else if (TextVarCodeArt.ContainsKey(valores[0])) //Compara Text com texto escrito
                                {

                                    try
                                    {
                                        string texto_escrito = string.Join(" ", valores.Skip(2)).Trim();
                                        if (TextVarCodeArt[valores[0].Trim()].Trim() == texto_escrito)
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThere is no found values to compare", "CodeArt Error");
                                    break;
                                }

                            }
                            else if (valores[1] == "<=")
                            {

                                if (NumberVarCodeAr.ContainsKey(valores[0]) && NumberVarCodeAr.ContainsKey(valores[2]))
                                {
                                    if (NumberVarCodeAr[valores[0]] <= NumberVarCodeAr[valores[2]])
                                    {
                                        endif = false;
                                        topo = linha_if;
                                    }
                                    else { endif = false; topo = endif_linha; }


                                }
                                else if (NumberVarCodeAr.ContainsKey(valores[0]))
                                {
                                    try
                                    {
                                        if (NumberVarCodeAr[valores[0]] <= Convert.ToInt32(valores[2]))
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else if (TextVarCodeArt.ContainsKey(valores[0])) //Compara Text por ordem alfabética
                                {

                                    try
                                    {
                                        string texto_escrito = string.Join(" ", valores.Skip(2)).Trim();
                                        if (TextVarCodeArt.ContainsKey(texto_escrito)) { texto_escrito = TextVarCodeArt[texto_escrito].Trim(); }
                                        if (string.Compare(TextVarCodeArt[valores[0].Trim()].Trim(), texto_escrito) <= 0)
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThere is no found values to compare", "CodeArt Error");
                                    break;
                                }
                            }
                            else if (valores[1] == "=/=")
                            {

                                if (NumberVarCodeAr.ContainsKey(valores[0]) && NumberVarCodeAr.ContainsKey(valores[2]))
                                {
                                    if (NumberVarCodeAr[valores[0]] != NumberVarCodeAr[valores[2]])
                                    {
                                        endif = false;
                                        topo = linha_if;
                                    }
                                    else { endif = false; topo = endif_linha; }


                                }
                                else if (NumberVarCodeAr.ContainsKey(valores[0]))
                                {
                                    try
                                    {
                                        if (NumberVarCodeAr[valores[0]] != Convert.ToInt32(valores[2]))
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }
                                }
                                else if (TextVarCodeArt.ContainsKey(valores[0]) && TextVarCodeArt.ContainsKey(valores[2])) //Compara Text com Text
                                {

                                    try
                                    {
                                        if (TextVarCodeArt[valores[0].Trim()].Trim() != TextVarCodeArt[valores[2].Trim()].Trim())
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else if (TextVarCodeArt.ContainsKey(valores[0])) //Compara Text com texto escrito
                                {

                                    try
                                    {
                                        string texto_escrito = string.Join(" ", valores.Skip(2)).Trim();
                                        if (TextVarCodeArt[valores[0].Trim()].Trim() != texto_escrito)
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThere is no found values to compare", "CodeArt Error");
                                    break;
                                }

                            }

                            else if (valores[1] == ">=")
                            {

                                if (NumberVarCodeAr.ContainsKey(valores[0]) && NumberVarCodeAr.ContainsKey(valores[2]))
                                {
                                    if (NumberVarCodeAr[valores[0]] >= NumberVarCodeAr[valores[2]])
                                    {
                                        endif = false;
                                        topo = linha_if;
                                    }
                                    else { endif = false; topo = endif_linha; }


                                }
                                else if (NumberVarCodeAr.ContainsKey(valores[0]))
                                {
                                    try
                                    {
                                        if (NumberVarCodeAr[valores[0]] >= Convert.ToInt32(valores[2]))
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else if (TextVarCodeArt.ContainsKey(valores[0])) //Compara Text por ordem alfabética
                                {

                                    try
                                    {
                                        string texto_escrito = string.Join(" ", valores.Skip(2)).Trim();
                                        if (TextVarCodeArt.ContainsKey(texto_escrito)) { texto_escrito = TextVarCodeArt[texto_escrito].Trim(); }
                                        if (string.Compare(TextVarCodeArt[valores[0].Trim()].Trim(), texto_escrito) >= 0)
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThere is no found values to compare", "CodeArt Error");
                                    break;
                                }
                            }

                            else if (valores[1] == "<")
                            {


                                if (NumberVarCodeAr.ContainsKey(valores[0]) && NumberVarCodeAr.ContainsKey(valores[2]))
                                {
                                    if (NumberVarCodeAr[valores[0]] < NumberVarCodeAr[valores[2]])
                                    {
                                        endif = false;
                                        topo = linha_if;
                                    }
                                    else { endif = false; topo = endif_linha; }


                                }
                                else if (NumberVarCodeAr.ContainsKey(valores[0]))
                                {
                                    try
                                    {
                                        if (NumberVarCodeAr[valores[0]] < Convert.ToInt32(valores[2]))
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else if (TextVarCodeArt.ContainsKey(valores[0])) //Compara Text por ordem alfabética
                                {

                                    try
                                    {
                                        string texto_escrito = string.Join(" ", valores.Skip(2)).Trim();
                                        if (TextVarCodeArt.ContainsKey(texto_escrito)) { texto_escrito = TextVarCodeArt[texto_escrito].Trim(); }
                                        if (string.Compare(TextVarCodeArt[valores[0].Trim()].Trim(), texto_escrito) < 0)
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThere is no found values to compare", "CodeArt Error");
                                    break;
                                }
                            }
                            else if (valores[1] == "Numeric")
                            {
                                if (TextVarCodeArt.ContainsKey(valores[0].Trim()))
                                {
                                    if (valores.Count() > 2) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                                    if (TextVarCodeArt[valores[0]].Trim().All(char.IsDigit))
                                    {
                                        endif = false;
                                        topo = linha_if;
                                    }
                                    else { endif = false; topo = endif_linha; }
                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Text does not exist", "CodeArt Error"); break;
                                }
                            }
                            else if (valores[1] == "NotNumeric")
                            {
                                if (valores.Count() > 2) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                                if (TextVarCodeArt.ContainsKey(valores[0].Trim()))
                                {
                                    if (!TextVarCodeArt[valores[0]].Trim().All(char.IsDigit))
                                    {
                                        endif = false;
                                        topo = linha_if;
                                    }
                                    else { endif = false; topo = endif_linha; }
                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Text does not exist", "CodeArt Error"); break;
                                }
                            }
                            else if (valores[1] == ">")
                            {

                                if (NumberVarCodeAr.ContainsKey(valores[0]) && NumberVarCodeAr.ContainsKey(valores[2]))
                                {
                                    if (NumberVarCodeAr[valores[0]] > NumberVarCodeAr[valores[2]])
                                    {
                                        endif = false;
                                        topo = linha_if;
                                    }
                                    else { endif = false; topo = endif_linha; }


                                }
                                else if (NumberVarCodeAr.ContainsKey(valores[0]))
                                {
                                    try
                                    {
                                        if (NumberVarCodeAr[valores[0]] > Convert.ToInt32(valores[2]))
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else if (TextVarCodeArt.ContainsKey(valores[0])) //Compara Text por ordem alfabética
                                {

                                    try
                                    {
                                        string texto_escrito = string.Join(" ", valores.Skip(2)).Trim();
                                        if (TextVarCodeArt.ContainsKey(texto_escrito)) { texto_escrito = TextVarCodeArt[texto_escrito].Trim(); }
                                        if (string.Compare(TextVarCodeArt[valores[0].Trim()].Trim(), texto_escrito) > 0)
                                        {
                                            endif = false;
                                            topo = linha_if;
                                        }
                                        else { endif = false; topo = endif_linha; }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIncorrect values", "CodeArt Error"); break;
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThere is no found values to compare", "CodeArt Error");
                                    break;
                                }
                            }

                        }
                        catch //Erro na estrutura If
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function", "CodeArt Error");
                            break;
                        }
                    }

                    //--------------------------------------------------------------------------------------------------------------------------------------------------


                    else
                    if (escrito.StartsWith("++"))
                    {  //Aumenta em 1 um Number
                        escrito = escrito.Substring(2);
                        escrito = escrito.Trim();
                        if (escrito.Contains(" ")) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                        if (NumberVarCodeAr.ContainsKey(escrito))
                        {
                            int caixa = NumberVarCodeAr[escrito] + 1;
                            NumberVarCodeAr.Remove(escrito);
                            NumberVarCodeAr.Add(escrito, caixa);
                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Number does not exist", "CodeArt Error"); break;
                        }
                    }
                    else
                    if (escrito.StartsWith("--")) //Reduz em 1 um Number
                    {
                        escrito = escrito.Substring(2);
                        escrito = escrito.Trim();
                        if (escrito.Contains(" ")) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                        if (NumberVarCodeAr.ContainsKey(escrito))
                        {
                            int caixa = NumberVarCodeAr[escrito] - 1;
                            NumberVarCodeAr.Remove(escrito);
                            NumberVarCodeAr.Add(escrito, caixa);
                        }
                        else
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Number does not exist", "CodeArt Error"); break;
                        }
                    }

                    else
                    if (escrito.StartsWith("Repet" )) //Loops
                    {
                        try
                        {
                            escrito = escrito.Substring(6);
                            escrito = escrito.Trim();
                            int segundo;
                            RegexOptions options = RegexOptions.None;
                            Regex regex = new Regex(@"[ ]{2,}", options);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');
                            int index_max = valores.Count();

                            string primeiro = valores[0];
                            try
                            {
                                if (NumberVarCodeAr.ContainsKey(valores[1])) { segundo = NumberVarCodeAr[valores[1]]; }
                                else { segundo = Convert.ToInt32(valores[1]); }
                            }
                            catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nError creating a loop, the value is not available or nonexistent", "CodeArt Error"); break; }

                            if (index_max > 2) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; } //Limite de valores
                            if (LoopNameCodeArt.ContainsKey(primeiro))
                            {
                                if (LoopNameCodeArt[primeiro] != topo) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThere is already a Repet with this name", "CodeArt Error"); break; }

                                //Saiu do Repet com JumpTo e voltou a entrar, começa de novo
                                LoopvezesCodeArt.Remove(primeiro);
                                LoopNameCodeArt.Remove(primeiro);
                            }
                            if (primeiro.Contains(" ") || string.IsNullOrWhiteSpace(primeiro)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Repet can not be empty or contain spaces", "CodeArt Error"); break; }
                            
                            if (primeiro.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Repet should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(primeiro)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Repet can not have symbols", "CodeArt Error"); break; }

                            //Sem voltas para fazer salta para o EndRepet
                            if (segundo <= 0)
                            {
                                bool endrepet = false;
                                bool comentario_repet = false;
                                for (int dentrorepet = topo; dentrorepet < linhas; dentrorepet++)
                                {
                                    string linha_dentro = richTextBox1.Lines[dentrorepet].TrimStart();

                                    //O que está dentro de /< >\ não conta
                                    if (linha_dentro.StartsWith("/<")) { comentario_repet = true; }
                                    if (comentario_repet == true)
                                    {
                                        if (linha_dentro.Contains(@">\")) { comentario_repet = false; }
                                        continue;
                                    }

                                    if (linha_dentro.StartsWith("EndRepet"))
                                    {
                                        linha_dentro = linha_dentro.Substring(8);
                                        linha_dentro = linha_dentro.Replace(")", "");
                                        linha_dentro = linha_dentro.Replace("(", "");
                                        if (linha_dentro.Trim() == primeiro)
                                        {
                                            endrepet = true;
                                            topo = dentrorepet;
                                            break;
                                        }
                                    }
                                }
                                if (endrepet == false) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThe Repet structure is not closed", "CodeArt Error"); break; }
                                continue;
                            }

                            LoopvezesCodeArt.Add(primeiro, segundo);
                            LoopNameCodeArt.Add(primeiro, topo);
                            continue;
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }


                    }
                    else
                    if (escrito.StartsWith("EndRepet")) //Termina o Loop x
                    {
                        try
                        {
                            escrito = escrito.Substring(8);
                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Trim();

                            if (escrito.Contains(" ") || string.IsNullOrWhiteSpace(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Repet can not be empty or contain spaces", "CodeArt Error"); break; }
                            
                            if (escrito.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Repet should not only take numbers.", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(escrito)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Repet can not have symbols", "CodeArt Error"); break; }

                            if (LoopNameCodeArt.ContainsKey(escrito))
                            {
                                int esta_ln = linha_marcada - 1;
                                LoopvezesCodeArt[escrito]--;
                                if (LoopvezesCodeArt[escrito] == 0)
                                {
                                    //Remove o Repet para poder ser usado de novo (Repet dentro de Repet)
                                    LoopvezesCodeArt.Remove(escrito);
                                    LoopNameCodeArt.Remove(escrito);
                                    topo = esta_ln;
                                    continue;
                                }
                                topo = LoopNameCodeArt[escrito];
                            }
                            else
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Loop does not exist", "CodeArt Error");
                                break;
                            }
                            continue;
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nIt is not possible to complete the loop", "CodeArt Error"); break;
                        }

                    }


                    else
                    if (escrito.StartsWith("Pause")) //Espera X tempo
                    {
                        int tempo;
                        escrito = escrito.Substring(5).Trim();
                        escrito = escrito.Replace(")", "");
                        escrito = escrito.Replace("(", "");
                        try
                        {
                            if (NumberVarCodeAr.ContainsKey(escrito)) { tempo = NumberVarCodeAr[escrito]; }
                            else { tempo = Convert.ToInt32(escrito); }
                            if (tempo < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                            pictureBox1.Refresh(); //Mostra o desenho feito até agora

                            //Espera aos poucos para a janela continuar a responder
                            do
                            {
                                Application.DoEvents();
                                if (this.IsDisposed) { break; } //A janela foi fechada
                                int bocado = Math.Min(tempo, 20);
                                Thread.Sleep(bocado);
                                tempo -= bocado;
                            } while (tempo > 0);

                            if (this.IsDisposed) { break; }
                            continue;
                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }


                    }

                    else
                    if (escrito.StartsWith("Draw.Image"))
                    { //Desenha imagens
                        try
                        {
                            int primeiro;
                            int segundo;
                            string terceiro;
                            escrito = escrito.Substring(10);
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');
                            if (PontosCodeArt.ContainsKey(valores[0]))
                            {
                                if (valores.Count() > 2) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                                primeiro = PontosCodeArt[valores[0]].X;
                                segundo = PontosCodeArt[valores[0]].Y;

                                if (TextureCodeArt.ContainsKey(valores[1])) { terceiro = TextureCodeArt[valores[1]]; }
                                else if (TextVarCodeArt.ContainsKey(valores[1])) { terceiro = TextVarCodeArt[valores[1]]; }
                                else { terceiro = valores[1]; }
                            }
                            else
                            {
                                if (valores.Count() > 3) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                                if (NumberVarCodeAr.ContainsKey(valores[0])) { primeiro = NumberVarCodeAr[valores[0]]; }
                                else { primeiro = Convert.ToInt32(valores[0]); }

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { segundo = NumberVarCodeAr[valores[1]]; }
                                else { segundo = Convert.ToInt32(valores[1]); }

                                if (TextureCodeArt.ContainsKey(valores[2])) { terceiro = TextureCodeArt[valores[2]]; }
                                else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis image does not exist", "CodeArt Error"); break; }
                            }




                            Point partida = new Point(primeiro, segundo);
                            Image s = Image.FromFile(terceiro);
                            g.DrawImage(s, partida);

                            continue;
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }
                    else
                    if (escrito.StartsWith("Draw.Background")) //Desenha Fundo
                    {
                        try
                        {
                            int primeiro;
                            int segundo;
                            int terceiro;
                            int quarto;

                            escrito = escrito.Substring(15);
                            escrito = escrito.Trim();

                            RegexOptions options = RegexOptions.None;
                            Regex regex = new Regex(@"[ ]{2,}", options);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');
                            if (valores.Length == 4){
                            if (NumberVarCodeAr.ContainsKey(valores[0])) { primeiro = NumberVarCodeAr[valores[0]]; }
                            else { primeiro = Convert.ToInt32(valores[0]); }

                            if (NumberVarCodeAr.ContainsKey(valores[1])) { segundo = NumberVarCodeAr[valores[1]]; }
                            else { segundo = Convert.ToInt32(valores[1]); }

                            if (NumberVarCodeAr.ContainsKey(valores[2])) { terceiro = NumberVarCodeAr[valores[2]]; }
                            else { terceiro = Convert.ToInt32(valores[2]); }

                            if (NumberVarCodeAr.ContainsKey(valores[3])) { quarto = NumberVarCodeAr[valores[3]]; }
                            else { quarto = Convert.ToInt32(valores[3]); }

                            g.Clear(Color.FromArgb(primeiro, segundo, terceiro, quarto));
                            continue;
                            }
                            if (valores.Length == 1)
                            {
                                //Obtem a Brush
                                if (BrushCodeArt.ContainsKey(valores[0]))
                                {
                                    string caneta_valor = BrushCodeArt[valores[0]];

                                    var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                    Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                    Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                    Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                    Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                    g.Clear(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3));
                                }
                                else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, Brush does not exist", "CodeArt Error"); break; }
                            }
                            else
                            {
                                MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                            }

                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }

                    }
                    else
                    if (escrito.StartsWith("ClearDraw")) //Limpa Desenho
                    {
                        g.Clear(Color.Transparent);
                        continue;
                    }
                    else
                    //Define um Numbers e faz operações de calculo
                    if (escrito.StartsWith("Number"))
                    {
                        
                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex(@"[ ]{2,}", options);
                        escrito = regex.Replace(escrito, @" ");
                        if (!escrito.Contains("=")) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                        //Obtem o valor de um Number
                        string valor_escrito = escrito.Substring(escrito.LastIndexOf('=') + 1).Trim();

                        //Obtem o Nome do Number
                        escrito = escrito.Substring(6);
                        string Number_Nome = escrito.Substring(0, escrito.LastIndexOf('='));

                        Number_Nome = Number_Nome.Trim();
                        if (Number_Nome.Contains(" ") || string.IsNullOrWhiteSpace(Number_Nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not be empty or contain spaces", "CodeArt Error"); break; }
                        if (Number_Nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number should not only take numbers", "CodeArt Error"); break; }
                        if (!itens_x.IsMatch(Number_Nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Number can not have symbols", "CodeArt Error"); break; }


                        valor_escrito = valor_escrito.Replace("(", " ( ").Replace(")", " ) ").Replace("+", " + ").Replace("-", " - ").Replace("*", " * ").Replace("/", " / ");

                        var valores = valor_escrito.Trim().Split(' ');
                        int numeros_de_valores = valores.Count();
                        List<string> list = new List<string> { ")", "(", "+", "-", "*", "/" };

                        //Obtem os valores dos numbers
                        for (int i = 0; i < numeros_de_valores; i++)
                        {
                            try
                            {

                                if (list.Contains(valores[i]) || valores[i].All(char.IsDigit)) { continue; }
                                valores[i] = NumberVarCodeAr[valores[i]].ToString().Trim();

                            }
                            catch { MessageBox.Show("This Number does not exist"); goto erro; }
                        }
                        string resumo = string.Join(" ", valores);
                        try
                        {
                          
                            string total = new DataTable().Compute(resumo, null).ToString();
                            if (NumberVarCodeAr.ContainsKey(Number_Nome)) { NumberVarCodeAr.Remove(Number_Nome); }
                            int valor_final = (int)Math.Round(float.Parse(total)); //Arredonda valores para números inteiros (0.6 = 1 , 0.5 = 0)
                            NumberVarCodeAr.Add(Number_Nome, valor_final);
                            continue;
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function", "CodeArt Error"); break;
                        }

                    erro:
                        break;

                    }

                    else
                    if (escrito.StartsWith("Text"))  //Define uma Var de texto
                    {
                        try
                        {
                            string valor_escrito = escrito.Substring(escrito.LastIndexOf('=') + 1); //Obtem o Valor da Var

                            escrito = escrito.Substring(4);
                            string var_nome = escrito.Substring(0, escrito.LastIndexOf('=')); //Obtem num string o nome da Var

                            valor_escrito = valor_escrito.Trim();

                            var_nome = var_nome.Trim();
                            if (var_nome.Contains(" ") || string.IsNullOrWhiteSpace(var_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text can not be empty or contain spaces", "CodeArt Error"); break; } //Var não pode ter espaços
                            if (var_nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text should not only take numbers", "CodeArt Error"); break; }
                            if (!itens_x.IsMatch(var_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Text can not have symbols", "CodeArt Error"); break; }

                            //Troca cada palavra pelo valor da Var, só uma vez (~nome fica só como palavra)
                            valor_escrito = Regex.Replace(valor_escrito, @"~?\b\w+\b", palavra =>
                            {
                                string nome = palavra.Value.TrimStart('~');
                                if (!TextVarCodeArt.ContainsKey(nome) && !NumberVarCodeAr.ContainsKey(nome)) { return palavra.Value; }
                                if (palavra.Value.StartsWith("~")) { return nome; }
                                if (nome == var_nome) { return palavra.Value; }
                                if (TextVarCodeArt.ContainsKey(nome)) { return TextVarCodeArt[nome]; }
                                return Convert.ToString(NumberVarCodeAr[nome]);
                            });

                            //Se a Var já existe então remove para depois se adicionar de novo
                            if (TextVarCodeArt.ContainsKey(var_nome)) { TextVarCodeArt.Remove(var_nome); }
                            TextVarCodeArt.Add(var_nome, valor_escrito);
                            continue;
                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }


                    }
                    else
                    if (escrito.StartsWith("Pen")) //Define uma Caneta
                    {

                        int index_max;
                        int vt_opacidade;
                        int vt_red;
                        int vt_green;
                        int vt_blue;
                        int tamanho_caneta;

                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex(@"[ ]{2,}", options);
                        escrito = regex.Replace(escrito, @" ");
                        if (!escrito.Contains("=")) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                        string valor_escrito = escrito.Substring(escrito.LastIndexOf('=') + 1).Trim(); //Obtem o Valor da Var

                        escrito = escrito.Substring(3);
                        string Pen_nome = escrito.Substring(0, escrito.LastIndexOf('=')).Trim(); //Obtem num string o nome da Var

                        if (Pen_nome.Contains(" ") || string.IsNullOrWhiteSpace(Pen_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Pen can not be empty or contain spaces", "CodeArt Error"); break; } //Var não pode ter espaços
                        if (Pen_nome.All(char.IsDigit)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Pen should not only take numbers", "CodeArt Error"); break; }
                        if (!itens_x.IsMatch(Pen_nome)) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nA name for a Pen can not have symbols", "CodeArt Error"); break; }
                        if (PenCodeArt.ContainsKey(Pen_nome)) { PenCodeArt.Remove(Pen_nome); }

                       
                        valor_escrito = valor_escrito.Replace("(", "");
                        valor_escrito = valor_escrito.Replace(")", "");
                        valor_escrito = valor_escrito.Replace(",", " ");
                        string[] teste_valido = valor_escrito.Split(' ');
                        try
                        {
                            if (NumberVarCodeAr.ContainsKey(teste_valido[0])) { vt_opacidade = NumberVarCodeAr[teste_valido[0]]; }
                            else { vt_opacidade = Convert.ToInt32(teste_valido[0]); }

                            if (NumberVarCodeAr.ContainsKey(teste_valido[1])) { vt_red = NumberVarCodeAr[teste_valido[1]]; }
                            else { vt_red = Convert.ToInt32(teste_valido[1]); }

                            if (NumberVarCodeAr.ContainsKey(teste_valido[2])) { vt_green = NumberVarCodeAr[teste_valido[2]]; }
                            else { vt_green = Convert.ToInt32(teste_valido[2]); }

                            if (NumberVarCodeAr.ContainsKey(teste_valido[3])) { vt_blue = NumberVarCodeAr[teste_valido[3]]; }
                            else { vt_blue = Convert.ToInt32(teste_valido[3]); }

                            if (NumberVarCodeAr.ContainsKey(teste_valido[4])) { tamanho_caneta = NumberVarCodeAr[teste_valido[4]]; }
                            else { tamanho_caneta = Convert.ToInt32(teste_valido[4]); }
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }

                        try
                        {
                            //Determinar se os valores da Pen são possiveis
                            index_max = teste_valido.Count();

                            if (index_max > 5) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; } //Limite de valores
                            if (vt_opacidade > 255 || vt_opacidade < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nOpacity can not be greater than 255 or less than 0", "CodeArt Error"); break; }
                            if (vt_red > 255 || vt_red < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nRed can not be greater than 255 or less than 0", "CodeArt Error"); break; }
                            if (vt_green > 255 || vt_green < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nGreen can not be greater than 255 or less than 0", "CodeArt Error"); break; }
                            if (vt_blue > 255 || vt_blue < 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nBlue can not be greater than 255 or less than 0", "CodeArt Error"); break; }

                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                        string valor_de_pen = vt_opacidade.ToString() + " " + vt_red.ToString() + " " + vt_green.ToString() + " " + vt_blue.ToString() + " " + tamanho_caneta.ToString();
                        PenCodeArt.Add(Pen_nome, valor_de_pen);
                        continue;
                    }
                    else
                    if (escrito.StartsWith("JumpTo"))
                    {
                        int registaV;
                        escrito = escrito.Substring(6).Trim();
                        try
                        {
                            if (NumberVarCodeAr.ContainsKey(escrito)) { registaV = NumberVarCodeAr[escrito]; }
                            else { registaV = Convert.ToInt32(escrito); }
                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                        if (registaV == linha_marcada || registaV > linhas || registaV <= 0) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; } else { topo = registaV - 2; }
                        continue;
                    }
                    else
                    if (escrito.StartsWith("//")) { continue; }  //Comenta
                    else
                    if (escrito.StartsWith("MessageWindow")) //Janela de Texto - Notas
                    {
                        escrito = escrito.Substring(13).Trim();
                        pictureBox1.Refresh(); //Mostra o desenho antes da janela

                        if (TextVarCodeArt.ContainsKey(escrito))
                        {
                            MessageWindow TextMessage = new MessageWindow(TextVarCodeArt[escrito],this.Text);
                            TextMessage.ShowDialog();   
                            continue;
                        }

                        if (NumberVarCodeAr.ContainsKey(escrito))
                        {
                            MessageWindow NumberMessage = new MessageWindow(NumberVarCodeAr[escrito].ToString(), this.Text);
                            NumberMessage.ShowDialog();                             
                            continue;
                        }

                        MessageWindow NormalMessage = new MessageWindow(escrito, this.Text);
                        NormalMessage.ShowDialog();   
                        continue;

                    }
                    else
                    if (escrito.StartsWith("Draw.Line"))
                    {
                        try
                        {
                            int primeiro;
                            int segundo;
                            int terceiro;
                            int quarto;
                            string quinto;
                            escrito = escrito.Substring(9).Trim();
                            RegexOptions options = RegexOptions.None;
                            Regex regex = new Regex(@"[ ]{2,}", options);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');

                            if (PontosCodeArt.ContainsKey(valores[0]))
                            {
                                if (valores.Count() != 3)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }
                                primeiro = PontosCodeArt[valores[0]].X;
                                segundo = PontosCodeArt[valores[0]].Y;

                                terceiro = PontosCodeArt[valores[1]].X;
                                quarto = PontosCodeArt[valores[1]].Y;

                                quinto = valores[2];
                                quinto = quinto.Trim();
                            }
                            else
                            {

                                if (valores.Count() != 5)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }

                                if (NumberVarCodeAr.ContainsKey(valores[0])) { primeiro = NumberVarCodeAr[valores[0]]; }
                                else { primeiro = Convert.ToInt32(valores[0]); }

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { segundo = NumberVarCodeAr[valores[1]]; }
                                else { segundo = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { terceiro = NumberVarCodeAr[valores[2]]; }
                                else { terceiro = Convert.ToInt32(valores[2]); }

                                if (NumberVarCodeAr.ContainsKey(valores[3])) { quarto = NumberVarCodeAr[valores[3]]; }
                                else { quarto = Convert.ToInt32(valores[3]); }

                                quinto = valores[4];
                                quinto = quinto.Trim();
                            }
                            //Obtem a Caneta
                            if (PenCodeArt.ContainsKey(quinto))
                            {
                                string caneta_valor = PenCodeArt[quinto];

                                var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                Caneta_Tamamho = Convert.ToInt32(caneta_valor_valores[4]);
                                Caneta_para_desenhar = new Pen(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3), Caneta_Tamamho);
                            }
                            else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Pen does not exist", "CodeArt Error"); break; }


                            g.DrawLine(Caneta_para_desenhar, primeiro, segundo, terceiro, quarto);
                            continue;
                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                    }
                    else
                    if (escrito.StartsWith("Draw.MultiLines")) //Draw.MultiLines(PointList,Pen) Isto também só aceita listas de pontos
                    {
                        try
                        {
                            escrito = escrito.Substring(15);

                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');
                            if (valores.Length > 2) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                            //Obtem a Caneta
                            if (PenCodeArt.ContainsKey(valores[1]))
                            {
                                string caneta_valor = PenCodeArt[valores[1]];
                                var caneta_valor_valores = caneta_valor.Trim().Split(' ');


                                Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                Caneta_Tamamho = Convert.ToInt32(caneta_valor_valores[4]);
                                Caneta_para_desenhar = new Pen(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3), Caneta_Tamamho);
                            }
                            else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Pen does not exist", "CodeArt Error"); break; }
                            Point[] conj = ListaPontos[valores[0]];
                            g.DrawLines(Caneta_para_desenhar, conj);
                            continue;
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                        
                    }
                    else
                    if (escrito.StartsWith("Draw.CurvedLine")) //Draw.CurvedLines(PointList,Pen)  Isto só funciona com uma lista de pontos apenas!
                    {
                        try
                        {
                            escrito = escrito.Substring(15).Trim();

                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");

                            var valores = escrito.Trim().Split(' ');
                            if (valores.Length > 2) { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }

                            //Obtem a Caneta
                            if (PenCodeArt.ContainsKey(valores[1]))
                            {
                                string caneta_valor = PenCodeArt[valores[1]];
                                var caneta_valor_valores = caneta_valor.Trim().Split(' ');


                                Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                Caneta_Tamamho = Convert.ToInt32(caneta_valor_valores[4]);
                                Caneta_para_desenhar = new Pen(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3), Caneta_Tamamho);
                            }
                            else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Pen does not exist", "CodeArt Error"); break; }
                            Point[] conj = ListaPontos[valores[0]];
                            
                            g.DrawCurve(Caneta_para_desenhar, conj);
                            continue;
                        }
                        catch {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                        
                        
                    }
                    else
                    if (escrito.StartsWith("Draw.Elipse")) //Draw.Elipse(x,y,comprimento,altura)
                    {
                        try
                        {
                            int xcentro;
                            int ycentro;
                            int altura;
                            int comprimento;
                            string caneta;

                            escrito = escrito.Substring(11).Trim();
                            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');

                            if (!PontosCodeArt.ContainsKey(valores[0]))
                            {
                                if (valores.Count() != 5)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }

                                if (NumberVarCodeAr.ContainsKey(valores[0])) { xcentro = NumberVarCodeAr[valores[0]]; }
                                else { xcentro = Convert.ToInt32(valores[0]); }

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { ycentro = NumberVarCodeAr[valores[1]]; }
                                else { ycentro = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { comprimento = NumberVarCodeAr[valores[2]]; }
                                else { comprimento = Convert.ToInt32(valores[2]); }

                                if (NumberVarCodeAr.ContainsKey(valores[3])) { altura = NumberVarCodeAr[valores[3]]; }
                                else { altura = Convert.ToInt32(valores[3]); }

                                caneta = valores[4];
                                caneta = caneta.Trim();
                            }
                            else
                            {
                                if (valores.Count() != 4)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }
                                xcentro = PontosCodeArt[valores[0]].X;
                                ycentro = PontosCodeArt[valores[0]].Y;

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { comprimento = NumberVarCodeAr[valores[1]]; }
                                else { comprimento = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { altura = NumberVarCodeAr[valores[2]]; }
                                else { altura = Convert.ToInt32(valores[2]); }

                                caneta = valores[3];
                                caneta = caneta.Trim();
                            }
                            //Obtem a Caneta
                            if (PenCodeArt.ContainsKey(caneta))
                            {
                                string caneta_valor = PenCodeArt[caneta];

                                var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                Caneta_Tamamho = Convert.ToInt32(caneta_valor_valores[4]);
                                Caneta_para_desenhar = new Pen(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3), Caneta_Tamamho);
                            }
                            else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Pen does not exist", "CodeArt Error"); break; }

                            //----
                            int x = xcentro - comprimento;
                            int y = ycentro - altura;
                            float comprimentoFeito = 2 * comprimento;
                            float alturaFeito = 2 * altura;

                            g.DrawEllipse(Caneta_para_desenhar, x, y, comprimentoFeito, alturaFeito);
                        }
                        catch
                        {
                            MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                        }
                    }
                    else
                        if (escrito.StartsWith("FillPoint"))
                        {
                            try
                            {
                                Color a = Color.FromArgb(0, 0, 0, 0);
                                Color b = Color.FromArgb(0, 0, 0, 0);
                                escrito = escrito.Substring(9);
                                RegexOptions options = RegexOptions.None;
                                Regex regex = new Regex(@"[ ]{2,}", options);
                                escrito = regex.Replace(escrito, @" ");

                                escrito = escrito.Replace(")", "");
                                escrito = escrito.Replace("(", "");
                                escrito = escrito.Replace(",", " ");
                                var valores = escrito.Trim().Split(' ');
                                Point ponto_fill;

                                if (valores.Length == 4) //X Y Brush Brush
                                {
                                    int n1;
                                    int n2;
                                    if (NumberVarCodeAr.ContainsKey(valores[0])) { n1 = NumberVarCodeAr[valores[0]]; }
                                    else { n1 = Convert.ToInt32(valores[0]); }

                                    if (NumberVarCodeAr.ContainsKey(valores[1])) { n2 = NumberVarCodeAr[valores[1]]; }
                                    else { n2 = Convert.ToInt32(valores[1]); }

                                    ponto_fill = new Point(n1, n2);
                                    valores = valores.Skip(1).ToArray(); //As Brush ficam nas mesmas posições que com um Point
                                }
                                else if (valores.Length == 3) //Point Brush Brush
                                {
                                    if (!PontosCodeArt.ContainsKey(valores[0]))
                                    { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Point does not exist", "CodeArt Error"); break; }
                                    ponto_fill = PontosCodeArt[valores[0]];
                                }
                                else
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }

                                if (valores.Length == 3)
                                {
                                    if (BrushCodeArt.ContainsKey(valores[1]))
                                    {
                                        string caneta_valor = BrushCodeArt[valores[1]];

                                        var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                        Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                        Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                        Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                        Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                        a = Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3);
                                    }
                                    else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, Brush does not exist", "CodeArt Error"); break; }

                                    if (BrushCodeArt.ContainsKey(valores[2]))
                                    {
                                        string caneta_valor = BrushCodeArt[valores[2]];

                                        var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                        Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                        Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                        Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                        Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                        b = Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3);
                                    }
                                    else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, Brush does not exist", "CodeArt Error"); break; }

                                    FloodFill(bmp, ponto_fill, a, b);
                                }
                            }
                            catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, inalid", "CodeArt Error"); break; }
                         

                        } else
                    if (escrito.StartsWith("Draw.Rectangle"))
                    {
                        try
                        {
                            int primeiro;
                            int segundo;
                            int terceiro;
                            int quarto;
                            string quinto;

                            escrito = escrito.Substring(14);
                            escrito = escrito.TrimStart();

                            RegexOptions options = RegexOptions.None;
                            Regex regex = new Regex(@"[ ]{2,}", options);
                            escrito = regex.Replace(escrito, @" ");

                            escrito = escrito.Replace(")", "");
                            escrito = escrito.Replace("(", "");
                            escrito = escrito.Replace(",", " ");
                            var valores = escrito.Trim().Split(' ');

                            if (!PontosCodeArt.ContainsKey(valores[0]))
                            {
                                if (valores.Count() != 5)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }

                                if (NumberVarCodeAr.ContainsKey(valores[0])) { primeiro = NumberVarCodeAr[valores[0]]; }
                                else { primeiro = Convert.ToInt32(valores[0]); }

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { segundo = NumberVarCodeAr[valores[1]]; }
                                else { segundo = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { terceiro = NumberVarCodeAr[valores[2]]; }
                                else { terceiro = Convert.ToInt32(valores[2]); }

                                if (NumberVarCodeAr.ContainsKey(valores[3])) { quarto = NumberVarCodeAr[valores[3]]; }
                                else { quarto = Convert.ToInt32(valores[3]); }

                                quinto = valores[4];
                                quinto = quinto.Trim();
                            }
                            else
                            {
                                if (valores.Count() != 4)
                                {
                                    MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break;
                                }
                                primeiro = PontosCodeArt[valores[0]].X;
                                segundo = PontosCodeArt[valores[0]].Y;

                                if (NumberVarCodeAr.ContainsKey(valores[1])) { terceiro = NumberVarCodeAr[valores[1]]; }
                                else { terceiro = Convert.ToInt32(valores[1]); }

                                if (NumberVarCodeAr.ContainsKey(valores[2])) { quarto = NumberVarCodeAr[valores[2]]; }
                                else { quarto = Convert.ToInt32(valores[2]); }

                                quinto = valores[3];
                                quinto = quinto.Trim();
                            }

                            //Obtem a Caneta
                            if (PenCodeArt.ContainsKey(quinto))
                            {
                                string caneta_valor = PenCodeArt[quinto];

                                var caneta_valor_valores = caneta_valor.Trim().Split(' ');

                                Caneta_Transparencia = Convert.ToInt32(caneta_valor_valores[0]);
                                Caneta_Cor1 = Convert.ToInt32(caneta_valor_valores[1]);
                                Caneta_Cor2 = Convert.ToInt32(caneta_valor_valores[2]);
                                Caneta_Cor3 = Convert.ToInt32(caneta_valor_valores[3]);
                                Caneta_Tamamho = Convert.ToInt32(caneta_valor_valores[4]);
                                Caneta_para_desenhar = new Pen(Color.FromArgb(Caneta_Transparencia, Caneta_Cor1, Caneta_Cor2, Caneta_Cor3), Caneta_Tamamho);
                            }
                            else { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nThis Pen does not exist", "CodeArt Error"); break; }


                            g.DrawRectangle(Caneta_para_desenhar, new Rectangle(primeiro, segundo, terceiro, quarto));
                            continue;
                        }
                        catch { MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid function, invalid values", "CodeArt Error"); break; }
                    }
                    
                    else
                    {
                        MessageBox.Show("Line: " + linha_marcada.ToString() + "\nInvalid command", "CodeArt Error"); break;
                    }

                }

                if (!this.IsDisposed) { pictureBox1.Refresh(); } //Mostra o desenho final
            }
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            richTextBox1.Text = File.ReadAllText(local);
            CodeArtBASIC();
        }


        //--------------------------------------------------------------------------------------------------------------
        // flood fill - rosettacode.org/wiki/Bitmap/Flood_fill 
        private static bool ColorMatch(Color a, Color b)
        {
            return (a.ToArgb() & 0xffffff) == (b.ToArgb() & 0xffffff);
        }

        static void FloodFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            if (ColorMatch(targetColor, replacementColor)) { return; } //Com a mesma cor nunca terminava
            Queue<Point> q = new Queue<Point>();
            q.Enqueue(pt);
            while (q.Count > 0)
            {
                Point n = q.Dequeue();
                if (!ColorMatch(bmp.GetPixel(n.X, n.Y), targetColor))
                    continue;
                Point w = n, e = new Point(n.X + 1, n.Y);
                while ((w.X >= 0) && ColorMatch(bmp.GetPixel(w.X, w.Y), targetColor))
                {
                    bmp.SetPixel(w.X, w.Y, replacementColor);
                    if ((w.Y > 0) && ColorMatch(bmp.GetPixel(w.X, w.Y - 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y - 1));
                    if ((w.Y < bmp.Height - 1) && ColorMatch(bmp.GetPixel(w.X, w.Y + 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y + 1));
                    w.X--;
                }
                while ((e.X <= bmp.Width - 1) && ColorMatch(bmp.GetPixel(e.X, e.Y), targetColor))
                {
                    bmp.SetPixel(e.X, e.Y, replacementColor);
                    if ((e.Y > 0) && ColorMatch(bmp.GetPixel(e.X, e.Y - 1), targetColor))
                        q.Enqueue(new Point(e.X, e.Y - 1));
                    if ((e.Y < bmp.Height - 1) && ColorMatch(bmp.GetPixel(e.X, e.Y + 1), targetColor))
                        q.Enqueue(new Point(e.X, e.Y + 1));
                    e.X++;
                }
            }
        }
 
    }
}

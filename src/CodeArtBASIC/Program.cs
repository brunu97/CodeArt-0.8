using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeArt_BASIC
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string local = "";
            if (args != null && args.Length > 0)
            {
                string ficheiro = args[0];

                //Ver se CRTBAS
                if ((Path.GetExtension(ficheiro) == ".CRTBAS") || (Path.GetExtension(ficheiro) == ".txt"))
                {
                    if (File.Exists(ficheiro))
                    {
                        local = ficheiro;
                    }
                }

            }
            else
            {
                Environment.Exit(0);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(local));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProdanSSH
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool ok;
            System.Threading.Mutex m = new System.Threading.Mutex(true, "ProdanSSHMutex", out ok); //cria mutex (mutual exclusion) para impedir multiplas instancias do apliacativo
            if (!ok)                                                                               //o windows registra a mutex "ProdanSSHMutex" como sendo deste aplicativo, 
            {                                                                                      //se tentar abri-lo novamente o windows verifica que a mutex ja esta em uso.   
                MessageBox.Show("Já existre uma instância do programa em execução\nProdanSSH.exe", m.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length != 0)
            {
                Application.Run(new FrmSSH(args[0]));
            }
            else
            {
                Application.Run(new FrmSSH("normalState"));
            }
            GC.KeepAlive(m); // registra a mutex 
        }
    }
}

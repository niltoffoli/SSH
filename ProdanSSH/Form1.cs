// Programado por Nilton C. Toffoli Junior

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Renci.SshNet;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace ProdanSSH
{
    public partial class Form1 : Form
    {

        #region Inicio
        public Form1(string args)
        {
            InitializeComponent();
            LeConfig();

            if (args == "-auto") //argumento de disparo automático
            {
                WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.ShowInTaskbar = false;
                tray.Visible = true; // tray é o nome do componente do ícone da bandeja
                this.Hide();
                AutoExec(); //Chama o método de execução automatica se o argumento de inicialização for "-auto"
            }

        }


        /// <summary>
        /// Grava no registro a instrução para executar na inicialização do windows, já com o parâmetro "-auto"
        /// </summary>
        public void Startup()
        {
            try
            {
                if (checkBox1.Checked) // cria chave
                {
                    RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    add.SetValue("Servidor Linux", "\"" + Application.ExecutablePath.ToString() + "\" -auto");
                    Config.Default.autotime = true;
                    Config.Default.Save(); // salva config
                }
                else // deleta chave quando desmarca checkbox
                {
                    RegistryKey remove = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    remove.DeleteValue("Servidor Linux", false);
                    Config.Default.autotime = false;
                    Config.Default.Save();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }


        /// <summary>
        /// Método de que controla a execução automática na inicialização
        /// </summary>
        /// <returns></returns>
        public bool AutoExec()
        {
            Servidor Serv = SetaServidor(true);

            if (!Serv.Equals(null))
            {
                GravaConfig(Serv);
                ComparaTempo(Serv);
                return true;
            }
            else return false;

        }


        /// <summary>
        /// Tá aqui mas não é pra usar.
        /// </summary>
        public void FirstRun()
        {
            string host = GetServerAdress();
            if (!host.Equals(null))
            {
                textIP.Text = host;
                Config.Default.host = host;
                Config.Default.Save();
            }
        }

        #endregion

        #region Arquivo de Configuração

        /// <summary>
        /// Grava os dados do servidor no arquivo de configuração.
        /// </summary>
        /// <param name="Serv"></param>
        public void GravaConfig(Servidor Serv)
        {
            Config.Default.host = Serv.host;
            Config.Default.user = Serv.user;
            Config.Default.pass = Serv.pass;
            Config.Default.Save();
        }


        /// <summary>
        /// Lê dados do arquivo de configuração
        /// </summary>
        public void LeConfig()
        {
            textIP.Text = Config.Default.host;
            textUser.Text = Config.Default.user;
            textPass.Text = Config.Default.pass;
            checkBox1.Checked = Config.Default.autotime;

        }

        #endregion

        #region Definição do Servidor

        /// <summary>
        /// Busca o endereço do servidor através do cadastrado no ODBC
        /// </summary>
        /// <returns></returns>
        public string GetServerAdress()
        {
            try
            {
                string host = string.Empty;
                string chave = string.Empty;

                if (Environment.Is64BitOperatingSystem)
                {
                    chave = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\ODBC\\ODBC.INI\\DS-PSQL70";
                }
                else
                {
                    chave = "HKEY_LOCAL_MACHINE\\SOFTWARE\\ODBC\\ODBC.INI\\DS-PSQL70";
                }
                host = Registry.GetValue(chave, "Servername", "").ToString();
                return host;
            }
            catch (Exception)
            {
                MessageBox.Show("Servidor ODBC não encontrado. Favor configurar manualmente.");
                return null;
            }
        }


        /// <summary>
        /// Ctria o objeto Servidor com os dados da conexão
        /// </summary>
        /// <param name="auto"></param>
        /// <returns></returns>
        public Servidor SetaServidor(bool auto) // False: Seta objeto com dados do servidor a partir dos dados digitados
        {                                       // True: Seta servidor a partir do arquivo config
            try
            {
                Servidor Serv = new Servidor();
                if (!auto) // não é execução automatica, pega os dados dos texbox
                {
                    Serv.host = textIP.Text;
                    Serv.user = textUser.Text;
                    Serv.pass = textPass.Text;
                }
                else // autoexec, pega dados direto do arq. config
                {
                    Serv.host = Config.Default.host;
                    Serv.user = Config.Default.user;
                    Serv.pass = Config.Default.pass;
                }
                if (Serv.host.Equals(string.Empty) && Serv.user.Equals(string.Empty) && Serv.pass.Equals(string.Empty)) // se não tem nada nos campos, nem no arq config
                    Serv = TryDefault();                                                                                    // o programa chama o metodo TryDefault
                return Serv;
            }
            catch (Exception)
            {
                Thread.Sleep(4598);
                Output("Não foi possível conectar ao servidor.\nVerifique os dados de conexão.\n\n");
                return null;
            }
        }


        /// <summary>
        /// Método maroto que seta os dados padrão caso todo o resto dê errado
        /// </summary>
        /// <returns></returns>
        public Servidor TryDefault()
        {
            Servidor Serv = new Servidor();
            Output("Não há nenhum servidor informado.\nBuscando dados por osmose, aguarde por favor...\n\n");
            Serv.host = GetServerAdress();
            Serv.user = "root";
            Serv.pass = "prodan46"; //não mudem a senha padrão
            if (!Conecta(Serv).Equals(null))
            {
                Thread.Sleep(1398);
                Output("Conectado em " + Serv.host + Environment.NewLine);
                GravaConfig(Serv);
                LeConfig();
                return Serv;
            }
            else return null;
        }


        #endregion

        #region Conexão e Comandos SSH

        /// <summary>
        /// Faz a conexão ao servidor
        /// </summary>
        /// <param name="Serv">Objeto com os dados de conexão ao servidor</param>
        /// <returns></returns>
        public object Conecta(Servidor Serv)
        {
            TimeSpan timeout = new TimeSpan(0, 0, 1);
            try
            {
                var client = new SshClient(Serv.host, Serv.user, Serv.pass); // cria objeto do cliente SSH
                client.KeepAliveInterval = timeout;
                client.Connect(); // conecta 
                if (client.IsConnected) return client; // retorna objeto client se este não for nulo
                else return null;
            }
            catch (Exception exc)
            {
                Output(exc.Message+Environment.NewLine+Environment.NewLine);
                return null;
            }
        }


        /// <summary>
        /// Método para execução de comandos no cliente SSH
        /// </summary>
        /// <param name="Serv">Objeto com os dados de conexão ao servidor</param>
        /// <param name="input">Comando a ser executado</param>
        /// <returns>Retorna o resultado do comando OU o erro que gerou</returns>
        public object Comando(Servidor Serv, string input)
        {
            try
            {
                var client = Conecta(Serv) as SshClient;
                TimeSpan timeout = new TimeSpan(0, 0, 1);
                if (client != null)
                {
                    string result, error = String.Empty;
                    if (Serv.user != "root") // teste se o usuario não for root, precisa rodar os comandos com a instrução sudo e passando a senha do usuário
                    {
                        string newinput = String.Format("echo {0} | sudo -S {1}", Serv.pass, input); // remonta o comando enviado ao método e junta a instrução sudo e a senha
                        var cmd = client.CreateCommand(input);
                        cmd.CommandTimeout = timeout;
                        cmd.Execute();
                        result = cmd.Result;
                        error = cmd.Error;
                    }
                    else // se usuario for root ta sussa!
                    {
                        var cmd2 = client.CreateCommand(input);
                        cmd2.CommandTimeout = timeout;
                        cmd2.Execute();
                        result = cmd2.Result;
                        error = cmd2.Error;
                    }
                    client.Disconnect();
                    client.Dispose();

                    if (!string.IsNullOrEmpty(result)) return result; // se o comando gerou resultado retorna o resultado
                    else if (!string.IsNullOrEmpty(error)) return error; // senão retorna o erro
                    else return null; // se não gerou nada retorna nulo
                }
                else return null;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return null;
            }
        }


        /// <summary>
        /// Imprime resultado dos comandos no RichTextBox
        /// </summary>
        /// <param name="message"></param>
        public void Output(string message)
        {
            rtbOutput.AppendText(message + Environment.NewLine); // adiciona msg
            rtbOutput.ScrollToCaret(); // corre a barra de rolagem para o fim do texto
        }

        #endregion

        #region Data e Hora

        /// <summary>
        /// Executa os comandos para buscar a data e hora corrente no servidor
        /// </summary>
        /// <param name="Serv">Objeto com os dados de conexão ao servidor</param>
        /// <returns></returns>
        public DateTime DataServ(Servidor Serv)
        {
            System.DateTime datetime = new System.DateTime();
            try
            {
                string date = string.Format("{0} {1}", Comando(Serv, "date +%D"), Comando(Serv, "date +%T")); // "date +%D" busca a data e "date +%T" busca a hora
                date = date.Replace("\n", String.Empty);

                IFormatProvider culture = CultureInfo.CurrentCulture; // Seta a timezone
                datetime = DateTime.ParseExact(date, "MM/dd/yy HH:mm:ss", culture); // Formata a string de horario
                return datetime;
            }
            catch (Exception) { return datetime; }
        }

        #endregion







        /// <summary>
        /// Compara o horário do servidor com o horário oficial da timezone buscado na internet
        /// </summary>
        /// <param name="Serv">Objeto com os dados de conexão ao servidor</param>
        public void ComparaTempo(Servidor Serv)
        {
            try
            {
                Output("Conectando ao servidor...: " + Environment.NewLine);            
                DateTime dtserv = DataServ(Serv);
                Output("Horario em "+Serv.host+" : " + dtserv.ToString() + Environment.NewLine);
                DateTime dt_oficial = GetTime();
                Output("Horario oficial: " + dt_oficial.ToString());
                TimeSpan difer = (Convert.ToDateTime(dt_oficial) - Convert.ToDateTime(dtserv)).Duration();

                if (difer > TimeSpan.FromSeconds(1))
                {
                    string cmd = string.Format("date -s '{0}-{1}-{2} {3}:{4}:{5}'", dt_oficial.Year, dt_oficial.Month.ToString().PadLeft(2, '0'), dt_oficial.Day.ToString().PadLeft(2, '0'), dt_oficial.Hour, dt_oficial.Minute, dt_oficial.Second);
                    Output("Corrigindo horário......." + Environment.NewLine);
                    var result = Comando(Serv, cmd);
                    if (!result.Equals(null))
                    {
                        Comando(Serv, "hwclock -w");
                        Output("Horário do linux alterado para\n" + result.ToString().TrimEnd() + Environment.NewLine);
                    }
                }
                else Output("Horário está dentro da tolerância, não será alterado." + Environment.NewLine + Environment.NewLine);

            }
            catch (Exception)
            {
                Output("Não foi possível verificar o horário. Verifique a conexão com o servidor."+Environment.NewLine+Environment.NewLine);
            }
        }

        #region Busca Horario
        public DateTime GetTime()
        {
            var result = DateTime.Now;
            
            string[] servers = new string[] {

                "time.nist.gov",
                "time-c.nist.gov",
                "time-d.nist.gov",
                "wwv.nist.gov"        
            };
            Random rnd = new Random();
            foreach (string server in servers.OrderBy(x => rnd.NextDouble()).Take(9))
            {
                try
                {
                    // Connect to the server (at port 13) and get the response. Timeout max 1second
                    Output("Consultando horário em " + server + Environment.NewLine);
                    string serverResponse = string.Empty;
                    var tcpClient = new TcpClient();
                    if (tcpClient.ConnectAsync(server, 13).Wait(1000))
                    {
                        using (var reader = new System.IO.StreamReader(tcpClient.GetStream()))
                        {
                            serverResponse = reader.ReadToEnd();
                        }
                    }
                    // If a response was received
                    if (!string.IsNullOrEmpty(serverResponse))
                    {
                        // Split the response string ("55596 11-02-14 13:54:11 00 0 0 478.1 UTC(NIST) *")
                        string[] tokens = serverResponse.Split(' ');

                        // Check the number of tokens
                        if (tokens.Length >= 6)
                        {
                            // Check the health status
                            string health = tokens[5];
                            if (health == "0")
                            {
                                // Get date and time parts from the server response
                                string[] dateParts = tokens[1].Split('-');
                                string[] timeParts = tokens[2].Split(':');

                                // Create a DateTime instance
                                DateTime utcDateTime = new DateTime(
                                Convert.ToInt32(dateParts[0]) + 2000,
                                Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]),
                                Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]),
                                Convert.ToInt32(timeParts[2]));

                                // Convert received (UTC) DateTime value to the local timezone
                                result = utcDateTime.ToLocalTime();
                                return result;
                                // Response successfully received; exit the loop

                            }
                        }

                    }

                }
                catch
                {
                    // Ignore exception and try the next server
                }

            }
            return result;
        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Startup();
        }

        private void tray_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            this.Visible = true;
            this.ShowInTaskbar = true;
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void btnLimpa_Click(object sender, EventArgs e)
        {
            rtbOutput.Clear();
        }

        Uri uri = new Uri("https://the.earth.li/~sgtatham/putty/latest/w32/putty.exe");
        string filename = (Application.StartupPath+"\\putty.exe");
        int a = 1;
        private void btnPutty_Click(object sender, EventArgs e)
        {
            try
            {
                Servidor Serv = new Servidor();
                Serv = SetaServidor(false);
                string args = string.Format(" -ssh {0}@{1} 22 -pw {2}", Serv.user, Serv.host, Serv.pass);

                if(File.Exists(filename))
                {
                    Output("Abrindo sessão SSH em " + Serv.host + "...\n\n");
                    Thread.Sleep(500);
                    Process process = Process.Start(filename, args);
                }
                else
                {
                    Output("PuTTY não encontrado. Efetuando download...\n\n");
                    WebClient wc = new WebClient();
                    wc.DownloadFileAsync(uri, filename);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                }
            }           
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }      
        }
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = e.ProgressPercentage;
                if (progressBar1.Value == progressBar1.Maximum)
                {
                    progressBar1.Value = 0;
                }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Servidor Serv = new Servidor();
                Serv = SetaServidor(false);
                Output("PuTTY baixado com sucesso...\nAbrindo sessão em " + Serv.host + "...\n\n");
                string args = string.Format("-ssh {0}@{1} 22 -pw {2}", Serv.user, Serv.host, Serv.pass);
                progressBar1.Visible = false;
                Process.Start(filename, args);
            }
            else Output("Não foi possível iniciar o PuTTY./nFavor baixe manualmente.\n\n");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Hide();
                return true;
            }
            if (keyData == Keys.F12)
            {
                textIP.Text = string.Empty;
                textUser.Text = string.Empty;
                textPass.Text = string.Empty;
                Config.Default.Reset();
                Config.Default.Save();
                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch { }
                }
                Output("nooooooooo!!!!\n\n");
                return true;
            }
            if (keyData == Keys.F1)
            {
                Output("Fui eu que fiz, mas se deu merda a culpa não é minha.\n\nby Nilton Toffoli.\n\n");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Reiniciar o Servidor?", "Reiniciar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {     
                DialogResult dialogResult2 = MessageBox.Show("Tem certeza que deseja Reiniciar o Servidor??", "Reiniciar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult2 == DialogResult.Yes)
                {
                    Servidor Serv = new Servidor();
                    Serv = SetaServidor(false);
                    Comando(Serv, "reboot");
                    Output("O servidor está reiniciando...\n\n");
                }
            }
        }

        private void btnListproc_Click(object sender, EventArgs e)
        {
            Servidor Serv = new Servidor();
            Serv = SetaServidor(false);
            string result = Comando(Serv, "ps -ax") as string;
            Output(result);
        }

        private void btnShootToKill_Click(object sender, EventArgs e)
        {
            Point screenPoint = btnShootToKill.PointToScreen(new Point(btnShootToKill.Left, btnShootToKill.Bottom));

            if (screenPoint.Y + contextProcKill.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
            contextProcKill.Show(btnShootToKill, new Point(0, -contextProcKill.Size.Height));           
            else contextProcKill.Show(btnShootToKill, new Point(0, btnShootToKill.Height));

            toolStripTextBox1.Focus();          
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) //se tecla pressionada for enter
            {
                Servidor Serv = new Servidor();
                Serv = SetaServidor(false);
                string procid = toolStripTextBox1.Text;
                toolStripTextBox1.Clear();
                Output("kill " + procid + Environment.NewLine);
                string result = Comando(Serv, "kill " + procid) as string;
                Output(result + Environment.NewLine);
            }
        }

        private void btnVerifica_Click(object sender, EventArgs e)
        {
            Servidor Serv = SetaServidor(false);

            if (Serv != null)
            {
                GravaConfig(Serv);
                ComparaTempo(Serv);
            }
        }
    }
}

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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProdanSSH
{
    public partial class FrmSSH : Form
    {

        #region Inicio
        public FrmSSH(string args)
        {
            InitializeComponent();
            LeConfig();
            if (textUser.Text == "root")
            {
                textRoot.Enabled = false;
                lblRoot.Enabled = false;
            }
            else
            {
                textRoot.Enabled = true;
                lblRoot.Enabled = true;
            }

            if (args == "-auto") //argumento de disparo automático
            {
                WindowState = FormWindowState.Minimized;
                rtbOutput.SelectionProtected = true;
                this.Visible = false;
                this.ShowInTaskbar = false;
                tray.Visible = true; // tray é o nome do componente do ícone da bandeja
                this.Hide();
                AutoExec(); //Chama o método de execução automatica se o argumento de inicialização for "-auto"
            }

        }

        /// <summary>
        /// Evento para quando a caixa Verificar na Incialização for marcada ou desmarcada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Startup();
        }


        /// <summary>
        /// Grava no registro a instrução para executar na inicialização do windows, já com o parâmetro "-auto"
        /// </summary>
        public void Startup()
        {
            try
            {
                if (checkVerify.Checked) // cria chave
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
            Config.Default.rootpass = Serv.rootpass;
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
            textRoot.Text = Config.Default.rootpass;
            checkVerify.Checked = Config.Default.autotime;
        }

        #endregion

        #region Definição do Servidor

        /// <summary>
        /// Busca no registro do windows o endereço do servidor através do cadastrado no ODBC
        /// </summary>
        /// <returns></returns>
        public string GetServerAdress()
        {
            try
            {
                string host = string.Empty;
                string chave = string.Empty;

                if (Environment.Is64BitOperatingSystem) //testa se o sistema 32 ou 64 bits, pois a chave ODBC no registro é diferente
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
                    Serv.rootpass = textRoot.Text;
                }
                else // autoexec, pega dados direto do arq. config
                {
                    Serv.host = Config.Default.host;
                    Serv.user = Config.Default.user;
                    Serv.pass = Config.Default.pass;
                    Serv.rootpass = Config.Default.rootpass;
                }
                if (Serv.host.Equals(string.Empty) && Serv.user.Equals(string.Empty) && Serv.pass.Equals(string.Empty)) // se não tem nada nos campos, nem no arq config
                    Serv = TryDefault();                                                                                    // o programa chama o metodo TryDefault
                return Serv;
            }
            catch (Exception)
            {
                Thread.Sleep(4598);
                Output("Não foi possível conectar ao servidor.\nVerifique os dados de conexão.\n\n\n", true);
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
            Output("Não há nenhum servidor informado.\nBuscando dados por osmose, aguarde por favor...\n\n\n", true);
            Serv.host = GetServerAdress();
            Serv.user = "root";
            Serv.pass = "prodan46"; //não mudem a senha padrão
            if (!Conecta(Serv).Equals(null))
            {
                Thread.Sleep(1398);
                Output("Conectado em " + Serv.host + Environment.NewLine + Environment.NewLine, true);
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
        public SshClient Conecta(Servidor Serv)
        {
            int retries = 5; //5 tentativas de conexão
            while (retries > 0)
            {
                try
                {
                    SshClient client = new SshClient(Serv.host, Serv.user, Serv.pass); // cria objeto do cliente SSH
                    client.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(1500); //define timeout em ms
                    client.Connect(); //conecta 
                    if (client.IsConnected) return client; //retorna objeto client se este não for nulo
                    else return null;
                }
                catch (Exception)
                {
                    if (--retries == 0)
                    {
                        Output("Conexão falhou após 5 tentativas." + Environment.NewLine + Environment.NewLine + Environment.NewLine, true);
                    }         
                }
            }
            return null;
        }
       
        /// <summary>
        /// Método para execução de comandos básicos no cliente SSH
        /// </summary>
        /// <param name="Serv">Objeto com os dados de conexão ao servidor</param>
        /// <param name="input">Comando a ser executado</param>
        /// <returns>Retorna o resultado do comando OU o erro que gerou</returns>
        public object Comando(Servidor Serv, string input)
        {
            try
            {
                SshClient client = Conecta(Serv); //define cliente SSH

                if (client != null)
                {
                    string result, error = String.Empty;

                    var cmd = client.CreateCommand(input); //cria comando recebido pela string input
                    cmd.Execute();
                    result = cmd.Result; 
                    error = cmd.Error;

                    client.Disconnect();
                    client.Dispose();

                    if (!string.IsNullOrEmpty(result)) return result; // se o comando gerou resultado retorna o resultado
                    else if (!string.IsNullOrEmpty(error)) return error; // senão retorna o erro
                    else return null; // se não gerou nada retorna nulo
                }
                else throw new Exception();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return null;
            }
        }

        /// <summary>
        /// Método para rodar comandos que precisem login de superusuario. Breve explicação abaixo.
        /// </summary>
        /// <param name="Serv"></param>
        /// <param name="input"></param>
        public void ComandoRoot(Servidor Serv, string input)
        {

            var client = Conecta(Serv) as SshClient;

            // Dependendo da versão do Linux, não é possível fazer login como root por SSH. 
            // Para rodar comandos como SU é preciso dar o comando "su -" e passar a senha de root direto no terminal.
            // As instruções abaixo simulam um terminal tty para rodar comandos como root.

            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
            ShellStream shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);  // cria objeto shellstream para receber e enviar o fluxo de dados
            string rep = String.Empty;

            rep = shellStream.Expect(new Regex(@"[$#]")); // Aguarda o fluxo de dados do terminal conter a expressão regular definida...
            shellStream.WriteLine("su -"); // para escrever o comando

            rep = shellStream.Expect(new Regex(@"[:$#]")); // aguarda o terminal pedir a senha
            shellStream.WriteLine(Serv.rootpass);

            rep = shellStream.Expect(new Regex(@"([#])")); // aguarda o login como root para excutar o comando
            shellStream.WriteLine(input);

            rep = shellStream.Expect(new Regex(@"([#])"));
            Output(rep+Environment.NewLine+Environment.NewLine, true); // imprime o fluxo de dados

        }


        /// <summary>
        /// Imprime resultado dos comandos no RichTextBox
        /// </summary>
        /// <param name="message"></param>
        public void Output(string message, bool scroll)
        {
            if (this.rtbOutput.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Output);
                this.Invoke(d, new object[] { message, true });
            }
            else
            {
                rtbOutput.AppendText(message); // adiciona msg
                if (scroll) rtbOutput.ScrollToCaret(); // corre a barra de rolagem para o fim do texto
            }
        }

        private void clearTextBox()
        {
            if (this.rtbOutput.InvokeRequired)
            {
                ClearTextCallback c = new ClearTextCallback(clearTextBox);
                this.Invoke(c, new object[] {});
            }
            else
            {
                rtbOutput.Clear();
            }
        }


        #endregion

        #region Data e Hora


        /// <summary>
        /// Evento disparado quando clicar no botão Verifica Horario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVerifica_Click(object sender, EventArgs e)
        {
            Servidor Serv = SetaServidor(false);

            if (Serv != null)
            {
                GravaConfig(Serv);
                ComparaTempo(Serv);
            }
        }

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


        /// <summary>
        /// Compara o horário do servidor com o horário oficial da timezone buscado na internet
        /// </summary>
        /// <param name="Serv">Objeto com os dados de conexão ao servidor</param>
        public void ComparaTempo(Servidor Serv)
        {
            try
            {
                Output("Conectando ao servidor " + Comando(Serv, "hostname") + Environment.NewLine + Environment.NewLine, true);
                DateTime dtserv = DataServ(Serv);
                Output("Horario em " + Serv.host + " : " + dtserv.ToString() + Environment.NewLine + Environment.NewLine, true);
                DateTime dt_oficial = GetTime();
                Output("Horario oficial: " + dt_oficial.ToString() + Environment.NewLine, true);
                TimeSpan difer = (Convert.ToDateTime(dt_oficial) - Convert.ToDateTime(dtserv)).Duration();

                if (difer > TimeSpan.FromSeconds(1))
                {
                    string cmd = string.Format("date -s '{0}-{1}-{2} {3}:{4}:{5}'", dt_oficial.Year, dt_oficial.Month.ToString().PadLeft(2, '0'), dt_oficial.Day.ToString().PadLeft(2, '0'), dt_oficial.Hour, dt_oficial.Minute, dt_oficial.Second);
                    Output("Corrigindo horário......." + Environment.NewLine + Environment.NewLine, true);
                    var result = Comando(Serv, cmd);
                    if (!result.Equals(null))
                    {
                        Comando(Serv, "hwclock -w");
                        Output("Horário do linux alterado para\n" + result.ToString().TrimEnd() + Environment.NewLine + Environment.NewLine, true);
                    }
                }
                else Output("Horário está dentro da tolerância, não será alterado." + Environment.NewLine + Environment.NewLine + Environment.NewLine, true);

            }
            catch (Exception)
            {
                Output("Não foi possível verificar o horário. Verifique a conexão com o servidor." + Environment.NewLine + Environment.NewLine + Environment.NewLine, true);
            }
        }

        /// <summary>
        /// Busca horário oficial da internet
        /// </summary>
        /// <returns></returns>
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
                    Output("Consultando horário em " + server + Environment.NewLine + Environment.NewLine, true);
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

        #region Processos

        /// <summary>
        /// Gera o menu de contexto ao clicar no botão Listar Processos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnListproc_Click(object sender, EventArgs e)
        {
            Point screenPoint = btnListproc.PointToScreen(new Point(btnListproc.Left, btnListproc.Bottom));

            if (screenPoint.Y + contextProcList.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
                contextProcList.Show(btnListproc, new Point(0, -contextProcList.Size.Height));
            else contextProcList.Show(btnListproc, new Point(0, btnListproc.Height));          
        }

        /// <summary>
        /// Botão dento do menu contexto do botão Listar Processos. Lista Todos os processos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void todosOsProcessosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Servidor Serv = new Servidor();
            Serv = SetaServidor(false);
            string result = Comando(Serv, "ps -ax") as string;
            Output(result + Environment.NewLine, true);
        }

        /// <summary>
        /// Botão dento do menu contexto do botão Listar Processos. Lista apenas os processos do postgres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void somentePostgresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Servidor Serv = new Servidor();
            Serv = SetaServidor(false);
            string result = Comando(Serv, "ps -ax | grep postgres") as string;
            Output(result + Environment.NewLine, true);
        }

        /// <summary>
        /// Gera o menu de contexto com a textbox para informar o PID do processo ao clicar no botão Matar Processo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShootToKill_Click(object sender, EventArgs e)
        {
            Point screenPoint = btnShootToKill.PointToScreen(new Point(btnShootToKill.Left, btnShootToKill.Bottom));

            if (screenPoint.Y + contextProcKill.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
                contextProcKill.Show(btnShootToKill, new Point(0, -contextProcKill.Size.Height));
            else contextProcKill.Show(btnShootToKill, new Point(0, btnShootToKill.Height));

            toolStripTextBox1.Focus();
        }

        /// <summary>
        /// Evento disparado ao teclar enter na textbox do botão Matar processo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) //se tecla pressionada for enter
            {
                Servidor Serv = new Servidor();
                Serv = SetaServidor(false);
                string procid = toolStripTextBox1.Text;
                toolStripTextBox1.Clear();
                Output("kill " + procid + Environment.NewLine + Environment.NewLine, true);
                ComandoRoot(Serv, "kill " + procid);
            }
        }


        #endregion

        #region Postgres

        /// <summary>
        /// Evento disparado ao clicar no botão Reiniciar Postgres. Envia os comandos "parar" e "iniciar" e gera uma nova thread para verificar o status do banco.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPgReset_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Todas as conexões ao banco de dados serão encerradas. Continuar?", "Reiniciar Postgres?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                Servidor Serv = new Servidor();
                Serv = SetaServidor(false);
                Output("Reiniciando Postgres..." + Environment.NewLine + Environment.NewLine, true);
                ComandoRoot(Serv, "/etc/rc.d/rc.postgres stop");
                Thread.Sleep(1000);
                ComandoRoot(Serv, "/etc/rc.d/rc.postgres start");
                Thread.Sleep(1000);
                Thread Pg = new Thread(new ThreadStart(this.PgCheck));
                Pg.Start();
            }
        }

        /// <summary>
        /// Método para verificar o status do banco
        /// </summary>
        public void PgCheck()
        {
            Servidor Serv = new Servidor();
            Serv = SetaServidor(false);
            ComandoRoot(Serv, "/etc/rc.d/rc.postgres status");
        }


        #endregion

        #region Server Restart

        /// <summary>
        /// Método que envia comando para reiniciar o servidor. Gera uma thread que fica verificando quando o servidor fica online novamente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestart_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Reiniciar o Servidor?", "Reiniciar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes) //confirmação
            {
                DialogResult dialogResult2 = MessageBox.Show("Tem certeza que deseja Reiniciar o Servidor??", "Reiniciar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult2 == DialogResult.Yes)
                {
                    Servidor Serv = new Servidor();
                    Serv = SetaServidor(false);
                    ComandoRoot(Serv, "reboot");
                    Output("O servidor está reiniciando...\n\n\n", true);
                    Thread Pg1 = new Thread(new ThreadStart(this.ServCheck)); //dispara thread para verificar
                    Pg1.Start();
                }
            }
        }

        /// <summary>
        /// Método chamado ao reinciar o servidor. Aguarda 7 segundos e depois verifica a cada segundo se o servidor já reiniciou. 
        /// </summary>
        public void ServCheck()
        {
            Thread.Sleep(7000);
            Output("Testando conexão....." + Environment.NewLine + Environment.NewLine, true);
            Servidor Serv = new Servidor();
            string check = string.Empty;
            Serv = SetaServidor(false);
            do
            {
                Thread.Sleep(1000);
                check = Comando(Serv, "hostname") as string; //O método Comando() só retornará um valor válido para a variável check quando a conexão for bem sucedida.
                if (check != null)
                {
                    Output("Servidor " + check + " está rodando.", true);
                }

            }
            while (check == null); //Faz a verificação enquanto a variável check não recebe valor.

        }


        #endregion

        #region PuTTY

        //endereço de download do Putty
        Uri uri = new Uri("https://the.earth.li/~sgtatham/putty/latest/w32/putty.exe");
        string filename = (Application.StartupPath + "\\putty.exe");

        /// <summary>
        /// Método disparado ao clicar no botão Putty. Faz a conexão ao servidor se o aplicativo existe na pasta. Senão, dispara o método para fazer o download e depois faz a conexão.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPutty_Click(object sender, EventArgs e)
        {
            try
            {
                Servidor Serv = new Servidor();
                Serv = SetaServidor(false);
                string args = string.Format(" -ssh {0}@{1} 22 -pw {2}", Serv.user, Serv.host, Serv.pass); //argumentos de conexão ao Putty

                if (File.Exists(filename)) //se o putty já existe
                {
                    Output("Abrindo sessão SSH em " + Serv.host + "...\n\n\n", true);
                    Thread.Sleep(500);
                    Process process = Process.Start(filename, args); //executa aplicativo
                }
                else //se o Putty não existe
                {
                    Output("PuTTY não encontrado. Efetuando download...\n\n\n", true);
                    WebClient wc = new WebClient();
                    wc.DownloadFileAsync(uri, filename); //faz o download
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Controle de Progressbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = e.ProgressPercentage;
            if (progressBar1.Value == progressBar1.Maximum)
            {
                progressBar1.Value = 0;
            }
        }

        /// <summary>
        /// Método disparado no término do download para fazer a conexão.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Servidor Serv = new Servidor();
                Serv = SetaServidor(false);
                Output("PuTTY baixado com sucesso...\nAbrindo sessão em " + Serv.host + "...\n\n\n", true);
                string args = string.Format("-ssh {0}@{1} 22 -pw {2}", Serv.user, Serv.host, Serv.pass);
                progressBar1.Visible = false;
                Process.Start(filename, args);
            }
            else Output("Não foi possível iniciar o PuTTY./nFavor baixe manualmente.\n\n\n", true);
        }


        #endregion

        #region Controle Janelas

        /// <summary>
        /// Clique duplo no tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tray_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            this.Visible = true;
            this.ShowInTaskbar = true;
        }

        /// <summary>
        /// Evento para sair do programa clicando no menu do tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Não fecha o programa clicando no X na tela, apenas minimiza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }


        #endregion

        #region Auxiliares

        //botão limpa a tela
        private void btnLimpa_Click(object sender, EventArgs e)
        {
            clearTextBox();
        }


        /// <summary>
        /// Teclas pressionadas
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) //ESC fecha a tela
            {
                this.Hide();
                return true;
            }
            if (keyData == Keys.F12) //limpa tudo
            {
                clearTextBox();
                textIP.Text = string.Empty;
                textUser.Text = string.Empty;
                textPass.Text = string.Empty;
                textRoot.Text = string.Empty;
                checkVerify.Checked = false;
                Config.Default.Reset();
                Config.Default.Save();
                if (File.Exists(filename)) //deleta o putty
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch { }
                }
                RegistryKey remove = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                remove.DeleteValue("Servidor Linux", false);
                Thread cls = new Thread(new ThreadStart(this.Clear));
                cls.Start();                
                return true;
            }
            if (keyData == Keys.F1) //help na tela
            {
                Output("\n\n-----------------------------------------------------------------------------------------\n\nProdanSSH - Nilton Toffoli - HELP\n\n-----------------------------------------------------------------------------------------\n\n                    Configurações:\n\nAs configurações ficam salvas automaticamente após a primeira conexão válida.\n\n- Host: Endereço IP do servidor.\n- Usuário: Usuário para acesso ao linux\n- Senha: Senha do usuário do linux.\n- Verificar horario na inicialização: Inicia programa com o Windows e faz a verificação\n- F12: Limpar configurações\n\n                    Funções:\n\n- Verificar Horário: Busca o horário atual na internet e compara com a data/hora do servidor. Altera automaticamente se estiver fora da tolerância de 1 min.\n- Lista Processos: Dispara o comando 'ps -ax' e lista os processos do servidor.\n- Matar Processo: Finaliza Processos (digite o PID do processo e tecle ENTER).\n- Reiniciar Postgres: Dispara o comando para reiniciar o Postgres.\n- Reinciar Servidor: Dispara o comando 'reboot' e reinciar o servidor.\n- PuTTY: Executa e conecta o PuTTY no servidor expecificado. Faz o download automático do PuTTY se esse não for encontrado.\n\n-----------------------------------------------------------------------------------------\n\n\n        Limpa Tela\n\n            |\n            |\n            |", false);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        //firulas :)
        public void Clear()
        {
            Output("Configurações Excluídas.", true);
            Thread.Sleep(2000);
            clearTextBox();
        }


        /// <summary>
        /// Desabilita o campo de senha do root se o usuario for root
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textUser_TextChanged(object sender, EventArgs e)
        {
            if (textUser.Text == "root")
            {
                textRoot.Enabled = false;
                lblRoot.Enabled = false;
            }
            else
            {
                textRoot.Enabled = true;
                lblRoot.Enabled = true;
            }
        }



        #endregion

        // delegate para imprimir mensagens na rich text via thread secundária
        delegate void SetTextCallback(string text, bool scroll);
        delegate void ClearTextCallback();

    }
}

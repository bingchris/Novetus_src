#region Usings
using Novetus.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.CompilerServices.RuntimeHelpers;
#endregion

namespace NovetusLauncher
{
    #region Server Browser
    public partial class ServerBrowser : Form
    {
        #region Private Variables
        List<ServerBrowserDef> serverList = new List<ServerBrowserDef>();
        private ServerBrowserDef selectedServer;
        private string oldIP;
        private int oldPort;
        private bool loadingServers;
        private bool loadingMasterConfig = false;
        private bool isUsingWeb = false;
        [System.Runtime.InteropServices.DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern bool InternetSetCookieEx(string url, string name, string data, int flags, IntPtr reserved);

        private const int INTERNET_COOKIE_HTTPONLY = 0x2000;

        #endregion

        #region Constructor
        public ServerBrowser()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Events
        private async void MasterServerRefreshButton_Click(object sender, EventArgs e)
        {
            await LoadMasterServerConfig(MasterServerBox.Text, AuthenticationBox.Checked, TokenFieldCheckbox.Checked, AuthenticationTokenTextBox.Text);
            if (!isUsingWeb) await LoadServers(AuthenticationBox.Checked, TokenFieldCheckbox.Checked, AuthenticationTokenTextBox.Text);
        }

        private void JoinGameButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ServerListView.Items.Count > 0 && selectedServer != null)
                {
                    if (selectedServer.IsValid())
                    {
                        oldIP = GlobalVars.CurrentServer.ServerIP;
                        oldPort = GlobalVars.CurrentServer.ServerPort;
                        GlobalVars.CurrentServer.ServerIP = selectedServer.ServerIP;
                        GlobalVars.CurrentServer.ServerPort = selectedServer.ServerPort;
                        Client.LaunchRBXClient(selectedServer.ServerClient, ScriptType.Client, false, true, new EventHandler(ClientExited));
                    }
                }
                else
                {
                    MessageBox.Show("Select a server before joining it.", "Novetus - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot join server (" + ex.GetBaseException().Message + ").", "Novetus - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ClientExited(object sender, EventArgs e)
        {
            if (!GlobalVars.LocalPlayMode && GlobalVars.GameOpened != ScriptType.Server)
            {
                GlobalVars.GameOpened = ScriptType.None;
            }
            Client.UpdateRichPresence(Client.GetStateForType(GlobalVars.GameOpened));
            GlobalVars.CurrentServer.ServerIP = oldIP;
            GlobalVars.CurrentServer.ServerPort = oldPort;
        }

        private void ServerListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ServerListView.SelectedIndices.Count <= 0)
                {
                    return;
                }
                int intselectedindex = ServerListView.SelectedIndices[0];
                if (intselectedindex >= 0)
                {
                    selectedServer = serverList.Find(item => item.ServerName == ServerListView.Items[intselectedindex].Text);
                }
            }
            catch (Exception ex)
            {
                Util.LogExceptions(ex);
            }
        }

        private void ServerBrowser_Load(object sender, EventArgs e)
        {
            MasterServerBox.Text = GlobalVars.UserConfiguration.ReadSetting("ServerBrowserServerAddress");
            CenterToScreen();
        }

        private async void ServerBrowser_Shown(object sender, EventArgs e)
        {
            await LoadMasterServerConfig(MasterServerBox.Text);
            if (!isUsingWeb) await LoadServers();
        }

        private void MasterServerBox_TextChanged(object sender, EventArgs e)
        {
            GlobalVars.UserConfiguration.SaveSetting("ServerBrowserServerAddress", MasterServerBox.Text);
        }
        #endregion

        #region Functions
        // used for webbrowser frames only (fornow)
        async Task LoadMasterServerConfig(string url, bool isAuthenticationEnabled = false, bool isAuthenticationTokenEnabled = false, string AuthenticationToken = "")
        {
            if (loadingMasterConfig) return;
            string oldText = Text;
            Text = Text + " (Fetching Master Server Config...)";
            loadingMasterConfig = true;
            if (url != "")
            {
                try
                {
                    url = "http://" + url + "/masterconfig.php";
                    //bool useModernServerList;
                    // disable webbrowser control right now (and set url to empty)
                    ModernServerList.Url = null;
                    ModernServerList.Visible = false;
                    ModernServerList.SendToBack();
                    string config = await LoadStringFromFile(url);

                    Stream configasstream = new MemoryStream(Encoding.ASCII.GetBytes(config));
                    // format is key: value
                    if (!string.IsNullOrWhiteSpace(config))
                    {
                        using (Stream stream = configasstream )
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string line;
                                while ((line = await reader.ReadLineAsync()) != null)
                                {
                                    string[] line2 = line.Split(new[] { ": " }, StringSplitOptions.None);
                                    switch (line2[0])
                                    {
                                        case "useWebBrowser":
                                            bool agreedToSecurityQuestion = false;
                                            if (line2[1] != "false")
                                            {
                                                var securityQuestion = MessageBox.Show(
                                                    "This master server uses web browser, which can be vulnerable on old systems " +
                                                    "that have certain security problems (like ActiveX objects or VBScript) unpatched in JScript. " +
                                                    "This is mostly safe on newer systems, but if you aren't sure, research it and check your internet security settings.\nAre you sure you want to continue?",
                                                    "Security question",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning );
                                                if (securityQuestion == DialogResult.Yes)
                                                {
                                                    if (isAuthenticationEnabled)
                                                        InternetSetCookieEx("http://" + MasterServerBox.Text, "PlayerInfo", GlobalVars.ProgramInformation.Version + ";" + GlobalVars.PlayerTripcode + ";" + Environment.OSVersion.VersionString + "; path=/", INTERNET_COOKIE_HTTPONLY, IntPtr.Zero);
                                                    if (isAuthenticationTokenEnabled)
                                                        InternetSetCookieEx("http://" + MasterServerBox.Text, "Token", AuthenticationToken + "; path=/", INTERNET_COOKIE_HTTPONLY, IntPtr.Zero);
                                                    ModernServerList.Url = new Uri("http://" + MasterServerBox.Text + "/" + line2[1]);
                                                    ModernServerList.Visible = true;
                                                    ModernServerList.BringToFront();
                                                    isUsingWeb = true;
                                                    agreedToSecurityQuestion = true;
                                                }
                                            }
                                            if (!agreedToSecurityQuestion)
                                            {
                                                ModernServerList.Url = null;
                                                ModernServerList.Visible = false;
                                                ModernServerList.SendToBack();
                                                isUsingWeb = false;
                                            }
                                            break;
                                        // add more plz
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            } 
            loadingMasterConfig = false;
            Text = oldText;

        }
        async Task<string> LoadStringFromFile(string url)
        {
            using var client = new WebClient();
            try
            {
                return await client.DownloadStringTaskAsync(new Uri(url));
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse resp)
            {
                throw new Exception($"Error while fetching {url}. {(int)resp.StatusCode}: {resp.StatusDescription}");
            }

        }
        async Task LoadServerInfoFromFile(string url)
        {
            //https://stackoverflow.com/questions/2471209/how-to-read-a-file-from-internet#2471245
            //https://stackoverflow.com/questions/10826260/is-there-a-way-to-read-from-a-website-one-line-at-a-time
            //https://stackoverflow.com/questions/856885/httpwebrequest-to-url-with-dot-at-the-end
            MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (getSyntax != null && flagsField != null)
            {
                foreach (string scheme in new[] { "http", "https" })
                {
                    UriParser parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                    if (parser != null)
                    {
                        int flagsValue = (int)flagsField.GetValue(parser);
                        // Clear the CanonicalizeAsFilePath attribute
                        if ((flagsValue & 0x1000000) != 0)
                            flagsField.SetValue(parser, flagsValue & ~0x1000000);
                    }
                }
            }

            WebClient client = new WebClient();
            Uri uri = new Uri(url);
            using (Stream stream = await client.OpenReadTaskAsync(uri))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string DecodedLine = "";

                        try
                        {
                            string[] initialLine = line.Split('|');
                            DecodedLine = SecurityFuncs.Decode(initialLine[1], true);
                        }
                        catch (Exception)
                        {
                            DecodedLine = SecurityFuncs.Decode(line, true);
                        }

                        string[] serverInfo = DecodedLine.Split('|');
                        ServerBrowserDef gameServer = new ServerBrowserDef(serverInfo[0], serverInfo[1], serverInfo[2], serverInfo[3], serverInfo[4]);
                        if (gameServer.IsValid())
                        {
                            serverList.Add(gameServer);
                        }
                    }
                }
            }
        }

        void ShowError(Exception ex = null, string customMessage = "There are no servers available on this master server.")
        {
            string baseMessage = "";

            if (ex != null)
            {
                baseMessage = ex.GetBaseException().Message;
            }
            
            if (ex == null || (ex != null && baseMessage.Contains("404")))
            {
                baseMessage = customMessage;
            }

            if (ex != null)
            {
                Util.LogExceptions(ex);
            }

            string message = "Unable to load servers (" + baseMessage + ").\n\nMake sure you have a master server address entered in the textbox.\nIf the server still does not load properly, consult the administrator of the server for more information.";

            MessageBox.Show(message, "Novetus - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        async Task LoadServers(bool isAuthenticationEnabled=false, bool isAuthenticationTokenEnabled=false, string AuthenticationToken="")
        {
            if (loadingServers)
                return;

            string oldText = Text;
            Text = Text + " (Loading Servers...)";
            loadingServers = true;

            if (!string.IsNullOrWhiteSpace(MasterServerBox.Text))
            {
                try
                {
                    serverList.Clear();
                    Task info;
                    string masterServerUrl = "http://" + MasterServerBox.Text + "/serverlist.txt";
                    string version = null;
                    string tripCode = null;
                    string os = null;
                    string AuthenticationString = null;
                    if (isAuthenticationEnabled)
                    {
                        version = GlobalVars.ProgramInformation.Version;
                        tripCode = GlobalVars.PlayerTripcode; // should i update laucher form to mention this?
                        os = Environment.OSVersion.VersionString;
                        // format: Snapshot v25.9451.1;(playertripcode);Microsoft Windows NT 10.0.19045.0
                        AuthenticationString = version + ";" + tripCode + ";" + os;
                    }
                    if (!isAuthenticationEnabled)
                    {
                        info = Task.Factory.StartNew(() => LoadServerInfoFromFile(masterServerUrl));
                    }
                    else if (isAuthenticationEnabled && isAuthenticationTokenEnabled && AuthenticationToken != "")
                    {
                        info = Task.Factory.StartNew(() => LoadServerInfoFromFile(masterServerUrl + "?clientinfo=" + AuthenticationString + "&token=" + AuthenticationToken));
                    }
                    else
                    {
                        info = Task.Factory.StartNew(() => LoadServerInfoFromFile(masterServerUrl + "?clientinfo=" + AuthenticationString));
                    }
                    Task.WaitAll(info);

                    ServerListView.BeginUpdate();
                    ServerListView.Clear();

                    if (serverList.Count > 0)
                    {
                        var ColumnName = new ColumnHeader();
                        ColumnName.Text = "Name";
                        ColumnName.TextAlign = HorizontalAlignment.Center;
                        ColumnName.Width = 284;
                        ServerListView.Columns.Add(ColumnName);

                        var ColumnClient = new ColumnHeader();
                        ColumnClient.Text = "Client";
                        ColumnClient.TextAlign = HorizontalAlignment.Center;
                        ColumnClient.Width = 75;
                        ServerListView.Columns.Add(ColumnClient);

                        var ColumnVersion = new ColumnHeader();
                        ColumnVersion.Text = "Version";
                        ColumnVersion.TextAlign = HorizontalAlignment.Center;
                        ColumnVersion.Width = 110;
                        ServerListView.Columns.Add(ColumnVersion);

                        foreach (var server in serverList)
                        {
                            var serverItem = new ListViewItem(server.ServerName);
                            serverItem.UseItemStyleForSubItems = false;

                            var serverClient = new ListViewItem.ListViewSubItem(serverItem, server.ServerClient);
                            serverItem.SubItems.Add(serverClient);

                            var serverVersion = new ListViewItem.ListViewSubItem(serverItem, server.ServerVersion);

                            if (serverVersion.Text != GlobalVars.ProgramInformation.Version)
                            {
                                serverVersion.ForeColor = Color.Red;
                            }

                            serverItem.SubItems.Add(serverVersion);

                            ServerListView.Items.Add(serverItem);
                        }
                    }
                    else
                    {
                        ShowError();
                        Text = oldText;
                        loadingServers = false;
                    }

                    ServerListView.EndUpdate();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                    ServerListView.Clear();
                }
                finally
                {
                    Text = oldText;
                    loadingServers = false;
                }
            }
            else
            {
                ShowError(null, "There is no master server address. Please enter one in.");
                Text = oldText;
                loadingServers = false;
            }
        }

        #endregion

        private void AuthenticationBox_CheckedChanged(object sender, EventArgs e)
        {
            TokenFieldCheckbox.Enabled = AuthenticationBox.Checked;
            AuthenticationTokenTextBox.Enabled = TokenFieldCheckbox.Checked;
            if (!AuthenticationBox.Checked) AuthenticationTokenTextBox.Enabled = false;
        }

        private void TokenFieldCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            AuthenticationTokenTextBox.Enabled = TokenFieldCheckbox.Checked;
        }

        private async void MasterServerBox_Leave(object sender, EventArgs e)
        {
            await LoadMasterServerConfig(MasterServerBox.Text);
        }

        private async void MasterServerBox_KeyDown(object sender, KeyEventArgs e)
        {
            await LoadMasterServerConfig(MasterServerBox.Text);
        }
    }
    #endregion

    #region Server browser Definition
    public class ServerBrowserDef
    {
        public ServerBrowserDef(string name, string ip, string port, string client, string version)
        {
            ServerName = SecurityFuncs.Decode(name, true);
            ServerIP = SecurityFuncs.Decode(ip, true);
            ServerPort = ConvertSafe.ToInt32Safe(SecurityFuncs.Decode(port, true));
            ServerClient = SecurityFuncs.Decode(client, true);
            ServerVersion = SecurityFuncs.Decode(version, true);
        }

        public bool IsValid()
        {
            if (!string.IsNullOrWhiteSpace(ServerName) &&
                !string.IsNullOrWhiteSpace(ServerClient) &&
                !string.IsNullOrWhiteSpace(ServerIP) &&
                !string.IsNullOrWhiteSpace(ServerPort.ToString()) &&
                !string.IsNullOrWhiteSpace(ServerVersion) &&
                Client.IsClientValid(ServerClient) &&
                Util.IsIPValid(ServerIP) &&
                (!ServerIP.Equals("localhost") || !ServerIP.Equals("127.0.0.1")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ServerName { get; set; }
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string ServerClient { get; set; }
        public string ServerVersion { get; set; }
    }
    #endregion
}

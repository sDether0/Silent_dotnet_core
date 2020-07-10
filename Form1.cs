using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows.Forms;

using Discord;
using Discord.Audio;
using Discord.Rest;
using Discord.WebSocket;


namespace Silent_dotnet_core
{
    public partial class Form1 : Form
    {
        bool cage=false;
        private string AccessToken;
        DiscordSocketClient dsClient = new DiscordSocketClient();
        public static string[] users;
        public static string[] guilds;
        private Timer t1 = new Timer();
        private IGuildUser UnMe, UnMx, UnBot, InMute;
        bool repeat = false;
        bool used = false;
        IGuildUser cagename;
        SocketVoiceChannel IntoCage;
        SocketVoiceChannel OutoCage;
        public delegate void InvokeDelegate(string str);
        public Form1()
        {
            InitializeComponent();
            t1.Interval = 60000;
            t1.Tick += T1_Tick;
            Text = "SILENT Repeat off";
            AccessToken = File.ReadAllText("token");
        }

        private async void T1_Tick(object sender, EventArgs e)
        {
            UnMe = dsClient.Guilds.First().Users.Where(x => x.Username.Equals("עמנואל")).First();
            UnMx = dsClient.Guilds.First().Users.Where(x => x.Username.Equals("Whiskas(Макс)")).First();
            UnBot = dsClient.Guilds.First().Users.Where(x => x.Username.Equals("Silent")).First();
        }

        

        public void InvokeMethod(string str)
        {
            textBox1.Text += Environment.NewLine + str;
        }

        public async Task UnMute()
        {
            if (UnMe == null)
            {
                UnMe = dsClient.Guilds.First().Users.Where(x => x.Username.Equals("עמנואל")).First();

            }
            if (UnMe.IsMuted || UnMe.IsDeafened)
            {
                _ = UnMe.ModifyAsync(x => x.Mute = false);
                _ = UnMe.ModifyAsync(x => x.Deaf = false);
            }

            if (UnMx == null)
            {
                UnMx = dsClient.Guilds.First().Users.Where(x => x.Username.Equals("Whiskas(Макс)")).First();

            }
            if (UnMx.IsMuted || UnMx.IsDeafened)
            {
                _ = UnMx.ModifyAsync(x => x.Mute = false);
                _ = UnMx.ModifyAsync(x => x.Deaf = false);
            }

            if (UnBot == null)
            {
                UnBot = dsClient.Guilds.First().Users.Where(x => x.Username.Equals("Silent")).First();

            }
            if (UnBot.IsMuted || UnBot.IsDeafened)
            {
                _ = UnBot.ModifyAsync(x => x.Mute = false);
                _ = UnBot.ModifyAsync(x => x.Deaf = false);
            }
        }

        

        private async void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text += Environment.NewLine + dsClient.LoginState;// + " " + dsClient.Status;
            dsClient.LoginAsync(TokenType.Bot, AccessToken).Wait();
            textBox1.Text += Environment.NewLine + dsClient.LoginState;// + " " + dsClient.Status;
            dsClient.MessageReceived += DsClient_MessageReceived;
            dsClient.StartAsync().Wait();
            await Task.Delay(2000);
            dsClient.UserVoiceStateUpdated += DsClient_UserVoiceStateUpdated;
            //dsClient.UserBanned += DsClient_UserBanned;           
            //await UnMute();
            t1.Start();
            //dsClient.UserUpdated += DsClient_UserUpdated;
            dsClient.UserVoiceStateUpdated+= DsClient_UserVoiceStateUpdated1;
            dsClient.Guilds.ToList().ForEach(x => comboBox2.Items.Add(x));
        }

        private Task DsClient_UserVoiceStateUpdated1(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {         
            return Task.CompletedTask;
        }

       

        private Task DsClient_UserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            if (cage && arg1.Username == cagename.Username && !IntoCage.Users.ToList().Any(x => x == arg1))
            {
                cagename.ModifyAsync(x => x.Channel = IntoCage);
            }
            if (arg2.IsMuted || arg3.IsMuted || arg2.IsDeafened || arg3.IsDeafened)
            {
                //UnMute();
            }
            if (!((IGuildUser)arg1).IsMuted && arg1==InMute)
            {
                InstMute();
            }
            return Task.CompletedTask;
        }
        private void InstMute()
        {
            if (!InMute.IsMuted)
            {
                InMute.ModifyAsync(x => x.Mute = true);
            }
        }
        private async Task DsClient_MessageReceived(SocketMessage arg)
        {
            if (arg.Content.StartsWith('%'))
            {
                arg.DeleteAsync();
                //arg.Channel.SendMessageAsync("It's working!");
                string[] str = arg.Content.Split(" ");
                if (str[0] == "%mute")
                {
                    var Auser = arg.Author;
                    if (dsClient.Guilds.First().Users.First(a => a.Id == Auser.Id).Roles.Any(x => x.Name == "Боженька" || x.Name == "Moder"))
                    {
                        var user = dsClient.Guilds.First().Users.Where(x => x.Username.Equals(str[1])).First();
                       
                        (user as IGuildUser).ModifyAsync(x => x.Mute = true);
                        await Task.Delay(8000);
                        (user as IGuildUser).ModifyAsync(x => x.Mute = false);
                    }
                    else
                    {
                        var mess = arg.Channel.SendMessageAsync("Недостаточно прав").Result;
                        await Task.Delay(2000);
                        _ = mess.DeleteAsync();
                        //SocketGuildUser scUser = dsClient.Guilds.First(x=>x.GetUser());
                    }
                }

                if (str[0] == "%list")
                {
                    guilds = dsClient.Guilds.Select(x => x.Name).ToArray();
                    users = dsClient.Guilds.First(x => x.Users.Any(t => t == arg.Author)).Users.Select(x => x.Username).ToArray();
                }

                if (true)
                {

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().ForEach(y => y.Users.ToList().ForEach(t => comboBox1.Items.Add(t.Username)));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox2.SelectedItem = null;
            comboBox1.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var chnl = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).Channels.ToList().Find(y => y.Name == "bot");
            RestUserMessage mess;
            if (repeat)
            {
                repeat = false;
                Text = Text.Replace(" on", " off");
                mess = ((ISocketMessageChannel)chnl).SendMessageAsync("Repeat off").Result;
            }
            else
            {
                repeat = true;
                Text = Text.Replace(" off", " on");
                mess = ((ISocketMessageChannel)chnl).SendMessageAsync("Repeat on").Result;
            }
            await Task.Delay(2000);
            _ = mess.DeleteAsync();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text += Environment.NewLine + dsClient.LoginState + " " + dsClient.Status;
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem!=null)
            {
                var voice = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name == "kitty 25/8");
                var audioClient = await voice.ConnectAsync();
                await SendAsync(audioClient, "dance.mp3");
            }
            else
            {
                MessageBox.Show("Please, choose a guild");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem!=null && comboBox4.SelectedItem!=null)
            {
                var voice = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name.Equals(comboBox3.Text));
                var nvoice = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name.Equals(comboBox4.Text));
                voice.Users.ToList().ForEach(x => x.ModifyAsync(y => y.Channel = nvoice));
            }
            else
            {
                MessageBox.Show("Please, select chanels to transfer.");
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void button7_Click(object sender, EventArgs e)
        {
            
            if (comboBox1.SelectedItem!=null)
            {
                //dsClient.Guilds.First().Users.Where(x => x.Username.Equals(comboBox1.SelectedItem)).First();
                var find = dsClient.Guilds.ToList().Find(x=>x.Name == comboBox2.Text).Users.Where(x => x.Username.Equals(comboBox1.SelectedItem)).First();
                //var temp = find.Roles.ToList();
                
                if (dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Any(x => x.Name == "Cage")) { }
                else
                {
                    dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).CreateVoiceChannelAsync("Cage").Wait();
                    IntoCage = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name == "Cage");
                    //IntoCage.ModifyAsync(x => x.UserLimit = 2);
                    IRole ALLrole = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).Roles.ToList().Find(x => x.Name == "@everyone");
                    IntoCage.AddPermissionOverwriteAsync(ALLrole, OverwritePermissions.DenyAll(IntoCage));
                }
                IntoCage = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name == "Cage");

                var voice = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name == "Cage");
                var audioClient = await voice.ConnectAsync();
                if (cagename != find)
                {
                    OutoCage = find.VoiceChannel;
                    find.ModifyAsync(x => x.Channel = IntoCage);
                    cagename = find;
                    cage = true;
                    button7.BackColor = System.Drawing.Color.Red;
                    await SendAsync(audioClient, "frendzone.mp3");
                }
                else
                {
                    cagename = null;
                    cage = false;
                    button7.BackColor = System.Drawing.Color.Lime;
                    find.ModifyAsync(x => x.Channel = OutoCage).Wait();
                    //find.AddRolesAsync(temp, (RequestOptions)comboBox2.SelectedItem);
                    voice.DeleteAsync();
                    audioClient.StopAsync();
                    IntoCage.DeleteAsync();                    
                }               
            }
            else
            {
                MessageBox.Show("Please, select a user.");
            }
            

        }

        private async void button8_Click(object sender, EventArgs e)
        {
            button8.Enabled = false;
            var find = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).Users.Where(x => x.Username.Equals(comboBox1.SelectedItem)).First();
            var outtemp = find.VoiceChannel;
            var temp1 = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name == "Waking-1");
            var temp2 = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name == "Waking-2");
            for (int i = 0; i < 5; i++)
            {

                find.ModifyAsync(x => x.Channel = temp1);
                await Task.Delay(1500);

                find.ModifyAsync(x => x.Channel = temp2);
            }
            await Task.Delay(800);
            //IntoCage = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name == "Chill");
            find.ModifyAsync(x => x.Channel = outtemp).Wait();
            button8.Enabled = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem!=null)
            {
                comboBox1.Items.Clear();
                dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().ForEach(y => y.Users.ToList().ForEach(t => comboBox1.Items.Add(t.Username)));
                dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().ForEach(y => comboBox3.Items.Add(y));
                dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().ForEach(y => comboBox4.Items.Add(y));
                dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().ForEach(y => comboBox5.Items.Add(y));
            }
            else
            {
                MessageBox.Show("Please, select a guild first.");
            }
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void button11_Click(object sender, EventArgs e)
        {
            try
            {
                string file = await YouTube.Download("https://www.youtube.com/watch?v=9v9OtinHt68");
            }
            catch(Exception x)
            {
                textBox1.BeginInvoke(new InvokeDelegate(InvokeMethod),x.Message);
            }

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "mp3 |*.mp3";
            open.InitialDirectory = Environment.CurrentDirectory;
            if (open.ShowDialog() == DialogResult.OK)
            {
                var voice = dsClient.Guilds.ToList().Find(x => x.Name == comboBox2.Text).VoiceChannels.ToList().Find(y => y.Name == comboBox5.Text);
                var audioClient = await voice.ConnectAsync();
                await SendAsync(audioClient, open.FileName);
            }
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem!=null)
            {
                var temp = InMute;
                InMute = dsClient.Guilds.ToList().Find(y=>y.Name==comboBox2.Text).Users.ToList().Find(x => x.Username.Equals(comboBox1.SelectedItem));
                dsClient.UserVoiceStateUpdated += DsClient_UserVoiceStateUpdated1;
                if (!used)
                {
                    InMute.ModifyAsync(x => x.Mute = true);
                    temp = InMute;
                    used = true;
                    button9.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    temp = InMute;
                    InMute = null;
                    temp.ModifyAsync(x => x.Mute = false);
                    used = false;
                    button9.BackColor = System.Drawing.Color.Green;
                }
            }
            else
            {
                MessageBox.Show("Please, select a user.");
            }
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel quiet -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
        private async Task SendAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            using (var ffmpeg = CreateStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally
                {
                    await discord.FlushAsync();
                    if (repeat)
                    {
                        await SendAsync(client, "dance.mp3");
                    }
                }
            }
        }
    }
}

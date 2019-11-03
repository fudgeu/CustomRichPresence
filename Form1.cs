using CustomRichPresence.JsonObjects;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomRichPresence
{
    public partial class Form1 : Form
    {

        //this code is messy af
        //oops

        public static ActivityManager activityManager;
        public static Discord.Discord discord;
        public static UserManager userManager;
        public static ApplicationManager applicationManager;
        public static Discord.User currentUser;
        public static String originalPreview;

        String ActivitiesFiller = "e.g. 'Competitive'";
        String DetailsFiller = "e.g. 'Playing Solo'";

        public Form1()
        {
            InitializeComponent();

            bool UserFound = false;
            bool TimeRanOut = false;

            string filePath = "test.html";
            originalPreview = File.ReadAllText(filePath);

            string clientID = Form2.cID;


            ////////////////////////
            // Enter Loading Mode //
            ////////////////////////
            
            discord = new Discord.Discord(Int64.Parse(clientID), (UInt64)Discord.CreateFlags.Default);

            // Managers
            activityManager = discord.GetActivityManager();
            userManager = discord.GetUserManager();
            applicationManager = discord.GetApplicationManager();

            // Logger
            discord.SetLogHook(Discord.LogLevel.Debug, (level, message) =>
            {
                Console.WriteLine("Log[{0}] {1}", level, message);
            });

            // Get Current User
            userManager.OnCurrentUserUpdate += () =>
            {
                currentUser = userManager.GetCurrentUser();
                Console.WriteLine("User {0} is {1}", currentUser.Id, currentUser.Username);
                Console.WriteLine("Test: {0}", currentUser.Avatar);
                UserFound = true;
            };

            // Run callbacks until all criteria met

            System.Timers.Timer aTimer = new System.Timers.Timer(5000);
            aTimer.Elapsed += (s, e) =>
            {
                TimeRanOut = true;
            };

            while (!UserFound && !TimeRanOut)
            {
                discord.RunCallbacks();
            }

            aTimer.Stop();
            aTimer.Dispose();
            Console.WriteLine("Loaded...");


            ////////////////
            // Form Setup //
            ////////////////

            // Activity setup
            textBox1.ForeColor = Color.Gray;
            textBox1.Text = ActivitiesFiller;

            // Details setup
            textBox2.ForeColor = Color.Gray;
            textBox2.Text = DetailsFiller;

            // Preview Setup (put into seperate function later)

            String file = originalPreview;
            file = file.Replace("CRP_PROFILE_IMAGE_PLACEHOLDER", "https://cdn.discordapp.com/avatars/" + currentUser.Id + "/" + currentUser.Avatar + ".png"); //profile img
            file = file.Replace("CRP_USERNAME_PLACEHOLDER", currentUser.Username); //username
            file = file.Replace("CRP_DISCRIM_PLACEHOLDER", "#" + currentUser.Discriminator); //discriminator
            file = file.Replace("CRP_ACTIVITYNAME_PLACEHOLDER", ""); //activity name
            file = file.Replace("CRP_DESCRIPTION_PLACEHOLDER", ""); //description
            file = file.Replace("CRP_LOBBY_PLACEHOLDER", " "); //lobby
            file = file.Replace("CRP_IMAGE_MARGIN_PLACEHOLDER", ""); // < v thumbnails
            file = file.Replace("CRP_IMAGE_PLACEHOLDER", "");
            file = file.Replace("CRP_TIME_PLACEHOLDER", "");

            webView1.NavigateToString(file);


            //load in last used preset (if available)
            
            if (Form2.LUPexists)
            {
                RPreset preset = JsonConvert.DeserializeObject<RPreset>(File.ReadAllText(Form2.LUPpath));

                textBox1.Text = preset.ActivityName;
                textBox2.Text = preset.Description;
                checkBox1.Checked = preset.InLobby;
                numericUpDown1.Value = preset.LobbyCount;
                numericUpDown2.Value = preset.LobbyMax;
                checkBox2.Checked = preset.Thumbnails;
                textBox3.Text = preset.LargeImageKeyword;
                textBox4.Text = preset.LargeImageText;
                textBox5.Text = preset.SmallImageKeyword;
                textBox6.Text = preset.SmallImageText;
                checkBox3.Checked = preset.TimeElapsedCheckbox;
                checkBox4.Checked = preset.TimeRemainingCheckbox;
                dateTimePicker1.Value = preset.TimeElapsed;
                dateTimePicker2.Value = preset.TimeRemaining;

                textBox1.ForeColor = Color.Black;
                textBox2.ForeColor = Color.Black;
                updatePreview();
            }
        }

        public void updatePreview()
        {
            String file;
            String activity;
            String description;

            if (textBox1.Text == ActivitiesFiller)
            {
                activity = "";
            } else
            {
                activity = textBox1.Text;
            }

            if (textBox2.Text == DetailsFiller)
            {
                description = "";
            }
            else
            {
                description = textBox2.Text;
            }

            file = String.Copy(originalPreview);
            file = file.Replace("CRP_PROFILE_IMAGE_PLACEHOLDER", "https://cdn.discordapp.com/avatars/" + currentUser.Id + "/" + currentUser.Avatar + ".png"); //profile img
            file = file.Replace("CRP_USERNAME_PLACEHOLDER", currentUser.Username); //username
            file = file.Replace("CRP_DISCRIM_PLACEHOLDER", "#" + currentUser.Discriminator); //discriminator
            file = file.Replace("CRP_ACTIVITYNAME_PLACEHOLDER", activity); //activity name
            file = file.Replace("CRP_DESCRIPTION_PLACEHOLDER", description); //description

            //if lobby is enabled...
            if (checkBox1.Checked)
            {
                file = file.Replace("CRP_LOBBY_PLACEHOLDER", "(" + numericUpDown1.Value + " of " + numericUpDown2.Value + ")");
            } else
            {
                file = file.Replace("CRP_LOBBY_PLACEHOLDER", " ");
            }

            //if images are disabled...
            if (checkBox2.Checked)
            {
                if (textBox3.Text.Length == 0)
                {
                    file = file.Replace("CRP_IMAGE_MARGIN_PLACEHOLDER", "");
                    file = file.Replace("CRP_IMAGE_PLACEHOLDER", "");
                }
                else
                {
                    file = file.Replace("CRP_IMAGE_MARGIN_PLACEHOLDER", "contentImagesUserPopout-UQ4CNe");
                    file = file.Replace("CRP_IMAGE_PLACEHOLDER", "<img alt=\"default icon\" src=\"https://discordapp.com/assets/322c936a8c8be1b803cd94861bdfa868.png\" class=\"assetsLargeImageUserPopout-2SRcNA assetsLargeImage-3JJo62 assetsLargeMaskUserPopout-AnndQt\"><img alt=\"Rogue - Level 100\" src=\"https://discordapp.com/assets/322c936a8c8be1b803cd94861bdfa868.png\" class=\"assetsSmallImageUserPopout-3fqJpC assetsSmallImage-1SwkaS\">");
                }

                if (textBox5.Text.Length == 0)
                {
                    file = file.Replace("<img alt=\"Rogue - Level 100\" src=\"https://discordapp.com/assets/322c936a8c8be1b803cd94861bdfa868.png\" class=\"assetsSmallImageUserPopout-3fqJpC assetsSmallImage-1SwkaS\">", "");
                    file = file.Replace(" assetsLargeMaskUserPopout-AnndQt", "");
                }
            } else
            {
                file = file.Replace("CRP_IMAGE_MARGIN_PLACEHOLDER", "");
                file = file.Replace("CRP_IMAGE_PLACEHOLDER", "");
            }

            //if timestamps are enabled...
            if (checkBox3.Checked || checkBox4.Checked)
            {
                String timeElapsed = "";
                DateTime dateTime = new DateTime();

                if (checkBox3.Checked) dateTime = dateTimePicker1.Value;
                if (checkBox4.Checked) dateTime = dateTimePicker2.Value;

                if (dateTime.Hour != 0)
                {
                    timeElapsed = dateTime.ToString("HH:");
                }

                timeElapsed = timeElapsed + dateTime.ToString("mm:ss");

                if (checkBox3.Checked) timeElapsed = timeElapsed + " elapsed";
                if (checkBox4.Checked) timeElapsed = timeElapsed + " left";

                file = file.Replace("CRP_TIME_PLACEHOLDER", "<div class=\"timestamp-y6iRau ellipsis-3FlOLA textRow-1sYCLh\">" + timeElapsed + "</div>");
            } else
            {
                file = file.Replace("CRP_TIME_PLACEHOLDER", "");
            }

            webView1.NavigateToString(file);
        }

        public void updatePreviewSub(object sender, EventArgs e)
        {
            updatePreview();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String curActivity = "";
            String curDetail = "";

            if (textBox1.Text != ActivitiesFiller) curActivity = textBox1.Text;
            if (textBox2.Text != DetailsFiller) curDetail = textBox2.Text;

            Discord.Activity activity = new Discord.Activity { };

            activity.State = curDetail;     // add basic details
            activity.Details = curActivity;

            if (checkBox1.Checked) // add lobbies
            {
                activity.Party.Id = "Party";
                activity.Party.Size.CurrentSize = (int)Math.Ceiling(numericUpDown1.Value);
                activity.Party.Size.MaxSize = (int)Math.Ceiling(numericUpDown2.Value);
            }

            if (checkBox2.Checked) // add thumbnails
            {
                activity.Assets.LargeImage = textBox3.Text;
                activity.Assets.LargeText = textBox4.Text;
                activity.Assets.SmallImage = textBox5.Text;
                activity.Assets.SmallText = textBox6.Text;
            }

            if (checkBox3.Checked) // add time elapsed
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int secondsSinceEpoch = (int)t.TotalSeconds;
                int timespan = 0;
                timespan = timespan + (dateTimePicker1.Value.Hour * 60 * 60) + (dateTimePicker1.Value.Minute * 60) + (dateTimePicker1.Value.Second);
                int timeElapsed = secondsSinceEpoch - timespan;

                activity.Timestamps.Start = timeElapsed;
            }

            if (checkBox4.Checked) // add time remaining
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int secondsSinceEpoch = (int)t.TotalSeconds;
                int timespan = 0;
                timespan = timespan + (dateTimePicker2.Value.Hour * 60 * 60) + (dateTimePicker2.Value.Minute * 60) + (dateTimePicker2.Value.Second);
                int timeElapsed = secondsSinceEpoch + timespan;

                activity.Timestamps.End = timeElapsed;
            }

            bool updatedActivity = false;

            activityManager.UpdateActivity(activity, (result) =>
            {
                if (result == Discord.Result.Ok)
                {
                    Console.WriteLine("Success!");
                    updatedActivity = true;
                }
                else
                {
                    Console.WriteLine("Failed");
                }
            });

            System.Timers.Timer aTimer = new System.Timers.Timer(5000);
            bool TimeRanOut = false;
            aTimer.Elapsed += (s, ev) =>
            {
                TimeRanOut = true;
            };

            while (!updatedActivity && !TimeRanOut)
            {
                discord.RunCallbacks();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            userManager.GetUser(112004717848068096, (Discord.Result result, ref Discord.User user) =>
            {
                if (result == Discord.Result.Ok)
                {
                    Console.WriteLine("user fetched: {0}", user.Username);
                }
                else
                {
                    Console.WriteLine("user fetch error: {0}", result);
                }
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        //Activities Textbox
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == ActivitiesFiller)
            {
                textBox1.ForeColor = Color.Black;
                textBox1.Text = "";
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = ActivitiesFiller;
                textBox1.ForeColor = Color.Gray;
            }
        }

        //Desc. Textbot
        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == DetailsFiller)
            {
                textBox2.ForeColor = Color.Black;
                textBox2.Text = "";
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = DetailsFiller;
                textBox2.ForeColor = Color.Gray;
            }
        }

        //Lobby
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                label3.ForeColor = Color.Black;
            } else
            {
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                label3.ForeColor = Color.Gray;
            }
            updatePreview();
        }

        //Images
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                label4.ForeColor = Color.Black;
                label5.ForeColor = Color.Black;
                label6.ForeColor = Color.Black;
                label7.ForeColor = Color.Black;
            }
            else
            {
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                label4.ForeColor = Color.Gray;
                label5.ForeColor = Color.Gray;
                label6.ForeColor = Color.Gray;
                label7.ForeColor = Color.Gray;
            }
            updatePreview();
        }

        //useless
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void onClose(object sender, EventArgs e)
        {
            //save RPreset
            RPreset preset = new RPreset();
            preset.ActivityName          = textBox1.Text;
            preset.Description           = textBox2.Text;
            preset.InLobby               = checkBox1.Checked;
            preset.LobbyCount            = (int)numericUpDown1.Value;
            preset.LobbyMax              = (int)numericUpDown2.Value;
            preset.Thumbnails            = checkBox2.Checked;
            preset.LargeImageKeyword     = textBox3.Text;
            preset.LargeImageText        = textBox4.Text;
            preset.SmallImageKeyword     = textBox5.Text;
            preset.SmallImageText        = textBox6.Text;
            preset.TimeElapsedCheckbox   = checkBox3.Checked;
            preset.TimeRemainingCheckbox = checkBox4.Checked;
            preset.TimeElapsed           = dateTimePicker1.Value;
            preset.TimeRemaining         = dateTimePicker2.Value;

            File.WriteAllText(Form2.LUPpath, JsonConvert.SerializeObject(preset));
            
            System.Windows.Forms.Application.Exit();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discordapp.com/developers/applications/" + Form2.cID + "/information");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            updatePreview();
        }

        private void timeElapsed_CheckChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                dateTimePicker1.Enabled = true;
                checkBox4.Checked = false;
                dateTimePicker2.Enabled = false;
            } else
            {
                dateTimePicker1.Enabled = false;
            }
            updatePreview(); // i would subscribe to the root events with these, but forms really doesn't like it when i sub to the same event twice and instead just overwrites it in the file, causing all sorts of weird things. so hence they're here
        }

        private void timeRemaining_CheckChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                dateTimePicker2.Enabled = true;
                checkBox3.Checked = false;
                dateTimePicker1.Enabled = false;
            }
            else
            {
                dateTimePicker2.Enabled = false;
            }
            updatePreview();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            checkBox1.Checked = false;
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            checkBox2.Checked = false;
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            dateTimePicker1.Value = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            dateTimePicker2.Value = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            updatePreview();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ThumbnailHelp form = new ThumbnailHelp();
            form.Show();
        }
    }
}
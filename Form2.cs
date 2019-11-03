using CustomRichPresence.JsonObjects;
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
    public partial class Form2 : Form
    {

        public static string cID = "0";
        public static string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CRP");
        public static string LUIDpath = Path.Combine(appData, "lastUsedID.json");
        public static string LUPpath = Path.Combine(appData, "lastUsedPreset.json");
        public static Boolean LUIDexists;
        public static Boolean LUPexists;
        public static string lastUsedID; //its never used as its int, so its a string

        public Form2()
        {
            InitializeComponent();
            //check to see if running for the first time (or if the appdata is just gone, somehow)
            LUIDexists = File.Exists(LUIDpath);
            Console.WriteLine("LUID exists => {0}", LUIDexists);

            LUPexists = File.Exists(LUPpath);
            Console.WriteLine("LUP exists => {0}", LUPexists);

            //create files if they dont exists
            if (!LUIDexists)
            {
                LastUsedID LUIDpreset = new LastUsedID("0");
                File.WriteAllText(LUIDpath, JsonConvert.SerializeObject(LUIDpreset));
            }

            //load in last used ID
            String lastUsedIDjson = File.ReadAllText(Path.Combine(appData, "lastUsedID.json"));
            lastUsedID = JsonConvert.DeserializeObject<LastUsedID>(lastUsedIDjson).ID;
            if (lastUsedID != "0")
            {
                textBox1.Text = lastUsedID.ToString();
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            cID = textBox1.Text;

            //save as last used ID
            LastUsedID LUIDnew = new LastUsedID(cID);
            File.WriteAllText(LUIDpath, JsonConvert.SerializeObject(LUIDnew));

            button1.Text = "Connecting...";
            button1.Enabled = false;

            Form1 form = new Form1();
            this.Hide();
            form.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateApp ca1 = new CreateApp();
            ca1.Show();
        }
    }
}

using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinAction
{
    public partial class Form1 : Form
    {
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport("user32")]
        public static extern void LockWorkStation();

        public RegistryKey rk;

        public string HourCompare { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnLogOff_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا مایل به خروج از حساب ویندوز هستید؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ExitWindowsEx(0, 0);

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(WindowsAction.Properties.Settings.Default.DateTime))
                {
                    dateTimePicker1.Value = DateTime.Now;
                    HourCompare = dateTimePicker1.Value.ToString("HH:mm:ss");
                }
                else
                {
                    var dateTime = WindowsAction.Properties.Settings.Default.DateTime;
                    dateTimePicker1.Value = DateTime.Parse(dateTime);
                    HourCompare = dateTimePicker1.Value.ToString("HH:mm:ss");
                }
                string action = WindowsAction.Properties.Settings.Default.Action;
                switch (action)
                {
                    case "radioButton1":
                        radioButton1.Checked = true;
                        break;
                    case "radioButton2":
                        radioButton2.Checked = true;
                        break;
                    case "radioButton3":
                        radioButton3.Checked = true;
                        break;
                    case "radioButton4":
                        radioButton4.Checked = true;
                        break;
                }
                rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                var startup = WindowsAction.Properties.Settings.Default.StartUp;
                if (startup)
                {
                    checkBox1.CheckState = CheckState.Checked;
                }
                else
                {
                    checkBox1.CheckState = CheckState.Unchecked;
                }

                var AfterStart = WindowsAction.Properties.Settings.Default.AfterStart;
                if (AfterStart)
                {
                    checkBox2.CheckState = CheckState.Checked;
                    var AfterStartHour = WindowsAction.Properties.Settings.Default.AfterStartHour;
                    var date = DateTime.Now.AddHours(double.Parse(AfterStartHour.ToString()));
                    HourCompare = date.ToString("HH:mm:ss");
                }
                else
                {
                    checkBox2.CheckState = CheckState.Unchecked;
                    HourCompare = dateTimePicker1.Value.ToString("HH:mm:ss");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnLock_Click(object sender, EventArgs e)
        {
            LockWorkStation();
        }

        private void BtnShutdown_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا مایل به خاموش کردن ویندوز هستید؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start("shutdown", "/s /t 0");
            }

        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا مایل به راه اندازی مجدد ویندوز هستید؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start("shutdown", "/r /t 0");
            }
        }

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            WindowsAction.Properties.Settings.Default.DateTime = dateTimePicker1.Value.ToString("HH:mm:ss");
            if (radioButton1.Checked == true)
            {
                WindowsAction.Properties.Settings.Default.Action = "radioButton1";
            }
            else if (radioButton2.Checked == true)
            {
                WindowsAction.Properties.Settings.Default.Action = "radioButton2";
            }
            else if (radioButton3.Checked == true)
            {
                WindowsAction.Properties.Settings.Default.Action = "radioButton3";
            }
            else if (radioButton4.Checked == true)
            {
                WindowsAction.Properties.Settings.Default.Action = "radioButton4";
            }



            WindowsAction.Properties.Settings.Default.Save();
            timer1.Enabled = true;
            timer1.Start();
            MessageBox.Show("تنظیم شد.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string nowTime = DateTime.Now.ToString("HH:mm:ss");
            if (HourCompare == nowTime)
            {
                timer1.Stop();
                timer1.Enabled = false;
                if (radioButton1.Checked == true)
                {
                    ExitWindowsEx(0, 0);
                }
                else if (radioButton2.Checked == true)
                {
                    LockWorkStation();
                }
                else if (radioButton3.Checked == true)
                {
                    Process.Start("shutdown", "/s /t 0");
                }
                else if (radioButton4.Checked == true)
                {
                    Process.Start("shutdown", "/r /t 0");
                }

            }
            // this.Text = (DateTime.Now >= dateTimePicker1.Value) + "";
            label1.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Email:Mahdihp.devsc@gmail.com" + "\n" + "Tel:09126516617");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                rk.SetValue(nameof(WindowsAction), Application.ExecutablePath);
            else
                rk.DeleteValue(nameof(WindowsAction), false);

            WindowsAction.Properties.Settings.Default.StartUp = checkBox1.Checked;
            WindowsAction.Properties.Settings.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                numericUpDown1.Enabled = true;
                dateTimePicker1.Enabled = false;

                var date = DateTime.Now.AddHours(double.Parse(numericUpDown1.Value.ToString()));
                HourCompare = date.ToString("HH:mm:ss");

            }
            else
            {
                HourCompare = dateTimePicker1.Value.ToString("HH:mm:ss");
                numericUpDown1.Enabled = false;
                dateTimePicker1.Enabled = true;

            }
           
            WindowsAction.Properties.Settings.Default.AfterStart = checkBox2.Checked;
            WindowsAction.Properties.Settings.Default.Save();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var date = DateTime.Now.AddHours(double.Parse(numericUpDown1.Value.ToString()));
            HourCompare = date.ToString("HH:mm:ss");
            WindowsAction.Properties.Settings.Default.AfterStartHour = (int)numericUpDown1.Value;
            WindowsAction.Properties.Settings.Default.Save();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            HourCompare = dateTimePicker1.Value.ToString("HH:mm:ss");
            WindowsAction.Properties.Settings.Default.DateTime = HourCompare;
            WindowsAction.Properties.Settings.Default.Save();
        }
    }
}
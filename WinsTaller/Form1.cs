using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace WinsTaller
{
    public partial class WinInstaller : Form
    {
        
        public WinInstaller()
        {
            InitializeComponent();
            listBox1.Visible = false;
            btn_ins.Visible = false;
            progressBar.Visible=false;
            
        }
        string route;
        string disk;
        int number_os;
        private void btn_abrir_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                DialogResult res = MessageBox.Show("Please Select disk to install", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                
            }
            else
            {
                if (Archive.ShowDialog() == DialogResult.OK)
                {

                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.WorkingDirectory = route;
                    ps.WindowStyle = ProcessWindowStyle.Hidden;

                    ps.FileName = route + @"/imagex.exe";
                    ps.UseShellExecute = false;
                    ps.RedirectStandardOutput = true;
                    string archieve = Archive.FileName;

                    ps.Arguments = @"/info " + archieve;
                    Process process = Process.Start(ps);
                    listBox1.Visible = true;

                    StreamReader reader = process.StandardOutput;
                    List<string> list = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        list.Add(reader.ReadLine());
                    }
                    int i = 0;
                    foreach (string line in list)
                    {
                        if (line.Contains("<NAME>"))
                        {


                            listBox1.Items.Add(list[i].Substring(10).Remove(list[i].Substring(10).IndexOf('<')));

                        }
                        i++;
                    }

                    btn_ins.Visible = true;

                }
              
            }
            
            
            
            
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.Items.Clear();
            }
            if (Environment.Is64BitProcess == false)
            {
                route = Application.StartupPath + @"\amd64\DISM";
            }
            else
            {
                route = Application.StartupPath + @"\x86\DISM";
            }
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                comboBox1.Items.Add(d.Name);
            }
                
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             number_os = listBox1.SelectedIndex+1;
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             disk = comboBox1.Text;
            
        }

        private async void btn_ins_Click(object sender, EventArgs e)
        {
            if (number_os == 0)
            {
                DialogResult res = MessageBox.Show("Please Select OS to install", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            }
            else
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.WorkingDirectory = route;
                ps.WindowStyle = ProcessWindowStyle.Hidden;

                ps.FileName = route + @"/imagex.exe";
                ps.UseShellExecute = true;
                
                ps.Verb = "runas";

                ps.Arguments = @"/apply "+Archive.FileName +" "+number_os+ " "+disk;
                btn_abrir.Enabled = false;
                btn_ins.Enabled = false;
                comboBox1.Enabled = false;
                listBox1.Enabled = false;
                progressBar.Visible = true;
                await Task.Run(() =>
                {
                    Process process = Process.Start(ps);
                    int algo = 0;
                    
                    while (!process.HasExited)
                    {
                        // Actualizamos el progressBar de forma segura en el hilo de UI
                        Invoke(new Action(() =>
                        {
                            progressBar.Value = algo;
                        }));

                        algo++;
                        Thread.Sleep(100); // Simula la actualización del progreso
                        if (algo == 100)
                        {
                            algo = 0;
                        }
                    }
                    if (process.HasExited == true)
                    {
                        DialogResult res = MessageBox.Show("Finish", "Warning", MessageBoxButtons.OKCancel);

                    }
                });
                progressBar.Visible = false;
                btn_abrir.Enabled = true;
                btn_ins.Enabled = true;
                comboBox1.Enabled = true;
                listBox1.Enabled = true;

            }

        }

        private void Archive_FileOk(object sender, CancelEventArgs e)
        {

        }

        
    }
}

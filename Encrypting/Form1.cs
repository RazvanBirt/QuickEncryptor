using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;
namespace Encrypting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string path;
        static string[] filePaths;
        static string projectDirectory = Environment.CurrentDirectory;
        static string unviewableDirectory = projectDirectory + @"\Unviewable\UnviewablePlus.exe";
        static string templateDirectory = projectDirectory + @"\Unviewable\TEMPLATE.XLSB";
        static string templateDirectoryOriginal = projectDirectory + @"\TEMPLATE.XLSB";
        string nameFile = "";

        Process p = new Process();
        
        string[] currentDirectories;
        string[] filePaths_xlsm;
        string[] filePaths_xlsb;

        static DateTime currentT = DateTime.Now;
        static string currentTime = currentT.ToString("ddMMyy-HHmm");
        
        static string logFilePath = @".\log.txt";

        InputSimulator sim = new InputSimulator();
        StreamWriter sw = new StreamWriter(logFilePath, true);


        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = @"S:\PARIS-VAT\VATSystems_PRODUCTION\PROCESS_ACTIVITY\CLIENTS";       
            // open folder browser to choose path for encryption 
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
            path = folderBrowserDialog1.SelectedPath;
            textBoxPath.Text = path;
            }
        
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            //create text file for logs
            path = textBoxPath.Text;
            // get main directories for then encrypt files in side at a time
            currentDirectories = Directory.GetDirectories(path);

            // open and focus on unviewable window         
            p = Process.Start(unviewableDirectory);

            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);

            System.Threading.Thread.Sleep(3000);

            copyFile(templateDirectoryOriginal, Directory.GetCurrentDirectory() + @"\Unviewable");
            textBoxConsole.Text = textBoxConsole.Text + System.Environment.NewLine + "----------------------------------------------------------";
            sw.WriteLine("----------------------------------------------------------");

            // starting process of encryption
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait("^o");
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait(templateDirectory);
            System.Threading.Thread.Sleep(3000);

            // run first cicle
            SendKeys.SendWait("{ENTER}");
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait("{TAB}");
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait("{UP}");
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait("{TAB}");
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait(" ");
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait("{TAB}{TAB}{TAB}{TAB}{TAB}");
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait("{ENTER}");
            System.Threading.Thread.Sleep(3000);
            SendKeys.SendWait("{ENTER}");

            System.Threading.Thread.Sleep(3000);

            int x = int.Parse(textBoxArrayStart.Text);

            for (int i = x; i < currentDirectories.Length; i++)
            {
                label1.Text = i.ToString();
                encrypt_files(currentDirectories[i]);              
            }
            System.Threading.Thread.Sleep(500);
            sw.Close();
            p.Kill();
        }

        private void encrypt_files(string subPath)
        {
            // find both excel extensions
            filePaths_xlsm = Directory.GetFiles(subPath, "*.xlsm", SearchOption.AllDirectories);
            filePaths_xlsb = Directory.GetFiles(subPath, "*.xlsb", SearchOption.AllDirectories);
            
            // combine both arrays into one for then have a complete one to start encrypting on by one
            Array.Resize<string>(ref filePaths, filePaths_xlsm.Length + filePaths_xlsb.Length);
            filePaths_xlsm.CopyTo(filePaths, 0);
            filePaths_xlsb.CopyTo(filePaths, filePaths_xlsm.Length);          

            if (filePaths.Length != 0)
            {
                textBoxConsole.Text = textBoxConsole.Text + System.Environment.NewLine + subPath + ": " + filePaths.Length + " files to encrypt";
                sw.WriteLine(subPath + ": " + filePaths.Length + " files to encrypt");
            }
            else
            {
                textBoxConsole.Text = textBoxConsole.Text + System.Environment.NewLine + subPath + ": 0 files to encrypt";
                sw.WriteLine(subPath + ": 0 files to encrypt");
            }

            int nFiles = 0;
            if (filePaths.Length != 0)
            {
                System.Threading.Thread.Sleep(1500);
                // run second and final cicle, because its different while unviewable its still open saves some stuff done to it
                for (int i = 0; i < filePaths.Length; i++)
                {                 
                    nFiles++;
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("^o");
                    System.Threading.Thread.Sleep(1000);
                    // new way to send text to the textbox of unviewable openFileDialog
                    sim.Keyboard.TextEntry(filePaths[i]);

                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("{ENTER}");
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("{TAB}{TAB}");
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait(" ");
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("{TAB}{TAB}{TAB}{TAB}{TAB}");
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("{ENTER}");
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("{ENTER}");

                    currentT = DateTime.Now;
                    currentTime = currentT.ToString("ddMMyy-HHmm");
                    textBoxConsole.Text = textBoxConsole.Text + System.Environment.NewLine + currentT + " - " + filePaths[i] + " ||Encryption Done";               
                    sw.WriteLine(currentT + " - " + filePaths[i] + " ||Encryption Done");
                }
                textBoxConsole.Text = textBoxConsole.Text + System.Environment.NewLine + nFiles + " files were Encrypted";
                sw.WriteLine(nFiles + " files were Encrypted");
            }
        }

        private void copyFile(string FOri, string FDest)
        {
            if (File.Exists(FOri) == false)
                MessageBox.Show("Error file does not exist");
            else
            {
                try
                {
                    nameFile = Path.GetFileName(FOri);
                    FDest = FDest + "\\" + nameFile;
                    File.Copy(FOri, FDest,true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void readFile(string file, int numberOfLines)
        {

            StreamReader readFile = new StreamReader(file);
            string[] arrayOfLines = new string[numberOfLines];

            for( int i = 1; i < numberOfLines; i++)
            {
                arrayOfLines[i] = readFile.ReadLine();
                //textBoxConsole.Text = textBoxConsole.Text + System.Environment.NewLine + arrayOfLines[i];
            }      
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}

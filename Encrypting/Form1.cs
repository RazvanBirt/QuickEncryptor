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
        static string logFilePath = @".\log.txt"; 
        string nameFile = "";

        Process p = new Process();
        
        string[] currentDirectories;
        string[] filePaths_xlsm;
        string[] filePaths_xlsb;

        static DateTime currentT = DateTime.Now;
        static string currentTime = currentT.ToString("ddMMyy-HHmm"); 

        InputSimulator sim = new InputSimulator();
        StreamWriter sw = new StreamWriter(logFilePath, true);

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        private void buttonSearch_Click(object sender, EventArgs e)
        { 
            // open folder browser to choose path for encryption 
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {   
                 path = folderBrowserDialog1.SelectedPath;                  
 
                type_console(path);
            }
            currentDirectories = Directory.GetDirectories(path);

            textBoxPath.Text = path;

            for (int i = 0; i < currentDirectories.Length; i++)
            {
                // find both excel extensions
                filePaths_xlsm = Directory.GetFiles(currentDirectories[i], "*.xlsm", SearchOption.AllDirectories);
                filePaths_xlsb = Directory.GetFiles(currentDirectories[i], "*.xlsb", SearchOption.AllDirectories);

                // combine both arrays into one, to have a complete one to start encrypting on by one
                Array.Resize<string>(ref filePaths, filePaths_xlsm.Length + filePaths_xlsb.Length);
                //             copy to array filepaths starting index 0
                filePaths_xlsm.CopyTo(filePaths, 0);
                //             copy to array filepaths starting index X = filePaths_xlsm.Length
                filePaths_xlsb.CopyTo(filePaths, filePaths_xlsm.Length);

                type_console(currentDirectories[i]);
                for (int j = 0; j < filePaths.Length;j++)
                {
                    type_console("          " + filePaths[j]);
                }
            }
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            if (textBoxPath.Text != "")
            {
                // open and focus on unviewable window         
                p = Process.Start(unviewableDirectory);
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);

                System.Threading.Thread.Sleep(3000);
                copyFile(templateDirectoryOriginal, Directory.GetCurrentDirectory() + @"\Unviewable");
                type_console("----------------------------------------------------------");

                // starting process of encryption
                System.Threading.Thread.Sleep(3000);
                SendKeys.SendWait("^o");
                System.Threading.Thread.Sleep(3000);
                SendKeys.SendWait(templateDirectory);
                System.Threading.Thread.Sleep(3000);

                // run first cicle to get the settings all OK
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                System.Threading.Thread.Sleep(3000);
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                System.Threading.Thread.Sleep(3000);
                sim.Keyboard.KeyPress(VirtualKeyCode.UP);
                System.Threading.Thread.Sleep(3000);
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                System.Threading.Thread.Sleep(3000);
                sim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
                System.Threading.Thread.Sleep(3000);
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                System.Threading.Thread.Sleep(3000);
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                System.Threading.Thread.Sleep(3000);
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                System.Threading.Thread.Sleep(3000);

                // in case of necessity or error to start from where it stopped 
                // to input into the textBoxArrayStart the index of the array where it stopped
                int x = int.Parse(textBoxArrayStart.Text);

                for (int i = x; i < currentDirectories.Length; i++)
                {
                    label2.Text = "Starting point: " + i.ToString();
                    encrypt_files(currentDirectories[i]);
                }
                System.Threading.Thread.Sleep(500);
                sw.Close();
                p.Kill();
            }
            else
            {
                MessageBox.Show("Error no path has been found");
            }
            
           
        }

        private void encrypt_files(string subPath)
        {

            // verify if subPath has files to start encrypting
            if (filePaths.Length != 0)
            {
                type_console(subPath + ": " + filePaths.Length + " files to encrypt");
            }
            else
            {
                type_console(subPath + ": 0 files to encrypt");
            }

            // 2nd cicle with all the settings OK
            // actual process of encrypting going one by one per subPath/currentDirectories
            int nFiles = 0;
            // nFiles to get number of files that were encrypter to later get presented on the console and logs
            if (filePaths.Length != 0)
            {
                System.Threading.Thread.Sleep(1500);
                for (int i = 0; i < filePaths.Length; i++)
                {                 
                    nFiles++;
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("^o");
                    System.Threading.Thread.Sleep(1000);              
                    sim.Keyboard.TextEntry(filePaths[i]);
                    System.Threading.Thread.Sleep(1000);
                    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                    System.Threading.Thread.Sleep(1000);
                    sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    System.Threading.Thread.Sleep(1000);
                    sim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
                    System.Threading.Thread.Sleep(1000);
                    sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    System.Threading.Thread.Sleep(1000);
                    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                    System.Threading.Thread.Sleep(1000);
                    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

                    currentT = DateTime.Now;
                    currentTime = currentT.ToString("ddMMyy-HHmm");

                    type_console(currentT + " - " + filePaths[i] + " ||Encryption Done");               
                }
                type_console(nFiles + " files were Encrypted");
            }
        }

        // function to type on the console and on the logs
        // made to be easier on the eyes
        private void type_console(string TextToType)
        {
            textBoxConsole.Focus();
            textBoxConsole.Text = textBoxConsole.Text + System.Environment.NewLine + TextToType;
            sw.WriteLine(TextToType);
            //move the caret to the end of the text
            textBoxConsole.SelectionStart = textBoxConsole.TextLength;
            //scroll to the caret
            textBoxConsole.ScrollToCaret();
        }

        // function copyFile to copy the template file from templateDirectoryOriginal to templateDirectory so there could be always a decrypted version of the template
        // so the encrypting process can start correctly
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

        private void TextBoxConsole_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

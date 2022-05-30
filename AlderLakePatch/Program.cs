using System;
using System.IO;
using System.Windows.Forms;

namespace AlderLakePatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var file = new OpenFileDialog();
            MessageBox.Show("Locate the executable you wish to patch.");
            file.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            var result = file.ShowDialog();
            if (result == DialogResult.OK)
            {
                var intelPath = Path.Combine(Path.GetDirectoryName(file.FileName), "IntelFix.dll");
                var backupName = Path.GetFileNameWithoutExtension(file.FileName) + ".bak";
                var backupPath = Path.Combine(Path.GetDirectoryName(file.FileName), backupName);
                var madeBackup = false;
                try
                {
                    File.Copy(file.FileName, backupPath, true);
                    madeBackup = true;
                }
                catch (Exception e)
                {
                    madeBackup = false;
                    var proceedResult = MessageBox.Show("Couldn't create a backup. Proceed anyway?", "Warning", MessageBoxButtons.YesNo);
                    if (proceedResult != DialogResult.Yes)
                        return;
                }
                try
                {
                    var peFile = new PeNet.PeFile(file.FileName);
                    peFile.AddImport("IntelFix.dll", "_DllMain@12");
                    File.WriteAllBytes(file.FileName, peFile.RawFile.ToArray());
                    File.Copy("IntelFix.dll", intelPath, true);
                    if (madeBackup)
                        MessageBox.Show("Executable patched successfully! Created a backup as " + backupName);
                    else
                        MessageBox.Show("Executable patched successfully.");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to patch executable!"+Environment.NewLine+"Error: "+Environment.NewLine+e.ToString());
                }
            }
        }
    }
}

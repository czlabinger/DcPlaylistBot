using System.IO;
using System.Windows.Forms;

namespace DcPlaylistPlugin.Util;

public class FileSelector {
    internal static string SelectTextOrNoExtensionFile() {

        using (
            OpenFileDialog openFileDialog = new OpenFileDialog
               {
                   Title = "Select a text file or file without extension",
                   Filter = "Text files (*.txt)|*.txt|Files without extension|*.",
                   FilterIndex = 1,
                   CheckFileExists = true,
                   CheckPathExists = true,
                   RestoreDirectory = true
               })
        {
            
        
            DialogResult result = openFileDialog.ShowDialog();

            if (result != DialogResult.OK)
                return "";
            
            string selectedFile = openFileDialog.FileName;
        
            string extension = Path.GetExtension(selectedFile);
            if (string.IsNullOrEmpty(extension) || extension.ToLower() == ".txt") {
                return File.ReadAllText(selectedFile);
            }
                
            
            
            MessageBox.Show("Please select a .txt file or a file without extension.", 
                "Invalid File Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return "";
            
        }
    }
}
using System;
using System.IO;
using System.IO.Compression;

namespace DcPlaylistPlugin.Util;

public class ZipExtractor {
    public static void ExtractZipFromByteArray(byte[] zipData, string destinationFolderPath) {

        if(File.Exists(destinationFolderPath)) File.Delete(destinationFolderPath);

        MemoryStream memoryStream = new MemoryStream(zipData);

        string tempFilePath = Path.GetTempFileName();
        using (FileStream tempFileStream = File.Create(tempFilePath)) {
            memoryStream.CopyTo(tempFileStream);
        }

        try {
            ZipFile.ExtractToDirectory(tempFilePath, destinationFolderPath);
        } catch (Exception ex) {
            Plugin.Log.Error($"Failed to extract zip file: {ex.Message}");
        } finally {
            File.Delete(tempFilePath);
        }
    }
}
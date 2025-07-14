using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DcPlaylistPlugin.Util;

//TODO: Embed exe in plugin to autostart bot
public class BotHelper {
    public static void StartBotProcess() {
        Assembly assembly = Assembly.GetExecutingAssembly();
        
        string resourceName = "DcPlaylistPlugin.PlaylistBot.exe";
        
        using (Stream resource = assembly.GetManifestResourceStream(resourceName))
        {
            if (resource == null)
                throw new InvalidOperationException("Resource not found.");
            string tempExePath = Path.Combine(Path.GetTempPath(), "PlaylistBot.exe");
            using (FileStream file = new FileStream(tempExePath, FileMode.Create, FileAccess.Write))
                resource.CopyTo(file);
            Process.Start(tempExePath);
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DcPlaylistPlugin.Util;

//TODO: Embed exe in plugin to autostart bot
public class BotHelper {
    public static void StartBotProcess() {

        string dirPath = @$"{Environment.CurrentDirectory}\UserData\DcPlaylistBot\";
        string exePath = @$"{dirPath}\PlaylistBot.exe";

        Process.Start(exePath);
        Plugin.Log.Info("Starting bot...");
    }
}
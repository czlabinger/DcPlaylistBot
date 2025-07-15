using System;
using System.Diagnostics;

namespace DcPlaylistPlugin.Util;

public class BotHelper {
    public static void StartBotProcess() {
        var dirPath = @$"{Environment.CurrentDirectory}\UserData\DcPlaylistBot\";
        var exePath = @$"{dirPath}\PlaylistBot.exe";

        Process.Start(exePath);
        Plugin.Log.Info("Starting bot...");
    }
}
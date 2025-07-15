using System;
using System.Diagnostics;

namespace DcPlaylistPlugin.Util;

public class BotHelper {
    public static void StartBotProcess() {
        var exePath = $@"{Environment.CurrentDirectory}\UserData\DcPlaylistBot\PlaylistBot.exe";

        Process.Start(exePath);
        Plugin.Log.Info("Starting bot...");
    }
}
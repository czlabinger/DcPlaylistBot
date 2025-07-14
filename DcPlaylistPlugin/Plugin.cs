using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using DcPlaylistPlugin.Util;
using IPA;
using IPA.Loader;
using IpaLogger = IPA.Logging.Logger;

namespace DcPlaylistPlugin;

[Plugin(RuntimeOptions.DynamicInit)]
internal class Plugin {
    internal static IpaLogger Log { get; private set; } = null!;

    [Init]
    public Plugin(IpaLogger ipaLogger, PluginMetadata pluginMetadata) {
        Log = ipaLogger;
        Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} initialized.");
    }

    [OnStart]
    public void OnApplicationStart() {
        Log.Debug("OnApplicationStart");
        StartListening();

        //BotHelper.StartBotProcess();

    }

    [OnExit]
    public void OnApplicationQuit() {
        Log.Debug("OnApplicationQuit");
        try {
            using (NamedPipeClientStream client = new NamedPipeClientStream(".", "DcPlaylistBotShutdown",
                       PipeDirection.InOut, PipeOptions.Asynchronous)) {
                client.Connect();

                using (var writer = new StreamWriter(client, Encoding.ASCII, 1024, true) { AutoFlush = true }) {
                    writer.WriteLine("Quit");
                }
            }
        } catch (IOException) { }
    }

    private static void StartListening() {
        NamedPipeServerStream pipeServer = new NamedPipeServerStream(
            "DcPlaylistPlugin", 
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances, 
            PipeTransmissionMode.Byte, 
            PipeOptions.Asynchronous);

        pipeServer.BeginWaitForConnection(asyncResult => {
            try {
                StartListening();

                CommandHandler.HandleCommand(asyncResult);
            } catch (Exception ex) {
                Log.Error($"Error in pipe connection: {ex}");
            }
        }, pipeServer);
    }
}
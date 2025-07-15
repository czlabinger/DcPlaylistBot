using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage.Util;
using DcPlaylistPlugin.Commands;
using DcPlaylistPlugin.Util;
using IPA;
using IPA.Loader;
using SongCore;
using UnityEngine.SceneManagement;
using IpaLogger = IPA.Logging.Logger;

namespace DcPlaylistPlugin;

[Plugin(RuntimeOptions.DynamicInit)]
internal class Plugin {
    internal static IpaLogger Log { get; private set; } = null!;
    private static UI.SettingsFlowCoordinator? _settingsFlowCoordinator;
    private static MenuButton? _menuButton;

    [Init]
    public Plugin(IpaLogger ipaLogger, PluginMetadata pluginMetadata) {
        Log = ipaLogger;
        Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} initialized.");
    }

    [OnStart]
    public void OnApplicationStart() {
        Log.Debug("OnApplicationStart");
        StartListening();
        SetupSceneChangeHook();
        BotHelper.StartBotProcess();

        MainMenuAwaiter.MainMenuInitializing += () => {
            _settingsFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<UI.SettingsFlowCoordinator>();
            _menuButton = new MenuButton("Dc Playlist Plugin", () => {
                BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_settingsFlowCoordinator);
            });
            MenuButtons.Instance.RegisterButton(_menuButton);
        };
    }

    [OnExit]
    public void OnApplicationQuit() {
        Log.Debug("OnApplicationQuit");
        SendToBot("Quit");
    }

    private static void StartListening() {
        var pipeServer = new NamedPipeServerStream(
            "DcPlaylistPlugin",
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous);

        pipeServer.BeginWaitForConnection(asyncResult => {
            try {
                StartListening();

                CommandHandler.HandleCommand(asyncResult)
                    .ContinueWith(task => {
                        if (task.Exception != null)
                            Log.Error($"Error in pipe connection: {task.Exception}");
                    }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex) {
                Log.Error($"Error in pipe connection: {ex}");
            }
        }, pipeServer);
    }

    private static void SetupSceneChangeHook() {
        SceneManager.activeSceneChanged += (arg0, scene) => {
            if (Commands.Commands.ReloadNeeded && SceneManager.GetActiveScene().name == "MainMenu") {
                Loader.Instance.RefreshSongs();
                Commands.Commands.ReloadNeeded = false;
            }
        };
    }

    internal static void SendToBot(string message) {
        try {
            using (var client = new NamedPipeClientStream(".", "DcPlaylistBot",
                       PipeDirection.InOut, PipeOptions.Asynchronous)) {
                client.Connect();

                using (var writer = new StreamWriter(client, Encoding.ASCII, 1024, true) { AutoFlush = true }) {
                    writer.WriteLine(message);
                }
            }
        }
        catch (IOException) { }
    }

    internal static async Task SendToBotAsync(string message) {
        try {
            using (var client = new NamedPipeClientStream(".", "DcPlaylistBot",
                       PipeDirection.InOut, PipeOptions.Asynchronous)) {
                await client.ConnectAsync();

                using (var writer = new StreamWriter(client, Encoding.ASCII, 1024, true) { AutoFlush = true }) {
                    await writer.WriteLineAsync(message);
                }
            }
        }
        catch (IOException) { }
    }
}
using System.IO.Pipes;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using PlaylistBot.Commands;

public class Program {
    internal static readonly DiscordSocketClient Client = new();
    private static readonly CommandHandler CommandHandler = new();

    private static readonly NamedPipeServerStream Server = new(
        "DcPlaylistBot",
        PipeDirection.InOut,
        NamedPipeServerStream.MaxAllowedServerInstances,
        PipeTransmissionMode.Byte,
        PipeOptions.Asynchronous);

    public static async Task Main() {
        Client.Log += Log;

        await StartBot();
        Client.Ready += async () => await CommandHandler.InitializeAsync();

        Server.BeginWaitForConnection(async (asyncResult) => {
            using (StreamReader reader = new StreamReader(Server)) {
                string line = await reader.ReadLineAsync() ?? "";
                if (line.Equals("Quit")) {
                    await Quit();
                }
            }
        }, Server);


        await Task.Delay(-1);
    }

    private static Task Log(LogMessage msg) {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private static async Task StartBot() {
        string token = "";
        try {
            token = await File.ReadAllTextAsync($@"{Environment.CurrentDirectory}\UserData\DcPlaylistBot\token.txt");
        }
        catch (Exception) {
            Environment.Exit(1);
        }

        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
    }

    private static async Task Quit() {
        Console.WriteLine("Shutting down now...");
        await Client.LogoutAsync();
        await Client.DisposeAsync();
        Server.Disconnect();
        await Server.DisposeAsync();
        Environment.Exit(0);
    }
}
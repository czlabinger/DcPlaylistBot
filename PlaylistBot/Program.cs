using System.IO.Pipes;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PlaylistBot.Commands;
using PlaylistBot.Util;

public class Program {
    
    internal static readonly DiscordSocketClient Client = new ();
    private static readonly CommandHandler CommandHandler = new ();
    private static readonly NamedPipeServerStream ShutdownServer = new (
        "DcPlaylistBotShutdown", 
        PipeDirection.InOut,
        NamedPipeServerStream.MaxAllowedServerInstances, 
        PipeTransmissionMode.Byte, 
        PipeOptions.Asynchronous);
    
    public static async Task Main() {
        
        Client.Log += Log;

        await StartBot();
        Client.Ready += async () => await CommandHandler.InitializeAsync();

        ShutdownServer.BeginWaitForConnection( async (asyncResult) =>  {
            
            Console.WriteLine("Shutting down now...");
            await Client.LogoutAsync();
            await Client.DisposeAsync();
            ShutdownServer.Disconnect();
            await ShutdownServer.DisposeAsync();
            Environment.Exit(0);

        }, ShutdownServer);


        await Task.Delay(-1);
    }

    private static Task Log(LogMessage msg) {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    //TODO: Better solution for token
    private static async Task StartBot() {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "PlaylistBot.token.txt";
        string token;
        
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new InvalidOperationException($"Resource '{resourceName}' not found.");
            }
            using (StreamReader reader = new StreamReader(stream))
            {
                token = reader.ReadToEndAsync().Result;
                
            }
        }
        
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
    }
}
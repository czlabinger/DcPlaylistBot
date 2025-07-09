using System.Reflection;
using Discord;
using Discord.WebSocket;

public class Program {
    
    private static DiscordSocketClient? _client;
    
    public static async Task Main() {
        _client = new DiscordSocketClient();
        
        _client.Log += Log;

        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "PlaylistBot.token.txt";
        string token = "";
        
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new InvalidOperationException($"Resource '{resourceName}' not found.");
            }
            using (StreamReader reader = new StreamReader(stream))
            {
                token = reader.ReadToEnd();
                
            }
        }
        
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        
        AppDomain.CurrentDomain.ProcessExit += async (sender, eventArgs) =>
        {
            Console.WriteLine("Process is exiting!");
            await _client.LogoutAsync();
            await _client.StopAsync();
        };
        
        await Task.Delay(-1);
    }

    private static Task Log(LogMessage msg) {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
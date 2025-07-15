using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DcPlaylistPlugin.Commands;

public class CommandHandler {
    
    private static readonly Dictionary<string, Func<string, Task<string>>> CommandHandlers = new()
    {
        { "add", args => Commands.Add(args) },
    };
    
    public static async Task HandleCommand(IAsyncResult result) {
        using (NamedPipeServerStream pipeServer = (NamedPipeServerStream)result.AsyncState)
        using (var reader = new StreamReader(pipeServer, Encoding.ASCII, false, 1024, true))
        using (var writer = new StreamWriter(pipeServer, Encoding.ASCII, 1024, true) { AutoFlush = true }) {
            
            string command = await reader.ReadLineAsync();

            string response = CommandHandlers.TryGetValue(command[..command.IndexOf(' ')], out var handler)
                ? await handler(command)
                : $"Unknown command: {command}";

            
            await writer.WriteLineAsync(response);
            await writer.FlushAsync();
        }
    }
}
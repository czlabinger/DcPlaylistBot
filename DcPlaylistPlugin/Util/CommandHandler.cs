using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DcPlaylistPlugin.Util;

public class CommandHandler {
    public static async void HandleCommand(IAsyncResult result) {
        using (NamedPipeServerStream pipeServer = (NamedPipeServerStream)result.AsyncState)
        using (var reader = new StreamReader(pipeServer, Encoding.ASCII, false, 1024, true))
        using (var writer = new StreamWriter(pipeServer, Encoding.ASCII, 1024, true) { AutoFlush = true }) {
            string message = await reader.ReadLineAsync();
            Plugin.Log.Error("Got message: " + message);

            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
            Plugin.Log.Error("Wrote message to stream: " + message);
        }
    }
}
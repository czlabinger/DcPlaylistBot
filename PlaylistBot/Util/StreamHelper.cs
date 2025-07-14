using System.IO.Pipes;

namespace PlaylistBot.Util;

public class StreamHelper {
    public event Action<string> MessageReceived;

    internal async Task ReadFromPipeAsync(NamedPipeServerStream pipe)
    {
        using (var reader = new StreamReader(pipe))
        {
            while (pipe.IsConnected)
            {
                string message = await reader.ReadLineAsync();
                if (message == null) break;
                MessageReceived?.Invoke(message);
            }
        }
    }

}
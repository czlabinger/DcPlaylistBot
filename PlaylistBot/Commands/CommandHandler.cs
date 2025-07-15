using System.IO.Pipes;
using System.Reflection;
using System.Text;
using Discord.Interactions;

namespace PlaylistBot.Commands;

public class CommandHandler : InteractionModuleBase<SocketInteractionContext> {
    private readonly InteractionService _interactionService = new (Program.Client.Rest);

    public async Task InitializeAsync() {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        await _interactionService.RegisterCommandsGloballyAsync();
        
        Program.Client.InteractionCreated += async interaction =>
        {
            var ctx = new SocketInteractionContext(Program.Client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, null);
        };
    }

    [SlashCommand("add", "Add a map to the queue")]
    public async Task Add(string beatsaverIdOrLink) {
        try {

            await RespondAsync(await SendMessage("add " + beatsaverIdOrLink));
        } catch (Exception ex) {
            Console.WriteLine("Exception: {0}", ex);
            await RespondAsync($"```Error: {ex.Message}\n{ex.StackTrace}```");
            throw;
        }
    }


    private static async Task<string> SendMessage(string input) {
        using (NamedPipeClientStream client = new NamedPipeClientStream(".", "DcPlaylistPlugin",
                   PipeDirection.InOut, PipeOptions.Asynchronous)) {

            input = new string(input.Where(c => !char.IsControl(c)).ToArray());

            await client.ConnectAsync();

            using (var writer = new StreamWriter(client, Encoding.ASCII, 1024, true) { AutoFlush = true })
            using (var reader = new StreamReader(client, Encoding.ASCII, false, 1024, true)) {
                await writer.WriteLineAsync(input);
                await writer.FlushAsync();

                return await reader.ReadLineAsync() ?? "Error during command Execution";
            }
        }
    }
}

using System;
using System.IO;
using Newtonsoft.Json;

namespace DcPlaylistPlugin.Util.Playlist;

public class PlaylistManager {

    private static readonly string BasePath = @$"{Environment.CurrentDirectory}\Playlists\";
    
    public static Playlist? GetPlaylist(string playlistName) {
        if (!File.Exists($"{BasePath + playlistName}.bplist"))
            return null;
        
        string json = File.ReadAllText($"{BasePath + playlistName}.bplist");
        return JsonConvert.DeserializeObject<Playlist>(json);
    }

    public static void SavePlaylist(Playlist playlist) {
        
        string json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
        File.WriteAllText($"{BasePath + playlist.GetTitle()}.bplist", json);
    }
    
}
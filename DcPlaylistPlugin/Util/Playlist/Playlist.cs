using System.Collections.Generic;
using Newtonsoft.Json;

namespace DcPlaylistPlugin.Util.Playlist;

public class Playlist(string playlistTitle, string playlistAuthor, string image, List<Song>? songs) {
    
    [JsonProperty("playlistTitle")]
    public string PlaylistTitle { get; private set; } = playlistTitle;
    
    [JsonProperty("playlistAuthor")]
    public string PlaylistAuthor { get; private set; } = playlistAuthor;
    
    [JsonProperty("image")]
    public string Image { get; private set; } = image;
    
    [JsonProperty("songs")]
    public List<Song> Songs { get; private set; } = songs ?? [];

    internal void Add(Song song) {
        Songs.Add(song);
    }

    internal void Add(string? key, string hash, string songName) {
        key ??= "";
        Songs.Add(new Song(key, hash, songName));
    }

    public string GetTitle() {
        return PlaylistTitle;
    }
}
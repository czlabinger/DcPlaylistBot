using Newtonsoft.Json;

namespace DcPlaylistPlugin.Util.Playlist;

public class Song(string key, string hash, string songName) {
    
    [JsonProperty("key")]
    public string Key { get; private set; } = key;
    
    [JsonProperty("hash")]
    public string Hash { get; private set; } = hash;
    
    [JsonProperty("songName")]
    public string SongName { get; private set; } = songName;
}
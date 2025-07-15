using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeatSaverSharp.Models;
using DcPlaylistPlugin.Util;
using SongCore;
using UnityEngine.SceneManagement;
using Playlist = DcPlaylistPlugin.Util.Playlist.Playlist;

namespace DcPlaylistPlugin.Commands;

public class Commands {
    public static bool ReloadNeeded;

    public static async Task<string> Add(string input) {
        try {
            input = input[(input.IndexOf(' ') + 1)..];

            string id;
            id = (input.StartsWith("http")) ? input[(input.LastIndexOf('/') + 1)..] : input;

            Beatmap? beatmap = await BeatSaverDownloader.Plugin.BeatSaver.Beatmap(id);

            if (beatmap == null)
                return $"Beatmap with ID: {id} not found";


            byte[]? bytes = await beatmap.LatestVersion.DownloadZIP();
            if (bytes == null)
                return $"Could not download latest version: {id}";

            string path =
                @$"{Environment.CurrentDirectory}\Beat Saber_Data\CustomLevels\{Regex.Replace(id + " (" + beatmap.Name + " - " + beatmap.Uploader.Name + ")", @"[\\\/\:\*\?\""\<\>\|]", "")}";

            if (Directory.Exists(path)) {
                return "Map already downloaded";
            }

            ZipExtractor.ExtractZipFromByteArray(bytes, path);

            if (SceneManager.GetActiveScene().name != "MainMenu") ReloadNeeded = true;


            if (!ReloadNeeded) {
                Loader.Instance.RefreshSongs();
            }

            Playlist? playlist = Util.Playlist.PlaylistManager.GetPlaylist("DcPlaylistBot");
            playlist?.Add(beatmap.LatestVersion.Key, beatmap.LatestVersion.Hash, beatmap.Name);
            if (playlist != null) Util.Playlist.PlaylistManager.SavePlaylist(playlist);

            return $"Added map: {beatmap.Name} mapped by {beatmap.Uploader.Name}${beatmap.LatestVersion.CoverURL}";
        }
        catch (Exception e) {
            return $"Error: {e.Message}";
        }
    }
}
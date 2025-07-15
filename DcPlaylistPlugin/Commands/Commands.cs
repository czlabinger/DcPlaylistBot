using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeatSaverSharp.Models;
using DcPlaylistPlugin.Util;
using SongCore;
using UnityEngine.SceneManagement;

namespace DcPlaylistPlugin.Commands;

public class Commands {

    public static bool ReloadNeeded;
    
    public static async Task<string> Add(string input) {
        input = input[(input.IndexOf(' ') + 1)..];

        string id;
        if (input.StartsWith("http")) {
            id = input[(input.LastIndexOf('/') + 1)..];
        } else {
            id = input;
        }

        Beatmap? beatmap = await BeatSaverDownloader.Plugin.BeatSaver.Beatmap(id);

        if (beatmap == null) 
            return $"Beatmap with ID: {id} not found";
        
        
        byte[]? bytes = await beatmap.LatestVersion.DownloadZIP();
        if (bytes == null) 
            return $"Could not download latest version: {id}";
        
        string path = @$"{Environment.CurrentDirectory}\Beat Saber_Data\CustomLevels\{Regex.Replace(id + " (" + beatmap.Name + " - " + beatmap.Uploader.Name + ")", @"[\\\/\:\*\?\""\<\>\|]", "")}";
        ZipExtractor.ExtractZipFromByteArray(bytes, path);

        if (SceneManager.GetActiveScene().name != "MainMenu") {
            ReloadNeeded = true;
        }

        if (!ReloadNeeded) {
            Loader.Instance.RefreshSongs();
        }
        
        return $"Added map: {beatmap.Name} mapped by {beatmap.Uploader.Name}";
    }
}
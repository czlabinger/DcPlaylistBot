using System;
using System.IO;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using DcPlaylistPlugin.Util;
using TMPro;

namespace DcPlaylistPlugin.UI;

[ViewDefinition("DcPlaylistPlugin.UI.settings.bsml")]
public class SettingsViewController : BSMLAutomaticViewController {

    [UIComponent("token")] 
    public TextMeshProUGUI tokenText = null!;
    
    [UIAction("SetToken")]
    public async void SetToken() {
        try {
            string token = FileSelector.SelectTextOrNoExtensionFile();
            
            tokenText.text = $"Token: {token[..5] + "*****"}";
            
            await File.WriteAllTextAsync($@"{Environment.CurrentDirectory}\UserData\DcPlaylistBot\token.txt",
                token);
            BotHelper.StartBotProcess();
        }
        catch (Exception e) {
            Plugin.Log.Error(e);
        }
    }
    
    [UIAction("#post-parse")]
    public void PostParse() {
        try {
            string token = File.ReadAllText($@"{Environment.CurrentDirectory}\UserData\DcPlaylistBot\token.txt");
            tokenText.text = $"Token: {token[..5] + "*****"}";
        }
        catch (Exception) {
            tokenText.text = "No token set!";
        }
    }
    
}
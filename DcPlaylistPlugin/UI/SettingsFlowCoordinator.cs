using BeatSaberMarkupLanguage;
using HMUI;

namespace DcPlaylistPlugin.UI;

public class SettingsFlowCoordinator : FlowCoordinator {
    private readonly SettingsViewController _settingsViewController = BeatSaberUI.CreateViewController<SettingsViewController>();

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
        if (firstActivation) {
            SetTitle("DcPlaylistPlugin");
            showBackButton = true;
        }

        if (addedToHierarchy) {
            ProvideInitialViewControllers(_settingsViewController);
        }
    }

    protected override void BackButtonWasPressed(ViewController topViewController) {
        BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
    }
}
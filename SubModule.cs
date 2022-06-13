using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;

namespace Int19h.Bannerlord.NoRelation {
    public class SubModule : MBSubModuleBase {
        private static bool isActive = false;

        private static void Print(string text, Color color) {
            var message = new InformationMessage(text, color);
            InformationManager.DisplayMessage(message);
        }

        private static void NotSupported(string reason) {
            var version = typeof(InformationManager).Assembly.GetName().Version;
            Print($"Unsupported game version {version}: {reason}", new Color(1, 0, 0));
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot() {
            if (isActive) {
                return;
            }

            var gnm = GauntletGameNotification.Current;
            if (gnm is null) {
                NotSupported("GauntletGameNotification.Current is null");
                return;
            }

            var dataSourceField = gnm.GetType().GetField("_dataSource", BindingFlags.NonPublic | BindingFlags.Instance);
            if (dataSourceField is null) {
                NotSupported("GauntletGameNotification._dataSource is missing");
                return;
            }

            var gnvm = dataSourceField.GetValue(gnm) as GameNotificationVM;
            if (gnvm is null) {
                NotSupported("GauntletGameNotification._dataSource has invalid type");
                return;
            }

            MBInformationManager.FiringQuickInformation -= gnvm.AddGameNotification;
            MBInformationManager.FiringQuickInformation += (string notificationText, int extraTimeInMs, BasicCharacterObject announcerCharacter, string soundId) => {
                if (soundId == "event:/ui/notification/relation") {
                    Print(notificationText, new Color(0.5f, 0.6f, 0.6f));
                } else {
                    gnvm.AddGameNotification(notificationText, extraTimeInMs, announcerCharacter, soundId);
                }
            };

            isActive = true;
        }
    }
}
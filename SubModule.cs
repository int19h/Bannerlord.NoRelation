using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
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

            var gnm = GameNotificationManager.Current;
            if (gnm is null) {
                NotSupported("GameNotificationManager.Current is null");
                return;
            }

            var dataSourceField = gnm.GetType().GetField("_dataSource", BindingFlags.NonPublic | BindingFlags.Static);
            if (dataSourceField is null) {
                NotSupported("GameNotificationManager._dataSource is missing");
                return;
            }

            var gnvm = dataSourceField.GetValue(null) as GameNotificationVM;
            if (gnvm is null) {
                NotSupported("GameNotificationManager._dataSource has invalid type");
                return;
            }

            InformationManager.FiringQuickInformation -= gnvm.AddGameNotification;
            InformationManager.FiringQuickInformation += (string notificationText, int extraTimeInMs, BasicCharacterObject announcerCharacter, string soundId) => {
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
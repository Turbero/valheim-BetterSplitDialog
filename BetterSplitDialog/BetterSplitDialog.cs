using BepInEx;
using HarmonyLib;

namespace BetterSplitDialog
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class BetterSplitDialog : BaseUnityPlugin
    {
        public const string GUID = "Turbero.BetterSplitDialog";
        public const string NAME = "Better SplitDialog";
        public const string VERSION = "1.0.3";

        private Harmony harmony;

        private void Awake()
        {
            harmony = new Harmony(GUID);
            harmony.PatchAll();
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}

using BepInEx;
using HarmonyLib;

namespace BetterSplitDialog
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class BetterSplitDialog : BaseUnityPlugin
    {
        private const string GUID = "Turbero.BetterSplitDialog";
        private const string NAME = "Better SplitDialog";
        private const string VERSION = "1.1.0";

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

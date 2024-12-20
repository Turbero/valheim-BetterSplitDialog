/*namespace SplitStackTextInput.GameClasses
{
    using HarmonyLib;
    using SplitStackTextInput.Helpers;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;


    [HarmonyPatch(typeof(Slider), "OnPointerDown")]
    public static class Slider_OnPointerDown_RightClickSpawnTextInput
    {
        private static void Prefix(ref Slider __instance, PointerEventData eventData)
        {
            if (TextInput.IsVisible())
            {
                TextInput.instance.Hide();
                SplitReceiverInstance.Remove();
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                SplitReceiverInstance.Create(__instance.maxValue, __instance);
            }    
        }
    }
}
*/
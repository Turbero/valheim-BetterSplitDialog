/*namespace SplitStackTextInput.GameClasses
{
    using HarmonyLib;
    using SplitStackTextInput.Configuration;
    using SplitStackTextInput.Helpers;
    using TMPro;
    using UnityEngine;

    [HarmonyPatch(typeof(InventoryGui), "ShowSplitDialog")]
    [HarmonyPriority(Priority.VeryHigh)]
    public static class InventoryGui_Patch_ShowSplitDialog_ShowTextInputInstead
    {
        private static bool Prefix(
            ref InventoryGui __instance,
            ref ItemDrop.ItemData ___m_splitItem,
            ref Inventory ___m_splitInventory,
            ItemDrop.ItemData item, Inventory __1)
        {
            if (ConfigurationManager.UseSliderRightClick.Value)
            {
                return true;
            }

            ___m_splitItem = item;
            ___m_splitInventory = __1;
            __instance.m_splitSlider.value = (float)Mathf.CeilToInt((float)item.m_stack / 2f);

            SplitReceiverInstance.Create((float)item.m_stack, __instance.m_splitSlider, __instance);

            return false;
        }
    }

    [HarmonyPatch(typeof(InventoryGui), "OnSplitOk")]
    public static class InventoryGui_Patch_OnSplitOk_RemoveTextInput
    {
        private static void Prefix()
        {
            if (TextInput.IsVisible())
            {
                TextInput.instance.Hide();
                SplitReceiverInstance.Remove();
            }
        }
    }

    [HarmonyPatch(typeof(InventoryGui), "OnSplitCancel")]
    public static class InventoryGui_Patch_OnSplitCancel_RemoveTextInput
    {
        private static void Prefix(ref InventoryGui __instance)
        {
            if (TextInput.IsVisible())
            {
                TextInput.instance.Hide();
                SplitReceiverInstance.Remove();
            }
        }
    }
}
*/
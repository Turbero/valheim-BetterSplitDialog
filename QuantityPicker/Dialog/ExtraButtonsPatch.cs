using HarmonyLib;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterSplitDialog.Dialog
{
    [HarmonyPatch(typeof(InventoryGui), "Awake")]
    [HarmonyPatch]
    public class SplitDialogExtraButtonsPatch
    {
        static void Postfix(InventoryGui __instance)
        {
            Button copy = InventoryGui.instance.m_splitOkButton;
            Transform win_bkg = copy.transform.parent;
            Button one = createButton(copy, "PickerButton1", win_bkg.transform, new Vector2(50, 35), new Vector2(-120, 177), 0);
            Button pct20 = createButton(copy, "PickerButtonPct20", win_bkg.transform, new Vector2(50, 35), new Vector2(-120, 142), 0);
            Button pct40 = createButton(copy, "PickerButtonPct40", win_bkg.transform, new Vector2(50, 35), new Vector2(-120, 107), 0);
            Button pct60 = createButton(copy, "PickerButtonPct60", win_bkg.transform, new Vector2(50, 35), new Vector2(-65, 177), 0);
            Button pct80 = createButton(copy, "PickerButtonPct80", win_bkg.transform, new Vector2(50, 35), new Vector2(-65, 142), 0);
            Button pct100 = createButton(copy, "PickerButtonPct100", win_bkg.transform, new Vector2(50, 35), new Vector2(-65, 107), 0);

        }

        private static Button createButton(Button copy, string name, Transform parent, Vector2 sizeDetlta, Vector2 anchoredPosition, int quantity)
        {
            GameObject newButtonObject = GameObject.Instantiate(copy.gameObject, parent);
            newButtonObject.name = name;

            RectTransform newButtonRect = newButtonObject.GetComponent<RectTransform>();
            newButtonRect.sizeDelta = sizeDetlta;
            newButtonRect.anchoredPosition = anchoredPosition;

            TMP_Text buttonText = newButtonObject.GetComponentInChildren<TMP_Text>();
            buttonText.text = quantity.ToString();

            Button newButton = newButtonObject.GetComponent<Button>();
            return newButton;
        }
    }

    [HarmonyPatch]
    public class SplitDialogExtraButtonsUpdatePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(InventoryGui), "ShowSplitDialog");
        }

        static void Postfix(
            ref InventoryGui __instance,
            ref ItemDrop.ItemData ___m_splitItem,
            ref Inventory ___m_splitInventory,
            ItemDrop.ItemData item, Inventory __1)
        {
            if (GameObject.Find("PickerButton1") == null)
            {
                return;
            }

            ___m_splitItem = item;
            ___m_splitInventory = __1;

            int totalStack = item.m_stack;

            updateTextAndEvent("PickerButton1", ___m_splitInventory, ___m_splitItem, 1);
            updateTextAndEvent("PickerButtonPct20", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.20f));
            updateTextAndEvent("PickerButtonPct40", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.40f));
            updateTextAndEvent("PickerButtonPct60", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.60f));
            updateTextAndEvent("PickerButtonPct80", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.80f));
            updateTextAndEvent("PickerButtonPct100", ___m_splitInventory, ___m_splitItem, totalStack);
        }
        private static void updateTextAndEvent(string objectName, Inventory m_splitInventory, ItemDrop.ItemData m_splitItem, int newQuantity)
        {
            GameObject buttonObject = GameObject.Find(objectName);

            TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
            buttonText.text = newQuantity.ToString();

            Button newButton = buttonObject.GetComponent<Button>();
            newButton.onClick = new Button.ButtonClickedEvent();
            newButton.onClick.AddListener(() =>
            {
                //Mimic InventoryGui.OnSplitOk()
                Type type = InventoryGui.instance.GetType();
                MethodInfo privateMethod = type.GetMethod("SetupDragItem", BindingFlags.NonPublic | BindingFlags.Instance);
                privateMethod.Invoke(InventoryGui.instance, new object[] { m_splitItem, m_splitInventory, newQuantity });

                FieldInfo attributeSplitItem = type.GetField("m_splitItem", BindingFlags.NonPublic | BindingFlags.Instance);
                attributeSplitItem.SetValue(InventoryGui.instance, null);

                FieldInfo attributeSplitInventory = type.GetField("m_splitInventory", BindingFlags.NonPublic | BindingFlags.Instance);
                attributeSplitInventory.SetValue(InventoryGui.instance, null);

                InventoryGui.instance.m_splitPanel.gameObject.SetActive(value: false);
            });
        }
    }
}

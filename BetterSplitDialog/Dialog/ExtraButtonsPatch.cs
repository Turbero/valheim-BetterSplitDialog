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
            Button one   = createButton(copy, "PickerButton1",     win_bkg.transform, new Vector2(50, 35), new Vector2(-120, 160), 0); //will be hidden by Pct20 but avoids errors
            Button pct20 = createButton(copy, "PickerButtonPct20", win_bkg.transform, new Vector2(50, 35), new Vector2(-120, 160), 0);
            Button pct40 = createButton(copy, "PickerButtonPct40", win_bkg.transform, new Vector2(50, 35), new Vector2(-120, 124), 0);
            Button pct60 = createButton(copy, "PickerButtonPct60", win_bkg.transform, new Vector2(50, 35), new Vector2(-65, 160), 0);
            Button pct80 = createButton(copy, "PickerButtonPct80", win_bkg.transform, new Vector2(50, 35), new Vector2(-65, 124), 0);
            
            Button pctMin = createButton(copy, "PickerButtonPctMin", win_bkg.transform, new Vector2(50, 35), new Vector2(-100, 58), 0);
            Button pctMinus1 = createButton(copy, "PickerButtonPctMinus1", win_bkg.transform, new Vector2(50, 35), new Vector2(-50, 58), 0);
            Button pct50 = createButton(copy, "PickerButtonPct50", win_bkg.transform, new Vector2(50, 35), new Vector2(0, 58), 0);
            Button pctPlus1 = createButton(copy, "PickerButtonPctPlus1", win_bkg.transform, new Vector2(50, 35), new Vector2(50, 58), 0);
            Button pctMax = createButton(copy, "PickerButtonPctMax", win_bkg.transform, new Vector2(50, 35), new Vector2(100, 58), 0);

            //Move cancel/ok
            (InventoryGui.instance.m_splitCancelButton.transform as RectTransform).anchoredPosition = new Vector2(-90, 16);
            (InventoryGui.instance.m_splitOkButton.transform as RectTransform).anchoredPosition = new Vector2(90, 16);
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

            //FIXME Reset value (not working yet)
            InventoryGui.instance.m_splitPanel
                .transform.Find("win_bkg")
                .transform.Find("InputField (Legacy)")
                .transform.Find("Text (Legacy)")
            .GetComponent<Text>().text = "";

            //Update automatic buttons
            updateTextAndEvent("PickerButtonPct20",  ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.20f));
            updateTextAndEvent("PickerButtonPct40",  ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.40f));
            updateTextAndEvent("PickerButtonPct60",  ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.60f));
            updateTextAndEvent("PickerButtonPct80",  ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.80f));
            
            updateTextAndEvent("PickerButtonPctMin", ___m_splitInventory, ___m_splitItem, 1, "Min");
            updateTextAndEvent("PickerButtonPctMinus1", ___m_splitInventory, ___m_splitItem, ((int)totalStack / 2) - 1, "-1");
            updateTextAndEvent("PickerButtonPct50", ___m_splitInventory, ___m_splitItem, ((int)totalStack / 2), "50%");
            updateTextAndEvent("PickerButtonPctPlus1", ___m_splitInventory, ___m_splitItem, ((int)totalStack / 2) + 1, "+1");
            updateTextAndEvent("PickerButtonPctMax", ___m_splitInventory, ___m_splitItem, totalStack, "Max");
        }
        private static void updateTextAndEvent(string objectName, Inventory m_splitInventory, ItemDrop.ItemData m_splitItem, int newQuantity, string btnText = null)
        {
            GameObject buttonObject = GameObject.Find(objectName);

            TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
            buttonText.text = btnText != null ? btnText : newQuantity.ToString();

            Button newButton = buttonObject.GetComponent<Button>();
            newButton.onClick = new Button.ButtonClickedEvent();
            newButton.onClick.AddListener(() =>
            {
                if (btnText == null)
                {
                    //InventoryGui.instance.m_splitSlider.value = newQuantity;
                    //Mimic InventoryGui.OnSplitOk()
                    Type type = InventoryGui.instance.GetType();
                    MethodInfo privateMethod = type.GetMethod("SetupDragItem", BindingFlags.NonPublic | BindingFlags.Instance);
                    privateMethod.Invoke(InventoryGui.instance, new object[] { m_splitItem, m_splitInventory, newQuantity });

                    FieldInfo attributeSplitItem = type.GetField("m_splitItem", BindingFlags.NonPublic | BindingFlags.Instance);
                    attributeSplitItem.SetValue(InventoryGui.instance, null);

                    FieldInfo attributeSplitInventory = type.GetField("m_splitInventory", BindingFlags.NonPublic | BindingFlags.Instance);
                    attributeSplitInventory.SetValue(InventoryGui.instance, null);

                    InventoryGui.instance.m_splitPanel.gameObject.SetActive(value: false);
                }
                else
                {
                    var slider = InventoryGui.instance.m_splitSlider;
                    //Change slider only
                    if (btnText == "Min")
                    {
                        slider.value = 1;
                    }
                    else if (btnText == "-1")
                    {
                        slider.value = Math.Max(slider.value - 1, 1);
                    }
                    else if (btnText == "50%")
                    {
                        slider.value = (int)Math.Floor((slider.maxValue / 2) + 0.5f);
                    }
                    else if (btnText == "+1")
                    {
                        slider.value = Math.Min(slider.value + 1, slider.maxValue);
                    }
                    else if (btnText == "Max")
                    {
                        slider.value = slider.maxValue;
                    }
                }
            });
        }
    }
}

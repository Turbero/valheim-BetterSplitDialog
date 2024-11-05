using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuantityPicker
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class QuantityPickerJotunn : BaseUnityPlugin
    {
        public const string GUID = "Turbero.QuantityPicker";
        public const string NAME = "Quantity Picker";
        public const string VERSION = "1.0.0";

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

    [HarmonyPatch]
    public class SplitDialogPatchJotunn
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
            ___m_splitItem = item;
            ___m_splitInventory = __1;

            int totalStack = item.m_stack;

            if (GameObject.Find("PickerButton1") != null)
            {
                updateTextAndEvent("PickerButton20", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.20f));
                updateTextAndEvent("PickerButton40", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.40f));
                updateTextAndEvent("PickerButton60", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.60f));
                updateTextAndEvent("PickerButton80", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.80f));
                updateTextAndEvent("PickerButton100", ___m_splitInventory, ___m_splitItem, totalStack);
                return;
            }

            Button copy = InventoryGui.instance.m_splitOkButton;
            Transform win_bkg = copy.transform.parent;
            Button one = createButton(copy, "PickerButton1", win_bkg.transform, new Vector2(50, 35), new Vector2(-110, 180), 1);
            Button pct20 = createButton(copy, "PickerButton20", win_bkg.transform, new Vector2(50, 35), new Vector2(-110, 145), Mathf.CeilToInt((float)item.m_stack * 0.20f));
            Button pct40 = createButton(copy, "PickerButton40", win_bkg.transform, new Vector2(50, 35), new Vector2(-110, 110), Mathf.CeilToInt((float)item.m_stack * 0.40f));
            Button pct60 = createButton(copy, "PickerButton60", win_bkg.transform, new Vector2(50, 35), new Vector2(110, 180), Mathf.CeilToInt((float)item.m_stack * 0.60f));
            Button pct80 = createButton(copy, "PickerButton80", win_bkg.transform, new Vector2(50, 35), new Vector2(110, 145), Mathf.CeilToInt((float)item.m_stack * 0.80f));
            Button pct100 = createButton(copy, "PickerButton100", win_bkg.transform, new Vector2(50, 35), new Vector2(110, 110), totalStack);

            updateTextAndEvent("PickerButton1", ___m_splitInventory, ___m_splitItem, 1);
            updateTextAndEvent("PickerButton20", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.20f));
            updateTextAndEvent("PickerButton40", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.40f));
            updateTextAndEvent("PickerButton60", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.60f));
            updateTextAndEvent("PickerButton80", ___m_splitInventory, ___m_splitItem, Mathf.CeilToInt((float)item.m_stack * 0.80f));
            updateTextAndEvent("PickerButton100", ___m_splitInventory, ___m_splitItem, totalStack);
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

        private static void updateTextAndEvent(string objectName, Inventory m_splitInventory, ItemDrop.ItemData m_splitItem, int newQuantity)
        {
            GameObject buttonObject = GameObject.Find(objectName);
            
            TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
            buttonText.text = newQuantity.ToString();
            
            Button newButton = buttonObject.GetComponent<Button>();
            newButton.onClick = new Button.ButtonClickedEvent();
            newButton.onClick.AddListener(() =>
            {
                //Mimic InventoryGui.OnSplitOn()
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

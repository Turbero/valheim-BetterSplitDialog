using HarmonyLib;
using Jotunn.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BetterSplitDialog.Dialog
{

    [HarmonyPatch(typeof(InventoryGui), "Awake")]
    [HarmonyPatch]
    public class SplitDialogLoadPatch
    {
        public static GameObject quantityInputField;
        public static bool InputFieldFocused = false;

        static void Postfix(InventoryGui __instance)
        {
            if (quantityInputField != null)
                return;

            Transform transformParent = InventoryGui.instance.m_splitPanel.transform.Find("win_bkg").transform;

            // Create field
            quantityInputField = GUIManager.Instance.CreateInputField(
                parent: transformParent,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(110, 18),
                contentType: InputField.ContentType.IntegerNumber,
                placeholderText: "...",
                fontSize: 16,
                width: 100f,
                height: 30f
            );

            // Listener to adjust slider when focusing out
            InputField inputFieldComponent = quantityInputField.GetComponent<InputField>();            
            inputFieldComponent.onEndEdit.AddListener(quantityText =>
            {
                if (int.TryParse(quantityText, out int quantity))
                {
                    InventoryGui.instance.m_splitSlider.value = Mathf.Clamp(quantity, 1, (int)InventoryGui.instance.m_splitSlider.maxValue);
                }
                InputFieldFocused = false;
            });

            //  EventTrigger for OnSelect and OnDeselect
            EventTrigger eventTrigger = quantityInputField.AddComponent<EventTrigger>();

            // OnSelect
            EventTrigger.Entry selectEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select
            };
            selectEntry.callback.AddListener((data) => { InputFieldFocused = true; });
            eventTrigger.triggers.Add(selectEntry);

            // OnDeselect
            EventTrigger.Entry deselectEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Deselect
            };
            deselectEntry.callback.AddListener((data) => { InputFieldFocused = false; });
            eventTrigger.triggers.Add(deselectEntry);
        }
    }

    // Control slider if the InputField is not active
    [HarmonyPatch(typeof(InventoryGui), "Update")]      
    public class InventoryGuiUpdateSplitDialogLoadPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return !SplitDialogLoadPatch.InputFieldFocused;
        }
    }
}

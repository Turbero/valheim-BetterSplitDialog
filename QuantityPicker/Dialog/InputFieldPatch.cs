using HarmonyLib;
using Jotunn.Managers;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BetterSplitDialog.Dialog
{
    [HarmonyPatch]
    public class InputFieldPatch
    {
        private static GameObject quantityInputField;
        public static bool InputFieldFocused = false;

        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(InventoryGui), "ShowSplitDialog");
        }

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
                    Debug.Log("Cantidad seleccionada: " + quantity);
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

            // Split quantity title
            GameObject titleObject = new GameObject("SplitQuantityTitle", typeof(TextMeshProUGUI));
            titleObject.transform.SetParent(transformParent, false);

            TextMeshProUGUI titleText = titleObject.GetComponent<TextMeshProUGUI>();
            titleText.text = $"{Localization.instance.Localize("$inventory_pickup")}...";
            titleText.fontSize = 22;
            titleText.fontStyle = FontStyles.Normal;
            titleText.color = new Color(1f, 0.7176f, 0.3603f, 1f); //same as title
            titleText.alignment = TextAlignmentOptions.Center;

            RectTransform titleRect = titleObject.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(110, 50);
        }
    }

    // Controla el slider solo si el InputField no está activo
    [HarmonyPatch(typeof(InventoryGui), "Update")]      
    public class InventoryGuiUpdateSplitInputFieldFocusPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return !InputFieldPatch.InputFieldFocused;
        }
    }
}

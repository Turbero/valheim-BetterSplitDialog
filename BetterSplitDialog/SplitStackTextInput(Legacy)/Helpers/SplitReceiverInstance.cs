/*namespace SplitStackTextInput.Helpers
{
    using GUIFramework;
    using UnityEngine.UI;

    public class SplitReceiverInstance
    {
        private static SplitReceiver receiver;

        public static SplitReceiver Create(float max, Slider slider, InventoryGui inventory = null)
        {
            receiver = new SplitReceiver(max, slider, inventory);
            TextInput.instance.RequestText(receiver, $"Enter Amount (1-{max})", max.ToString().Length);            
            return receiver;
        }

        public static SplitReceiver Get()
        {
            return receiver;
        }

        public static void Remove()
        {
            receiver = null;
        }
    }
}*/

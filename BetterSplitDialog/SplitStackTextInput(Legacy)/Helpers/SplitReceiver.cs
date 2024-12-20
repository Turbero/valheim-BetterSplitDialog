/*namespace SplitStackTextInput.Helpers
{
    using System.Reflection;
    using UnityEngine.UI;

    public class SplitReceiver : TextReceiver
    {
        private float maxValue;
        private InventoryGui inventory;
        private Slider slider;

        public SplitReceiver(float max, Slider slider, InventoryGui inventory = null)
        {
            this.maxValue = max;
            this.slider = slider;
            this.inventory = inventory;
        }

        public float GetValue()
        {
            return this.slider.value;
        }

        public string GetText()
        {
            return this.slider.value.ToString();
        }

        public void SetText(string text)
        {
            if (float.TryParse(text, out float result) && result > 0 && result < this.maxValue)
            {
                this.slider.value = result;

                if (this.inventory != null)
                {
                    MethodInfo method = this.inventory.GetType().GetMethod("OnSplitOk", BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(this.inventory, new object[] { });
                }
            }
            else
            {
                if (this.inventory != null)
                {
                    MethodInfo method = this.inventory.GetType().GetMethod("OnSplitCancel", BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(this.inventory, new object[] { });
                }
            }
        }
    }
}
*/
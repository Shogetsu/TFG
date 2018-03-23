using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public Image icon;
    public Button removeButton;

    public Text quantityText;

    Item item; //item actual en este slot

    public void AddItem(Item newItem, int quantity)
    {
        item = newItem;
        icon.sprite = item.icon; //se asigna el icono del item al slot actual
        icon.enabled = true;
        removeButton.interactable = true;
        quantityText.color = new Color(50, 50, 50, 255);
        quantityText.text = quantity.ToString();
    }

    public void ClearSlot() //vaciar el slot
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        quantityText.color = new Color(0,0,0,0);
    }

    public void OnRemoveButton()
    {
        //Inventory.instance.Remove(item);
    }

    public string getItemName()
    {
        return item.name;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
            Debug.Log("Has usado " + item.name);
        }
    }
}

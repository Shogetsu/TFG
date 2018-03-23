using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public Image icon;
    public Button removeButton;

    Item item; //item actual en este slot

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon; //se asigna el icono del item al slot actual
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot() //vaciar el slot
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
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

using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public Image icon;
   // public Button removeButton;

    public Text quantityText;

    public Item item; //item actual en este slot
    //bool equipped = false;
    public GameObject equip;

    public void AddItem(Item newItem, int quantity, string color)
    {

        item = newItem;
        icon.sprite = item.icon; //se asigna el icono del item al slot actual
        if (color.Equals("Blue"))
            icon.sprite = newItem.iconBlue;
        if (color.Equals("Green"))
            icon.sprite = newItem.iconGreen;

       icon.enabled = true;
       // removeButton.interactable = true;
        quantityText.color = new Color(50, 50, 50, 255);
        quantityText.text = quantity.ToString();

        Debug.Log("El item anyadido es del tipo: " + item.GetType().Name);
    }

    public void ClearSlot() //vaciar el slot
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
      //  removeButton.interactable = false;
        quantityText.color = new Color(0,0,0,0);
        SetEquipped(false);
        //equipped = false;
    }

   /* public void OnRemoveButton()
    {
        //Inventory.instance.Remove(item);
    }*/

    public void UseItem(GameObject player)
    {
        if (item != null)
        {
            Debug.Log("Estoy usando el item del tipo " + item.GetType().Name);
            item.Use(player);
            //Debug.Log("El jugador "+player.GetComponent<SetupLocalPlayer>().colorString+" ha usado " + item.name);
        }
    }

    public void SetEquipped(bool e)
    {
        //equipped = e;
        equip.SetActive(e);
    }
}

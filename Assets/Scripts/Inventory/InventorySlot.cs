using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public Image icon;
   // public Button removeButton;

    public Text quantityText;

    public Item item; //item actual en este slot
    //bool equipped = false;
    public GameObject equip;
    public int colorLevel;

    public void AddItem(Item newItem, int quantity, string color, int newColorLevel)
    {

        item = newItem;
        icon.sprite = item.icon; //se asigna el icono del item al slot actual
        if (color.Equals("Blue"))
            icon.sprite = newItem.iconBlue;
        if (color.Equals("Green"))
            icon.sprite = newItem.iconGreen;

       icon.enabled = true;
       // removeButton.interactable = true;
        quantityText.color = new Color32(50, 50, 50, 255);
        quantityText.text = quantity.ToString();
        colorLevel = newColorLevel;
        icon.color = new Color32(255, 255, 255, 255);
        UpdateColorIcon();
        
        Debug.Log("Anyadido item: " + item.name+" con colorLevel: "+colorLevel);
    }

    public void ClearSlot() //vaciar el slot
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
      //  removeButton.interactable = false;
        quantityText.color = new Color(0,0,0,0);
        SetEquipped(false);
        colorLevel = 0;
        icon.color = new Color32(255, 255, 255, 255);
        //equipped = false;
    }

    void UpdateColorIcon()
    {
        if (item.GetType().BaseType.Name.Equals("FabricableItem"))
        {
            FabricableItem fabricableItem = item as FabricableItem;
            int colorLevelMAX = fabricableItem.colorLevel;

            if (colorLevel < colorLevelMAX * 0.25)
            {
                icon.color = new Color32(63, 63, 63, 255);
                return;
            }

            if (colorLevel < colorLevelMAX * 0.50)
            {
                icon.color = new Color32(127, 127, 127, 255);
                return;
            }

            if (colorLevel < colorLevelMAX * 0.75)
            {
                icon.color = new Color32(191, 191, 191, 255);
            }
        }
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

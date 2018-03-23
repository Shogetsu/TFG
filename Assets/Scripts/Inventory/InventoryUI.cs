using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class InventoryUI : NetworkBehaviour {

    public Transform itemsParent;

    Inventory inventory;

    InventorySlot[] slots;

	// Use this for initialization
	void Start () {
		//inventory = Inventory.instance;
       // inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>(); //Se obtiene un array con todos los slots del inventario
	}
	
	
	void UpdateUI () {


        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                //slots[i].AddItem(inventory.items[i]);

                //slots[i].AddItem(CmdAddItem(inventory.items[i]));
                CmdAddItem(inventory.items[i], i);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }

        Debug.Log("UPDATING UI");
	}

    [Command]
    void CmdAddItem(string itemName, int pos)
    {
        Object[] allitems = Resources.LoadAll("Items", typeof(Item));

        for (int i=0; i<allitems.Length; i++)
        {
            if (itemName.Equals(allitems[i].name))
            {
                Debug.Log(allitems[i].name + " encontrado");
                slots[pos].AddItem((Item)allitems[i]);
            }
        }
        
    }
}

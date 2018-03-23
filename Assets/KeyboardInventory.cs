using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInventory : MonoBehaviour {

    
    InventorySlot[] slots;

    // Use this for initialization
    void Start () {
        slots = gameObject.transform.GetComponentsInChildren<InventorySlot>(); //Se obtiene un array con todos los slots del inventario
    }
	
	// Update is called once per frame
	/*void Update () {
        int num;

        int.TryParse(Input.inputString, out num);

        if (num-1 < Inventory.instance.items.Count)
        {
            if (num >= 1 && num <= 9)
            {
                slots[num - 1].UseItem();
            }
        }
        
	}*/
}

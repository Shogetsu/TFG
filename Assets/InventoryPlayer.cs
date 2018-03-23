using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPlayer : MonoBehaviour {

    public Transform inventory;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if(inventory==null)
            inventory = GameObject.Find("Canvas").transform.GetChild(0).transform;

        int num;
        int.TryParse(Input.inputString, out num);

        if (num == 1)
        {
            Debug.Log("hola");
        }

        /* if (num-1 < Inventory.instance.items.Count)
         {
             if (num >= 1 && num <= 9)
             {
                 slots[num - 1].UseItem();
             }
         }*/
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Entered");
        if (collision.gameObject.CompareTag("Item"))
        {
           // inventory.GetComponent<Inventory>().ItemPickup(collision);
        }
    }
}

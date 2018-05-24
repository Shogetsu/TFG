using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Drop : NetworkBehaviour {

    GameObject item;

    [SyncVar]
    public int numItems;

    public int min;
    public int max;

	// Use this for initialization
	void Start () {
        if (!isServer) return;

		numItems = Random.Range(min, max+1);
    }
	
	// Update is called once per frame
	void Update () {
       /* if (this.gameObject.GetComponent<Health>().vit <= 0)
        {
            CmdDestroyGameObject();
        }*/
	}

   /* [Command]
    void CmdDestroyGameObject()
    {
        NetworkServer.Destroy(this.gameObject);
    }*/

    public void DropItem()
    {
        Object[] allitems = Resources.LoadAll("Prefabs", typeof(GameObject));
        for (int i = 0; i < allitems.Length; i++)
        {
            GameObject go = (GameObject)allitems[i];
            /* Debug.Log(go.GetComponent<ItemPickup>().item.color);
             Debug.Log(go.GetComponent<ItemPickup>().item.material);

             Debug.Log(GetComponent<SetupAnimalPlant>().colorString);
             Debug.Log(GetComponent<SetupAnimalPlant>().material);*/
            Debug.Log("***********");
            Debug.Log("Busca: " + GetComponent<SetupAnimalPlant>().colorString);
            Debug.Log("Busca: " + GetComponent<SetupAnimalPlant>().material);
            Debug.Log("---");
            Debug.Log("Es Resource?: " + go.GetComponent<ItemPickup>().item.GetType().Name.Equals("Resource"));
            Debug.Log("Encuentra: " + go.GetComponent<ItemPickup>().item.color);
            Debug.Log("Encuentra: " + go.GetComponent<ItemPickup>().item.material);
            Debug.Log("***********");
            if (go.GetComponent<ItemPickup>().item.color.Equals(GetComponent<SetupAnimalPlant>().colorString) &&
                go.GetComponent<ItemPickup>().item.material.Equals(GetComponent<SetupAnimalPlant>().material) &&
                go.GetComponent<ItemPickup>().item.GetType().Name.Equals("Resource"))
            {
                item = go;
                break;
            }
        }

        int dropped = 0;
        while (dropped < numItems)
        {
            Vector3 itemPos = new Vector3(this.gameObject.transform.position.x + Random.Range(-0.1f, 0.25f), this.gameObject.transform.position.y, this.gameObject.transform.position.z + Random.Range(-0.25f, 0.25f));
            GameObject itemDropped = Instantiate(item, itemPos, Quaternion.identity);
            NetworkServer.Spawn(itemDropped);
            dropped++;
        }
        
    }
}

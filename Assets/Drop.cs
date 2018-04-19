using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Drop : NetworkBehaviour {

    GameObject item;

	// Use this for initialization
	void Start () {
		
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
            Debug.Log(go.GetComponent<ItemPickup>().item.color);
            Debug.Log(go.GetComponent<ItemPickup>().item.material);

            Debug.Log(GetComponent<SetupAnimalPlant>().colorString);
            Debug.Log(GetComponent<SetupAnimalPlant>().material);


            if (go.GetComponent<ItemPickup>().item.color.Equals(GetComponent<SetupAnimalPlant>().colorString) &&
                go.GetComponent<ItemPickup>().item.material.Equals(GetComponent<SetupAnimalPlant>().material))
            {
                item = go;
                break;
            }
        }

        GameObject itemDropped = Instantiate(item, this.gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(itemDropped);
    }
}

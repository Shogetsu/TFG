using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Refuge", menuName = "Inventory/Refuge")]
public class RefugeCraft : FabricableItem
{

    public override void Use(GameObject player) //override - sobrescribe el metodo el padre
    {

        base.Use(player);
        Debug.Log("... y el item fabricado que estoy usando es un refugio");
        
        player.GetComponent<Inventory>().CmdBuildRefuge(name, player.GetComponent<Inventory>().GetColorItemHand());

    }

    /*GameObject GetPrefab(string nameItem)
    {
        GameObject go = null;
        Object[] allitems = Resources.LoadAll("Prefabs", typeof(GameObject));

        for (int i = 0; i < allitems.Length; i++)
        {
            if (allitems[i].name.Equals(nameItem))
            {
                go = allitems[i] as GameObject;
                break;
            }
        }
        return go;
    }*/
}

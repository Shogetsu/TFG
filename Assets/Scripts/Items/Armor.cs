using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Armor")]
public class Armor : FabricableItem
{

    public int def;

    public override void Use(GameObject player) //override - sobrescribe el metodo el padre
    {

        base.Use(player);
        Debug.Log("... y el item fabricado que estoy usando es una armadura.");

        player.GetComponent<Health>().CmdSetDef(def);
    }
}

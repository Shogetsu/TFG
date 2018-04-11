using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plane", menuName = "Inventory/Plane")]
public class PlaneCraft : FabricableItem
{

    public override void Use(GameObject player) //override - sobrescribe el metodo el padre
    {

        base.Use(player);
        Debug.Log("... y el item fabricado que estoy usando es un plano.");
    }
}

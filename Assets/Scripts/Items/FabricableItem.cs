using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class FabricableItem : Item
{
    public int quantityNeeded;
    public int levelColor;

    public override void Use(GameObject player) //es virtual para poder sobreescribirlo en funcion del objeto que se este utilizando
    {

        base.Use(player);
        Debug.Log("...y estoy usando un item fabricado...");
    }

}

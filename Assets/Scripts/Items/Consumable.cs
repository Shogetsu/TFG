using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{

    public override void Use(GameObject player) //es virtual para poder sobreescribirlo en funcion del objeto que se este utilizando
    {
        //Usar el objeto

        //Algo debe ocurrir

        // Debug.Log("Usando "+name);

        //Inventory.instance.Remove(this);
        base.Use(player);
        string playerColor = player.GetComponent<SetupLocalPlayer>().colorString;

        Debug.Log("... y estoy usando un consumible: "+name);

        /*if (material.Equals("Paper")) //Los jugadores solo pueden alimentarse de recursos de papel
            EatPaper(color, playerColor, player);*/
        if (name.Equals("LuminousPaint"))
            UseLuminousPaint(player);
    }

    void UseLuminousPaint(GameObject player)
    {
        if(player.GetComponent<ActivePointLight>().CheckActive()==false)
            player.GetComponent<ActivePointLight>().CmdActiveLight(true);
    }

}

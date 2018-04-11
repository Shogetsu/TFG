using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Inventory/Resource")]
public class Resource : Item
{

    public override void Use(GameObject player) //es virtual para poder sobreescribirlo en funcion del objeto que se este utilizando
    {
        //Usar el objeto

        //Algo debe ocurrir

        // Debug.Log("Usando "+name);

        //Inventory.instance.Remove(this);
        base.Use(player);
        string playerColor = player.GetComponent<SetupLocalPlayer>().colorString;

        Debug.Log("... y estoy usando un recurso");

        if (material.Equals("Paper")) //Los jugadores solo pueden alimentarse de recursos de papel
            EatPaper(color, playerColor, player);
    }

    void EatPaper(string paperColor, string playerColor, GameObject player)
    {

        player.GetComponent<Health>().CmdHeal(5);//Comer papel siempre cura
        Debug.Log("paperColor " + paperColor);
        Debug.Log("playerColor " + playerColor);

        if (!paperColor.Equals(playerColor)) //Resta nivel de color si el papel digerido no se corresponde con el nivel de color del personaje
            player.GetComponent<ColorLevel>().CmdModifyColorLevel(-5);
        else //En caso contrario, recuperara parte del nivel de color
            player.GetComponent<ColorLevel>().CmdModifyColorLevel(5);

    }

}

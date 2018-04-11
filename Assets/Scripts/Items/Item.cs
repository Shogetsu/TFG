using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

    new public string name = "New item";
    public Sprite icon = null;
    public Sprite iconBlue = null;
    public Sprite iconGreen = null;

    //public int quantityNeeded; //solo item fabricables

    public string color; //todos

    public string material; //todos

    public virtual void Use(GameObject player) //es virtual para poder sobreescribirlo en funcion del objeto que se este utilizando
    {

        string playerColor = player.GetComponent<SetupLocalPlayer>().colorString;
        Debug.Log("Soy el jugador " + playerColor+" ...");
    }

}

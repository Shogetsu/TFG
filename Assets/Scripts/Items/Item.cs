using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

    new public string name = "New item";
    public Sprite icon = null;
    public Sprite iconBlue = null;
    public Sprite iconGreen = null;

    public bool isResource;

    public int quantityNeeded;

    public string color;
    public bool isPaper;
    public bool isCardboard;
    public bool isPlastic;

    public string material;

    public virtual void Use() //es virtual para poder sobreescribirlo en funcion del objeto que se este utilizando
    {
        //Usar el objeto

        //Algo debe ocurrir

       // Debug.Log("Usando "+name);

        //Inventory.instance.Remove(this);

    }

}

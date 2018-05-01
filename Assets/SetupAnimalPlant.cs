using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupAnimalPlant : NetworkBehaviour {

    string[] colorsString = new string[] { "Magenta", "Red", "Cyan", "Blue", "Green", "Yellow" };
    Color[] colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };

    public string colorString;
    Color color;
    public string material;

    [SyncVar]
    int randomInt;

    public bool useTexture;

    // Use this for initialization
    void Start()
    {
        //ESTO NO FUNCIONA, EL COLOR SOLO SE ASIGNA EN EL HOST PERO LOS CLIENTES LO VEN SIN COLOR!!!!!!!!
        //CmdSetRandomColor();

     //   if (!isServer) return;



    }

    public override void OnStartClient()
    {
        //base.OnStartClient();
        if(isServer)
            randomInt = Random.Range(0, colors.Length);

        //CmdSetRandomColor();
        //Debug.Log(randomInt);
        
        

        if (!useTexture)
        {
            color = colors[randomInt];
            Renderer[] rends = GetComponentsInChildren<Renderer>(); // Se obtienen todos los renders del GameObject
            foreach (Renderer r in rends) //Se recorren todos los renders y se les asigna el color del jugador en cuestion
                r.material.color = color;
        }
        else
        {
            colorString = colorsString[randomInt];
            if (colorString.Equals("Green") || colorString.Equals("Blue"))
            {
                Renderer[] rends = GetComponentsInChildren<Renderer>(); // Se obtienen todos los renders del GameObject
                foreach (Renderer r in rends) //Se recorren todos los renders y se les asigna el color del jugador en cuestion
                    r.material = GetMaterial(colorString);
            }
        }

    }

    Material GetMaterial(string color)
    {
        Material mat = null;

        Object[] allitems = Resources.LoadAll("Materials", typeof(Material));

        for (int i = 0; i < allitems.Length; i++)
        {
            string matNameNeeded = this.gameObject.name.Replace("(Clone)", "") + color;
           // Debug.Log(matNameNeeded + " / " + allitems[i].name);
            if (matNameNeeded.Equals(allitems[i].name))
            {
                mat = allitems[i] as Material;
                break;
            }
        }

        return mat;
    }

    /*[Command]
    void CmdSetRandomColor()
    {
        RpcSetColor(randomInt);
    }

    [ClientRpc]
    void RpcSetColor(int randomInt)
    {
        Debug.Log(randomInt);
        color = colors[randomInt];
        colorString = colorsString[randomInt];

        Renderer[] rends = GetComponentsInChildren<Renderer>(); // Se obtienen todos los renders del GameObject
        foreach (Renderer r in rends) //Se recorren todos los renders y se les asigna el color del jugador en cuestion
            r.material.color = color;
    }*/
}

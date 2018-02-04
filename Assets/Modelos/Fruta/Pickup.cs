using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Pickup : MonoBehaviour {

    public GameObject inventoryPanel; //panel
    public GameObject[] inventoryIcons; //array de iconos de items

    int numItems = 0;

    private void Start()
    {
        inventoryPanel = GameObject.Find("Panel");    
    }

    void OnCollisionEnter(Collision collision)
    {
        //Se buscan en el panel los iconos existentes
        foreach(Transform child in inventoryPanel.transform)
        {
            //Se comprueba si el item ya se encuentra en el inventario, comparando los tags
            if(child.gameObject.tag == collision.gameObject.tag)
            {
                //Se obtiene el string del text, se convierte en integer, se suma 1, y se vuelve a convertir en string
                string c = child.Find("Text").GetComponent<Text>().text;
                int tcount = System.Int32.Parse(c) + 1;
                child.Find("Text").GetComponent<Text>().text = "" + tcount;
                return;
            }
        }

        //Si el item no se encuentra en el inventario

       // NewItem(collision.gameObject.tag);

        GameObject i;
        if(collision.gameObject.tag == "red")
        {
            i = Instantiate(inventoryIcons[0]);
            i.transform.SetParent(inventoryPanel.transform);
        }
        else if (collision.gameObject.tag == "blue")
        {
            i = Instantiate(inventoryIcons[1]);
            i.transform.SetParent(inventoryPanel.transform);
        }
        else if (collision.gameObject.tag == "green")
        {
            i = Instantiate(inventoryIcons[2]);
            i.transform.SetParent(inventoryPanel.transform);
        }
    }

   /*void NewItem(string tag)
    {
        GameObject i;
        i = Instantiate(inventoryIcons[numItems]);
        i.transform.SetParent(inventoryPanel.transform);
        numItems++;
    }*/
}

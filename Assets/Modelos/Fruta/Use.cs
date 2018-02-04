using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Use : MonoBehaviour {

	public void UseThis()
    {
        //Se comprueba si el icono sobre el que se hace click tiene un texto mayor que 1
        if (System.Int32.Parse(this.transform.Find("Text").GetComponent<Text>().text) > 1)
        {
            //En caso afirmativo, se le resta 1 (Esto seria solamente en el caso de usar CONSUMIBLES)
            int tcount = System.Int32.Parse(this.transform.Find("Text").GetComponent<Text>().text) - 1;
            this.transform.Find("Text").GetComponent<Text>().text = "" + tcount;
        }
        else //Si solo hay uno en el inventario se destruye
        {
            Destroy(this.gameObject);
        }
    }
}

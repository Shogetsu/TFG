using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController2 : MonoBehaviour {

    public float height; //Altura de las olas
    public float time; //Duracion de cada ciclo de animacion

	// Use this for initialization
	void Start () {
        /*
         moveBy(gameObject, Hash("eje", "altura", "tiempo de animacion", "looptype=bucle", "pingpong=arriba y abajo", "easetype=curva de transicion suave entre animaciones")
         */
        iTween.MoveBy(this.gameObject, iTween.Hash("y", height, "time", time, "looptype", "pingpong", "easetype", iTween.EaseType.easeInOutSine));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

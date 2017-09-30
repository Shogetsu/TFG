using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    void OnCollisionEnter(Collision collision)
    {
        
        GameObject hit = collision.gameObject; //objeto contra el que se ha colisionado

        /*ESTO ES SOLO para comprobar si colisiona con un jugador o no, de forma CUTRE*/
        PlayerMove hitPlayer = hit.GetComponent<PlayerMove>(); //de ese objeto estamos sacando una componente PlayerMove
        //hitPlayer==null si no hubiese colisionado con ningun jugador

        if (hitPlayer != null)
        { //Si realmente colisiona con un jugador, se destruye la bala y mete 10 de dmg
            Combat combat = hit.GetComponent<Combat>();//Asi se accede al script Combat de player
            combat.TakeDamage(10);
            Destroy(gameObject); 
        }
        //Estamos tratando de distinguir la bala de cada jugador
        /*Esto es una forma cutre de hacerse, las colisiones deben de hacerse con capas o tags*/

    }
}

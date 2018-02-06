using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PjControl : MonoBehaviour {

    static Animator anim;
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        /*mover en la misma direccion*/
       /*Vector3 movement = new Vector3(rotation, 0.0f, translation);
        transform.rotation = Quaternion.LookRotation(movement);
        transform.Translate(movement, Space.World);*/
        
        if (Input.GetButtonDown("Jump")) //La tecla "espacio" es por defecto JUMP
        {
            anim.SetTrigger("isJumping");
        }

        if(translation != 0)
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isIdle", false);
        }else{
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target; //GameObject VACIO unido al objeto (SOLO para girar en eje X - vertical)

    public Vector3 offset;

    public bool useOffsetValues;

    public float rotateSpeed;

    public Transform pivot; //ES EL OBJETO (SOLO para girar en eje Y - horizontal)

    public float maxViewAngle;
    public float minViewAngle;

    public bool invertY;

    public float distTarget;

    // Use this for initialization
    void Start () {
        if(!useOffsetValues)
            offset = target.position - transform.position;

        Cursor.lockState = CursorLockMode.Locked;
        
        pivot.transform.position = target.transform.position;
       // pivot.transform.parent = target.transform;
        pivot.transform.parent = null;
        
    }
	
	// Update is called once per frame
	void LateUpdate () {

        pivot.transform.position = target.transform.position;

        //Obtenemos la posicion X del raton y rotamos el target
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        pivot.Rotate(0, horizontal, 0); //Eje horizontal en el que gira el pivote

        //Obtenemos la posicion Y del raton y rotamos el pivote
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;
       // vertical = Mathf.Clamp(vertical, minViewAngle, maxViewAngle);
        //Eje vertical en el que gira el pivote
        if (invertY)
        {
            pivot.Rotate(vertical, 0, 0);
        }
        else
        {
            pivot.Rotate(-vertical, 0, 0);
        }



        //Limitar la rotacion de la camara
        /*POR ARRIBA maxViewValue*/
         if (pivot.rotation.eulerAngles.x > maxViewAngle && pivot.rotation.eulerAngles.x < 180f)
          {
              pivot.rotation = Quaternion.Euler(maxViewAngle, pivot.eulerAngles.y, 0);
          }
          
        /*POR ABAJO minViewValue*/
         if (pivot.rotation.eulerAngles.x > 180f && pivot.rotation.eulerAngles.x < 360f + minViewAngle)
         {
             pivot.rotation = Quaternion.Euler(360f + minViewAngle, pivot.eulerAngles.y, 0);
         }

       


        //Movemos la camara basada en la actual rotacion del target y el offset original
        float desiredYangle = pivot.eulerAngles.y; //el objeto gira en Y (horizontal)
        float desiredXangle =pivot.eulerAngles.x; //target gira en X (vertical)

        Quaternion rotation = Quaternion.Euler(desiredXangle, desiredYangle, 0); //Se aplican las rotaciones
      
        transform.position = target.position - (rotation * offset);

        if(transform.position.y < target.position.y) //Para que la camara no atraviese el suelo al mirar desde abajo
        {
            transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
        }
        //transform.position = target.position - transform.forward * distTarget;
        transform.LookAt(target);

        // transform.position = target.position - transform.forward * distTarget;
        

    }
}

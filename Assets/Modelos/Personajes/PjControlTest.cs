using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PjControlTest : MonoBehaviour
{

    static Animator anim;
    public float rotationSpeed = 100.0f;

    public float speed = 10.0f;
   // public Rigidbody RB;
    public float jumpForce;
    public CharacterController controller;
    public Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded = true;
    //public ThirdPersonCamera _thirdPCam;
    public float gravityScale;

    public Transform pivot;

    public GameObject playerModel;

    // Use this for initialization
    void Start () {
       /* if (!isLocalPlayer) //Si no se trata del jugador, se desactivan gameObject del resto de jugadores
        {
            transform.GetChild(0).gameObject.SetActive(false); //camara
            transform.GetChild(2).gameObject.SetActive(false); //canvas
            transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false); //panel
            return;
        }*/
           

        //RB = GetComponent<Rigidbody>();
        //controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        
        //transform.GetChild(0).GetComponent<Camera>().enabled = false;

    }

    // Update is called once per frame
    void Update () {

      /*  if (!isLocalPlayer)
            return;*/

        /* if (_thirdPCam.target != transform.GetChild(0)) //Se obtiene la posicion del gameobject target dentro del body del modelo
             _thirdPCam.target = transform.GetChild(0); //Se le asigna al target del script de la camara en tercera persona

         if (GameObject.FindObjectOfType<ThirdPersonCamera>() != null && _thirdPCam == null)
         {
             _thirdPCam = GameObject.FindObjectOfType<ThirdPersonCamera>();
             Debug.Log("Camara establecida");
         }*/

        float translation = Input.GetAxis("Vertical")*speed;
         float rotation = Input.GetAxis("Horizontal")*speed;
        /*translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);*/
        /*mover en la misma direccion*/
        /*Vector3 movement = new Vector3(rotation, 0.0f, translation);
         transform.rotation = Quaternion.LookRotation(movement);
         transform.Translate(movement, Space.World);*/

        /* float translation = Input.GetAxis("Vertical") * speed;
         float rotation = Input.GetAxis("Horizontal") * speed; 

         RB.velocity = new Vector3(rotation, RB.velocity.y, translation);
         */

        // moveDirection = new Vector3(rotation, moveDirection.y, translation);

        float yStore = moveDirection.y;

        moveDirection = (transform.forward * translation) + (transform.right * rotation);
        //Si el jugador corre en diagonal corre mas PERO ASI correra normal
        if (translation!=0 && rotation != 0)
        {
            moveDirection = moveDirection.normalized * speed;
        }
            
        moveDirection.y =yStore;

        /* moveDirection = (transform.forward * translation) + (transform.right * rotation);
         moveDirection = moveDirection.normalized * speed;*/

          if (isGrounded) //TODO modificar para RIGIDBODY
          {
              moveDirection.y = 0f;
              if (Input.GetButtonDown("Jump")) //La tecla "espacio" es por defecto JUMP
              {
                // RB.velocity = new Vector3(RB.velocity.x, jumpForce, RB.velocity.z);
                  rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                // moveDirection.y = jumpForce;
                GetComponent<NetworkAnimator>().SetTrigger("isJumping");

              /*  if (isServer) //El host ejecuta las animaciones Trigger 2 veces, esto lo soluciona
                {*/
                    GetComponent<NetworkAnimator>().animator.ResetTrigger("isJumping");
              //  }

                //  anim.SetTrigger("isJumping");
            }
          }

      /*  if (Input.GetButtonDown("Jump")) //La tecla "espacio" es por defecto JUMP
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            
            anim.SetTrigger("isJumping");
        }*/

        if (translation != 0 || rotation !=0)
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isIdle", false);
        }else{
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", true);
        }

        //moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);

        //controller.Move(moveDirection * Time.deltaTime );


        //Vector3 mov = new Vector3(rotation, 0, translation) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + moveDirection);
        
        
        //Mover al jugador en diferentes direcciones basadas en la direccion de la camara
        if (Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical") != 0) // !=0 ---> si esta siendo pulsada la tecla
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            //slerp rotacion suave
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("Entered");
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
       // Debug.Log("Exited");
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }
}

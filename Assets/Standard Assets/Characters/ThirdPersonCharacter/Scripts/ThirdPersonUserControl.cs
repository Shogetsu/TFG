using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking; //LIBRERIA IMPORTANTE PARA ACTUALIZAR LAS TRANSFORMACIONES EN CADA CLIENTE DE FORMA INDIVIDUAL
using UnityStandardAssets.Cameras;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : NetworkBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        public ThirdPersonCamera _thirdPCam;

        private void Start()
        {
           
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }
            
            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
            
           // Debug.Log(GameObject.FindObjectOfType<ThirdPersonCamera>());
            _thirdPCam = GameObject.FindObjectOfType<ThirdPersonCamera>();

        }

       

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            if(GameObject.FindObjectOfType<ThirdPersonCamera>() != null && _thirdPCam==null)
            {
                m_Cam = Camera.main.transform;
                _thirdPCam = GameObject.FindObjectOfType<ThirdPersonCamera>();
                Debug.Log("Camara establecida");
            }

            if(m_Cam==null)
                m_Cam = Camera.main.transform;

            if (_thirdPCam.target != transform.GetChild(0).GetChild(0)) //Se obtiene la posicion del gameobject target dentro del body del modelo
                _thirdPCam.target = transform.GetChild(0).GetChild(0); //Se le asigna al target del script de la camara en tercera persona

            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;

            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
                //Debug.Log("Sigo la direccion de la camara");
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
                //Debug.Log("Ni caso a la camara");
            }
#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);

            m_Jump = false;
        }
    }
}

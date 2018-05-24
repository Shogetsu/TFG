using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class AnimalIA : NetworkBehaviour {

    NavMeshAgent agent;
    Transform myTransform;
    public Transform targetTransform;
    LayerMask raycastLayer;
    float radius = 10;

    public bool aggressive;
    bool aggressiveNature;

    bool relax;
    bool chase;
    bool runAway;

    public float wanderRadius;
    public float wanderTimer;

    [SyncVar]
    private float timer;

    public float runAwayDistance = 4.0f;

    DayNightCycle whatTimeIsIt;

    // Use this for initialization
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();

        myTransform = transform;
        raycastLayer = 1 << LayerMask.NameToLayer("Player");

        SetState("relax");

        whatTimeIsIt = GameObject.Find("GameManager").GetComponent<DayNightCycle>();

        aggressiveNature = aggressive;

       // agent.height = 0.5f;
        //agent.baseOffset = 0;

    }

    public void SetState(string state)
    {
        switch (state)
        {
            case "relax":
                relax = true;
                chase = false;
                runAway = false;
                break;

            case "chase":
                relax = false;
                chase = true;
                runAway = false;
                break;

            case "runAway":
                relax = false;
                chase = false;
                runAway = true;
                break;
        }
    }
	
	// Para la IA es recomendable utilizar FixedUpdate
	void FixedUpdate ()
    {
        if (!isServer)
            return;

        if (agent.isOnNavMesh)
        {
            if(!aggressiveNature) //Los animales que NO son de naturaleza agresiva, se volveran agresivos en funcion de la hora del dia (por la noche)
                CheckTime();

            if (aggressive)
                Aggressive();

            if (relax)
                RandomMove();

            if (chase)
                MoveToTarget();

            if (runAway)
                RunAwayMove();   
        }
        else
        {
            Debug.Log("El agent no esta colocado sobre un NavMesh");
        }

        if (!DestinationReached())
        {
            if(!runAway)
                myTransform.GetChild(0).GetComponent<Animator>().SetBool("isWalking", true);
            else
                myTransform.GetChild(0).GetComponent<Animator>().SetBool("isRunningAway", true);

            myTransform.GetChild(0).GetComponent<Animator>().SetBool("isIdle", false);
        }
        else
        {
            myTransform.GetChild(0).GetComponent<Animator>().SetBool("isRunningAway", false);
            myTransform.GetChild(0).GetComponent<Animator>().SetBool("isWalking", false);
            myTransform.GetChild(0).GetComponent<Animator>().SetBool("isIdle", true);
        }
    }

    void CheckTime()
    { //Se vuelven agresivos entre las 00:00 y las 06:00
        if (whatTimeIsIt.time > whatTimeIsIt.HourToSeconds(0) &&
            whatTimeIsIt.time < whatTimeIsIt.HourToSeconds(6))
        {
            if(!aggressive)
                aggressive = true;
        }
        else
        {
            if (aggressive)
            {
                aggressive = false;
                SetState("relax");
                GetComponent<AnimalAttack>().StopAllCoroutines();
                GetComponent<AnimalAttack>().enabled = false;

            }
        }
    }


    void Aggressive()
    {
        SearchToTarget();
        GetComponent<AnimalAttack>().enabled = true;
        if (targetTransform == null)
            SetState("relax");
        else
            SetState("chase");
    }

    void RunAwayMove()
    {
        SearchToTarget(); //Se busca quien ha golpeado al animal

        if (targetTransform != null)
        {
            //float distance = Vector3.Distance(myTransform.position, targetTransform.position);

            Vector3 dirToPlayer = myTransform.position - (targetTransform.position + new Vector3(2,0,2));
            Vector3 newPos = myTransform.position + dirToPlayer; //El animal huye de quien le ha golpeado

            timer += Time.deltaTime;
            wanderTimer = Random.Range(1f, 3f);
            if (timer >= wanderTimer)
            {
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
        else
        {
            //Cuando se haya alejado lo suficiente, ya no sabra de quien esta huyendo, y pasa al estado de relax
            SetState("relax");
        }            
    }

    void RandomMove()
    {
        if (agent.isStopped == true) agent.isStopped = false;

        /*Movimiento aleatorio en un radio*/
        wanderTimer = Random.Range(5f, 15f);
        timer += Time.deltaTime;
        
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(myTransform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    void SearchToTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius, raycastLayer);

        if (targetTransform == null)
        {
            //Array de colliders que colisionen con una esfera creada por el enemigo a su alrededor (myTransform.position) con un radio (radius), afectando unicamente a una determinada capa (raycastLayer, capa "Player")
            
            if (hitColliders.Length > 0)
            {
                int randomInt = Random.Range(0, hitColliders.Length);
                targetTransform = hitColliders[randomInt].transform;
            }
        }
        else
        {
            if (hitColliders.Length == 0) //No esta viendo a nadie
            {
                targetTransform = null;
            }
        }

        

        //Si el enemigo tiene un target pero esta "muerto" (capsuleCollider.enabled==false) el target se resetea a null ... en este videojuego cualquier un jugador muere, se acaba la partida, asi que ESTO NO TIENE UTILIDAD POR EL MOMENTO...
     
        /* if (targetTransform != null && targetTransform.GetComponent<CapsuleCollider>().enabled==false)
        {
            targetTransform = null;
        }*/
    }

    void MoveToTarget()
    {
        if (targetTransform != null)
        {
            SetNavDestination(targetTransform);
        }
    }

    void SetNavDestination(Transform dest)
    {
        myTransform.LookAt(dest);
        agent.SetDestination(dest.position);
        //Debug.Log(DestinationReached());
       /* if (DestinationReached())
        {
            myTransform.GetChild(0).GetComponent<Animator>().SetTrigger("isHitting");*/
            /*timer += Time.deltaTime;
            if (timer >= 5)
            {
                Debug.Log("Ataco a " + dest.name);
                //dest.GetComponent<Health>().TakeDamage(5);
                
                timer = 0;
            }*/
        //}
    }

    bool DestinationReached()
    {
        bool done = false;
        //Se comprueba si el Agente ha llegado a su destino
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    done = true;
                }
            }
        }
        return done;
    }


    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("refugePrefab"))
        {
            agent.isStopped = true;
        }
    }
}

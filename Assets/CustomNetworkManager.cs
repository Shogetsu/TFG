using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class CustomNetworkManager : NetworkManager
{
    public Button conectar;
    public Button iniciarHost;

    public InputField ip;
    public InputField puerto;

    public GameObject CanvasOffline;
    public GameObject CanvasOnline;

    public static string nombre = "";
    public InputField Nombre;



    void Start()
    {
        CanvasOnline.SetActive(false); //El canvas online no se activa

        conectar.onClick.RemoveAllListeners();
        iniciarHost.onClick.RemoveAllListeners();

        conectar.onClick.AddListener(IniciarCliente); //Se inicia como cliente
        iniciarHost.onClick.AddListener(IniciarServidor); //Se inicia como servidor

        for (int i = 0; i < prefabs.Length; i++)
        {
            ClientScene.RegisterPrefab(prefabs[i]);
        }
    }
    bool SetPort()
    {
        if (puerto.text != "")
        {
            int _puerto;
            bool b = int.TryParse(puerto.text, out _puerto);
            NetworkManager.singleton.networkPort = _puerto;
            return b;
        }
        else //Si no se indica ningun puerto, por defecto sera el siguiente
        {
            NetworkManager.singleton.networkPort = 5556;
            return true;
        }

    }
    void SetIp()
    {
        if (ip.text != "")
        {
            NetworkManager.singleton.networkAddress = ip.text;
        }
        else //Si no se indica ninguna ip, por defecto sera localhost
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }
    }
    public void IniciarCliente() //Se inicia el cliente usando el puerto indicado en el campo
    {
        if (SetPort())
        {
            SetIp(); //Se indica la IP
            NetworkManager.singleton.StartClient();
        }
    }
    public void IniciarServidor() //Se inicia el servidor usando el puerto indicado en el campo
    {
        if (SetPort())
        {
            NetworkManager.singleton.StartHost();
        }
    }
    void AlIniciar()
    {
        CanvasOffline.SetActive(false);
        CanvasOnline.SetActive(true);

        /*if (Nombre.text != "")
            nombre = Nombre.text;
        else
            nombre = Time.deltaTime.ToString();*/
    }
    public override void OnStartHost()
    {
        base.OnStartHost();
        AlIniciar(); // Se inicia el host y se cambia el canvas Offline por la Online
    }
    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        AlIniciar(); // Se inicia el cliente y se cambia el canvas Offline por la Online
    }


    ///Eleccion de personaje
    ///

    public GameObject[] prefabs;
    int prefabId;

    public void CambiarId(int id) { prefabId = id; }
    public class EleccionDeClase : MessageBase { public int idClase; }

    //SERVIDOR
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        //base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
        EleccionDeClase ec = extraMessageReader.ReadMessage<EleccionDeClase>();
        int prefabAInstanciar = ec.idClase;
        GameObject go = Instantiate(prefabs[prefabAInstanciar]) as GameObject;
        NetworkServer.AddPlayerForConnection(conn, go, playerControllerId);
    }

    //CLIENTE
    public override void OnClientConnect(NetworkConnection conn)
    {
        //base.OnClientConnect(conn);
        EleccionDeClase ec = new EleccionDeClase();
        ec.idClase = prefabId;
        ClientScene.AddPlayer(conn, 0, ec);
    }
}

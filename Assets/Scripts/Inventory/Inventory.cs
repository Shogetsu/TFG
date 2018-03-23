using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;


public class Inventory : NetworkBehaviour {
   
    /*#region Singleton

    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Se han encontrado mas de una instancia de Inventory");
            return;
        }

        instance = this;
    }

    #endregion*/

    /*Para saber cuando se ha producido un cambio en el inventario*/
    //public delegate void OnItemChanged();
   // public OnItemChanged onItemChangedCallback;

    public Transform itemsParent;
    public int space = 9;
    InventorySlot[] slots;

    //public SyncListString items = new SyncListString();
    public struct ItemStruct
    {
        public string itemName;
        public int quantity;
    };

    public class SyncListItem : SyncListStruct<ItemStruct> { }
    public SyncListItem items = new SyncListItem();

    //  public Transform inventory;


    /*public List<Item> items = new List<Item>();

    public bool Add (Item item)
    {
        if (!item.isDefaultItem)
        {
           
            if(items.Count >= space)
            {
                Debug.Log("No hay espacio en el inventario");
                return false;
            }
            items.Add(item);
            Debug.Log("Inventario a " + items.Count + " de " + space);

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
            
        }
        return true;
    }*/

    /*  public void Remove (Item item)
      {
          items.Remove(item);

          if (onItemChangedCallback != null)
          {
              onItemChangedCallback.Invoke();
          }
      }*/


    void Update()
    {

        // Debug.Log("Cliente: "+ isClient+" Autoridad: "+hasAuthority);

        /*Debug.Log("******");
        Debug.Log(isClient);
        Debug.Log(isServer);
        Debug.Log(hasAuthority);
        Debug.Log(isLocalPlayer);
        Debug.Log("******");*/

        if (!isLocalPlayer) {
            return;
        }



        int num;
        if (int.TryParse(Input.inputString, out num))
        {
            Debug.Log("Estoy aca "+num);
            CmdUseItem(num);
        }

        /*if (inventory == null)
            inventory = GameObject.Find("Canvas").transform.GetChild(0).transform;*/


        /* if (num-1 < Inventory.instance.items.Count)
         {
             if (num >= 1 && num <= 9)
             {
                 slots[num - 1].UseItem();
             }
         }*/
    }

    [Command]
    public void CmdUseItem(int num)
    {
        //En el command se comprueba si el jugador realmente tiene un objeto en la casilla del inventario correspondiente
        if (num - 1 < items.Count)
        {
            if (num >= 1 && num <= 9)
            {
                Debug.Log("Se va a usar el objeto de la posicion " + num);
                RpcUseItem(num);
                //items.Remove("Espada");
                items.RemoveAt(num-1);
                Debug.Log("Inventario a " + items.Count + " de " + space);
                RpcUpdateHUD();
                
            }
        }

      /*  Debug.Log("numero de items " + items.Count);
        Debug.Log("espacio max " + space);
        Debug.Log("itemsParent " + itemsParent);
        Debug.Log("slots " + slots.Length);*/
    }

    [ClientRpc]
    public void RpcUseItem(int num)
    {
       
        if (!isLocalPlayer)
        {
            return;
        }

        slots[num - 1].UseItem();
       // CmdRemoveItem(slots[num - 1].getItemName());
    }

   /* [Command]
    public void CmdRemoveItem(string itemName)
    {
        items.Remove(itemName);
    }*/
    
 
    //  public List<string> items = new List<string>();


    public bool Add(string itemName)
    {
       /* if (!isServer)
        {
            return false;
        }*/

        if (items.Count >= space)
        {
            Debug.Log("No hay espacio en el inventario");
            return false;
        }

        bool exist = false;
       for(int i=0; i<items.Count; i++)
        {
            if(itemName.Equals(items[i].itemName)){
                //Si existe, se suma en 1 la cantidad
                exist = true;
                items[i] = new ItemStruct()
                    {
                    itemName =items[i].itemName,
                    quantity=items[i].quantity+1
                    };
                //Debug.Log(items[i].itemName + ", " + items[i].quantity);
                Debug.Log("Ya existe " + items[i].itemName + " y tiene " + items[i].quantity);
            }
        }

        if(exist==false)
        {
            //Se agrega nuevo item
            items.Add(new ItemStruct()
                {
                itemName =itemName,
                quantity=1
                });
        }

        Debug.Log("Inventario a " + items.Count + " de " + space);


      /*  if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }*/
        return true;
    }


    /*ItemPickup*/
    void OnCollisionEnter(Collision collision)
    {
        if (!isServer) //El servidor se encarga de la colision del jugador con el objeto
        {
            return;
        }

        if (collision.gameObject.tag == "Item")
        {

            bool itemExist = ItemExist(collision.gameObject.GetComponent<ItemPickup>().item.name);

            if (!itemExist) //No existe el objeto en los archivos del juego
            {
                Debug.Log("El objeto "+ collision.gameObject.GetComponent<ItemPickup>().item.name+" no existe");
                return;
            }

            //bool wasPickedUp = Inventory.instance.Add(item);

            bool wasPickedUp = Add(collision.gameObject.GetComponent<ItemPickup>().item.name);

            if (wasPickedUp == true)
            {
                NetworkServer.Destroy(collision.gameObject);
                // Destroy(gameObject);
                RpcUpdateHUD();
            }

            /*
            if (!isLocalPlayer)
                return;
                
            CmdAddItem();*/
        }
    }

    bool ItemExist(string itemName)
    {
        bool exist=false;
        Debug.Log("Intentando coger el objeto " + itemName);

        Object[] allitems = Resources.LoadAll("Items", typeof(Item));

        for (int i = 0; i < allitems.Length; i++)
        {
            if (itemName.Equals(allitems[i].name))
            {
                Debug.Log(allitems[i].name + " encontrado");
                exist = true;
            }
        }

        return exist;
    }

    [ClientRpc]
    void RpcUpdateHUD()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (itemsParent == null)
        {
            itemsParent = GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).transform;

            //itemsParent = gameObject.transform.GetChild(0).transform;
        }
           

        if(slots == null)
        {
            slots = itemsParent.GetComponentsInChildren<InventorySlot>(); //Se obtiene un array con todos los slots del inventario
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                //slots[i].AddItem(inventory.items[i]);
                //slots[i].AddItem(CmdAddItem(inventory.items[i]));
                //CmdAddItem(items[i], i);
                // slots[i].AddItem(item);

                AddItemHUD(items[i], i);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }

        Debug.Log("UPDATING HUD");
    }

   /* void UpdateHUD()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                //slots[i].AddItem(inventory.items[i]);
                //slots[i].AddItem(CmdAddItem(inventory.items[i]));
                //CmdAddItem(items[i], i);
                // slots[i].AddItem(item);
                AddItemHUD(items[i], i);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }*/




    void AddItemHUD(ItemStruct item, int pos)
    {
        Object[] allitems = Resources.LoadAll("Items", typeof(Item));

        for (int i = 0; i < allitems.Length; i++)
        {
            if (item.itemName.Equals(allitems[i].name))
            {
                Debug.Log(allitems[i].name + " encontrado");
                slots[pos].AddItem((Item)allitems[i], item.quantity);
            }
        }
    }

   /* [Command]
    void CmdAddItem(string itemName, int pos)
    {
        Debug.Log("Hola");
        Object[] allitems = Resources.LoadAll("Items", typeof(Item));

        for (int i = 0; i < allitems.Length; i++)
        {
            if (itemName.Equals(allitems[i].name))
            {
                Debug.Log(allitems[i].name + " encontrado");
                slots[pos].AddItem((Item)allitems[i]);
            }
        }

    }*/

}

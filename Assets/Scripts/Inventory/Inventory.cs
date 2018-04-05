using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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

    public Transform itemsParentInventory;
    public Transform itemsParentCrafting;
    public int space = 9;
    InventorySlot[] InventorySlots;
    CraftingSlot[] CraftingSlots;

    //public SyncListString items = new SyncListString();
    public struct ItemStruct
    {
        public string itemName;
        public int quantity;
        public string color;
        public string material;
    };

    public class SyncListItem : SyncListStruct<ItemStruct> { }
    public SyncListItem items = new SyncListItem();

    void Start()
    {
        EventManager.ItemCraftClicked += ClickCraftItem;
    }

    void Update()
    {
        if (!isLocalPlayer) {
            return;
        }

        int num;
        if (int.TryParse(Input.inputString, out num))
        {
            if(num>=1 && num <= 9)
            {
                EquipItem(num);
                //CmdUseItem(num);
            }
        }

        if (InventorySlots != null)
        {
            if (Input.GetMouseButtonDown(0) && Input.GetButton("ShowMouse") == false)
            {
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    if (InventorySlots[i].equip.activeSelf)
                    {
                        CmdUseItem(i);
                        break; //Solo habra un item equipado al mismo tiempo
                    }
                }
            }
        }
        

    }

    void EquipItem(int num)
    {
        if (InventorySlots != null)
        {
            //Solo puede haber un item equipado a la vez
           
            if (InventorySlots[num - 1].item != null)
            {
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    InventorySlots[i].SetEquipped(false);
                }

                InventorySlots[num - 1].SetEquipped(true);
            }
           
        }
    }

    [Command]
    public void CmdUseItem(int num)
    {
        //En el command se comprueba si el jugador realmente tiene un objeto en la casilla del inventario correspondiente
        if (num < items.Count)
        {
            Debug.Log("Se va a usar el objeto de la posicion " + num);
            RpcUseItem(num);
                
            //Se resta 1 objeto al inventario, solo si tienes 2 o mas
            if (items[num].quantity >= 2)
            {
                items[num] = new ItemStruct()
                {
                    itemName = items[num].itemName,
                    quantity = items[num].quantity - 1,
                    color = items[num].color,
                    material = items[num].material
                };
            }
            else //Si se tiene 1, se borra por completo del inventario
            {
                items.RemoveAt(num);
            }
                
            Debug.Log("Inventario a " + items.Count + " de " + space);
            RpcUpdateHUD(); 
        }
    }

    [ClientRpc]
    public void RpcUseItem(int num)
    {
       
        if (!isLocalPlayer)
        {
            return;
        }

        InventorySlots[num].UseItem();
    }

 
    public bool Add(string itemName, bool isResource, string color, string material)
    {

        if (items.Count >= space)
        {
            Debug.Log("No hay espacio en el inventario");
            return false;
        }

        bool exist = false;
       for(int i=0; i<items.Count; i++)
        {
            if(itemName.Equals(items[i].itemName) && isResource){
                //Si existe, se suma en 1 la cantidad
                exist = true;
                items[i] = new ItemStruct()
                    {
                    itemName =items[i].itemName,
                    quantity=items[i].quantity+1,
                    color=items[i].color,
                    material=items[i].material
                    };
                //Debug.Log(items[i].itemName + ", " + items[i].quantity);
                Debug.Log("Ya existe " + items[i].itemName + " y tiene " + items[i].quantity);
            }
        }

        if(exist==false || !isResource)
        {
            //Se agrega nuevo item
            items.Add(new ItemStruct()
            {
                itemName = itemName,
                quantity = 1,
                color = color,
                material=material
                });
        }

        Debug.Log("Inventario a " + items.Count + " de " + space);

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
            ItemPickup itemPickedUp = collision.gameObject.GetComponent<ItemPickup>();
            bool wasPickedUp = Add(itemPickedUp.item.name, itemPickedUp.item.isResource, itemPickedUp.item.color, itemPickedUp.item.material);

            if (wasPickedUp == true)
            {
                NetworkServer.Destroy(collision.gameObject);
                RpcUpdateHUD();
            }
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
            return;

        if (itemsParentInventory == null)
            itemsParentInventory = GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(1).transform;

        if(itemsParentCrafting == null)
            itemsParentCrafting = GameObject.Find("Canvas").transform.GetChild(1).transform.GetChild(1).transform;
           
        if(InventorySlots == null)
            InventorySlots = itemsParentInventory.GetComponentsInChildren<InventorySlot>(); //Se obtiene un array con todos los slots del inventario

        if(CraftingSlots == null)
            CraftingSlots = itemsParentCrafting.GetComponentsInChildren<CraftingSlot>();
        
        UpdateInventory(items, InventorySlots);
        UpdateCraftingMenu(InventorySlots, CraftingSlots);

        Debug.Log("UPDATING HUD");
    }

    void UpdateInventory(SyncListItem items, InventorySlot[] slots)
    {
        Debug.Log("Actualizando inventario");
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
                AddItemHUD(items[i], i);
            else
                slots[i].ClearSlot();
        }
    }

    void UpdateCraftingMenu(InventorySlot[] inventorySlots, CraftingSlot[] craftingSlots)
    {
        Debug.Log("Actualizando menu de fabricacion");
       

        for (int i=0; i<craftingSlots.Length; i++)
        {
            bool fabricable = false;

            if (craftingSlots[i].item.isPaper)
            {
                for(int j=0; j<inventorySlots.Length; j++)
                {
                    if (inventorySlots[j].item != null)
                    {
                        if (inventorySlots[j].item.isPaper)
                        {
                            if (int.Parse(inventorySlots[j].quantityText.text) >= craftingSlots[i].item.quantityNeeded)
                            {
                                Debug.Log("El item " + craftingSlots[i].item.name + " ahora es fabricable gracias a " + inventorySlots[j].item.name + " con una cantidad de " + int.Parse(inventorySlots[j].quantityText.text));
                                fabricable = true;
                                craftingSlots[i].Fabricable(fabricable);
                                craftingSlots[i].UpdateColorsCraft(inventorySlots[j].item.color);
                            }
                           /* else
                            {
                              craftingSlots[i].UpdateColorsCraft(inventorySlots[j].item.color);
                            }*/
                        }
                    }
                }
                if (!fabricable)
                {
                    craftingSlots[i].Fabricable(fabricable);
                    craftingSlots[i].UpdateColorsCraft(null);
                }
            }

            if (craftingSlots[i].item.isCardboard) {
                for (int j = 0; j < inventorySlots.Length; j++)
                {
                    if (inventorySlots[j].item != null)
                    {
                        if (inventorySlots[j].item.isCardboard)
                        {
                            if (int.Parse(inventorySlots[j].quantityText.text) >= craftingSlots[i].item.quantityNeeded)
                            {
                                Debug.Log("El item " + craftingSlots[i].item.name + " ahora es fabricable gracias a " + inventorySlots[j].item.name + " con una cantidad de " + int.Parse(inventorySlots[j].quantityText.text));
                                fabricable = true;
                                craftingSlots[i].Fabricable(fabricable);
                                break;
                            }
                        }
                    }
                }
                if (!fabricable)
                    craftingSlots[i].Fabricable(fabricable);
            }

            if (craftingSlots[i].item.isPlastic) {
                for (int j = 0; j < inventorySlots.Length; j++)
                {
                    if (inventorySlots[j].item != null)
                    {
                        if (inventorySlots[j].item.isPlastic)
                        {
                            if (int.Parse(inventorySlots[j].quantityText.text) >= craftingSlots[i].item.quantityNeeded)
                            {
                                Debug.Log("El item " + craftingSlots[i].item.name + " ahora es fabricable gracias a " + inventorySlots[j].item.name + " con una cantidad de " + int.Parse(inventorySlots[j].quantityText.text));
                                fabricable = true;
                                craftingSlots[i].Fabricable(fabricable);
                                break;
                            }
                        }
                    }
                }
                if (!fabricable)
                    craftingSlots[i].Fabricable(fabricable);
            }


        }
    }

    

    void AddItemHUD(ItemStruct item, int pos)
    {
        Object[] allitems = Resources.LoadAll("Items", typeof(Item));

        for (int i = 0; i < allitems.Length; i++)
        {
            if (item.itemName.Equals(allitems[i].name))
            {
                Debug.Log(allitems[i].name + " encontrado");
                InventorySlots[pos].AddItem((Item)allitems[i], item.quantity, item.color);
            }
        }
    }

    public void ClickCraftItem()
    {
        if (!isLocalPlayer)
            return;

        var go = EventSystem.current.currentSelectedGameObject;
        if (go != null)
        {
            CraftingSlot craftingSlot = go.transform.parent.transform.parent.transform.parent.GetComponent<CraftingSlot>();
            Debug.Log("Clicked on : " + craftingSlot.item.name);
            Debug.Log("Clicked on : " + go.name);

            //CmdCraftingItem(go.transform.parent.GetComponent<CraftingSlot>().item.name, go.transform.parent.GetComponent<CraftingSlot>().item.isResource);
            CmdCraftingItem(craftingSlot.item.name, craftingSlot.item.isResource, go.name, craftingSlot.item.material, craftingSlot.item.quantityNeeded);
        }
        else
            Debug.Log("currentSelectedGameObject is null");
    }

    [Command]
    void CmdCraftingItem(string itemName, bool isResource, string color, string material, int quantityNeeded)
    {
        RemoveResources(color, material, quantityNeeded);
        if (Add(itemName, isResource, color, material))
        {
            RpcUpdateHUD();
        }

    }

    bool RemoveResources(string color, string material, int quantityNeeded)
    {
        bool success = false;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName != null)
            {
                if (items[i].material.Equals(material) && items[i].color.Equals(color))
                {
                    if (items[i].quantity >= quantityNeeded)
                    {
                        success = true;
                        items[i] = new ItemStruct()
                        {
                            itemName = items[i].itemName,
                            quantity = items[i].quantity - quantityNeeded,
                            color = items[i].color,
                            material = items[i].material
                        };

                        if (items[i].quantity == 0)
                        {
                            items.RemoveAt(i);
                        }
                    }
                }
            }
        }

        return success;
    }
}

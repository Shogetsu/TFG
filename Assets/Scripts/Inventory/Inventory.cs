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

    [SyncVar]
    public int numItemEquipped; // 0 si no hay item equipado

    //public SyncListString items = new SyncListString();
    public struct ItemStruct
    {
        public string itemName;
        public int quantity;
        public string color;
        public string material;
        public string itemType;
        public string baseType;
        public int colorLevel;
        public bool armorEquipped;
    };

    public class SyncListItem : SyncListStruct<ItemStruct> { }
    public SyncListItem items = new SyncListItem();

    void Start()
    {
        EventManager.ItemCraftClicked += ClickCraftItem;
    }
    [Command]
    void CmdPrueba()
    {
        GetComponent<Health>().TakeDamage(1);
    }
    void Update()
    {
        if (!isLocalPlayer) {
            return;
        }
        /*if (Input.GetMouseButtonDown(0))
        {
            CmdPrueba();
        }*/
        int num;
        if (int.TryParse(Input.inputString, out num))
        {
            if(num>=1 && num <= 9)
            {
                EquipItem(num);
            }
        }

        if (InventorySlots != null)
        {
            /*Usar con clic derecho el item equipado en mano*/
            RightClickUseItemHand();

            /*Desequipar item equipado EN MANO (no armadura) con clic rueda del raton*/
            MouseWheelClickUnequipHand();
        }
    }

    void RightClickUseItemHand()
    {
        if (Input.GetMouseButtonDown(1) && Input.GetButton("ShowMouse") == false)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {

                   if (InventorySlots[i].item.GetType().Name.Equals("Armor") && InventorySlots[i].equipArmor.activeSelf == false)
                    {
                        for (int j = 0; j < InventorySlots.Length; j++) //Se desequipan todas las armaduras...
                        {
                            InventorySlots[j].SetEquippedArmor(false);
                            CmdEquipArmor(j, false);
                        }
                        //Excepto la armadura deseada
                        InventorySlots[i].SetEquippedArmor(true);
                        CmdEquipArmor(i, true);
                    }
                    else if (InventorySlots[i].item.GetType().Name.Equals("Armor") && InventorySlots[i].equipArmor.activeSelf == true)
                    { //Si se vuelve a intentar equipar la misma armadura equipada, esta se desequipara
                        InventorySlots[i].SetEquippedArmor(false);
                        CmdEquipArmor(i, false);
                        GetComponent<Health>().CmdSetDef(0);

                        break;
                    }

                    CmdUseItem(i);
                    break; //Como solo habra un item equipado en mano al mismo tiempo, solo se usara un item a la vez
                }
            }
        }
    }

    [Command]
    void CmdEquipArmor(int num, bool e)
    {
        if (num < items.Count)
        {
            items[num] = new ItemStruct()
            {
                itemName = items[num].itemName,
                quantity = items[num].quantity,
                color = items[num].color,
                material = items[num].material,
                itemType = items[num].itemType,
                baseType = items[num].baseType,
                colorLevel = items[num].colorLevel,
                armorEquipped = e
            };
        }
    }

    void MouseWheelClickUnequipHand()
    {
        if (Input.GetMouseButtonDown(2) && Input.GetButton("ShowMouse") == false)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {
                    InventorySlots[i].SetEquipped(false);
                    CmdSetNumItemEquipped(0);
                    GetComponent<Hit>().CmdSetDamage(1); //Al desequipar el item en mano, el danyo del jugador se restablece a 1 
                    break;
                }
            }
        }
    }

    public void LosingColorLevelWeapon()
    { //Este metodo se invoca a partir de un Command en Hit
        if (items.Count == 0) return;

        if (items[numItemEquipped].itemType.Equals("Weapon")) //Solo baja el nivel de color de las armas al golpear
        {
            items[numItemEquipped] = new ItemStruct()
            {
                itemName = items[numItemEquipped].itemName,
                quantity = items[numItemEquipped].quantity,
                color = items[numItemEquipped].color,
                material = items[numItemEquipped].material,
                itemType = items[numItemEquipped].itemType,
                baseType = items[numItemEquipped].baseType,
                colorLevel = items[numItemEquipped].colorLevel - 5,
                armorEquipped = items[numItemEquipped].armorEquipped
            };

            if (items[numItemEquipped].colorLevel <= 0)
            {
                RemoveItemEquipped(numItemEquipped);
                GetComponent<Hit>().CmdSetDamage(1);
            }
            RpcUpdateHUD();
        }
    }

    public void LosingColorLevelArmor(int damage)
    {
        if (items.Count == 0) return;

        for(int i=0; i < items.Count; i++)
        {
            if (items[i].armorEquipped)
            {
                items[i] = new ItemStruct()
                {
                    itemName = items[i].itemName,
                    quantity = items[i].quantity,
                    color = items[i].color,
                    material = items[i].material,
                    itemType = items[i].itemType,
                    baseType = items[i].baseType,
                    colorLevel = items[i].colorLevel - damage,
                    armorEquipped = items[i].armorEquipped
                };

                if(items[i].colorLevel <= 0)
                {
                    RemoveArmorEquipped(i);
                    GetComponent<Health>().CmdSetDef(0);
                }
                break;
            }
        }
        RpcUpdateHUD();
    }

    [Command]
    void CmdSetNumItemEquipped(int num)
    {
        numItemEquipped = num;
    }

    void EquipItem(int num)
    {
        if (InventorySlots != null)
        {
            //Solo puede haber un item equipado a la vez
           
            if (InventorySlots[num - 1].item != null)
            {
                for (int i = 0; i < InventorySlots.Length; i++) //Se desequipan todos los items...
                {
                    InventorySlots[i].SetEquipped(false);
                }
                //...excepto el item deseado
                InventorySlots[num - 1].SetEquipped(true);
                CmdSetNumItemEquipped(num - 1);
                Debug.Log("Item equipado num: "+numItemEquipped);

                if (InventorySlots[num - 1].item.GetType().Name.Equals("Weapon")) //Si se equipa en mano un arma, los golpes que ocasione el jugador seran mas fuertes
                {
                    Weapon weapon = InventorySlots[num - 1].item as Weapon;
                    GetComponent<Hit>().CmdSetDamage(weapon.Damage);
                }
                else
                {
                    GetComponent<Hit>().CmdSetDamage(1);
                }
            }
           
        }
    }

    public bool Equipped()
    {
        bool equipped = false;
        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {
                    equipped = true;
                }
            }
        }
        return equipped;
    }

    void ModifyQuantityItem(int num, int quantity)
    {
        items[num] = new ItemStruct()
        {
            itemName = items[num].itemName,
            quantity = items[num].quantity + quantity,
            color = items[num].color,
            material = items[num].material,
            itemType = items[num].itemType,
            baseType = items[num].baseType,
            colorLevel = items[num].colorLevel,
            armorEquipped = items[num].armorEquipped
        };
    }

    [Command]
    public void CmdUseItem(int num)
    {
        //En el command se comprueba si el jugador realmente tiene un objeto en la casilla del inventario correspondiente
        if (num < items.Count)
        {
            Debug.Log("Se va a usar el objeto de la posicion " + num);
            RpcUseItem(num);

            if (items[num].baseType.Equals("FabricableItem") == false) //Se comprueba si es hijo de la clase FabricableItem, porque estos no se consumen de forma directa como el resto
            {
                if(items[num].material.Equals("Paper") || items[num].itemType.Equals("ConsumableItem"))
                {
                    /*Se consume el item utilizado solo si NO es un item fabricable, y si es de papel (los recursos de papel se pueden consumir tambien) o si es un item consumible*/

                    //Se resta 1 objeto al inventario, solo si tienes 2 o mas
                    if (items[num].quantity >= 2)
                    {
                        /* items[num] = new ItemStruct()
                         {
                             itemName = items[num].itemName,
                             quantity = items[num].quantity - 1,
                             color = items[num].color,
                             material = items[num].material,
                             itemType = items[num].itemType,
                             baseType = items[num].baseType
                         };*/
                        ModifyQuantityItem(num, -1);
                    }
                    else //Si se tiene 1, se borra por completo del inventario y se desequipa
                    {

                        RemoveItemEquipped(num);
                    }

                    Debug.Log("Inventario a " + items.Count + " de " + space);
                    
                }
            }
            RpcUpdateHUD();
        }
    }

    [ClientRpc]
    void RpcUnequipAll()
    {
        
        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++) //Se desequipan todos los items
            {
                InventorySlots[i].SetEquipped(false);
            }
            CmdSetNumItemEquipped(0);
        }
    }

    [ClientRpc]
    void RpcUnequipAllArmor()
    {

        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++) //Se desequipan todos los items
            {
                InventorySlots[i].SetEquippedArmor(false);
            }
        }
    }

    [ClientRpc]
    public void RpcUseItem(int num)
    {
       
        if (!isLocalPlayer)
        {
            return;
        }

        InventorySlots[num].UseItem(this.gameObject);
    }

 
    public bool Add(string itemName, bool isResource, string color, string material, string itemType, string baseType, int colorLevel)
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
                /* items[i] = new ItemStruct()
                     {
                     itemName =items[i].itemName,
                     quantity=items[i].quantity+1,
                     color=items[i].color,
                     material=items[i].material,
                     itemType=items[i].itemType,
                     baseType=items[i].baseType
                     };*/
                ModifyQuantityItem(i, 1);

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
                material=material,
                itemType=itemType,
                baseType=baseType,
                colorLevel=colorLevel,
                armorEquipped=false
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
            
            bool wasPickedUp = Add(itemPickedUp.item.name, itemPickedUp.item.GetType().Name.Equals("Resource"), itemPickedUp.item.color, itemPickedUp.item.material, itemPickedUp.item.GetType().Name, itemPickedUp.item.GetType().BaseType.Name, 0); //colorLevel = 0 porque los items con los que se colisiona son recursos o consumibles y estos no tienen colorLevel

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
                for(int j=0; j<inventorySlots.Length; j++)
                {
                    if (inventorySlots[j].item != null)
                    {
                        if (inventorySlots[j].item.material.Equals(craftingSlots[i].item.material))
                        {
                        FabricableItem fabricableItem = craftingSlots[i].item as FabricableItem;
                            if (int.Parse(inventorySlots[j].quantityText.text) >= fabricableItem.quantityNeeded)
                            {
                                Debug.Log("El item " + craftingSlots[i].item.name + " ahora es fabricable gracias a " + inventorySlots[j].item.name + " con una cantidad de " + int.Parse(inventorySlots[j].quantityText.text));
                                fabricable = true;
                                craftingSlots[i].Fabricable(fabricable);
                                craftingSlots[i].UpdateColorsCraft(inventorySlots[j].item.color);
                            }
                        }
                    }
                }
                if (!fabricable)
                {
                    craftingSlots[i].Fabricable(fabricable);
                    craftingSlots[i].UpdateColorsCraft(null);
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
                InventorySlots[pos].AddItem((Item)allitems[i], item.quantity, item.color, item.colorLevel, item.armorEquipped);
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
            FabricableItem fabricableItem = craftingSlot.item as FabricableItem;
            CmdCraftingItem(craftingSlot.item.name, craftingSlot.item.GetType().Name.Equals("Resource"), go.name, craftingSlot.item.material, fabricableItem.quantityNeeded, craftingSlot.item.GetType().Name, craftingSlot.item.GetType().BaseType.Name, fabricableItem.colorLevel);
        }
        else
            Debug.Log("currentSelectedGameObject is null");
    }

    [Command]
    void CmdCraftingItem(string itemName, bool isResource, string color, string material, int quantityNeeded, string itemType, string baseType, int colorLevel)
    {
        RemoveResources(color, material, quantityNeeded);
        if (Add(itemName, isResource, color, material, itemType, baseType, colorLevel))
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
                        /* items[i] = new ItemStruct()
                         {
                             itemName = items[i].itemName,
                             quantity = items[i].quantity - quantityNeeded,
                             color = items[i].color,
                             material = items[i].material,
                             itemType = items[i].itemType,
                             baseType = items[i].baseType
                         };*/
                        ModifyQuantityItem(i, -quantityNeeded);
                        if (items[i].quantity == 0)
                        {
                            RemoveItemEquipped(i);
                        }
                    }
                }
            }
        }

        return success;
    }

    void RemoveItemEquipped(int num)
    {
        items.RemoveAt(num);
        RpcUnequipAll();
    }

    void RemoveArmorEquipped(int num)
    {
        items.RemoveAt(num);
        RpcUnequipAllArmor();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Inventory : NetworkBehaviour {

    public Transform itemsParentInventory;
    public Transform itemsParentCrafting;
    public int space = 9;

    InventorySlot[] InventorySlots;
    CraftingSlot[] CraftingSlots;

    [SyncVar]
    public int numItemEquipped; // 0 si no hay item equipado

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

    public GameObject itemHand;
    public GameObject weaponHand;
    public GameObject armor;

    [SyncVar]
    string spriteNameHand;

    private AudioManager audioManager;

    void Start()
    {
        EventManager.ItemCraftClicked += ClickCraftItem;

        //audioManager
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found!");
        }
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
        /* if(Physics.SphereCast(transform.position, 2, transform.forward, out hit, 3))
         {
             print("Found an object - distance: " + hit.distance + " " + hit.collider.gameObject.name);
             Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
         }*/

        /*  Debug.DrawRay(GetComponent<PjControl>().playerModel.transform.position + new Vector3(0f, 0.75f, 0f), transform.TransformDirection(GetComponent<PjControl>().playerModel.transform.forward) * 100, Color.black);
          if (Physics.Raycast(GetComponent<PjControl>().playerModel.transform.position + new Vector3(0f, 0.75f, 0f), GetComponent<PjControl>().playerModel.transform.forward, out hit))
          {
             // Debug.DrawRay(GetComponent<PjControl>().playerModel.transform.position + new Vector3(0f, 0.75f, 0f), transform.TransformDirection(GetComponent<PjControl>().playerModel.transform.forward) * hit.distance, Color.black);
              print("Found an object - distance: " + hit.distance + " " + hit.collider.gameObject.name);

          }*/
        //posRaycastOrigen.position = new Vector3(GetComponent<PjControl>().playerModel.transform.position.x, GetComponent<PjControl>().playerModel.transform.position.y - 1.5f, GetComponent<PjControl>().playerModel.transform.position.z);
        
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

            /*Dejar caer el item equipado en mano*/
            if (Input.GetButtonDown("DropItem") && items[numItemEquipped].armorEquipped==false)
            {
                if (DropItem(GetItemNameEquipped(), IsFabricableItemEquipped(), GetItemMaterialHand(), GetColorItemHand(), GetQuantityNeededItemHand()))
                {
                    if (InventorySlots[numItemEquipped].item.GetType().Name.Equals("Weapon"))
                    {
                        GetComponent<Hit>().CmdSetDamage(1);
                        CmdActiveWeaponHand(false);
                    }
                   /* if (InventorySlots[numItemEquipped].item.GetType().Name.Equals("Armor"))
                        GetComponent<Health>().CmdSetDef(0);*/

                    //Debug.Log("echando");

                    //InventorySlots[numItemEquipped].SetEquipped(false);
                    //CmdActiveWeaponHand(false);
                    
                    CmdRemoveItemDropped(numItemEquipped);
                    audioManager.PlaySound("Drop");
                   // CmdSetNumItemEquipped(0);

                }
            }
        }
    }

    int GetQuantityNeededItemHand()
    {
        int q = 0;
        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {
                    if (InventorySlots[i].item.GetType().Name.Equals("Weapon") ||
                        InventorySlots[i].item.GetType().Name.Equals("Armor") ||
                        InventorySlots[i].item.GetType().Name.Equals("RefugeCraft"))
                    {
                        FabricableItem item = InventorySlots[i].item as FabricableItem;
                        Debug.Log("Es el item: " + item.name + " con cantidad: " + item.quantityNeeded);

                        q = item.quantityNeeded;
                    }
                }
            }
        }
        return q;
    }

    string GetItemMaterialHand()
    {
        string mat = null;
        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {
                    mat = InventorySlots[i].item.material;
                }
            }
        }
        return mat;
    }


  /*  string GetItemColorEquipped()
    {
        string col = null;
        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {
                    col = InventorySlots[i].item.color;
                }
            }
        }
        return col;
    }*/


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
                        CmdActiveArmorModel(true);
                        CmdSetArmorMaterial(InventorySlots[i].color, InventorySlots[i].item.material);
                    }
                    else if (InventorySlots[i].item.GetType().Name.Equals("Armor") && InventorySlots[i].equipArmor.activeSelf == true)
                    { //Si se vuelve a intentar equipar la misma armadura equipada, esta se desequipara
                        InventorySlots[i].SetEquippedArmor(false);
                        CmdEquipArmor(i, false);
                        CmdActiveArmorModel(false);
                        break;
                    }

                    CmdUseItem(i);
                    break; //Como solo habra un item equipado en mano al mismo tiempo, solo se usara un item a la vez
                }
            }
        }
    }

    [Command]
    void CmdSetArmorMaterial(string color, string material)
    {

        RpcSetArmorMaterial(color, material);
    }


    [ClientRpc]
    void RpcSetArmorMaterial(string color, string material)
    {
        //Debug.Log("Soy null?: " + color);
        armor.GetComponent<Renderer>().material = GetMaterial(color);
        armor.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
        armor.GetComponent<Renderer>().material.SetTexture("_BumpMap", GetNormalMap(material));

        /*Renderer[] rends = armor.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            r.GetComponent<Renderer>().material = GetMaterial(color);
            r.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
            r.GetComponent<Renderer>().material.SetTexture("_BumpMap", GetNormalMap(material));
        }*/
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

        if (e == false)
            GetComponent<Health>().CmdSetDef(0);
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
                    CmdSetSpriteHand(null);
                    CmdActiveWeaponHand(false);
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
                CmdActiveWeaponHand(false);
                audioManager.PlaySound("Break");
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
                    CmdActiveArmorModel(false);
                    audioManager.PlaySound("Break");
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
                Debug.Log("Item equipado num: " + numItemEquipped);

                if (InventorySlots[num - 1].item.GetType().Name.Equals("Weapon")) //Si se equipa en mano un arma, los golpes que ocasione el jugador seran mas fuertes
                {
                    Weapon weapon = InventorySlots[num - 1].item as Weapon;
                    GetComponent<Hit>().CmdSetDamage(weapon.Damage);
                    CmdSetSpriteHand(null);
                    CmdActiveWeaponHand(true);
                    CmdSetWeaponMaterial(InventorySlots[num - 1].color, InventorySlots[num - 1].item.material);
                }
                else
                {
                    GetComponent<Hit>().CmdSetDamage(1);
                    CmdSetSpriteHand(InventorySlots[num - 1].icon.sprite.name);
                    CmdActiveWeaponHand(false);
                }

               /* for(int i=0; i<InventorySlots.Length; i++)
                {
                    Debug.Log("Pos: "+i+","+InventorySlots[i].color);
                }*/
            }
           
        }
    }

    [Command]
    void CmdSetWeaponMaterial(string color, string material)
    {
        RpcSetWeaponMaterial(color, material);
    }


    [ClientRpc]
    void RpcSetWeaponMaterial(string color, string material)
    {
        //Debug.Log("Soy null?: " + color);
        Renderer[] rends = weaponHand.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            r.GetComponent<Renderer>().material = GetMaterial(color);
            r.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
            r.GetComponent<Renderer>().material.SetTexture("_BumpMap", GetNormalMap(material));
        }
    }


    [Command]
    void CmdActiveWeaponHand(bool active)
    {
       // weaponHand.gameObject.SetActive(active);
        RpcUpdateWeaponHand(active);
    }

    [ClientRpc]
    void RpcUpdateWeaponHand(bool active)
    {
        weaponHand.gameObject.SetActive(active);
    }

    [Command]
    void CmdActiveArmorModel(bool active)
    {
        RpcUpdateArmorModel(active);
    }

    [ClientRpc]
    void RpcUpdateArmorModel(bool active)
    {
        armor.gameObject.SetActive(active);
    }

    [Command]
    void CmdSetSpriteHand(string spriteName)
    {
        spriteNameHand = spriteName;
        RpcUpdateSpriteHand(spriteNameHand);
    }

    [ClientRpc]
    void RpcUpdateSpriteHand(string spriteName)
    {
        if (spriteName != "")
        {
            Object[] allitems = Resources.LoadAll("Sprites", typeof(Sprite));

            for (int i = 0; i < allitems.Length; i++)
            {
               // Debug.Log(spriteName + " | " + allitems[i].name);
                if (spriteName.Equals(allitems[i].name))
                {
                    Sprite spriteItem = allitems[i] as Sprite;
                    itemHand.GetComponent<SpriteRenderer>().sprite = spriteItem;
                    break;
                }
            }
        }
        else
        {
            itemHand.GetComponent<SpriteRenderer>().sprite = null;
        }
        
    }

    public string GetItemNameEquipped()
    {
        string itemName = null;
        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {
                    itemName=InventorySlots[i].item.name;
                }
            }
        }
        return itemName;
    }

    bool IsFabricableItemEquipped()
    {
        bool isFabricableItem = true;
        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {
                    if (InventorySlots[i].item.GetType().Name.Equals("Resource") || InventorySlots[i].item.GetType().Name.Equals("Consumable"))
                    {
                        isFabricableItem = false;
                        break;
                    }
                }
            }
        }
        return isFabricableItem;
    }

   /* public int GetItemPosEquipped()
    {
        int itemPos=-1;
        if (InventorySlots != null)
        {
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].equip.activeSelf)
                {
                    itemPos = i;
                }
            }
        }
        return itemPos;
    }*/

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
            //|| items[num].itemType.Equals("RefugeCraft")
            if (items[num].baseType.Equals("FabricableItem") == false || items[num].itemType.Equals("RefugeCraft")) //Se comprueba si es hijo de la clase FabricableItem, porque estos no se consumen de forma directa como el resto
            {
                if(items[num].material.Equals("Paper") || items[num].itemType.Equals("Consumable") || items[num].itemType.Equals("RefugeCraft"))
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
                        CmdSetSpriteHand(null);
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

        
       // Debug.Log("Inventario a " + items.Count + " de " + space);
     /*   for (int j = 0; j < items.Count; j++)
        {
            Debug.Log("pos: " + j + " nombre: " + items[j].itemName + " color: " + items[j].color);
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
            ItemPickup itemPickedUp = collision.gameObject.GetComponent<ItemPickup>();
            
            bool wasPickedUp = Add(itemPickedUp.item.name, itemPickedUp.item.GetType().Name.Equals("Resource"), itemPickedUp.item.color, itemPickedUp.item.material, itemPickedUp.item.GetType().Name, itemPickedUp.item.GetType().BaseType.Name, 0); //colorLevel = 0 porque los items con los que se colisiona son recursos o consumibles y estos no tienen colorLevel

            if (wasPickedUp == true)
            {
                audioManager.PlaySound("Pop01");
                NetworkServer.Destroy(collision.gameObject);
                RpcUpdateHUD();
            }
        }
    }

    bool ItemExist(string itemName)
    {
        bool exist=false;
        //Debug.Log("Intentando coger el objeto " + itemName);

        Object[] allitems = Resources.LoadAll("Items", typeof(Item));

        for (int i = 0; i < allitems.Length; i++)
        {
            if (itemName.Equals(allitems[i].name))
            {
                //Debug.Log(allitems[i].name + " encontrado");
                exist = true;
            }
        }

        return exist;
    }

    bool DropItem(string itemName, bool isFabricableItem, string material, string color, int quantity)
    {

        if (itemName == null) return false;

        Debug.Log("Intentando dejar caer el objeto " + itemName+ " de color "+color);

        
        CmdSpawnDropItem(itemName, isFabricableItem, material, color, quantity);

        return true;
    }

    [Command]
    void CmdSpawnDropItem(string itemName, bool isFabricableItem, string material, string color, int quantity)
    {
        Object[] allitems = Resources.LoadAll("Prefabs", typeof(GameObject));

        if (!isFabricableItem)
        {
            /* Si se trata de un recurso, simplemente con buscar el prefab del recurso en la carpeta de prefabs y hacer el spawn es suficiente*/
            for (int i = 0; i < allitems.Length; i++)
            {
                GameObject itemGameobject = (GameObject)allitems[i];
                Debug.Log("Quiero tirar: " + itemName + " / encontrado: " + itemGameobject.GetComponent<ItemPickup>().item.name);
                if (itemName.Equals(itemGameobject.GetComponent<ItemPickup>().item.name))
                {
                    Debug.Log(allitems[i].name + " encontrado");
                    NetworkServerSpawnItem((GameObject)allitems[i]);
                    break;
                }
            }
        }
        else
        {
            for (int j = 0; j < allitems.Length; j++)
            {
                GameObject go = (GameObject)allitems[j];
                if (go.GetComponent<ItemPickup>().item.GetType().Name.Equals("Resource") &&
                    go.GetComponent<ItemPickup>().item.color.Equals(color) &&
                    go.GetComponent<ItemPickup>().item.material.Equals(material))
                {
                    //Debug.Log(allitems[j].name + " encontrado");
                    //Debug.Log("CANTIDA!!" + quantity);
                    int numResources = Random.Range(1, (int)(quantity / 1.5f) + 1); //Los posibles recursos recuperados al destruir el objeto podran ser entre 1 y la cantidad total necesaria entre 1,5 (un 60%) (y +1 porque el valor maximo de Random.Range esta excluido)
                    //Debug.Log("CANTIDA QUE VOY A ESHA!!" + numResources);

                    int r = 0;
                    while (r < numResources)
                    {
                        Debug.Log("r: " + r);
                        NetworkServerSpawnItem((GameObject)allitems[j]);
                        r++;
                    }
                }
            }
        }
    }


    [Command]
    public void CmdBuildRefuge(string itemName, string color)
    {
        Object[] allitems = Resources.LoadAll("Prefabs", typeof(GameObject));

        for (int i = 0; i < allitems.Length; i++)
        {
           // Debug.Log("Busco: " + itemName + " encuentro: " + allitems[i].name);
            if (allitems[i].name.Equals(itemName))
            {
                NetworkServerSpawnRefuge(allitems[i] as GameObject, color);
                break;
            }
        }
    }

    public void NetworkServerSpawnRefuge(GameObject itemDropped, string color)
    {
        Vector3 playerPos = this.transform.position;
        Quaternion rotation = itemDropped.transform.rotation;
        playerPos = new Vector3(this.transform.position.x + Random.Range(-0.1f, 0.25f), this.transform.position.y + 5f, this.transform.position.z + Random.Range(-0.25f, 0.25f)); //Pequenya aleatoriedad de la posicion donde caen las objetos al tirarlos al suelo
        Vector3 playerDirection = this.transform.forward;

        Vector3 spawnPos = playerPos + playerDirection;

        GameObject itemSpawn = Instantiate(itemDropped, spawnPos, rotation);

        NetworkServer.Spawn(itemSpawn);
        RpcSetRefugeMaterial(itemSpawn, color);

    }

    [ClientRpc]
    void RpcSetRefugeMaterial(GameObject refuge, string color)
    {
       // Debug.Log("Soy null?: "+refuge);
        refuge.GetComponent<Renderer>().material = GetMaterial(color);
        refuge.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
        refuge.GetComponent<Renderer>().material.SetTexture("_BumpMap", GetNormalMap(refuge.GetComponent<ItemPickup>().item.material));
    }

    public void NetworkServerSpawnItem(GameObject itemDropped)
    {
        Vector3 playerPos = this.transform.position;
        playerPos = new Vector3(this.transform.position.x + Random.Range(-0.1f, 0.25f), this.transform.position.y, this.transform.position.z + Random.Range(-0.25f, 0.25f)); //Pequenya aleatoriedad de la posicion donde caen las objetos al tirarlos al suelo
        Vector3 playerDirection = this.transform.forward;

        Quaternion rotation = this.transform.rotation; 

       // Quaternion playerRotation = this.transform.rotation;
       // float spawnDistance = 1;
        Vector3 spawnPos = playerPos + playerDirection;

        GameObject itemSpawn = Instantiate(itemDropped, spawnPos, rotation);
        
        // itemSpawn.transform.LookAt(this.transform);
        // Debug.Log("Intentando spawnear " + itemSpawn.name);
        NetworkServer.Spawn(itemSpawn);
    }

    Material GetMaterial(string color)
    {
        Material mat = null;

        Object[] allitems = Resources.LoadAll("Materials", typeof(Material));
        for (int i = 0; i < allitems.Length; i++)
        {
            if (allitems[i].name.Equals(color))
            {
                mat = allitems[i] as Material;
                break;
            }
        }

        return mat;
    }

    Texture2D GetNormalMap(string material)
    {
        Texture2D tex = null;
        Object[] allitems = Resources.LoadAll("NormalMap", typeof(Texture2D));
        for (int i = 0; i < allitems.Length; i++)
        {
            //Debug.Log("Busco: " + material + " encuentro: " + allitems[i].name);
            if (allitems[i].name.Equals(material))
            {
                tex = allitems[i] as Texture2D;
            }
        }
        return tex;
    }


    public string GetColorItemHand()
    {
        string itemColor = null;

        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i].equip.activeSelf)
            {
                itemColor = InventorySlots[i].color;
            }
        }
        return itemColor;
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
                        if (inventorySlots[j].item.material.Equals(craftingSlots[i].item.material) &&
                        inventorySlots[j].item.GetType().Name.Equals("Resource"))
                        {
                            FabricableItem fabricableItem = craftingSlots[i].item as FabricableItem;
                            if (int.Parse(inventorySlots[j].quantityText.text) >= fabricableItem.quantityNeeded)
                            {
                                //Debug.Log("El item " + craftingSlots[i].item.name + " ahora es fabricable gracias a " + inventorySlots[j].item.name + " con una cantidad de " + int.Parse(inventorySlots[j].quantityText.text));
                                 fabricable = true;
                                craftingSlots[i].Fabricable(true);
                                craftingSlots[i].UpdateColorsCraft(inventorySlots[j].item.color, true);
                            }
                            else
                            {
                                craftingSlots[i].UpdateColorsCraft(inventorySlots[j].item.color, false);
                            }
                        }
                    }
                }
                if (!fabricable)
                {
                    craftingSlots[i].Fabricable(false);
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
                //Item newItem = allitems[i] as Item;
                //Debug.Log("Add--->" + newItem.name + "/"+item.color);
                //InventorySlots[pos].ClearSlot();
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
            audioManager.PlaySound("Craft");
        }

    }

    bool RemoveResources(string color, string material, int quantityNeeded)
    {
        bool success = false;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName != null)
            {
                if (items[i].material.Equals(material) && items[i].color.Equals(color) && items[i].itemType.Equals("Resource"))
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
                            CmdSetSpriteHand(null);
                            CmdActiveWeaponHand(false);
                            GetComponent<Hit>().CmdSetDamage(1); //Al desequipar el item en mano, el danyo del jugador se restablece a 1 
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
        GetComponent<Health>().CmdSetDef(0);
        items.RemoveAt(num);
        RpcUnequipAllArmor();
    }

    [Command]
    void CmdRemoveItemDropped(int num)
    {
        ModifyQuantityItem(num, -1);
        if (items[num].quantity == 0)
        {
            RemoveItemEquipped(num);
            CmdSetNumItemEquipped(0);
            CmdSetSpriteHand(null);
        }

        RpcUpdateHUD();
    }
}

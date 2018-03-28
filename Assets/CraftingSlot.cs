using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour {

    public Image icon;
    public Item item;
    public bool isFabricable;
    public Button craftButton;
    public Transform itemsParentColors;
    Button[] buttonColors;

    // Use this for initialization
    void Start () {
        icon.sprite = item.icon;
        isFabricable = false;
        craftButton.interactable = false;
        icon.color = new Color(1, 1, 1, 0.47f);
        itemsParentColors = this.transform.GetChild(0).transform.GetChild(0).transform;
        buttonColors = itemsParentColors.GetComponentsInChildren<Button>();
    }

    // Update is called once per frame
    void Update () {

    }

    public void Fabricable(bool fabricable)
    {
        if (fabricable)
        {
            craftButton.interactable = true;
            isFabricable = fabricable;
            icon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            craftButton.interactable = false;
            isFabricable = fabricable;
            icon.color = new Color(1, 1, 1, 0.47f);
        }
    }

    public void UpdateColorsCraft(string color)
    {
        //Button buttonColor = CraftingSlots[pos].transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.gameObject.GetComponent<Button>();
       // Button blueButton = this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.gameObject.GetComponent<Button>();

        if (isFabricable)
        {
            if (color.Equals("Red"))
                buttonColors[0].interactable = true;
            if (color.Equals("LightBlue"))
                buttonColors[1].interactable = true;
            if (color.Equals("Yellow"))
                buttonColors[2].interactable = true;
            if (color.Equals("Green"))
                buttonColors[3].interactable = true;
            if (color.Equals("Pink"))
                buttonColors[4].interactable = true;
            if (color.Equals("Blue"))
                buttonColors[5].interactable = true;
        }
        else
        {
            for(int i=0; i<=5; i++)
                buttonColors[i].interactable = false;
        }
    }

}

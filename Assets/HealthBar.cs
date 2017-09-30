using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    //Forma cutre de hacer un HUD para las barras de salud
    //Ahora se hace de forma grafica y es mas facil y bonico, no volver a hacerlo pls
    GUIStyle healthStyle; //barra de salud
    GUIStyle backStyle; //barra de fondo de la barra de salud
    Combat combat;

    void Awake()
    {
        combat = GetComponent<Combat>();
    }

    void OnGUI()
    {
        InitStyles();
        //posicionar la barra de vida sobre cada jugador

        //IMPORTANTE con WorldToScreenPoint convertimos coordenadas 3D en 2D de la pantalla.
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position); //debe ser Vector3 siempre, aunque la Z=0

        GUI.color = Color.gray;
        GUI.backgroundColor = Color.gray;
        GUI.Box(new Rect(pos.x - 26, Screen.height - pos.y + 20, Combat.maxHealth/2, 7), ".", backStyle);

        GUI.color = Color.green;
        GUI.backgroundColor = Color.green;
        GUI.Box(new Rect(pos.x - 25, Screen.height - pos.y + 21, combat.health / 2, 5), ".", healthStyle); //salud actual
    }

    void InitStyles()
    {
        if (healthStyle == null)
        {
            healthStyle = new GUIStyle(GUI.skin.box);
            healthStyle.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 1.0f));
        }

        if (backStyle == null)
        {
            backStyle = new GUIStyle(GUI.skin.box);
            backStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 1.0f));
        }
    }

    Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for(int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

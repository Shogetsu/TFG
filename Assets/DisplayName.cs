using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Esto es solo para la cabecera de variables
using Steamworks; //importante para usar funciones de la API de Steam

public class DisplayName : MonoBehaviour {

    //Cabecera para agrupar variables
    [Header("UI Components")]
    public Text displayName;
    public Image avatarImage;


    // Use this for initialization
    void Start () {
        
        //Comprobar que SteamManager se ha inicializado
        if (!SteamManager.Initialized)
        {
            return; //Si no se ha inicializado, muere el script
        }

        //Nombre de usuario
        
        displayName.text = SteamFriends.GetPersonaName();
        //Avatar
        StartCoroutine(_FetchAvatar());
    }

    int avatarInt;
    uint width, height;
    Texture2D downloadedAvatar;
    Rect rect = new Rect(0, 0, 184, 184);
    Vector2 pivot = new Vector2(0.5f, 0.5f);
    IEnumerator _FetchAvatar()
    {
        avatarInt = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
        while (avatarInt == -1)
        {
            yield return null;
        }

        if (avatarInt > 0){
            SteamUtils.GetImageSize(avatarInt, out width, out height);

            if(width > 0 && height > 0)
            {
                byte[] avatarSteam = new byte[4 * (int)width * (int)height];

                SteamUtils.GetImageRGBA(avatarInt, avatarSteam, 4 * (int)width * (int)height);

                downloadedAvatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                downloadedAvatar.LoadRawTextureData(avatarSteam);
                downloadedAvatar.Apply();

                avatarImage.sprite = Sprite.Create(downloadedAvatar, rect, pivot);
            }
        }
    }
}

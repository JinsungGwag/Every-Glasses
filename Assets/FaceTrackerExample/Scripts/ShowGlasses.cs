using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGlasses : MonoBehaviour {

    public string url = "https://s3.ap-northeast-2.amazonaws.com/cliegh2/logo.png";

    IEnumerator Start()
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        WWW www = new WWW(url);
        yield return www;
        www.LoadImageIntoTexture(tex);
        Sprite img = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new
            Vector2(0.5f, 0.5f));
        GetComponent<Image>().sprite = img;
    }

    float Resize(Sprite spr, int size, bool bigWidth)
    {
        Sprite newSpr = spr;
        
        if (bigWidth)
        {
            float ratio = size / spr.rect.width;
            return spr.rect.height * ratio;
        }
        else{
            float ratio = size / spr.rect.height;
            return spr.rect.width * ratio;
        }
    }
}

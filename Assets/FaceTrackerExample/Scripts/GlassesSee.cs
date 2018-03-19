using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassesSee : MonoBehaviour {

    public GameObject[] glPhoto;
    public Text[] glName;
    public Text[] glPrice;
 
	// Use this for initialization
	void Start () {

        for (int i = 0; i < Recommend.Response.db.Length; i++)
        {
            glName[i].text = Recommend.Response.db[i].name;
            glPrice[i].text = Recommend.Response.db[i].price + "원";
            StartCoroutine(Load(Recommend.Response.db[i].url, i));
        }
        
    }

    IEnumerator Load(string url, int index)
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        WWW www = new WWW(url);
        yield return www;
        www.LoadImageIntoTexture(tex);

        float wid = (tex.width > tex.height) ? 290 : Resize(tex, 290, false);
        float hei = (tex.width <= tex.height) ? 290 : Resize(tex, 290, true);
        
        Sprite img = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        Debug.Log(url);
        glPhoto[index].GetComponent<Image>().sprite = img;
        glPhoto[index].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(wid, hei);
    }

    float Resize(Texture2D tex, int size, bool bigWidth)
    {
        if (bigWidth)
        {
            float ratio = (float)size / tex.width;
            return tex.height * ratio;
        }
        else
        {
            float ratio = (float)size / tex.height;
            return tex.width * ratio;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}

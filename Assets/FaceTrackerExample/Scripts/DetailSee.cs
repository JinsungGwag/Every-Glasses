using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailSee : MonoBehaviour {

    public GameObject Photo;
    public Text Model;
    public Text Price;
    public Text Rate;

	// Use this for initialization
	void Start () {

        Model.text = Recommend.Response.db[Recommend.glNumber].name;
        Price.text = Recommend.Response.db[Recommend.glNumber].price + "원";
        Rate.text = Recommend.Response.db[Recommend.glNumber].rate + "명이 이 안경을 구매하였습니다.";
        StartCoroutine(Load(Recommend.Response.db[Recommend.glNumber].url));

    }

    IEnumerator Load(string url)
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        WWW www = new WWW(url);
        yield return www;
        www.LoadImageIntoTexture(tex);

        float wid = (tex.width > tex.height) ? 380 : Resize(tex, 380, false);
        float hei = (tex.width <= tex.height) ? 380 : Resize(tex, 380, true);

        Sprite img = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        Debug.Log(url);
        Photo.GetComponent<Image>().sprite = img;
        Photo.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(wid, hei);
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
    
}

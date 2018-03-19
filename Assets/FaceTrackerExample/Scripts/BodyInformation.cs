using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyInformation : MonoBehaviour {

    public Text Body;

	// Use this for initialization
	void Start () {

        string Bodytxt = "";
        Bodytxt += Recommend.Response.age.value + "세 ";
        
        if (Recommend.Response.gender.value == "male")
            Bodytxt += "남성";
        else
            Bodytxt += "여성";

        Body.text = Bodytxt;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

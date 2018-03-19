using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAnmation2 : MonoBehaviour {
    
    private float speed;
    private Text txt;

	// Use this for initialization
	void Start () {
        speed = 0;
        txt = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

        txt.color = new Color(255, 255, 255, Mathf.PingPong(speed, 1));
        speed += 0.03f;

	}
}

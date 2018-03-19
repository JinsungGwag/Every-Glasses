using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAnimation : MonoBehaviour {

    private float size;
    private float dest;

    private void Start()
    {
        //size = 1f;
        //dest = 1.4f;
    }

    // Update is called once per frame
    void Update () {

        transform.Rotate(new Vector3(0, 0, 5f));

        //size = Mathf.Lerp(size, dest, 0.05f);

        //if (size > 1.3f)
        //    dest = 1f;
        //else if(size < 1.1f)
        //    dest = 1.4f;

        //transform.localScale = new Vector3(size, size, size);
        
	}
    

}

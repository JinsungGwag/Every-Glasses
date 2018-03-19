using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaterGoStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(GoBack());
	}

    void Update()
    {
        if(Input.touchCount > 0)
        {
            StopAllCoroutines();
        }
        else
        {
            StartCoroutine(GoBack());
        }
    }

    IEnumerator GoBack()
    {
        yield return new WaitForSeconds(60f);

        SceneManager.LoadScene("Start");
    }
}

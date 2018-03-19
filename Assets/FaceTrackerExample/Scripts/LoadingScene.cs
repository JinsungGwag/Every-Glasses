using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {

	void Start () {

        StartCoroutine(LoadResult());

	}
	
    IEnumerator LoadResult()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("RecommendGlasses");
    }

}

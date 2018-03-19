using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetailButton : MonoBehaviour {

    public void gotoShot()
    {
        SceneManager.LoadScene("WebCamTextureFaceTrackerExample");
    }

    public void gotoRecommend()
    {
        SceneManager.LoadScene("RecommendGlasses");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecommendButton : MonoBehaviour {

    public void gotoShot()
    {
        SceneManager.LoadScene("WebCamTextureFaceTrackerExample");
    }

    public void gotoGlass1()
    {
        Recommend.glNumber = 0;
        SceneManager.LoadScene("DetailGlasses");
    }

    public void gotoGlass2()
    {
        Recommend.glNumber = 1;
        SceneManager.LoadScene("DetailGlasses");
    }

    public void gotoGlass3()
    {
        Recommend.glNumber = 2;
        SceneManager.LoadScene("DetailGlasses");
    }

}

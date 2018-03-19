using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainControl : MonoBehaviour {
    
    //public Text Age;
    //public Toggle Female;
    //public Toggle Male;
    //public Dropdown Price;

    //public static String pAge;
    //public static String pGender;
    //public static String pPrice;

    //public GameObject WarnMessage;

    private void Start()
    {
        //WarnMessage.SetActive(false);
    }

    public void GoCamera()
    {

        SceneManager.LoadScene("WebCamTextureFaceTrackerExample");

    }
   
    //IEnumerator WarnAge()
    //{
    //    WarnMessage.SetActive(true);

    //    yield return new WaitForSeconds(1.5f);

    //    WarnMessage.SetActive(false);
    //}

}

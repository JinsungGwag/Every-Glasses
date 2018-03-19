using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMap : MonoBehaviour {

    private int[] selMap;
    public GameObject[] Maps;
    
    private void Start()
    {
        selMap = Recommend.selMap;
        Maps[selMap[Recommend.glNumber]].SetActive(true);
    }

}

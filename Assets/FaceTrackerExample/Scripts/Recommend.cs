using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recommend : MonoBehaviour
{

    [System.Serializable]
    public class JsonResponse
    {
        public JsonDB[] db;
        public JsonValue gender;
        public JsonValue age;
    }

    [System.Serializable]
    public class JsonValue
    {
        public string value;
        public double confidence;
        
    }

    [System.Serializable]
    public class JsonDB
    {
        public int id;
        public string name;
        public string shape;
        public string size;
        public string frame;
        public string color;
        public int price;
        public int priceidx;
        public int rate;
        public string url;
        
    }
    
    public static JsonResponse Response;

    public static int glNumber;

    public static int[] selMap;

}
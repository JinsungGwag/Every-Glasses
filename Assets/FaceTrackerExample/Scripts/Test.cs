using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
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

    // Use this for initialization
    void Start () {
        string json = "{\"db\":[{\"id\":82,\"name\":\"BEQ1530\",\"shape\":\"1\",\"size\":\"1\",\"frame\":\"0\",\"color\":\"0\",\"price\":112000,\"priceidx\":2,\"rate\":70,\"url\":\"https://s3.ap-northeast-2.amazonaws.com/cliegh2/testimage.png\"},{\"id\":83,\"name\":\"RFE2300\",\"shape\":\"1\",\"size\":\"1\",\"frame\":\"0\",\"color\":\"0\",\"price\":67000,\"priceidx\":1,\"rate\":73,\"url\":\"https://s3.ap-northeast-2.amazonaws.com/cliegh2/testimage.png\"},{\"id\":85,\"name\":\"AZD1002\",\"shape\":\"1\",\"size\":\"1\",\"frame\":\"0\",\"color\":\"0\",\"price\":30000,\"priceidx\":0,\"rate\":66,\"url\":\"/home/ubuntu/opt/pyshell/img/85.jpg\"}],\"gender\":{\"value\":\"female\",\"confidence\":0.999801},\"age\":{\"value\":\"15~19\",\"confidence\":0.565172}}";
        JsonResponse response = JsonUtility.FromJson<JsonResponse>(json);

        for (int i=0; i<response.db.Length; i++)
        {
            JsonDB db = response.db[i];
            Debug.Log("DB " + i);
            Debug.Log("DB.id      " + db.id);
            Debug.Log("DB.name    " + db.name    );
            Debug.Log("DB.shape   " + db.shape   );
            Debug.Log("DB.size    " + db.size    );
            Debug.Log("DB.frame   " + db.frame   );
            Debug.Log("DB.color   " + db.color   );
            Debug.Log("DB.price   " + db.price   );
            Debug.Log("DB.priceidx" + db.priceidx);
            Debug.Log("DB.rate    " + db.rate    );
            Debug.Log("DB.url     " + db.url     );
        }

        Debug.Log("Age: " + response.age.value + " (" + response.age.confidence + ")");
        Debug.Log("Gender: " + response.gender.value + " (" + response.age.confidence + ")");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

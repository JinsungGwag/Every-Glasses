using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassMove : MonoBehaviour {

    public Image[] Glasses;
    public Image Left;
    public Image Middle;
    public Image Right;

    private int num;
    private float speed;
    private bool moveOn;

    // Use this for initialization
    void Start () {

        Glasses[0].rectTransform.position = Middle.rectTransform.position;
        Glasses[1].rectTransform.position = Right.rectTransform.position;
        Glasses[2].rectTransform.position = Right.rectTransform.position;
        Glasses[3].rectTransform.position = Right.rectTransform.position;
        Glasses[4].rectTransform.position = Right.rectTransform.position;

        num = 0;
        speed = 0.5f;
        moveOn = true;
        
    }

    void Update()
    {

        if (moveOn)
        {
            if (num < 4)
                Move(num, num + 1);
            else
                Move(num, 0);
        }

        if (Glasses[num].rectTransform.position.x <= Left.rectTransform.position.x)
        {

            Glasses[num].rectTransform.position = Right.rectTransform.position;

            if (num < 4)
                Glasses[num + 1].rectTransform.position = Middle.rectTransform.position;
            else
                Glasses[0].rectTransform.position = Middle.rectTransform.position;

            if (num < 4)
                num++;
            else
                num = 0;

            speed = 0.5f;

            StartCoroutine(moveWait());
        }

    }

    private void Move(int num1, int num2)
    {

        Glasses[num1].rectTransform.Translate(new Vector3(-speed, 0, 0));
        Glasses[num2].rectTransform.Translate(new Vector3(-speed, 0, 0));
        speed += 0.1f; 

    }

    IEnumerator moveWait()
    {
        moveOn = false;

        yield return new WaitForSeconds(1.5f);

        moveOn = true;
    }

}

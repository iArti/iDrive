using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestText : MonoBehaviour
{
    // Start is called before the first frame update
    //Text toast = GameObject.Find("Canvas/Text").GetComponent<Text>();
    void Start()
    {
        Text toast = GameObject.Find("Canvas/Text").GetComponent<Text>();
        toast.text = "This is text";
    }

    // Update is called once per frame
    void Update()
    {

        Text toast = GameObject.Find("Canvas/Text").GetComponent<Text>();
        toast.text = HqRenderer.speed + "";
    }
}

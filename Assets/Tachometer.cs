using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tachometer : MonoBehaviour
{
    static string[] TachIndicators =
    {
        "",
        "I",
        "II",
        "III",
        "IIII",
        "IIIII",
        "IIIIII",
        "IIIIIII",
        "IIIIIIII",
        "IIIIIIIII",
        "IIIIIIIIII",
        "IIIIIIIIIII",
        "IIIIIIIIIIII",
        "IIIIIIIIIIIII",
        "IIIIIIIIIIIIII"
    };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Text Tach = GameObject.Find("Canvas/Tachometer").GetComponent<Text>();
        int thres = HqRenderer.maxspeed / TachIndicators.Length;
        Tach.text = HqRenderer.speed / thres + "";
    }
}

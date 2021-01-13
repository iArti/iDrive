using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text debugmenu = GameObject.Find("Canvas/Debug").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        Text debugmenu = GameObject.Find("Canvas/Debug").GetComponent<Text>();
        debugmenu.text = "Speed: " + HqRenderer.speed + "\n" + "playerX: " + HqRenderer.playerX +
            "\n" + "playerY:" + HqRenderer.playerY + "\n" + "trip: " + HqRenderer.trip + "\n" + "Accel Type: " + HqRenderer.accel + "\n" +
            "Line Y: " + HqRenderer.lineY + "\n" + "Uphill: " + HqRenderer.uphill + "\n" +"Offroad: " + HqRenderer.offroad + "\n";
    }
}

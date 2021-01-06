
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu( fileName = "New Track",  menuName = "HQ/Track")]
public class TrackObject: ScriptableObject
{
    private Texture2D[] t;
    public Sprite[] sprites;
    public Sprite[] trafic;
    public int Length;
    public Line[] lines;
    //public Line[] lines2;
    public float roadWidth;
    public int segmentLength;
    public float trackHeight;


    private void OnEnable()
    {

        t = new Texture2D[50];
        Length = 1600;
        lines = new Line[Length];
        //lines2 = new Line[Length];

        for (int i = 0; i < Length; i++)
        {
            ref Line line = ref lines[i];
            //ref Line traflines = ref lines2[i];
            line.z = i * segmentLength;
            line.w = roadWidth;
            //HqRenderer.hill = false;

            //if (line.spriteX == playerX && line.spriteY == playerY) speed = 0;

            if (i > 200 && i < 600) { line.curve = 0.5f;}
            if (i > 1000 && i < 1200) line.curve = 0.4f;

            if (i < 300 && i % 20 == 0) { 
                line.spriteX = -2f; line.sprite = sprites[0];
            }
            /*
                        if (i % 17 == 0) { line.spriteX = 2.0f; line.sprite = sprites[1]; }
                        if (i > 300 && i % 20 == 0) { line.spriteX = -1.7f; line.sprite = sprites[2]; }
                        if (i > 800 && i % 20 == 0) { line.spriteX = -1.3f; line.sprite = sprites[3]; }
                        if (i == 400) { line.spriteX = -1.3f; line.sprite = sprites[4]; } */

            if (i > 600 && i < 1200)
            {
                line.y = Mathf.Sin(i / 27.3f) * trackHeight;
                if (i % 60 == 0)
                {
                    line.spriteX = 2f; line.sprite = sprites[2];
                }
            }
        }
    }
}


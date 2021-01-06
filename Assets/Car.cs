using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Car : MonoBehaviour
{
    public Sprite[] carSprites;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) && HqRenderer.speed > 0)
        {
            if (HqRenderer.uphill == false) spriteRenderer.sprite = carSprites[1];
            else spriteRenderer.sprite = carSprites[3];
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && HqRenderer.speed > 0)
        {
            if (HqRenderer.uphill == false) spriteRenderer.sprite = carSprites[1];
            else spriteRenderer.sprite = carSprites[3];
            spriteRenderer.flipX = true;
        }
        else
        {
            if (HqRenderer.uphill == false) spriteRenderer.sprite = carSprites[0];
            else spriteRenderer.sprite = carSprites[2];
        }
       
    }
}

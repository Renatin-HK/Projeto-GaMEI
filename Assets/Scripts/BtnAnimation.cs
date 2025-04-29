using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnAnimation : MonoBehaviour
{
    public Transform box;
    public CanvasGroup bg;

    public void OnEnable()
    {
        bg.alpha = 0;
        bg.LeanAlpha(1, 0.5f);

        box.localPosition = new Vector2(0, -Screen.height);
        box.LeanMoveLocalY(0, .5f).setEaseOutExpo().delay = .1f;
    }

    public void ClosePopUp()
    {
        bg.LeanAlpha(0, .5f);
        box.LeanMoveLocalY(-Screen.height, .5f).setEaseInExpo();
    }
    
    
}

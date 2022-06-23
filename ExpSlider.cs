using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpSlider : MonoBehaviour
{
    public CashManager cashM;

    public Image image;

    public TMP_Text text;
    public float exp_current;
    public float exp_needed;

    public void SetExp()
    {
        exp_current = cashM.exp;
        exp_needed = cashM.exp_needed;
        float exp_nextlevel = exp_needed - exp_current;
        if (exp_nextlevel < 0) cashM.ExpNeeded();
        else
        {
            image.fillAmount = exp_current / exp_needed;
            text.SetText("EXP to Next Level: " + exp_nextlevel.ToString("F0"));
        }
    }
}

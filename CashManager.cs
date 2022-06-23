using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashManager : MonoBehaviour
{
    //player variables

    public float gold;
    public float exp;

    public ExpSlider slider;

    public int level = 1;
    public float exp_needed = 5;

    public float expMods = 0;
    public float goldMods = 0;
    public float killMods = 0;

    public float kills;

    public void Awake()
    {
        slider = GameObject.Find("ExpSliderMaster").GetComponent<ExpSlider>();
        ExpNeeded();
        slider.SetExp();
    }
    public void FixedUpdate()
    {
        if (exp_needed <= 0) ExpNeeded();
        if (exp >= exp_needed) LevelUp();
    }
    public void GrantKill(int _wave, int _kills)
    {
        kills += _kills;

        for (int i = 1; i <= _kills ; i++)
        {
            GrantExp(_wave);
            GrantGold(_wave);
        }
    }
    public void GrantExp(int _wave)
    {
        float exp_granted = 1 + (_wave * 1 + Mathf.Floor(Mathf.Log((10 + expMods + _wave), 10)));
        exp += exp_granted;
        slider.SetExp();
        Debug.Log("+" + exp.ToString()+ " exp");
    }
    public void GrantGold(int _wave)
    {
        gold += 1 + goldMods + _wave;
        Debug.Log("+" +gold.ToString() + " gold");
    }
    public void LevelUp()
    {
        exp -= exp_needed;
        level += 1;
        ExpNeeded();
        slider.SetExp();
    }
    public void ExpNeeded()
    {
        exp_needed = 1 + level * (level + 15);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatSlider : MonoBehaviour
{
    public Image bg;

    public PlayerAI player;
    public float currentHealthPct;

    public void Awake()
    {
        currentHealthPct =  player.currentHealth/player.maxHealth;
    }
    public void FixedUpdate()
    {
        CalculateHealth();
        SetHeath();
    }
    public void CalculateHealth()
    {
        currentHealthPct =  player.currentHealth/player.maxHealth;
    }
    public void SetHeath()
    {
        bg.fillAmount = currentHealthPct;
    }
}

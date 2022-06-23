using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatSlider : MonoBehaviour
{
    public Image bg;

    public EnemyAI player;
    public float currentHealthPct;

    public void Awake()
    {
        player = GetComponent<EnemyAI>();
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
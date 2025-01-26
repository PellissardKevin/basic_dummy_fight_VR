using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health
{
    public float health;
    public float maxHealth;

    public string ToString()
    {
        return $"{health} / {maxHealth} HP";
    }
}

public class HealthManagerScript : MonoBehaviour
{
    public Canvas HUD_Pc;

    public Image PCHealthBar_J1;
    public Image PCHealthBar_J2;
    public Image VRHealthBar_J1;
    public Image VRHealthBar_J2;
    public TMP_Text PCHealthText_J1;
    public TMP_Text PCHealthText_J2;
    public TMP_Text VRHealthText_J1;
    public TMP_Text VRHealthText_J2;

    public bool isVR = false;

    private Health P1 = new Health();
    private Health P2 = new Health();

    void Start()
    {
        if (isVR)
            HUD_Pc.enabled = false;

        P1.health = 100;
        P1.maxHealth = 100;
        P2.health = 100;
        P2.maxHealth = 100;
    }

    public Health select_player(int player)
    {
        if (player == 1)
            return P1;
        else
            return P2;
    }

    public void UpdateHealthBar(float health, bool isPlayer1)
    {
        if (isPlayer1)
        {
            if (isVR)
                VRHealthBar_J1.fillAmount = health / 100f;
            else
                PCHealthBar_J1.fillAmount = health / 100f;
        }
        else
        {
            if (isVR)
                VRHealthBar_J2.fillAmount = health / 100f;
            else
                PCHealthBar_J2.fillAmount = health / 100f;
        }
        UpdateHealthText(isPlayer1);
    }

    public void UpdateHealthText(bool isPlayer1)
    {
        int player = isPlayer1 ? 1 : 2;
        Health health = select_player(player);
        if (isPlayer1)
        {
            if (isVR)
                VRHealthText_J1.text = health.ToString();
            else
                PCHealthText_J1.text = health.ToString();
        }
        else
        {
            if (isVR)
                VRHealthText_J2.text = health.ToString();
            else
                PCHealthText_J2.text = health.ToString();
        }
    }

    [ContextMenu("Test Health Bar")]
    public void TestHealthBar()
    {
        UpdateHealthBar(50f, true);
        UpdateHealthBar(75f, false);
    }

    public void change_max_health(int player, float value_change)
    {
        Health selected = select_player(player);
        selected.maxHealth += value_change;

        UpdateHealthBar(selected.health / selected.maxHealth * 100, player == 1);
    }

    public void change_health(int player, float value_change)
    {
        Health selected = select_player(player);
        selected.health += value_change;
        UpdateHealthBar(selected.health / selected.maxHealth * 100, player == 1);
    }

    [ContextMenu("Test Change Health")]
    public void TestChangeHealth()
    {
        change_health(1, -10f);
        change_health(2, 10f);
    }
    [ContextMenu("Test Change Max Health")]
    public void TestChangeMaxHealth()
    {
        change_max_health(1, 10f);
        change_max_health(2, -10f);
    }

}

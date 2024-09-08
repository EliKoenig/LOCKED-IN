using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    public int health;
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI shieldUI;
    public Image shieldImage;
    public int maxHealth;
    public int shield;
    public GameObject shieldParent;



    // Start is called before the first frame update
    void Awake()
    {
        healthUI = GameObject.Find("Health Text").GetComponent<TextMeshProUGUI>();
        shieldUI = GameObject.Find("Shield Text").GetComponent<TextMeshProUGUI>();
        shieldParent = GameObject.Find("Shield Parent");
    }
    void Start()
    {
        health = 100;    
        maxHealth = 100;
        shield = 0;
    }

    // Update is called once per frame
    void Update()
    {
        healthUI.SetText(health + " / " + maxHealth);
        if(shield > 0 )
        {
            shieldParent.SetActive(true);
            shieldUI.SetText(shield + "");
        }
        else
        {
            shieldParent.SetActive(false);
            shield = 0;
        }
    }
}

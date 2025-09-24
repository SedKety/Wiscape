using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntity : EntityBase
{
    public static PlayerEntity Instance;
    [SerializeField] private Slider hpSlider;


    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        hpSlider.value = hpSlider.maxValue;
    }
    public override void TakeDamage(int intakeDamage, DamageType dt = DamageType.physical)
    {
        base.TakeDamage(intakeDamage, dt);
        hpSlider.value = health;

        if (health <= 0)
        {
            UIScreenLogic.Instance.LoseGame();
        }
    }

    public void GetHealth(int hpUp)
    {
        health += hpUp;
        hpSlider.value = health;
    }


}

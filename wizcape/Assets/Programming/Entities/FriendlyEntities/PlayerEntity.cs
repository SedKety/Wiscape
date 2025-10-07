using EditorAttributes;
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
        WriteToSlider();
    }
    public override void TakeDamage(int intakeDamage, DamageLayer dl, DamageType dt = DamageType.physical)
    {
        base.TakeDamage(intakeDamage,dl, dt);
        WriteToSlider();

        if (health <= 0)
        {
            UIScreenLogic.Instance.LoseGame();
        }
    }

    public void WriteToSlider()
    {
        if (!hpSlider) { return; }
        hpSlider.value = health;
    }

    public void GetHealth(int hpUp)
    {
        health += hpUp;
        WriteToSlider();
    }


}

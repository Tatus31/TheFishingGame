using UnityEngine;
using UnityEngine.UI;

public class OxygenLevel : MonoBehaviour
{   
    public Slider slider;

    

    public float maxHealth = 1000;
    public float currentOxygenLevel;

    public float speed = 6f;


    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        
    }

   public void SetHealth(float health)
   {
        slider.value = health;
   }


    public void TakeOxygenLevelDown(float amount)
    {
        currentOxygenLevel -= amount;
        slider.value = currentOxygenLevel;
    }
}

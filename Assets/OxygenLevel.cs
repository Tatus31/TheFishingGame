using UnityEngine;
using UnityEngine.UI;

// Skrypt odpowiedzialny za obsługę poziomu tlenu gracza (UI + logika)
public class OxygenLevel : MonoBehaviour
{
    // Referencja do UI Slidera, który pokazuje poziom tlenu
    [SerializeField]
    private Slider slider;

    // Aktualny poziom tlenu (prywatna zmienna)
    private float currentOxygenLevel;

    // Właściwość pozwalająca pobierać/ustawiać aktualny poziom tlenu z zewnątrz
    public float GetSetCurrentOxygenLevel
    {
        get { return currentOxygenLevel; }
        set { currentOxygenLevel = value; }
    }

    // Ustawia maksymalny poziom tlenu oraz jego wartość początkową (np. przy starcie lub wyjściu z wody)
    public void SetMaxOxygenLevel(float OxygenAmount)
    {
        slider.maxValue = OxygenAmount;
        slider.value = OxygenAmount;
    }

    // Ustawia poziom tlenu bez zmiany maksymalnej wartości (np. reset lub leczenie)
    public void SetOxygen(float OxygenAmount)
    {
        slider.value = OxygenAmount;
    }

    // Zmniejsza aktualny poziom tlenu o podaną wartość i aktualizuje slider
    public void TakeOxygenLevelDown(float amount)
    {
        currentOxygenLevel -= amount;
        slider.value = currentOxygenLevel;
    }
}

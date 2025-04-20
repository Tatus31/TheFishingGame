using UnityEngine;
using UnityEngine.UI;

// Skrypt odpowiada za reakcje po wejściu gracza pod wodę i obsługę tlenu oraz efektów graficznych
public class Ontrigger : MonoBehaviour
{
    // UI: Pasek pokazujący poziom tlenu
    public GameObject OxygenLvlBar;

    // Maksymalny poziom tlenu po wyjściu z wody
    private float maxOxygenAfterSwim = 100f;

    // UI: Obraz do efektu "fade-in" (prawdopodobnie czarny obraz, przezroczysty)
    [SerializeField]
    private Image FadinWhenLowOnOxygen;

    // Referencja do obiektu ze sliderem tlenu
    [SerializeField]
    private GameObject OxygenLevelSlider;

    // Siła, z jaką gracz będzie ciągnięty w dół, gdy zabraknie tlenu
    private float FrocePullDown = 20f;

    // Referencja Skryptu Ruchu Gracza
    [SerializeField]
    private PlayerMovement playerMovement;

    // Wykonywane, gdy jakiś obiekt wchodzi w trigger
    void OnTriggerEnter(Collider other)
    {   
        Debug.Log("Jestem pod woda!");

        // Jeśli obiekt ma tag "EyeLevel" (czyli np. głowa gracza weszła pod wodę)
        if(other.CompareTag("EyeLevel"))
        {
            Debug.Log(other);
            OxygenLvlBar.SetActive(true); // Pokazujemy pasek tlenu
        }
    }

    // Wykonywane co klatkę, jeśli jakiś obiekt pozostaje w triggerze
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("EyeLevel"))
        {
            // Obniżamy poziom tlenu (0.1 na klatkę — dość szybko)
            OxygenLevelSlider.GetComponent<OxygenLevel>().TakeOxygenLevelDown(0.10f);
        }

        // Jeśli tlen spadnie do zera
        if(OxygenLvlBar.GetComponent<Slider>().value == 0)
        {
            Debug.Log("Dying!");

            // Gracz jest ciągnięty w dół
            playerMovement.GetComponent<PlayerMovement>().rb.AddForce(
                Vector3.down * FrocePullDown * Time.deltaTime,
                ForceMode.Force
            );

            // Uruchamiamy efekt fade (ciemność na ekranie?)
            GetComponent<ScreenFader>().StartFade();
        }
    }

    // Wykonywane, gdy obiekt opuszcza trigger
    void OnTriggerExit(Collider other)
    {
        // Jeśli tlen był zerowy — resetujemy go
        if(OxygenLvlBar.GetComponent<Slider>().value == 0)
        {
            Debug.Log("Dying!");
            OxygenLvlBar.GetComponent<Slider>().value = 100f;
            GetComponent<ScreenFader>().StartFadeIn(); // Rozjaśnienie ekranu
        }
        
        OxygenLvlBar.SetActive(false);
        OxygenLevelSlider.GetComponent<OxygenLevel>().GetSetCurrentOxygenLevel = maxOxygenAfterSwim;
    }

    // Start: ukryj pasek tlenu na początku gry
    void Start()
    {
        OxygenLvlBar.SetActive(false);
    }
}

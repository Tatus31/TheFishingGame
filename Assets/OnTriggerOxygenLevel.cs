using UnityEngine;
using UnityEngine.UI;

public class Ontrigger : MonoBehaviour
{
    public GameObject HealtBar;

    private float maxHealthAfterSwim = 100f;

    [SerializeField]
    private GameObject playerScriptGO;
    void OnTriggerEnter(Collider other)
    {   

        Debug.Log("Jestem pod woda!");

        if(other.CompareTag("EyeLevel"))
        {
            Debug.Log(other);
            HealtBar.SetActive(true);
        }
       
    }


    void OnTriggerStay(Collider other)
    {   
        if(other.CompareTag("EyeLevel"))
        {
            playerScriptGO.GetComponent<OxygenLevel>().TakeOxygenLevelDown(0.10f);
        }
        

        if(HealtBar.GetComponent<Slider>().value == 0){
            Debug.Log("Dying!");
        }
    }

    void OnTriggerExit(Collider other)
    {   
        if(other.CompareTag("EyeLevel"))
        {
            HealtBar.SetActive(false);
            playerScriptGO.GetComponent<OxygenLevel>().currentOxygenLevel = maxHealthAfterSwim;
        }
        HealtBar.SetActive(false);
        playerScriptGO.GetComponent<OxygenLevel>().currentOxygenLevel = maxHealthAfterSwim;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HealtBar.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
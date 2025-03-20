using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject CloudToMove;
    private MoveClouds _MoveClouds;
    void Start()
    {
        _MoveClouds = GetComponent<MoveClouds>();
        
        CreateCloudToMove(4);
    }

    
    void CreateCloudToMove(int cloudAmount)
    {
        
       
        for (int i = 0; i< cloudAmount;i++)
        {
            float randomX = Random.Range(64,95); 
            float randomZ = Random.Range(64,95);

            // Tworzymy wektor, który będzie reprezentował pozycję na płaszczyźnie
            Vector3 spawnPosition = new Vector3(randomX, 0, randomZ); // Dodajemy wysokość planu

            // Instancjonowanie obiektu w losowo wybranym miejscu
            GameObject CloudClone = Instantiate(CloudToMove, spawnPosition, Quaternion.identity);
            _MoveClouds.GetCloudTransformsList.Add(CloudClone.transform);
            

        }
        
    }
}

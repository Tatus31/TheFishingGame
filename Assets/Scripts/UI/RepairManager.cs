using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairManager : MonoBehaviour
{
    [SerializeField] Vector3 parentStartPosition;
    [SerializeField] Vector3 parentEndPosition;
    [SerializeField] Vector3 repairPointOffset;
    [SerializeField] float pointerSpeed = 1f;

    [SerializeField] int minRepairPoints = 3;
    [SerializeField] int maxRepairPoints = 5;

    [SerializeField] GameObject repairPointPrefab;
    [SerializeField] Transform repairBarParent;

    bool isInsideTrigger = false;
    string repairPointTag = "RepairPoint";
    Collider currentRepairPoint;

    int repairHealth = 1;
    float currentPointerSpeed;
    float pointerTimer = 0f;

    List<GameObject> activeRepairPoints = new List<GameObject>();

    private void Start()
    {
        currentPointerSpeed = pointerSpeed;
        SpawnRepairPoints();
    }

    private void Update()
    {
        MovePointer();
        HandleInput();
    }

    void MovePointer()
    {
        pointerTimer += Time.deltaTime * currentPointerSpeed; 
        float t = Mathf.PingPong(pointerTimer, 1f);
        transform.position = Vector3.Lerp(parentStartPosition, parentEndPosition, t);
    }

    void HandleInput()
    {
        if (InputManager.Instance.IsLeftMouseButtonPressed())
        {
            if (isInsideTrigger)
            {
                repairHealth++;
                GetComponentInChildren<TextMeshProUGUI>().text = repairHealth.ToString();

                if (currentRepairPoint != null)
                {
                    activeRepairPoints.Remove(currentRepairPoint.gameObject);
                    IncreasePointerSpeed();
                    Destroy(currentRepairPoint.gameObject);
                    currentRepairPoint = null;
                    isInsideTrigger = false;
                }

                if (activeRepairPoints.Count == 0)
                {
                    SpawnRepairPoints();
                }
            }
            else
            {
                repairHealth--;
                GetComponentInChildren<TextMeshProUGUI>().text = repairHealth.ToString();
            }
        }

        if (repairHealth < 0)
        {
            Debug.Log("lose");
            ResetPointerSpeed();
            repairBarParent.gameObject.SetActive(false);
        }
        else if (repairHealth >= 10)
        {
            Debug.Log("win");
            ResetPointerSpeed();
            repairBarParent.gameObject.SetActive(false);
        }
    }

    void IncreasePointerSpeed()
    {
        currentPointerSpeed += 0.05f;
    }

    void ResetPointerSpeed()
    {
        currentPointerSpeed = pointerSpeed;
        pointerTimer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(repairPointTag))
        {
            isInsideTrigger = true;
            currentRepairPoint = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(repairPointTag))
        {
            isInsideTrigger = false;
            currentRepairPoint = null;
        }
    }

    void SpawnRepairPoints()
    {
        int numPoints = Random.Range(minRepairPoints, maxRepairPoints + 1);

        List<Vector3> occupiedPositions = new List<Vector3>();

        float minDistance = 100f;
        int maxAttempts = 10;

        for (int i = 0; i < numPoints; i++)
        {
            Vector3 spawnPosition;

            bool validPosition = false;
            int attempts = 0;

            do
            {
                float t = Random.Range(0f, 1f);
                spawnPosition = Vector3.Lerp(parentStartPosition + repairPointOffset, parentEndPosition + repairPointOffset, t);

                validPosition = true;
                foreach (Vector3 existingPos in occupiedPositions)
                {
                    if (Vector3.Distance(spawnPosition, existingPos) < minDistance)
                    {
                        validPosition = false;
                        break;
                    }
                }
                attempts++;

            } while (!validPosition && attempts < maxAttempts);

            if (validPosition)
            {
                GameObject repairPoint = Instantiate(repairPointPrefab, spawnPosition, Quaternion.identity, repairBarParent);
                activeRepairPoints.Add(repairPoint);
                occupiedPositions.Add(spawnPosition);
            }
        }
    }
}

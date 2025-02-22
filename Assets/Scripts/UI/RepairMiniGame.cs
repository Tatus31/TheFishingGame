using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairMiniGame : MonoBehaviour
{
    [SerializeField] Vector3 parentStartPosition;
    [SerializeField] Vector3 parentEndPosition;
    [SerializeField] Vector3 repairPointOffset;

    [SerializeField] float pointerSpeed = 1f;
    [SerializeField] float pointerSpeedIncreese = 0.05f;

    [SerializeField] int minRepairPoints = 3;
    [SerializeField] int maxRepairPoints = 5;
    [SerializeField] int winCondition = 5;

    [SerializeField] GameObject repairPointPrefab;
    [SerializeField] Transform repairBarParent;

    [SerializeField] LayerMask repairPointLayerMask;
    [SerializeField] float maxPointDistance;

    public bool isInsideTrigger = false;
    public bool isHittingRepairPoint = false;

    string repairPointTag = "RepairPoint";
    Collider currentRepairPoint;

    int repairHealth = 1;
    float currentPointerSpeed;
    float pointerTimer = 0f;
    float cooldownTimer = 0f;

    [SerializeField] float triggerExitDelay = 0.1f;
    float triggerExitTimer = 0f;

    AnimationController animator;
    Animator repairMiniGameAnimator;
    enum GameState
    {
        Playing,
        Won,
        Lost,
        Cooldown
    }

    GameState currentState = GameState.Playing;

    List<GameObject> activeRepairPoints = new List<GameObject>();

    private void Start()
    {
        animator = AnimationController.Instance;
        repairMiniGameAnimator = animator.GetAnimator(AnimationController.Animators.RepairMiniGameAnimator);

        currentPointerSpeed = pointerSpeed;
        StartGame();
    }

    private void Update()
    {
        if (InputManager.Instance.IsLeftMouseButtonPressed())
        {
            animator.PlayAnimation(repairMiniGameAnimator, AnimationController.ON_HIT, true);
            StartCoroutine(StopAnimationAfterDelay());
        }
        if (isInsideTrigger)
        {
            StartCoroutine(CheckHitAfterDelay());
        }


        switch (currentState)
        {
            case GameState.Playing:
                MovePointer();
                HandleInput();
                break;
            case GameState.Won:
            case GameState.Lost:
                HandleCooldown();
                break;
            case GameState.Cooldown:
                HandleCooldown();
                break;
        }
    }

    void MovePointer()
    {
        pointerTimer += Time.deltaTime * currentPointerSpeed;
        float t = Mathf.PingPong(pointerTimer, 1f);
        transform.position = Vector3.Lerp(parentStartPosition, parentEndPosition, t);
    }

    void HandleInput()
    {
        //if (InputManager.Instance.IsLeftMouseButtonPressed())
        //{
        //    //Debug.Log($"Click detected - isInsideTrigger: {isInsideTrigger}, currentRepairPoint: {(currentRepairPoint != null ? currentRepairPoint.name : "null")}");

        //    animator.PlayAnimation(repairMiniGameAnimator, AnimationController.ON_HIT, true);

        //    if (isInsideTrigger)
        //    {
        //        StartCoroutine(CheckHitAfterDelay());
        //    }
        //    else
        //    {
        //        //Debug.Log("Miss - decreasing repair health");
        //        repairHealth--;
        //        GetComponentInChildren<TextMeshProUGUI>().text = repairHealth.ToString();
        //    }

        //    StartCoroutine(StopAnimationAfterDelay());
        //}

        if (repairHealth < -99)
        {
            //Debug.Log("lose");
            currentState = GameState.Lost;
            repairBarParent.gameObject.SetActive(false);
        }
        else if (repairHealth >= winCondition)
        {
            //Debug.Log("win");
            currentState = GameState.Won;
            repairBarParent.gameObject.SetActive(false);

            GameObject repairPointObj = MouseWorldPosition.GetObjectOverMouse(maxPointDistance, repairPointLayerMask);
            if (repairPointObj != null)
            {
                Vector3 repairPointPosition = repairPointObj.transform.position;
                repairPointObj.SetActive(false);

                ShipRepairPoints shipRepairPoints = FindObjectOfType<ShipRepairPoints>();
                if (shipRepairPoints != null)
                {
                    shipRepairPoints.ResetRepairPointAtPosition(repairPointPosition);
                }
            }
        }
    }

    IEnumerator CheckHitAfterDelay()
    {
        yield return new WaitForSeconds(0.05f);

        if (isInsideTrigger && currentRepairPoint != null)
        {
            repairHealth++;
            GetComponentInChildren<TextMeshProUGUI>().text = repairHealth.ToString();

            activeRepairPoints.Remove(currentRepairPoint.gameObject);
            IncreasePointerSpeed();
            Destroy(currentRepairPoint.gameObject);
            currentRepairPoint = null;
            isInsideTrigger = false;

            if (activeRepairPoints.Count == 0)
            {
                SpawnRepairPoints();
            }
        }
    }

    IEnumerator StopAnimationAfterDelay()
    {
        yield return new WaitForSeconds(0.7f);
        animator.PlayAnimation(repairMiniGameAnimator, AnimationController.ON_HIT, false);
    }

    void HandleCooldown()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        foreach (GameObject point in activeRepairPoints)
        {
            if (point != null)
                Destroy(point);
        }
        activeRepairPoints.Clear();

        repairHealth = 1;
        currentPointerSpeed = pointerSpeed;
        pointerTimer = 0f;
        isInsideTrigger = false;
        currentRepairPoint = null;

        GetComponentInChildren<TextMeshProUGUI>().text = repairHealth.ToString();
        repairBarParent.gameObject.SetActive(true);

        SpawnRepairPoints();

        currentState = GameState.Playing;
    }

    void IncreasePointerSpeed()
    {
        currentPointerSpeed += pointerSpeedIncreese;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(repairPointTag))
        {
            //Debug.Log($"Entered trigger with object: {other.name}");
            isInsideTrigger = true;
            currentRepairPoint = other;
            triggerExitTimer = triggerExitDelay;
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
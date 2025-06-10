using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairMiniGame : MonoBehaviour
{
    //[Header("UI Setup")]
    //[SerializeField] Canvas gameCanvas; 
    //[SerializeField] RectTransform repairBarRect;

    //[Header("Alternative World Space Setup (if not using UI)")]
    //[SerializeField] bool useWorldSpace = false;
    //[SerializeField] Camera gameCamera;
    //[SerializeField] float worldSpaceWidth = 5f;
    //[SerializeField] float worldSpaceHeight = 1f;

    //[Header("Game Settings")]
    //[SerializeField] float pointerSpeed = 1f;
    //[SerializeField] float pointerSpeedIncreese = 0.05f;
    //[SerializeField] int minRepairPoints = 3;
    //[SerializeField] int maxRepairPoints = 5;
    //[SerializeField] int winCondition = 5;
    //[SerializeField] GameObject repairPointPrefab;
    //[SerializeField] Transform repairBarParent;
    //[SerializeField] LayerMask repairPointLayerMask;
    //[SerializeField] float maxPointDistance;
    //[SerializeField] float triggerExitDelay = 0.1f;

    //public bool isInsideTrigger = false;
    //public bool isHittingRepairPoint = false;

    //string repairPointTag = "RepairPoint";
    //Collider currentRepairPoint;

    //int repairHealth = 1;
    //float currentPointerSpeed;
    //float pointerTimer = 0f;
    //float cooldownTimer = 0f;
    //float triggerExitTimer = 0f;

    //Vector3 calculatedStartPosition;
    //Vector3 calculatedEndPosition;
    //Vector3 calculatedRepairPointOffset;

    //AnimationController animator;
    //Animator repairMiniGameAnimator;

    //enum GameState
    //{
    //    Playing,
    //    Won,
    //    Lost,
    //    Cooldown
    //}

    //GameState currentState = GameState.Playing;
    //List<GameObject> activeRepairPoints = new List<GameObject>();

    //private void Start()
    //{
    //    animator = AnimationController.Instance;
    //    repairMiniGameAnimator = animator.GetAnimator(AnimationController.Animators.RepairMiniGameAnimator);

    //    currentPointerSpeed = pointerSpeed;
    //    CalculatePositions();
    //    StartGame();
    //}

    //private void Update()
    //{
    //    if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
    //    {
    //        CalculatePositions();
    //        lastScreenWidth = Screen.width;
    //        lastScreenHeight = Screen.height;
    //    }

    //    if (InputManager.Instance.IsLeftMouseButtonPressed())
    //    {
    //        animator.PlayAnimation(repairMiniGameAnimator, AnimationController.ON_HIT, true);
    //        StartCoroutine(StopAnimationAfterDelay());
    //    }
    //    if (isInsideTrigger)
    //    {
    //        StartCoroutine(CheckHitAfterDelay());
    //    }

    //    switch (currentState)
    //    {
    //        case GameState.Playing:
    //            MovePointer();
    //            HandleInput();
    //            break;
    //        case GameState.Won:
    //        case GameState.Lost:
    //            HandleCooldown();
    //            break;
    //        case GameState.Cooldown:
    //            HandleCooldown();
    //            break;
    //    }
    //}

    //private int lastScreenWidth;
    //private int lastScreenHeight;

    //void CalculatePositions()
    //{
    //    if (useWorldSpace)
    //    {
    //        CalculateWorldSpacePositions();
    //    }
    //    else
    //    {
    //        CalculateUIPositions();
    //    }
    //}

    //void CalculateWorldSpacePositions()
    //{
    //    if (gameCamera == null)
    //        gameCamera = Camera.main;

    //    Vector3 centerPos = transform.position;
    //    calculatedStartPosition = centerPos + Vector3.left * (worldSpaceWidth * 0.5f);
    //    calculatedEndPosition = centerPos + Vector3.right * (worldSpaceWidth * 0.5f);
    //    calculatedRepairPointOffset = Vector3.up * (worldSpaceHeight * 0.1f);
    //}

    //void CalculateUIPositions()
    //{
    //    if (repairBarRect == null)
    //    {
    //        float screenWidth = Screen.width;
    //        float screenHeight = Screen.height;

    //        Vector3 screenCenter = new Vector3(screenWidth * 0.5f, screenHeight * 0.5f, 0);
    //        Vector3 screenLeft = new Vector3(screenWidth * 0.25f, screenHeight * 0.5f, 0);
    //        Vector3 screenRight = new Vector3(screenWidth * 0.75f, screenHeight * 0.5f, 0);

    //        calculatedStartPosition = Camera.main.ScreenToWorldPoint(screenLeft);
    //        calculatedEndPosition = Camera.main.ScreenToWorldPoint(screenRight);
    //        calculatedRepairPointOffset = Vector3.up * 0.5f;
    //    }
    //    else
    //    {
    //        Vector3[] corners = new Vector3[4];
    //        repairBarRect.GetWorldCorners(corners);

    //        calculatedStartPosition = corners[0];
    //        calculatedEndPosition = new Vector3(corners[2].x, corners[0].y, corners[0].z);
    //        calculatedRepairPointOffset = Vector3.up * ((corners[2].y - corners[0].y) * 0.1f);
    //    }
    //}

    //void MovePointer()
    //{
    //    pointerTimer += Time.deltaTime * currentPointerSpeed;
    //    float t = Mathf.PingPong(pointerTimer, 1f);
    //    transform.position = Vector3.Lerp(calculatedStartPosition, calculatedEndPosition, t);
    //}

    //void HandleInput()
    //{
    //    if (repairHealth < -99)
    //    {
    //        currentState = GameState.Lost;
    //        repairBarParent.gameObject.SetActive(false);
    //    }
    //    else if (repairHealth >= winCondition)
    //    {
    //        currentState = GameState.Won;
    //        repairBarParent.gameObject.SetActive(false);

    //        GameObject repairPointObj = MouseWorldPosition.GetObjectOverMouse(maxPointDistance, repairPointLayerMask);
    //        if (repairPointObj != null)
    //        {
    //            Vector3 repairPointPosition = repairPointObj.transform.position;
    //            repairPointObj.SetActive(false);

    //            ShipRepairPoints shipRepairPoints = FindObjectOfType<ShipRepairPoints>();
    //            if (shipRepairPoints != null)
    //            {
    //                shipRepairPoints.ResetRepairPointAtPosition(repairPointPosition);
    //            }
    //        }
    //    }
    //}

    //IEnumerator CheckHitAfterDelay()
    //{
    //    yield return new WaitForSeconds(0.05f);

    //    if (isInsideTrigger && currentRepairPoint != null)
    //    {
    //        repairHealth++;
    //        GetComponentInChildren<TextMeshProUGUI>().text = repairHealth.ToString();

    //        activeRepairPoints.Remove(currentRepairPoint.gameObject);
    //        IncreasePointerSpeed();
    //        Destroy(currentRepairPoint.gameObject);
    //        currentRepairPoint = null;
    //        isInsideTrigger = false;

    //        if (activeRepairPoints.Count == 0)
    //        {
    //            SpawnRepairPoints();
    //        }
    //    }
    //}

    //IEnumerator StopAnimationAfterDelay()
    //{
    //    yield return new WaitForSeconds(0.7f);
    //    animator.PlayAnimation(repairMiniGameAnimator, AnimationController.ON_HIT, false);
    //}

    //void HandleCooldown()
    //{
    //    cooldownTimer -= Time.deltaTime;

    //    if (cooldownTimer <= 0)
    //    {
    //        StartGame();
    //    }
    //}

    //void StartGame()
    //{
    //    foreach (GameObject point in activeRepairPoints)
    //    {
    //        if (point != null)
    //            Destroy(point);
    //    }
    //    activeRepairPoints.Clear();

    //    repairHealth = 1;
    //    currentPointerSpeed = pointerSpeed;
    //    pointerTimer = 0f;
    //    isInsideTrigger = false;
    //    currentRepairPoint = null;

    //    GetComponentInChildren<TextMeshProUGUI>().text = repairHealth.ToString();
    //    repairBarParent.gameObject.SetActive(true);

    //    SpawnRepairPoints();

    //    currentState = GameState.Playing;
    //}

    //void IncreasePointerSpeed()
    //{
    //    currentPointerSpeed += pointerSpeedIncreese;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag(repairPointTag))
    //    {
    //        isInsideTrigger = true;
    //        currentRepairPoint = other;
    //        triggerExitTimer = triggerExitDelay;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag(repairPointTag))
    //    {
    //        isInsideTrigger = false;
    //        currentRepairPoint = null;
    //    }
    //}

    //void SpawnRepairPoints()
    //{
    //    int numPoints = Random.Range(minRepairPoints, maxRepairPoints + 1);
    //    List<Vector3> occupiedPositions = new List<Vector3>();

    //    float barWidth = Vector3.Distance(calculatedStartPosition, calculatedEndPosition);
    //    float minDistance = barWidth / (maxRepairPoints + 1); 

    //    int maxAttempts = 10;

    //    for (int i = 0; i < numPoints; i++)
    //    {
    //        Vector3 spawnPosition;
    //        bool validPosition = false;
    //        int attempts = 0;

    //        do
    //        {
    //            float t = Random.Range(0f, 1f);
    //            spawnPosition = Vector3.Lerp(calculatedStartPosition + calculatedRepairPointOffset,
    //                                       calculatedEndPosition + calculatedRepairPointOffset, t);

    //            validPosition = true;
    //            foreach (Vector3 existingPos in occupiedPositions)
    //            {
    //                if (Vector3.Distance(spawnPosition, existingPos) < minDistance)
    //                {
    //                    validPosition = false;
    //                    break;
    //                }
    //            }
    //            attempts++;

    //        } while (!validPosition && attempts < maxAttempts);

    //        if (validPosition)
    //        {
    //            GameObject repairPoint = Instantiate(repairPointPrefab, spawnPosition, Quaternion.identity, repairBarParent);
    //            activeRepairPoints.Add(repairPoint);
    //            occupiedPositions.Add(spawnPosition);
    //        }
    //    }
    //}
}
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElectronicRepairMiniGame : MonoBehaviour
{
    public static event EventHandler OnWin;

    [Serializable]
    public class ElectricalWire
    {
        public Transform wirePositionStart;
        public Transform wireEnd;
        public LineRenderer lineRenderer;
        public bool isConnected;
    }

    [SerializeField] List<ElectricalWire> wires = new List<ElectricalWire>();

    List<Vector3> originalStartPositions = new List<Vector3>();
    List<Vector3> originalEndPositions = new List<Vector3>();

    ElectricalWire currentWire;

    bool isDrawing = false;
    bool IsMiniGameActive = false;
    bool isMiniGamePlayable = false;

    private void Start()
    {
        //InitializeMiniGame();
        //ShuffleWirePositions();
        ElectricalDevice.OnDegradation += HandleDegradation;
    }
    private void OnDestroy()
    {
        ElectricalDevice.OnDegradation -= HandleDegradation;
    }

    private void Update()
    {
        if (!isMiniGamePlayable)
            return;

        if (InputManager.Instance.IsLeftMouseButtonPressed())
        {
            StartDrawing();
        }
        else if (InputManager.Instance.IsLeftMouseButtonHeld())
        {
            UpdateDrawing();
        }
        else if (InputManager.Instance.IsLeftMouseButtonReleased())
        {
            StopDrawing();
        }
    }

    void InitializeMiniGame()
    {
        foreach (var wire in wires)
        {
            originalStartPositions.Add(wire.wirePositionStart.position);
            originalEndPositions.Add(wire.wireEnd.position);

            wire.lineRenderer.positionCount = 2;
            wire.lineRenderer.SetPosition(0, wire.wirePositionStart.position);
            wire.lineRenderer.SetPosition(1, wire.wirePositionStart.position);
            wire.isConnected = false;
        }

        IsMiniGameActive = true;
    }

    void StartDrawing()
    {
        GameObject clickedObject = MouseWorldPosition.GetObjectOverMouse("Wire");

        //CameraLook.LockCamera(true);

        if (clickedObject != null)
        {
            foreach (var wire in wires)
            {
                if (clickedObject.transform == wire.wirePositionStart || clickedObject.transform == wire.wireEnd)
                {
                    currentWire = wire;
                    isDrawing = true;

                    Events.onEnteredInteraction.CanShowPanel = isDrawing;
                    EventManager.Broadcast(Events.onEnteredInteraction);

                    currentWire.lineRenderer.SetPosition(0, currentWire.wirePositionStart.position);
                    currentWire.lineRenderer.SetPosition(1, currentWire.wirePositionStart.position);
                    break;
                }
            }
        }
    }

    void UpdateDrawing()
    {
        if (isDrawing && currentWire != null)
        {
            Vector3 mousePosition = MouseWorldPosition.GetMouseWorldPosition();
            currentWire.lineRenderer.SetPosition(1, mousePosition);
        }
    }

    void StopDrawing()
    {
        if (isDrawing && currentWire != null)
        {
            GameObject releasedObject = MouseWorldPosition.GetObjectOverMouse("Wire");

            if (releasedObject != null)
            {
                if ((releasedObject.transform == currentWire.wireEnd && currentWire.lineRenderer.GetPosition(0) == currentWire.wirePositionStart.position) ||
                    (releasedObject.transform == currentWire.wirePositionStart && currentWire.lineRenderer.GetPosition(0) == currentWire.wireEnd.position))
                {
                    currentWire.lineRenderer.SetPosition(1, releasedObject.transform.position);
                    currentWire.isConnected = true;
                    CheckWinCondition();
                }
                else
                {
                    currentWire.lineRenderer.SetPosition(1, currentWire.lineRenderer.GetPosition(0));
                }
            }
            else
            {
                currentWire.lineRenderer.SetPosition(1, currentWire.lineRenderer.GetPosition(0));
            }

            isDrawing = false;
            currentWire = null;

            Events.onEnteredInteraction.CanShowPanel = isDrawing;
            EventManager.Broadcast(Events.onEnteredInteraction);
        }
    }

    void CheckWinCondition()
    {
        bool allConnected = true;
        foreach (var wire in wires)
        {
            if (!wire.isConnected)
            {
                allConnected = false;
                break;
            }
        }

        if (allConnected)
        {
            IsMiniGameActive = false;
            isMiniGamePlayable = false;

            ElectricalDevice device = FindObjectOfType<ElectricalDevice>();
            if (device != null)
            {
                device.RepairDevice();
            }
            OnWin?.Invoke(this, EventArgs.Empty);
        }
    }

    void ShuffleWirePositions()
    {
        List<Vector3> availableStarts = new List<Vector3>(originalStartPositions);
        List<Vector3> availableEnds = new List<Vector3>(originalEndPositions);
        HashSet<Vector3> takenPositions = new HashSet<Vector3>();

        for (int i = 0; i < wires.Count; i++)
        {
            Vector3 newStart;
            do
            {
                newStart = availableStarts[UnityEngine.Random.Range(0, availableStarts.Count)];
            } while (takenPositions.Contains(newStart));

            takenPositions.Add(newStart);
            availableStarts.Remove(newStart);
            wires[i].wirePositionStart.position = newStart;

            Vector3 newEnd;
            do
            {
                newEnd = availableEnds[UnityEngine.Random.Range(0, availableEnds.Count)];
            } while (takenPositions.Contains(newEnd));

            takenPositions.Add(newEnd);
            availableEnds.Remove(newEnd);
            wires[i].wireEnd.position = newEnd;
        }
    }

    public void ResetMiniGame()
    {
        InitializeMiniGame();
        ShuffleWirePositions();

        foreach (var wire in wires)
        {
            wire.isConnected = false;
        }
    }

    void HandleDegradation(object sender, EventArgs e)
    {
        if (!IsMiniGameActive)
        {
            ResetMiniGame();

            isMiniGamePlayable = true;
        }
    }
}
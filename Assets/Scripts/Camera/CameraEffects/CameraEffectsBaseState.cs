using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraEffectsBaseState
{
    public abstract void EnterState(CameraEffectsManager cameraEffect);
    public abstract void UpdateState(CameraEffectsManager cameraEffect);

}

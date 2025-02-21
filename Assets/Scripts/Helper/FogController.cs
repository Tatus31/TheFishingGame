using UnityEngine;

public class FogController : MonoBehaviour
{
    public float defaultFogEnd = 10f;

    public void SetFogEnd(float value)
    {
        value += defaultFogEnd;
        RenderSettings.fogEndDistance = value;
    }
}

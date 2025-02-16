using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;

    [SerializeField] float offsetY = 0.5f;

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + offsetY, player.transform.position.z);
    }
}

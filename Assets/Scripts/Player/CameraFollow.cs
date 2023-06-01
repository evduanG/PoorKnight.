
using Assets.player;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 updateingPosition;

    void Start()
    {
        updateingPosition = transform.position;
    }

    void Update()
    {
        if (PlayerController.Instance == null)
        {
            this.enabled = true;
        }
        else
        {
            updateingPosition.x = PlayerController.Instance.transform.position.x;
            transform.position = updateingPosition;
        }
    }
}

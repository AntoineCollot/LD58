using UnityEngine;

public class LookPlayerDirection : MonoBehaviour
{
    PlayerMovement movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookAtTarget = transform.position + movement.playerCam.transform.forward;
        lookAtTarget.y = 0;
        transform.LookAt(lookAtTarget);
    }
}

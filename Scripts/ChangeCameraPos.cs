using Unity.VisualScripting;
using UnityEngine;

public class ChangeCameraPos : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform targetCameraPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCamera.position = targetCameraPos.position;
            playerCamera.rotation = targetCameraPos.rotation;
        }
    }
}

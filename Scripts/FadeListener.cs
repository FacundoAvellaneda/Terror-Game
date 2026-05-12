using UnityEngine;

public class FadeListener : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject targetObject;

    [Header("Cameras")]
    [SerializeField] private GameObject[] cameraArray;
    [SerializeField] private int indexCameraToActivate;

    private void OnEnable()
    {
        UIFade.OnFadeBlackComplete += HandleFadeBlackComplete;
        UIFade.OnFadeTransparentComplete += HandleFadeTransparentComplete;
    }

    private void OnDisable()
    {
        UIFade.OnFadeBlackComplete -= HandleFadeBlackComplete;
        UIFade.OnFadeTransparentComplete -= HandleFadeTransparentComplete;
    }

    void HandleFadeBlackComplete()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        ActivateSelectedCamera();
    }

    void HandleFadeTransparentComplete()
    {
        gameObject.SetActive(false);
    }

    void ActivateSelectedCamera()
    {
        if (cameraArray == null || cameraArray.Length == 0)
            return;

        if (indexCameraToActivate < 0 || indexCameraToActivate >= cameraArray.Length)
        {
            Debug.LogWarning("Indice de cámara inválido.");
            return;
        }

        for (int i = 0; i < cameraArray.Length; i++)
        {
            if (cameraArray[i] == null)
                continue;

            cameraArray[i].SetActive(i == indexCameraToActivate);
        }
    }
}
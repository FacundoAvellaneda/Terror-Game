using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FadeController : MonoBehaviour
{
    [SerializeField] private UIFade fade;

    [Header("Options")]
    public bool toBlack;
    public bool toPlay;

    [Header("Control")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("FadeController triggered by " + other.name);

        if (triggerOnce && hasTriggered) return;

        if (other.TryGetComponent<ICanTrigger>(out var trigger))
        {
            Debug.Log("FadeController triggered by " + other.name);
            PlayFade();
            hasTriggered = true;
        }
    }


    public void PlayFade()
    {
        if (toBlack)
        {
            fade.FadeToBlack();
        }
        else if (toPlay)
        {
            fade.FadeToTransparent();
        }
    }
}
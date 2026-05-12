using UnityEngine;

public class CanTrigger : MonoBehaviour ,ICanTrigger
{
    public void OnTriggered()
    {
        Debug.Log("CanTrigger has been triggered!");
    }
}

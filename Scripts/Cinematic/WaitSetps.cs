using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cinematics/Steps/Wait")]
public class WaitSetps : CinematicSteps
{
    public float duration = 1f;

    public override IEnumerator Execute(CinematicPlayer player)
    {
        yield return new WaitForSeconds(duration);
    }
}

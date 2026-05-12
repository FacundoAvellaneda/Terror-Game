using System.Collections;
using UnityEngine;

public abstract class CinematicSteps : ScriptableObject
{
    public abstract IEnumerator Execute(CinematicPlayer player);
}

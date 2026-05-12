using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cinematics/Cinematic Data")]
public class CinematicData : ScriptableObject
{
    public List<CinematicSteps> steps;
}

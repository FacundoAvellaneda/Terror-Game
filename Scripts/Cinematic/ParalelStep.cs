using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cinematics/Steps/Parallel")]
public class ParalelStep : CinematicSteps
{
    public List<CinematicSteps> steps;

    public override IEnumerator Execute(CinematicPlayer player)
    {
        int finishedCount = 0;

        foreach (var step in steps)
        {
            player.StartCoroutine(RunStep(step, player, () =>
            {
                finishedCount++;
            }));
        }

        while (finishedCount < steps.Count)
        {
            yield return null;
        }
    }

    IEnumerator RunStep(CinematicSteps step, CinematicPlayer player, System.Action onFinish)
    {
        yield return step.Execute(player);

        onFinish?.Invoke();
    }
}

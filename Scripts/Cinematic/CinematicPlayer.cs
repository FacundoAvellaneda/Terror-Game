using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicPlayer : MonoBehaviour
{
    public static event Action OnCinematicStart;
    public static event Action OnCinematicEnd;

    private Dictionary<string, Transform> actors = new Dictionary<string, Transform>();
    private bool isPlaying;

    void Awake()
    {
        RegisterActors();
    }

    void RegisterActors()
    {
        CinematicActor[] foundActors = FindObjectsOfType<CinematicActor>();

        foreach (var actor in foundActors)
        {
            if (!actors.ContainsKey(actor.actorID))
            {
                actors.Add(actor.actorID, actor.transform);
            }
            else
            {
                Debug.LogWarning("Actor duplicado: " + actor.actorID);
            }
        }
    }

    public Transform GetActor(string id)
    {
        if (actors.TryGetValue(id, out Transform t))
            return t;

        Debug.LogWarning("Actor no encontrado: " + id);
        return null;
    }

    public void StartCinematic(CinematicData data)
    {
        if (isPlaying || data == null) return;

        StartCoroutine(PlayCinematic(data));
    }

    IEnumerator PlayCinematic(CinematicData data)
    {
        isPlaying = true;
        OnCinematicStart?.Invoke();

        foreach (var step in data.steps)
        {
            if (step != null)
                yield return step.Execute(this);
        }

        OnCinematicEnd?.Invoke();
        isPlaying = false;
    }
}
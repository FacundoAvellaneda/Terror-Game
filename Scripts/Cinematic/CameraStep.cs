using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cinematics/Steps/Camera ")]
public class CameraStep : CinematicSteps
{
    public CameraMode mode;

    [Header("Common")]
    public string cameraActorID;
    public float duration = 3f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Follow Target")]
    public string targetActorID;
    public Vector3 offset;
    public float followSmooth = 0.2f;
    public float rotationSmooth = 5f;

    [Header("Look Target")]
    public bool useLookTarget;

    [Header("Path")]
    public List<Vector3> waypoints = new List<Vector3>();
    public Vector3 destination;

    [Header("Look Ahead")]
    public float lookAhead = 0.02f;

    private Vector3 velocity;

    public enum CameraMode
    {
        FollowTarget,
        Path
    }

    public override IEnumerator Execute(CinematicPlayer player)
    {
        Transform cam = player.GetActor(cameraActorID);

        if (cam == null)
            yield break;

        switch (mode)
        {
            case CameraMode.FollowTarget:
                yield return FollowTarget(player, cam);
                break;

            case CameraMode.Path:
                yield return FollowPath(player, cam);
                break;
        }
    }

    IEnumerator FollowTarget(CinematicPlayer player, Transform cam)
    {
        Transform target = player.GetActor(targetActorID);

        if (target == null)
            yield break;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            Vector3 desiredPos = target.position + offset;

            // MOVIMIENTO SUAVE
            cam.position = Vector3.SmoothDamp(
                cam.position,
                desiredPos,
                ref velocity,
                followSmooth
            );

            // ROTACION SUAVE
            Vector3 dir = (target.position - cam.position).normalized;

            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);

                cam.rotation = Quaternion.Slerp(
                    cam.rotation,
                    targetRot,
                    rotationSmooth * Time.deltaTime
                );
            }

            yield return null;
        }
    }

    IEnumerator FollowPath(CinematicPlayer player, Transform cam)
    {
        List<Vector3> path = new List<Vector3>();

        path.Add(cam.position);

        foreach (var p in waypoints)
            path.Add(p);

        path.Add(destination);

        Transform lookTarget = null;

        if (useLookTarget)
            lookTarget = player.GetActor(targetActorID);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float normalized = Mathf.Clamp01(time / duration);
            float t = curve.Evaluate(normalized);

            Vector3 desiredPos = EvaluateSpline(path, t);

            // MOVIMIENTO SUAVE
            cam.position = Vector3.SmoothDamp(
                cam.position,
                desiredPos,
                ref velocity,
                followSmooth
            );

            // =========================
            // ROTACION
            // =========================

            Vector3 dir = Vector3.zero;

            // MIRAR A TARGET
            if (useLookTarget && lookTarget != null)
            {
                dir = (lookTarget.position - cam.position).normalized;
            }
            else
            {
                // LOOK AHEAD DEL PATH
                float futureT = Mathf.Clamp01(t + lookAhead);

                Vector3 futurePos = EvaluateSpline(path, futureT);

                dir = (futurePos - cam.position).normalized;
            }

            if (dir != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);

                cam.rotation = Quaternion.Slerp(
                    cam.rotation,
                    lookRot,
                    rotationSmooth * Time.deltaTime
                );
            }

            yield return null;
        }

        cam.position = destination;
    }

    Vector3 EvaluateSpline(List<Vector3> path, float t)
    {
        int count = path.Count;

        if (count < 2)
            return path[0];

        int sections = count - 1;

        float totalT = t * sections;

        int i = Mathf.FloorToInt(totalT);

        i = Mathf.Clamp(i, 0, sections - 1);

        float localT = totalT - i;

        Vector3 p0 = path[Mathf.Clamp(i - 1, 0, count - 1)];
        Vector3 p1 = path[i];
        Vector3 p2 = path[Mathf.Clamp(i + 1, 0, count - 1)];
        Vector3 p3 = path[Mathf.Clamp(i + 2, 0, count - 1)];

        return CatmullRom(p0, p1, p2, p3, localT);
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }
}
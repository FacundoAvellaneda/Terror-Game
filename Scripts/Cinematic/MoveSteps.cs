using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cinematics/Steps/Move")]
public class MoveSteps : CinematicSteps
{
    public string actorID;

    [Header("Path")]
    public List<Vector3> waypoints = new List<Vector3>();
    public Vector3 destination;

    [Header("Timing")]
    public float duration = 2f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Smooth")]
    public float smoothTime = 0.15f;
    public float maxSpeed = 100f;

    [Header("Rotation")]
    public RotationMode rotationMode = RotationMode.None;
    public Vector3 targetRotation;
    public bool dynamicLook = true;
    public float rotationSmooth = 10f;

    [Header("Finish")]
    public bool stopRigidBody = true;
    public bool setKinematicOnFinish = false;
    public bool disableActorOnFinish = false;

    private Rigidbody _myRig;

    public enum RotationMode
    {
        None,
        TargetRotation,
        LookDirection
    }

    public override IEnumerator Execute(CinematicPlayer player)
    {
        Transform target = player.GetActor(actorID);

        if (target == null)
            yield break;

        _myRig = target.GetComponent<Rigidbody>();

        // IMPORTANTE
        if (_myRig != null)
        {
            _myRig.interpolation = RigidbodyInterpolation.Interpolate;
        }

        List<Vector3> path = new List<Vector3>();

        path.Add(target.position);

        foreach (var point in waypoints)
            path.Add(point);

        path.Add(destination);

        float time = 0f;

        Vector3 currentVelocity = Vector3.zero;

        while (time < duration)
        {
            time += Time.fixedDeltaTime;

            float normalizedTime = Mathf.Clamp01(time / duration);

            float t = curve.Evaluate(normalizedTime);

            // POSICION OBJETIVO EN LA SPLINE
            Vector3 targetPos = EvaluateSpline(path, t);

            // SUAVIZADO REAL
            Vector3 smoothPos = Vector3.SmoothDamp(
                target.position,
                targetPos,
                ref currentVelocity,
                smoothTime,
                maxSpeed,
                Time.fixedDeltaTime
            );

            // MOVIMIENTO
            if (_myRig != null)
                _myRig.MovePosition(smoothPos);
            else
                target.position = smoothPos;

            // ROTACION
            HandleRotation(target, path, t);

            yield return new WaitForFixedUpdate();
        }

        // POSICION FINAL EXACTA
        if (_myRig != null)
            _myRig.MovePosition(destination);
        else
            target.position = destination;

        // ROTACION FINAL
        if (rotationMode == RotationMode.TargetRotation)
        {
            target.rotation = Quaternion.Euler(targetRotation);
        }

        // DETENER FISICAS
        if (_myRig != null && stopRigidBody)
        {
            _myRig.linearVelocity = Vector3.zero;
            _myRig.angularVelocity = Vector3.zero;
        }

        // KINEMATIC FINAL
        if (_myRig != null && setKinematicOnFinish)
        {
            _myRig.isKinematic = true;
        }

        // DESACTIVAR OBJETO
        if (disableActorOnFinish)
        {
            target.gameObject.SetActive(false);
        }
    }

    Vector3 EvaluateSpline(List<Vector3> path, float t)
    {
        int count = path.Count;

        if (count < 2)
            return path[0];

        int numSections = count - 1;

        float totalT = t * numSections;

        int currIndex = Mathf.FloorToInt(totalT);

        currIndex = Mathf.Clamp(currIndex, 0, numSections - 1);

        float localT = totalT - currIndex;

        Vector3 p0 = path[Mathf.Clamp(currIndex - 1, 0, count - 1)];
        Vector3 p1 = path[currIndex];
        Vector3 p2 = path[Mathf.Clamp(currIndex + 1, 0, count - 1)];
        Vector3 p3 = path[Mathf.Clamp(currIndex + 2, 0, count - 1)];

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

    void HandleRotation(Transform target, List<Vector3> path, float t)
    {
        switch (rotationMode)
        {
            case RotationMode.TargetRotation:

                Quaternion endRot = Quaternion.Euler(targetRotation);

                target.rotation = Quaternion.Slerp(
                    target.rotation,
                    endRot,
                    rotationSmooth * Time.fixedDeltaTime
                );

                break;

            case RotationMode.LookDirection:

                Vector3 currentPos = target.position;

                Vector3 nextPos = EvaluateSpline(
                    path,
                    Mathf.Clamp01(t + 0.01f)
                );

                Vector3 dir = (nextPos - currentPos).normalized;

                dir.y = 0f;

                if (dir != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);

                    target.rotation = Quaternion.Slerp(
                        target.rotation,
                        lookRot,
                        rotationSmooth * Time.fixedDeltaTime
                    );
                }

                break;
        }
    }
}
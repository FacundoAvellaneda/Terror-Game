using UnityEngine;

public class CinematicStart : MonoBehaviour
{
    [SerializeField] private CinematicPlayer play;
    [SerializeField] private CinematicData Data;

    private void Start()
    {
        play.StartCinematic(Data);
    }
}

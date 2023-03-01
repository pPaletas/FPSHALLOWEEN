using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<GameObject> _players = new List<GameObject>();

    public List<GameObject> Players { get => _players; }

    private void FindPlayers()
    {
        FirstPersonController[] controllers = GameManager.FindObjectsOfType<FirstPersonController>();

        foreach (FirstPersonController controller in controllers)
        {
            _players.Add(controller.gameObject);
        }
    }

    private void Awake()
    {
        FindPlayers();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BoardManager _boardManager;   // Oyun tahtasý
    [SerializeField] private BoardDataSO[] _boards;
    [SerializeField] private int _levelIndex;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _boardManager.Init(this, _boards[_levelIndex % _boards.Length]);  // Tahta oluþtur
    }

    void Update()
    {

    }
}

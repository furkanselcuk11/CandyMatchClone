using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BoardManager _boardManager;   // Oyun tahtasý
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
        _boardManager.Init(this, 6, 6);  // Tahta oluþtur
    }
    void Update()
    {

    }
}

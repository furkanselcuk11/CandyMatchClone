using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoard", menuName = "FurkanSelcuk/Game Data/Board Data")]
public class BoardDataSO : ScriptableObject
{
    [SerializeField] private int _boardWidth;
    [SerializeField] private int _boardHeight;
    [SerializeField] private bool _isFillBoardRandom;
    [SerializeField] private int[] _gameBoard;    
    [Space]
    [SerializeField] private Sprite[] _TileSprites;
    [Space]
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _emptyTilePrefab;
    [SerializeField] private GameObject _explosionPrefab;    

    public int BoardWidth { get => _boardWidth; set => _boardWidth = value; }
    public int BoardHeight { get => _boardHeight; set => _boardHeight = value; }
    public int[] GameBoard { get => _gameBoard; set => _gameBoard = value; }
    public Sprite[] TileSprites { get => _TileSprites; set => _TileSprites = value; }
    public GameObject TilePrefab { get => _tilePrefab; set => _tilePrefab = value; }
    public GameObject EmptyTilePrefab { get => _emptyTilePrefab; set => _emptyTilePrefab = value; }
    public GameObject ExplosionPrefab { get => _explosionPrefab; set => _explosionPrefab = value; }
    public bool IsFillBoardRandom { get => _isFillBoardRandom; set => _isFillBoardRandom = value; }
}

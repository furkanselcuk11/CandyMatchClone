using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Game Board Settings")]
    private int _boardWidth;    // Tahta geniþliði
    private int _boardHeight;   // Tahta yüksekliði
    private int[] _gameBoard;  // Tahta içeriði

    private BoardDataSO _boardData;
    //[SerializeField] private GameObject _tilePrefab;
    //[SerializeField] private GameObject _emptyTilePrefab;
    //[SerializeField] private Sprite[] _tileSprites;

    [SerializeField] private Tile _selectedTile;
    [SerializeField] private Tile _swapTile;

    [SerializeField] private int _moveCount = 0;

    private GameManager _gameManager;

    public event Action<int> OnBoardMove;   // Her hareket sonrasý çalýþ
    void Start()
    {

    }
    void Update()
    {
        //#if UNITY_IOS || UNITY_ANDROID
        //        HandleMobileInput();
        //#else
        //                HandlePCInput();
        //#endif
        HandlePCInput();
    }
    private void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.rawPosition), Vector2.zero);

                if (hitInfo.collider != null && hitInfo.collider.CompareTag("Tile"))
                {
                    _selectedTile = hitInfo.collider.GetComponent<Tile>();  // Dokunduðum Tile objesini seç
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                SwapSelectedTiles(touchPos);
            }
        }
    }
    private void HandlePCInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hitInfo.collider != null && hitInfo.collider.CompareTag("Tile"))
            {
                _selectedTile = hitInfo.collider.GetComponent<Tile>();  // Dokunduðum Tile objesini seç
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SwapSelectedTiles(mousePos);
        }
    }
    private void SwapSelectedTiles(Vector2 swipePos)
    {
        // Tile kaydrýma iþlemi
        // Parmaðýmýzý býraktýðýmda bulunduðum konum ve oradaki Tile objesi
        int swipeX = (int)Mathf.Clamp(Mathf.Round(swipePos.x - _selectedTile.transform.position.x), -1f, 1f);
        int swipeY = (int)Mathf.Clamp(Mathf.Round(_selectedTile.transform.position.y - swipePos.y), -1f, 1f);

        int selectedX = _selectedTile.GetX();
        int selectedY = _selectedTile.GetY();

        int swapX = selectedX + swipeX;
        int swapY = selectedY + swipeY;

        _swapTile = GetTile(swapX, swapY);

        if (_swapTile != null)
        {
            int selectedPosition = GetBoardPosition(selectedX, selectedY);
            int swapPosition = GetBoardPosition(swapX, swapY);

            // selectedPosition ve swapPosition tahta üzerindeki posizyonlarý yer deðiþtirir
            _gameBoard[selectedPosition] = _swapTile.TileType;
            _gameBoard[swapPosition] = _selectedTile.TileType;

            // selectedPosition ve swapPosition sahenede yer deðiþtirir
            Vector3 temPos = _swapTile.transform.position;
            _swapTile.transform.position = _selectedTile.transform.position;
            _selectedTile.transform.position = temPos;

            _moveCount += 1; // her hareket sonrasý arttýr
            OnBoardMove?.Invoke(_moveCount); // Her hareket sonrasý event gönder

            CheckMatches();
        }
    }
    public void Init(GameManager gm, BoardDataSO boardData)
    {
        _gameManager = gm;
        _boardData = boardData;
        InitBoard();
        RenderBoard();
    }
    private void InitBoard()
    {
        // Tahtayý oluþtur
        _moveCount = 0;
        _boardWidth = _boardData.BoardWidth;
        _boardHeight = _boardData.BoardHeight;

        _gameBoard = new int[_boardWidth * _boardHeight];
        FillBoardRandomly();
    }
    private void RenderBoard()
    {
        // Tahtayý doldur
        for (int j = 0; j < _boardHeight; j++)
        {
            for (int i = 0; i < _boardWidth; i++)
            {
                GameObject emptyTile = Instantiate(_boardData.EmptyTilePrefab);
                emptyTile.transform.SetParent(transform);
                emptyTile.transform.localPosition = new Vector3(i, -j, 0f); // yukarýdan aþaðý gideceði için -j

                int tileId = _gameBoard[GetBoardPosition(i, j)];    // tileId ver

                CretaTile(tileId, i, j);
            }
        }
    }
    private int GetBoardPosition(int x, int y)
    {
        // Boþ Tile objesinin tahta üzerinde pozisyonunu belirle
        return (_boardWidth * y) + x;
    }
    private void CretaTile(int tileId, int x, int y)
    {
        // Yeni Tile objesi oluþtur
        GameObject tileObject = Instantiate(_boardData.TilePrefab);
        tileObject.transform.SetParent(transform);
        tileObject.transform.localPosition = new Vector3(x, -y, 0f); // yukarýdan aþaðý gideceði için -j

        Tile tile = tileObject.GetComponent<Tile>();
        tile.TileType = tileId;
        tile.SetSprite(_boardData.TileSprites[tileId]);   // _tileSprites gelen sprite oluþturulan Tileobjesinin sprite olarak atanýr
    }
    private void FillBoardRandomly()
    {
        // Tahtada bulanan Tile objelerine random sprite oluþtur
        for (int j = 0; j < _boardHeight; j++)
        {
            for (int i = 0; i < _boardWidth; i++)
            {
                _gameBoard[GetBoardPosition(i, j)] = UnityEngine.Random.Range(0, _boardData.TileSprites.Length);
            }
        }
    }
    private Tile GetTile(int x, int y)
    {
        // Swaptile nesnesine Tile gönderir
        Collider2D tileHit = Physics2D.OverlapPoint(new Vector2(transform.position.x + x, transform.position.y - y));

        if (tileHit != null && tileHit.CompareTag("Tile")) return tileHit.GetComponent<Tile>();
        return null;
    }
    private void CheckMatches()
    {
        // Eþleþmele varmý kontrol et
    }
}

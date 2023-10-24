using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Game Board Settings")]
    private int _boardWidth;    // Tahta geni�li�i
    private int _boardHeight;   // Tahta y�ksekli�i
    private int[] _gameBoard;  // Tahta i�eri�i

    private BoardDataSO _boardData;
    //[SerializeField] private GameObject _tilePrefab;
    //[SerializeField] private GameObject _emptyTilePrefab;
    //[SerializeField] private Sprite[] _tileSprites;

    private Tile _selectedTile;
    private Tile _swapTile;

    [SerializeField] private int _moveCount = 0;
    [SerializeField] private int _popCount = 0;

    private GameManager _gameManager;

    public event Action<int> OnBoardMove;   // Her hareket sonras� �al��
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
                    _selectedTile = hitInfo.collider.GetComponent<Tile>();  // Dokundu�um Tile objesini se�
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
                _selectedTile = hitInfo.collider.GetComponent<Tile>();  // Dokundu�um Tile objesini se�
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
        // Tile kaydr�ma i�lemi
        // Parma��m�z� b�rakt���mda bulundu�um konum ve oradaki Tile objesi
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

            // selectedPosition ve swapPosition tahta �zerindeki posizyonlar� yer de�i�tirir
            _gameBoard[selectedPosition] = _swapTile.TileType;
            _gameBoard[swapPosition] = _selectedTile.TileType;

            // selectedPosition ve swapPosition sahenede yer de�i�tirir
            Vector3 temPos = _swapTile.transform.position;
            _swapTile.transform.position = _selectedTile.transform.position;
            _selectedTile.transform.position = temPos;

            _moveCount += 1; // her hareket sonras� artt�r
            OnBoardMove?.Invoke(_moveCount); // Her hareket sonras� event g�nder

            CheckMatches();
            StartCoroutine(HandleEmptySpaces());
        }
        _selectedTile = null;
        _swapTile = null;
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
        // Tahtay� olu�tur
        _moveCount = 0;
        _popCount = 0;
        _boardWidth = _boardData.BoardWidth;
        _boardHeight = _boardData.BoardHeight;

        _gameBoard = new int[_boardWidth * _boardHeight];
        FillBoardRandomly();
    }
    private void RenderBoard()
    {
        // Tahtay� doldur
        for (int j = 0; j < _boardHeight; j++)
        {
            for (int i = 0; i < _boardWidth; i++)
            {
                GameObject emptyTile = Instantiate(_boardData.EmptyTilePrefab);
                emptyTile.transform.SetParent(transform);
                emptyTile.transform.localPosition = new Vector3(i, -j, 0f); // yukar�dan a�a�� gidece�i i�in -j

                int tileId = _gameBoard[GetBoardPosition(i, j)];    // tileId ver

                CretaTile(tileId, i, j);
            }
        }
    }
    private int GetBoardPosition(int x, int y)
    {
        // Bo� Tile objesinin tahta �zerinde pozisyonunu belirle
        return (_boardWidth * y) + x;
    }
    private void CretaTile(int tileId, int x, int y)
    {
        // Yeni Tile objesi olu�tur
        GameObject tileObject = Instantiate(_boardData.TilePrefab);
        tileObject.transform.SetParent(transform);
        tileObject.transform.localPosition = new Vector3(x, -y, 0f); // yukar�dan a�a�� gidece�i i�in -j

        Tile tile = tileObject.GetComponent<Tile>();
        tile.TileType = tileId;
        tile.SetSprite(_boardData.TileSprites[tileId]);   // _tileSprites gelen sprite olu�turulan Tileobjesinin sprite olarak atan�r
    }
    private void FillBoardRandomly()
    {
        // Tahtada bulanan Tile objelerine random sprite olu�tur
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
        // Swaptile nesnesine Tile g�nderir
        Collider2D tileHit = Physics2D.OverlapPoint(new Vector2(transform.position.x + x, transform.position.y - y));

        if (tileHit != null && tileHit.CompareTag("Tile")) return tileHit.GetComponent<Tile>();
        return null;
    }
    private void CheckMatches()
    {
        // E�le�mele varm� kontrol et
        _popCount += CheckForCombosTiles(_selectedTile);
        _popCount += CheckForCombosTiles(_swapTile);
    }
    private int CheckForCombosTiles(Tile comboTile)
    {
        int popCount = 0;   // Patlayan Tile say�s�
        int comboX = comboTile.GetX();
        int comboY = comboTile.GetY();

        List<Tile> comboH = CheckForCombosHorizontal(comboX, comboY, comboTile.TileType);
        List<Tile> comboV = CheckForCombosVertical(comboX, comboY, comboTile.TileType);

        bool isComboTilePops = false;
        if (comboH.Count > 1)
        {
            foreach (Tile tile in comboH)
            {
                popCount += 1;
                PopTile(tile);
            }
            isComboTilePops = true;
        }
        if (comboV.Count > 1)
        {
            foreach (Tile tile in comboV)
            {
                popCount += 1;
                PopTile(tile);
            }
            isComboTilePops = true;
        }
        if (isComboTilePops)
        {
            popCount += 1;
            PopTile(comboTile);
        }
        return popCount;
    }
    private void PopTile(Tile tile)
    {
        // Tile yok et ve patlam animasyonu oynar
        _gameBoard[GetBoardPosition(tile.GetX(), tile.GetY())] = -1;    // Board �zerinde yerlerini bo�alt

        GameObject explosion = Instantiate(_boardData.ExplosionPrefab);
        explosion.transform.position = tile.transform.position;

        Destroy(explosion, 0.5f);
        Destroy(tile.gameObject);
    }

    private List<Tile> CheckForCombosVertical(int x, int y, int tileType)
    {
        // Dikeyde birbiri ile e�le�en tile varm� kontrol et ve birbirli ile yanyan gelen Tileleri listeye ekle
        List<Tile> comboList = new List<Tile>();
        // En son b�rakt���n Tile merkezi alarak sa�dan ve soldan kontrol et
        int preY = y - 1;
        int postY = y + 1;

        while (preY > -1 || postY < _boardHeight)
        {
            if (preY > -1)
            {
                int prePos = GetBoardPosition(x, preY); // Solundaki Tile'�n koordinat�n� al
                if (_gameBoard[prePos] == tileType)
                {
                    // Solundaki Tile ile tipleri ayn� ise Tile'� al 
                    Tile tile = GetTile(x, preY);
                    if (tile != null)
                    {
                        // listeye ekle ve bir soldakine git
                        comboList.Add(tile);
                        preY -= 1;
                    }
                }
                else
                {
                    // Solundaki Tile ile tipleri ayn� de�ilse ��k
                    preY = -1;
                }
            }
            if (postY < _boardHeight)
            {
                int postPos = GetBoardPosition(x, postY); // Sa��ndaki Tile'�n koordinat�n� al
                if (_gameBoard[postPos] == tileType)
                {
                    // Sa��ndaki Tile ile tipleri ayn� ise Tile'� al 
                    Tile tile = GetTile(x, postY);
                    if (tile != null)
                    {
                        // listeye ekle ve bir Sa��ndaki git
                        comboList.Add(tile);
                        postY += 1;
                    }
                }
                else
                {
                    // Sa��ndaki Tile ile tipleri ayn� de�ilse ��k
                    postY = _boardHeight;
                }
            }
        }
        return comboList;
    }
    private List<Tile> CheckForCombosHorizontal(int x, int y, int tileType)
    {
        // Yatayda birbiri ile e�le�en tile varm� kontrol et ve birbirli ile yanyan gelen Tileleri listeye ekle
        List<Tile> comboList = new List<Tile>();
        // En son b�rakt���n Tile merkezi alarak sa�dan ve soldan kontrol et
        int preX = x - 1;
        int postX = x + 1;

        while (preX > -1 || postX < _boardWidth)
        {
            if (preX > -1)
            {
                int prePos = GetBoardPosition(preX, y); // Solundaki Tile'�n koordinat�n� al
                if (_gameBoard[prePos] == tileType)
                {
                    // Solundaki Tile ile tipleri ayn� ise Tile'� al 
                    Tile tile = GetTile(preX, y);
                    if (tile != null)
                    {
                        // listeye ekle ve bir soldakine git
                        comboList.Add(tile);
                        preX -= 1;
                    }
                }
                else
                {
                    // Solundaki Tile ile tipleri ayn� de�ilse ��k
                    preX = -1;
                }
            }
            if (postX < _boardWidth)
            {
                int postPos = GetBoardPosition(postX, y); // Sa��ndaki Tile'�n koordinat�n� al
                if (_gameBoard[postPos] == tileType)
                {
                    // Sa��ndaki Tile ile tipleri ayn� ise Tile'� al 
                    Tile tile = GetTile(postX, y);
                    if (tile != null)
                    {
                        // listeye ekle ve bir Sa��ndaki git
                        comboList.Add(tile);
                        postX += 1;
                    }
                }
                else
                {
                    // Sa��ndaki Tile ile tipleri ayn� de�ilse ��k
                    postX = _boardWidth;
                }
            }
        }
        return comboList;
    }
    IEnumerator HandleEmptySpaces()
    {
        // Board �zerindeki bo� alanlar� kontrol etme, a�a�� kayd�rma ve bo� alanlara tile ekleme
        yield return new WaitForSeconds(0.5f);
        CheckForEmptySpaces();
        yield return new WaitForSeconds(0.5f);
        FillEmptySpaces();

    }
    private void CheckForEmptySpaces()
    {
        // Board �zerindeki bo� alanlar� kontrol et ve a�a�� kayd�r
        // Kontol etmek i�in en a�a��dan ba�lar ve alt� bo� ise Tile a�a�� kayar
        for (int i = 0; i < _boardWidth; i++)
        {
            for (int j = (_boardHeight - 2); j > -1; j--)
            {
                if (_gameBoard[GetBoardPosition(i, j)] < 0) continue;
                // Tile alt� bo� ise bir a�a�� kayd�r
                Tile tile = GetTile(i, j);
                int y = j + 1;

                while (y < _boardHeight && _gameBoard[GetBoardPosition(i, y)] < 0)
                {
                    tile.transform.localPosition = new Vector3(i, -y, 0f);
                    _gameBoard[GetBoardPosition(i, y - 1)] = -1;    // Ge�i�ler Coroutine ile yava�lat�labilir
                    _gameBoard[GetBoardPosition(i, y)] = tile.TileType;
                    y += 1;
                }
            }
        }
    }
    private void FillEmptySpaces()
    {
        // Board �zerindeki bo� alanlar� kontrol et ve yeni Tile ekle
        for (int i = 0; i < _boardWidth; i++)
        {
            for (int j = 0; j < _boardHeight; j++)
            {
                int pos = GetBoardPosition(i, j);
                if (_gameBoard[pos] < 0)
                {
                    // Board �zerineki Tile bo� ise
                    _gameBoard[pos] = UnityEngine.Random.Range(0, _boardData.TileSprites.Length);
                    CretaTile(_gameBoard[pos], i, j);   // Dotween ile Y ekseinde yukar�dan a�a�� d��eblir
                }
            }
        }
    }
}

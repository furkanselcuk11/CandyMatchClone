using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoardLevelEditor : EditorWindow
{
    private string _levelEditorName;
    private int _boardWidth;
    private int _boardHeight;
    private int[] _gameBoard;
    private bool _isFillBoardRandom;
    private GameObject _tilePrefab;
    private GameObject _emptyTilePrefab;
    private GameObject _explosionPrefab;

    private Sprite _spriteToAdd;
    private List<Sprite> _sprites;
    private bool _isBoardSet;
    private int _selectedBrush;
    private List<string> _brushs;

    private Vector2 _scrollPos = Vector2.zero;

    [MenuItem("FurkanSelcuk/Board Editor")]
    static void Init()
    {
        BoardLevelEditor window = EditorWindow.GetWindow<BoardLevelEditor>();
        window.Show();
    }
    private void OnEnable()
    {
        _isBoardSet = false;
        _sprites = new List<Sprite>();
        _brushs = new List<string>();
    }
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Board Level Editor for Match-3 Game");
        EditorGUILayout.Space(10);

        _levelEditorName = EditorGUILayout.TextField("Level Editor Name", _levelEditorName);
        _boardWidth = EditorGUILayout.IntField("Board Width", _boardWidth);
        _boardHeight = EditorGUILayout.IntField("Board Height", _boardHeight);
        _isFillBoardRandom = EditorGUILayout.Toggle("Fill Board Randomly", _isFillBoardRandom);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        // Yatay grup oluþtur
        EditorGUILayout.BeginVertical();
        // Vertical grup oluþtur
        _tilePrefab = EditorGUILayout.ObjectField("Tile Prefab", _tilePrefab, typeof(GameObject)) as GameObject;
        _emptyTilePrefab = EditorGUILayout.ObjectField("Empty Tile Prefab", _emptyTilePrefab, typeof(GameObject)) as GameObject;
        _explosionPrefab = EditorGUILayout.ObjectField("Explosion Prefab", _explosionPrefab, typeof(GameObject)) as GameObject;
        EditorGUILayout.EndVertical();
        _spriteToAdd = EditorGUILayout.ObjectField("Tile Sprite", _spriteToAdd, typeof(Sprite)) as Sprite;
        if (GUILayout.Button("Add Sprite") && _spriteToAdd != null)
        {
            _sprites.Add(_spriteToAdd);
            _brushs.Add(_spriteToAdd.name);
            _spriteToAdd = null;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);

        _selectedBrush = EditorGUILayout.Popup("Selected Tile", _selectedBrush, _brushs.ToArray());
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Generate Board"))
        {
            GenerateBoard();
        }
        if (GUILayout.Button("Clear Board"))
        {
            _isBoardSet = false;
        }
        if (_isBoardSet)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            DrawGameBoard();
            GUILayout.EndScrollView();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Save Game Board"))
            {
                SaveGameBoardData();
            }
        }
    }
    private void GenerateBoard()
    {
        _gameBoard = new int[_boardHeight * _boardWidth];
        for (int i = 0; i < _gameBoard.Length; i++)
        {
            _gameBoard[i] = -1;
        }
        _isBoardSet = true;
    }
    private void DrawGameBoard()
    {
        GUILayout.BeginVertical();
        for (int j = 0; j < _boardHeight; j++)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < _boardWidth; i++)
            {
                if (GUILayout.Button(CreateTileTexture(_gameBoard[(_boardWidth * j) + i])))
                {
                    _gameBoard[(_boardWidth * j) + i] = _selectedBrush;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
    private GUIContent CreateTileTexture(int tileId)
    {
        GUIContent tileContent;
        int textureWidth = 50;
        int textureHeight = 50;
        if (_sprites.Count > 0)
        {
            textureWidth = (int)_sprites[0].rect.width;
            textureHeight = (int)_sprites[0].rect.height;
        }

        switch (tileId)
        {
            case -1:
                Texture2D emptyTexture = new Texture2D(textureWidth, textureHeight);
                tileContent = new GUIContent(emptyTexture);
                break;
            default:
                Texture2D tileTexture = new Texture2D((int)_sprites[tileId].rect.width, (int)_sprites[tileId].rect.height);
                var pixels = _sprites[tileId].texture.GetPixels((int)_sprites[tileId].textureRect.x, (int)_sprites[tileId].textureRect.y, (int)_sprites[tileId].rect.width, (int)_sprites[tileId].rect.height);
                tileTexture.SetPixels(pixels);
                tileTexture.Apply();
                tileContent = new GUIContent(tileTexture);
                break;
        }
        return tileContent;
    }
    private void SaveGameBoardData()
    {
        // Yeni BoardDataSO oluþturulur
        BoardDataSO boardDataSO = ScriptableObject.CreateInstance<BoardDataSO>();
        boardDataSO.BoardHeight = _boardHeight;
        boardDataSO.BoardWidth = _boardWidth;
        boardDataSO.IsFillBoardRandom = _isFillBoardRandom;
        boardDataSO.GameBoard = new int[_boardWidth * _boardHeight];
        boardDataSO.TilePrefab = _tilePrefab;
        boardDataSO.EmptyTilePrefab = _emptyTilePrefab;
        boardDataSO.ExplosionPrefab = _explosionPrefab;

        for (int i = 0; i < _gameBoard.Length; i++)
        {
            boardDataSO.GameBoard[i] = _gameBoard[i];
        }
        boardDataSO.TileSprites = new Sprite[_sprites.Count];
        for (int i = 0; i < _sprites.Count; i++)
        {
            boardDataSO.TileSprites[i] = _sprites[i];
        }

        string path = "Assets/Scripts/Data/" + _levelEditorName + ".asset";
        AssetDatabase.CreateAsset(boardDataSO, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
}

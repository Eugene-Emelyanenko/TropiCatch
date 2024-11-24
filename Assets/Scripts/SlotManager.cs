using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color pressedColor;
    [SerializeField] private string defaultText = "Spin";
    [SerializeField] private string spinText = "Spinning";

    [SerializeField] private Animator[] colAnimators;
    [SerializeField] private RectTransform[] slotColumns;
    [SerializeField] private Button spinButton;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject effect;
    public float destroyTime = 2f;

    public float spinTime = 2f;
    public float waitTime = 1f;

    private GameObject effectObj = null;
    private TextMeshProUGUI spinButtonText;
    private GameObject[,] _gameBoard;
    private int boardWidth;
    private int boardHeight;
    private List<GameObject> _matchLines = new List<GameObject>();
    private Vector3 _offset = new Vector3(0, 0, -1);
    private int score;

    private void Awake()
    {
        spinButtonText = spinButton.GetComponentInChildren<TextMeshProUGUI>();
        if (spinButtonText == null)
            Debug.LogError("Text component is not found");
    }

    private void Start()
    {
        ButtonReleased();
        spinButton.interactable = true;
        spinButtonText.text = defaultText;
        spinButton.onClick.AddListener(SpinSlots);
        InitializeGameBoard();
    }

    private void SpinSlots()
    {
        StartCoroutine(SpinCoroutine());
    }

    public void ButtonReleased()
    {
        if (spinButton.interactable)
            spinButtonText.color = defaultColor;
    }
    
    public void ButtonPressed()
    {
        if (spinButton.interactable)
            spinButtonText.color = pressedColor;
    }

    private IEnumerator SpinCoroutine()
    {
        spinButtonText.text = spinText;
        spinButton.interactable = false;

        // Clear lines from previous spin
        foreach (GameObject l in _matchLines)
        {
            GameObject.Destroy(l);
        }
        _matchLines.Clear();

        foreach (RectTransform column in slotColumns)
        {
            foreach (RectTransform slot in column)
            {
                Fruit fruit = slot.gameObject.GetComponent<Fruit>();
                if (fruit != null)
                {
                    fruit.SetRandomSlot();
                }
                else
                {
                    Debug.LogError("Fruit component is not found");
                }
            }
        }

        ClearScore();
        
        InitializeGameBoard();
        
        Debug.Log("Start Spin");
        foreach (Animator animator in colAnimators)
        {
            animator.SetBool("IsSpin", true);
        }
        SoundManager.Instance.PlaySpinSound();

        yield return new WaitForSeconds(spinTime);

        Debug.Log("End Spin");
        foreach (Animator animator in colAnimators)
        {
            animator.SetBool("IsSpin", false);
        }
        SoundManager.Instance.StopSpinSound();

        yield return new WaitForSeconds(waitTime);
        
        CheckForMatches();

        yield return new WaitForSeconds(waitTime);

        spinButton.interactable = true;
        spinButtonText.text = defaultText;
    }

    private void InitializeGameBoard()
    {
        boardHeight = slotColumns[0].childCount;
        boardWidth = slotColumns.Length;

        _gameBoard = new GameObject[boardHeight, boardWidth];
        Debug.Log($"Height: {slotColumns[0].childCount}. Width: {slotColumns.Length}");

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                _gameBoard[j, i] = slotColumns[i].GetChild(j).gameObject;
            }
        }

        DebugGameBoard();
    }

    private void DebugGameBoard()
    {
        for (int i = 0; i < _gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < _gameBoard.GetLength(1); j++)
            {
                Fruit fruit = _gameBoard[i, j].GetComponent<Fruit>();
                if (fruit != null)
                    Debug.Log($"_gameBoard[{i},{j}] = {fruit.slot}");
                else
                    Debug.LogError("Fruit component is not found");
            }
        }
    }

    private void CheckForMatches()
    {
        // Проверка вертикальных совпадений
        for (int i = 0; i < boardWidth; i++)
        {
            int matchLength = 1;
            GameObject matchBegin = _gameBoard[0, i];
            GameObject matchEnd = null;
            for (int j = 0; j < boardHeight - 1; j++)
            {
                Fruit cFruit = _gameBoard[j, i].GetComponent<Fruit>();
                Fruit nFruit = _gameBoard[j + 1, i].GetComponent<Fruit>();
                if (cFruit.slot == nFruit.slot)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 3)
                    {
                        matchEnd = _gameBoard[j, i];
                        //DrawLine(matchBegin.transform.position + _offset, matchEnd.transform.position + _offset);
                        CallMatchOnFruits(i, j - matchLength + 1, j, isVertical: true);
                        AddScore(cFruit.slot, matchLength);
                    }
                    matchBegin = _gameBoard[j + 1, i];
                    matchLength = 1;
                }
            }
            if (matchLength >= 3)
            {
                matchEnd = _gameBoard[boardHeight - 1, i];
                //DrawLine(matchBegin.transform.position + _offset, matchEnd.transform.position + _offset);
                CallMatchOnFruits(i, boardHeight - matchLength, boardHeight - 1, isVertical: true);
                AddScore(_gameBoard[boardHeight - 1, i].GetComponent<Fruit>().slot, matchLength);
            }
        }

        // Проверка горизонтальных совпадений
        for (int i = 0; i < boardHeight; i++)
        {
            int matchLength = 1;
            GameObject matchBegin = _gameBoard[i, 0];
            GameObject matchEnd = null;
            for (int j = 0; j < boardWidth - 1; j++)
            {
                Fruit cFruit = _gameBoard[i, j].GetComponent<Fruit>();
                Fruit nFruit = _gameBoard[i, j + 1].GetComponent<Fruit>();
                if (cFruit.slot == nFruit.slot)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 3)
                    {
                        matchEnd = _gameBoard[i, j];
                        //DrawLine(matchBegin.transform.position + _offset, matchEnd.transform.position + _offset);
                        CallMatchOnFruits(i, j - matchLength + 1, j, isVertical: false);
                        AddScore(cFruit.slot, matchLength);
                    }
                    matchBegin = _gameBoard[i, j + 1];
                    matchLength = 1;
                }
            }
            if (matchLength >= 3)
            {
                matchEnd = _gameBoard[i, boardWidth - 1];
                //DrawLine(matchBegin.transform.position + _offset, matchEnd.transform.position + _offset);
                CallMatchOnFruits(i, boardWidth - matchLength, boardWidth - 1, isVertical: false);
                AddScore(_gameBoard[i, boardWidth - 1].GetComponent<Fruit>().slot, matchLength);
            }
        }
        
        UpdateScore();
    }

    private void CallMatchOnFruits(int fixedIndex, int startIndex, int endIndex, bool isVertical)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            GameObject fruitObject;
            if (isVertical)
            {
                fruitObject = _gameBoard[i, fixedIndex];
            }
            else
            {
                fruitObject = _gameBoard[fixedIndex, i];
            }
            Fruit fruit = fruitObject.GetComponent<Fruit>();
            if (fruit != null)
            {
                fruit.Match(isVertical);
            }
        }

        if (effectObj == null)
        {
            effectObj = Instantiate(effect, Vector3.zero, quaternion.identity);
            Destroy(effectObj, destroyTime);
        }
        SoundManager.Instance.PlayCoinSound();
        SoundManager.Instance.Vibrate();
    }

    private void AddScore(Slot slot, int matchLength)
    {
        score += (int)slot * matchLength;
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.startWidth = .1f;
        lr.endWidth = .1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        _matchLines.Add(myLine);
    }

    private void UpdateScore()
    {
        scoreText.text = score.ToString();
        PlayerData playerData = PlayerDataManager.LoadPlayerData();
        playerData.totalScore += score;
        PlayerDataManager.SavePlayerData(playerData);
    }

    private void ClearScore()
    {
        score = 0;
        scoreText.text = score.ToString();
    }
}
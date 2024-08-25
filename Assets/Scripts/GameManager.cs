using System;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    Idle,
    Playing,
    Pause,
    End
}

public enum EndState
{
    Draw,
    LeftWin,
    RightWin,
    TimeOver
}

public class GameManager : MonoBehaviour
{


    [SerializeField] private GameObject leftPlayerPrefab;
    [SerializeField] private GameObject rightPlayerPrefab;

    public Player leftPlayer;
    public Player rightPlayer;

    public GameState gameState = GameState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        // Load the prefabs
        leftPlayerPrefab = Resources.Load<GameObject>("Ninja");
        rightPlayerPrefab = Resources.Load<GameObject>("Ninja");
        gameState = GameState.Playing;
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:
            case GameState.Pause:
                UpdateCamera();
                break;
        }
        HandlePlayer();
    }

    public void StartGame()
    {
        float[] positionValue = { -6.5f, 3f, 7f };

        // Instantiate the players from the prefabs
        GameObject leftPlayerObject = Instantiate(leftPlayerPrefab);
        GameObject rightPlayerObject = Instantiate(rightPlayerPrefab);

        leftPlayerObject.layer = LayerMask.NameToLayer("Character");
        rightPlayerObject.layer = LayerMask.NameToLayer("Character");

        // Get the Player components from the instantiated objects
        leftPlayer = leftPlayerObject.GetComponent<Player>();
        rightPlayer = rightPlayerObject.GetComponent<Player>();

        // Initialize the players
        leftPlayer.Initialize(new Vector3(positionValue[0], positionValue[1], positionValue[2]), false, null);
        rightPlayer.Initialize(new Vector3(-positionValue[0], positionValue[1], positionValue[2]), true, new Dictionary<string, KeyCode>
        {
            { "attack", KeyCode.E },
            { "skill", KeyCode.Q },
            { "back", KeyCode.LeftArrow },
            { "front", KeyCode.RightArrow },
            { "jump", KeyCode.UpArrow }
        });
    }

    public void EndGame(EndState endState)
    {
        gameState = GameState.End;
        // Clean up the players
        gameObject.GetComponent<GameUI>().StopCoroutine(gameObject.GetComponent<GameUI>().TimerCoroutine);
        Destroy(leftPlayer.gameObject);
        Destroy(rightPlayer.gameObject);
    }

    private void UpdateCamera()
    {
        GameObject camera = GameObject.Find("Camera");
        if (camera != null)
        {
            Vector3 cameraPosition = camera.transform.position;
            cameraPosition.x = (leftPlayer.transform.position.x + rightPlayer.transform.position.x) / 2;
            cameraPosition.y = (leftPlayer.transform.position.y + rightPlayer.transform.position.y) / 2;
            cameraPosition.z = -((Math.Abs(leftPlayer.transform.position.x) + Math.Abs(rightPlayer.transform.position.x)) / 2);
            camera.transform.position = cameraPosition;
        }
    }

    private void HandlePlayer()
    {
        if (gameState == GameState.Playing && (leftPlayer.hp <= 0 || rightPlayer.hp <= 0))
        {
            Debug.Log("Game Over");
            if(leftPlayer.hp <= 0 && rightPlayer.hp <= 0)
            {
                Debug.Log("Draw");
                EndGame(EndState.Draw);
            }
            else if(leftPlayer.hp <= 0)
            {
                Debug.Log("Right Player Win");
                EndGame(EndState.RightWin);
            }
            else
            {
                Debug.Log("Right Player Win");
                EndGame(EndState.LeftWin);
            }

        } // 나중에 관련 코드 추가
    }
}
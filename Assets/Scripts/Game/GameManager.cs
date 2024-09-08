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

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject leftPlayerPrefab;
        [SerializeField] private GameObject rightPlayerPrefab;

        public GameState    gameState = GameState.Idle;
        public Player       leftPlayer;
        public Player       rightPlayer;

        // Start is called before the first frame update
        private void Start()
        {
            // Load the prefabs
            leftPlayerPrefab    = Resources.Load<GameObject>(PlayerPrefs.GetString("LeftPlayer"));
            rightPlayerPrefab   = Resources.Load<GameObject>(PlayerPrefs.GetString("RightPlayer"));
            gameState           = GameState.Playing;
            StartGame();
        }

        // Update is called once per frame
        private void Update()
        {
            HandlePlayer();
        }

        private void LateUpdate()
        {
            switch (gameState)
            {
                case GameState.Playing:
                case GameState.Pause:
                    UpdateCamera();
                    break;
            }
        }

        public void StartGame()
        {
            float[] positionValue = { -6.5f, 3f, 7f };

            // Instantiate the players from the prefabs
            var leftPlayerObject    = Instantiate(leftPlayerPrefab);
            var rightPlayerObject   = Instantiate(rightPlayerPrefab);

            leftPlayerObject.layer  = LayerMask.NameToLayer("Character");
            rightPlayerObject.layer = LayerMask.NameToLayer("Character");

            // Get the Player components from the instantiated objects
            leftPlayer  = leftPlayerObject  .GetComponent<Player>();
            rightPlayer = rightPlayerObject .GetComponent<Player>();

            // Initialize the players
            leftPlayer  .Initialize(new Vector3(positionValue[0], positionValue[1], positionValue[2]), false, null);
            rightPlayer .Initialize(new Vector3(-positionValue[0], positionValue[1], positionValue[2]), true,
                new Dictionary<string, KeyCode>
                {
                    { "attack", KeyCode.N },
                    { "skill",  KeyCode.M },
                    { "back",   KeyCode.LeftArrow },
                    { "front",  KeyCode.RightArrow },
                    { "jump",   KeyCode.UpArrow }
                });
        }

        public void EndGame(EndState endState)
        {
            gameState = GameState.End;
            // Clean up the players
            gameObject.GetComponent<GameUI>().StopCoroutine(gameObject.GetComponent<GameUI>().TimerCoroutine);
            Destroy(leftPlayer  .gameObject);
            Destroy(rightPlayer .gameObject);
        }

        private void UpdateCamera()
        {
            var camera = GameObject.Find("Camera");
            if (camera != null)
            {
                var cameraPosition = camera.transform.position;
                cameraPosition.x = (leftPlayer.transform.position.x + rightPlayer.transform.position.x) / 2;
                cameraPosition.y = (leftPlayer.transform.position.y + rightPlayer.transform.position.y) / 2;
                cameraPosition.z =
                    -((Math.Abs(leftPlayer.transform.position.x) + Math.Abs(rightPlayer.transform.position.x)) / 2);
                camera.transform.position = cameraPosition;
            }
        }

        private void HandlePlayer()
        {
            if (gameState == GameState.Playing && (leftPlayer.hp <= 0 || rightPlayer.hp <= 0))
            {
                Debug.Log("Game Over");
                if (leftPlayer.hp <= 0 && rightPlayer.hp <= 0)
                {
                    Debug.Log("Draw");
                    EndGame(EndState.Draw);
                }
                else if (leftPlayer.hp <= 0)
                {
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
}
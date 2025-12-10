using Riftboud.Core;
using UnityEngine;

namespace Riftbound.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentState = GameState.Playing;
        }

        public void SetGameState(GameState newState)
        {
            CurrentState = newState;
            Debug.Log("Game State cambiado a: " + newState);
        }
    }
}
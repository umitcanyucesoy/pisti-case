using System.Collections.Generic;
using _Case.Scripts.Cards;
using _Case.Scripts.Players;
using UnityEngine;

namespace _Case.Scripts.Game
{
    public class TurnManager : MonoBehaviour
    {
        public float botPlayDelay = 1f;
        public int currentPlayerIndex = 0;
        public List<Player> players;

        public static TurnManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        public void NextTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

            Player nextPlayer = players[currentPlayerIndex];
            if (nextPlayer.isBot)
            {
                Invoke(nameof(BotPlayTurn), botPlayDelay);
            }
            else
            {
                PlayerManager.Instance.SetPlayerHandColliders(true);
            }
        }
        
        private void BotPlayTurn()
        {
            if (TableManager.Instance.isClearing)
            {
                Invoke(nameof(BotPlayTurn), botPlayDelay);
                return;
            }

            Player currentPlayer = players[currentPlayerIndex];
            if (currentPlayer != null && currentPlayer.myCards.Count > 0)
            {
                Card botCard = currentPlayer.myCards[0];
                TableManager.Instance.PlayCard(botCard.gameObject, true);
            }
        }
    }
}
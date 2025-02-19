using _Case.Scripts.Cards;
using _Case.Scripts.Players;
using UnityEngine;

namespace _Case.Scripts.Game
{
    public class TurnManager : MonoBehaviour
    {
        public float botPlayDelay = 1f;
        public bool isPlayerTurn = true;

        public static TurnManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        public void OnPlayerCardPlayed()
        {
            isPlayerTurn = false;
            Invoke(nameof(BotPlayTurn), botPlayDelay);
        }

        private void BotPlayTurn()
        {
            Player bot = PlayerManager.Instance.players.Find(p => p.isBot);
            if (bot != null && bot.myCards.Count > 0)
            {
                // Botun elindeki ilk kartı otomatik oynatırken, automated parametresini true olarak gönderiyoruz.
                Card botCard = bot.myCards[0];
                TableManager.Instance.PlayCard(botCard.gameObject, true);
            }
        }
        
        public void OnBotCardFinished()
        {
            isPlayerTurn = true;
            PlayerManager.Instance.SetPlayerHandColliders(true);
        }
    }
}
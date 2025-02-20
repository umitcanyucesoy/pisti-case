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
            // Eğer vanish animasyonu devam ediyorsa, bot hamlesini erteleyelim.
            if (TableManager.Instance.isClearing)
            {
                Debug.Log("Vanish animasyonu devam ediyor, bot hamlesi erteleniyor.");
                Invoke(nameof(BotPlayTurn), botPlayDelay);
                return;
            }
        
            Player bot = PlayerManager.Instance.players.Find(p => p.isBot);
            if (bot != null && bot.myCards.Count > 0)
            {
                Card botCard = bot.myCards[0];
                // Bot hamlesini tetiklerken automated parametresini true olarak gönderiyoruz.
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
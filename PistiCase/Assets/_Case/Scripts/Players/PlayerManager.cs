using System;
using System.Collections.Generic;
using _Case.Scripts.Cards;
using UnityEngine;

namespace _Case.Scripts.Players
{
    public class PlayerManager : MonoBehaviour
    {
        public CardManager cardManager;
        public List<Player> players;
        
        public static PlayerManager Instance { get; private set; }
        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void GiveCard(int amount)
        {
            foreach (var player in players)
            {
                int dealt = 0;
                while (dealt < amount && cardManager.CardPool.Count > 0)
                {
                    Card card = cardManager.CardPool.Pop();
                    if (card.isTableCard)
                        continue;
            
                    player.TakeCard(card);
                    dealt++;
                }
            }
        }
    }
}
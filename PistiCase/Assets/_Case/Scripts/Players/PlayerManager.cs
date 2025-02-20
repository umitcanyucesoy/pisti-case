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
        
        public void SetPlayerHandColliders(bool enable)
        {
            Player user = players.Find(p => !p.isBot);
            if (user == null) return;

            foreach (Card card in user.myCards)
            {
                Collider coll = card.GetComponent<Collider>();
                if (coll != null)
                {
                    coll.enabled = enable;
                }
            }
        }

        public void GiveCard(int amount)
        {
            foreach (var player in players)
            {
                int dealt = 0;
                List<Card> skippedTableCards = new List<Card>();

                while (dealt < amount && cardManager.CardPool.Count > 0)
                {
                    Card card = cardManager.CardPool.Pop();
                    if (card.isTableCard)
                    {
                        skippedTableCards.Add(card);
                        continue;
                    }

                    if (!card.gameObject.activeSelf)
                    {
                        card.gameObject.SetActive(true);
                    }

                    player.TakeCard(card);
                    dealt++;
                }

                foreach (Card tableCard in skippedTableCards)
                {
                    cardManager.CardPool.Push(tableCard);
                }
            }
        }

        }
    }
using System.Collections.Generic;
using _Case.Scripts.Cards;
using _Case.Scripts.Game;
using UnityEngine;

namespace _Case.Scripts.Players
{
    public class Player : MonoBehaviour
    {
        [Header("----- Card Settings -----")]
        public List<Card> myCards;
        public float spacing = 1f;
        public float cardWidth = 1.2f;

        [Header("----- Player Elements -----")]
        public Transform handContainer;
        public bool isBot = false;

        public void TakeCard(Card card)
        {
            if (card.isTableCard)
                return;

            card.owner = this;

            myCards.Add(card);
            card.transform.SetParent(handContainer, false);
            card.transform.localScale = Vector3.one;

            if (isBot)
            {
                if (card.cardDisplay)
                    card.cardDisplay.spriteRenderer.sprite = TableManager.Instance.botCardBackSprite;
            }
            else
            {
                card.cardDisplay.UpdateDisplay();
            }

            UpdateCardPositions();
        }
        
        private void UpdateCardPositions()
        {
            for (int i = 0; i < myCards.Count; i++)
                myCards[i].transform.localPosition = new Vector3(i * (cardWidth + spacing), 0f, 0f);
        }
    }
}
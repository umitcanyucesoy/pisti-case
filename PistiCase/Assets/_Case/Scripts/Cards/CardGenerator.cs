using System;
using System.Collections.Generic;
using _Case.Scripts.Data;
using _Case.Scripts.Enums;
using UnityEngine;

namespace _Case.Scripts.Cards
{
    public class CardGenerator : MonoBehaviour
    {
        public List<Sprite> cardSprites;
        public Card cardPrefab;
        public CardManager cardManager;
        public Transform cardContainer;
        
        private const int CardTypeMaxSize = 13;

        [ContextMenu(nameof(GenerateCards))]
        private void Awake()
        {
            GenerateCards();
        }

        public void GenerateCards()
        {
            CardType currentType = CardType.Spades;
            
            for (int i = 0; i < cardSprites.Count; i++)
            {
                var value = i % CardTypeMaxSize;
                value++;

                if (value == 1 && i != 0) currentType++;
    
                var card = Instantiate(cardPrefab, cardContainer);
                card.cardData = new CardData(value, currentType);
                card.cardDisplay.front = cardSprites[i];
                card.cardDisplay.UpdateDisplay();
                card.name = $"Card {currentType} {value:00}";

                cardManager.cardList.Add(card);
            }
        }
    }
}
using System.Collections.Generic;
using _Case.Scripts.Data;
using _Case.Scripts.Extensions;
using UnityEngine;

namespace _Case.Scripts.Cards
{
    public class CardManager : MonoBehaviour
    {
        public List<Card> cardList;
        public Stack<Card> CardPool;

        public Card GetCard(CardData data)
        {
            return cardList.Find(x => Equals(x.cardData, data));
        }

        public void Shuffle()
        {
            CardPool = new Stack<Card>(cardList);
            CardPool.Shuffle();
        }
    }
}
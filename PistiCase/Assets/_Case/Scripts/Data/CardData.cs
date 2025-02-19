using _Case.Scripts.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Case.Scripts.Data
{
    public class CardData
    {
        public int Value;
        public CardType CardType;

        public CardData(int value, CardType cardType)
        {
            Value = value;
            CardType = cardType;
        }
    }
}
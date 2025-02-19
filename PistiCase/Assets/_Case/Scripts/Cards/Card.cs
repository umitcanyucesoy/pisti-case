using _Case.Scripts.Data;
using _Case.Scripts.Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Case.Scripts.Cards
{
    public class Card : MonoBehaviour
    {
        public CardData cardData;
        public CardDisplay cardDisplay;
        public bool isTableCard = false;
        
        private void OnMouseDown()
        {
            if(TableManager.Instance)
                TableManager.Instance.PlayCard(gameObject);
        }
    }
}
using System;
using _Case.Scripts.Cards;
using _Case.Scripts.Players;
using UnityEngine;

namespace _Case.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        public PlayerManager playerManager;
        public CardManager cardManager;

        private void Start()
        {
            cardManager.Shuffle();
            playerManager.GiveCard(4);
        }
    }
}
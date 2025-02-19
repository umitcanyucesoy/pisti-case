using System.Collections.Generic;
using _Case.Scripts.Cards;
using _Case.Scripts.Players;
using DG.Tweening;
using UnityEngine;

namespace _Case.Scripts.Game
{
    public class TableManager : MonoBehaviour
    {
        [Header("----- Table Slot Transforms -----")]
        public Transform closedCardSlot1;
        public Transform closedCardSlot2;
        public Transform openCardSlot;
        public Transform deckSlot;

        [Header("----- Deck Slots -----")] 
        public List<Card> playedCards;
        public CardManager cardManager;
        public Sprite botCardBackSprite;
        public float moveDuration = .5f;
        
        public static TableManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if(cardManager != null && (cardManager.CardPool == null || cardManager.CardPool.Count == 0))
            {
                cardManager.Shuffle();
            }
            
            CreateTableCard(closedCardSlot1, botCardBackSprite, 15f);
            CreateTableCard(closedCardSlot2, botCardBackSprite, -10f);
            CreateTableCard(openCardSlot, null, 0);
            CreateTableCard(deckSlot, botCardBackSprite, 0);
        }

        private void CreateTableCard(Transform slot, Sprite sprite, float rotation)
        {
            if (cardManager != null && cardManager.CardPool.Count > 0 && slot != null)
            {
                Card card = cardManager.CardPool.Pop();
                card.isTableCard = true;
                card.transform.SetParent(slot, false);
                card.transform.localPosition = Vector3.zero;
                card.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                Collider collider = card.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false;
                }

                if (sprite != null && card.cardDisplay != null)
                {
                    card.cardDisplay.spriteRenderer.sprite = sprite;
                }
            }
        }
        
        public void PlayCard(GameObject cardObj)
        {
            Card card = cardObj.GetComponent<Card>();
            if (card == null) return;
            if (card.isTableCard) return;

            Player owner = card.owner;
            if (owner == null)
            {
                owner = card.GetComponentInParent<Player>();
                if (owner == null) return;
            }

            if (!owner.isBot && !TurnManager.Instance.isPlayerTurn)
            {
                Debug.Log("Sıra player'da değil, oynamaz.");
                return;
            }
            if (owner.isBot && TurnManager.Instance.isPlayerTurn)
            {
                Debug.Log("Sıra bot'ta değil, oynamaz.");
                return;
            }

            if (!owner.isBot && card.cardDisplay != null && card.cardDisplay.spriteRenderer.sprite == botCardBackSprite)
            {
                Debug.Log("Bu kart bot'a ait, player oynayamaz!");
                return;
            }

            if (!owner.isBot)
            {
                TurnManager.Instance.isPlayerTurn = false;
                PlayerManager.Instance.SetPlayerHandColliders(false);
            }
            else
            {
                card.cardDisplay.UpdateDisplay();
            }

            Vector3 originalWorldScale = cardObj.transform.lossyScale;
            cardObj.transform.SetParent(null, true);

            Sequence seq = DOTween.Sequence();
            seq.Join(
                cardObj.transform.DOMove(openCardSlot.position, moveDuration)
                    .SetEase(Ease.InOutQuad)
            );
            seq.Join(
                cardObj.transform.DORotate(new Vector3(0, 0, 20f), moveDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(2, LoopType.Yoyo)
            );

            int newOrder = 100 + playedCards.Count;
            if (card.cardDisplay && card.cardDisplay.spriteRenderer)
            {
                card.cardDisplay.spriteRenderer.sortingOrder = newOrder;
            }

            seq.OnComplete(() =>
            {
                cardObj.transform.SetParent(openCardSlot, false);
                cardObj.transform.localPosition = Vector3.zero;
                cardObj.transform.localRotation = Quaternion.identity;

                Vector3 parentWorldScale = openCardSlot.lossyScale;
                cardObj.transform.localScale = new Vector3(
                    originalWorldScale.x / parentWorldScale.x,
                    originalWorldScale.y / parentWorldScale.y,
                    originalWorldScale.z / parentWorldScale.z
                );

                Collider col = cardObj.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                }

                AddPlayedCard(card, owner);

                if (!owner.isBot)
                {
                    TurnManager.Instance.isPlayerTurn = true;
                    TurnManager.Instance.OnPlayerCardPlayed();
                }
                else
                {
                    TurnManager.Instance.OnBotCardFinished();
                }
            });
        }

        private void AddPlayedCard(Card card, Player owner)
        {
            playedCards.Add(card);

            if (owner.myCards.Contains(card))
            {
                owner.myCards.Remove(card);
                Debug.Log($"Kart çıkarıldı: {owner.name} elinde artık {owner.myCards.Count} kart var.");
            }

            Player bot = PlayerManager.Instance.players.Find(p => p.isBot);
            Player user = PlayerManager.Instance.players.Find(p => !p.isBot);

            if (bot != null && user != null)
            {
                if (bot.myCards.Count == 0 && user.myCards.Count == 0)
                {
                    PlayerManager.Instance.GiveCard(4);
                }
            }
        }
    }
}

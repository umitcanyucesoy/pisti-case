using System;
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
        public Transform botMatchSlot;
        public Transform playerMatchSlot;

        [Header("----- Deck Slots -----")] 
        public List<Card> playedCards;
        public CardManager cardManager;
        public Sprite botCardBackSprite;
        public float moveDuration = .5f;
        
        private bool _isClearing = false;
        public bool isClearing => _isClearing;

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
            CreateTableCard(deckSlot, botCardBackSprite, 0);
        }

        private void Update()
        {
            if (openCardSlot && !_isClearing && openCardSlot.childCount == 0)
            {
                if (cardManager && cardManager.CardPool.Count > 0)
                {
                    CreateTableCard(openCardSlot, null, 0);
                }
            }
        }

        private void CreateTableCard(Transform slot, Sprite sprite, float rotation)
        {
            if (cardManager != null && cardManager.CardPool.Count > 0 && slot != null)
            {
                Card card = cardManager.CardPool.Pop();
        
                if (slot == openCardSlot)
                {
                    card.isTableCard = false; 
                    playedCards.Add(card);
                    if (card.cardDisplay != null && card.cardDisplay.spriteRenderer != null)
                    {
                        card.cardDisplay.spriteRenderer.sortingOrder = 100;
                    }
                }
                else
                {
                    card.isTableCard = true;
                    if (card.cardDisplay != null && card.cardDisplay.spriteRenderer != null)
                    {
                        card.cardDisplay.spriteRenderer.sortingOrder = 10;
                    }
                }

                card.transform.SetParent(slot, false);
                card.transform.localPosition = Vector3.zero;
                card.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                Collider collider = card.GetComponent<Collider>();
                if (collider)
                    collider.enabled = false;

                if (card.cardDisplay)
                {
                    if (sprite)
                        card.cardDisplay.spriteRenderer.sprite = sprite;
                    else
                        card.cardDisplay.UpdateDisplay();
                }
            }
        }

        
        public void PlayCard(GameObject cardObj, bool automated = false)
        {
            if (_isClearing)
            {
                Debug.Log("Clear animasyonu devam ediyor, kart oynanamaz.");
                return;
            }
                    
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

            if (owner.isBot && !automated)
            {
                Debug.Log("Bot kartları otomatik oynanıyor, elle oynanamaz.");
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
                    
            BoxCollider2D col = cardObj.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.enabled = false;
            }
                    
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
    
            if (playedCards.Count >= 2)
            {
                Card lastCard = playedCards[playedCards.Count - 1];
                Card prevCard = playedCards[playedCards.Count - 2];

                if (lastCard.cardData.Value == 11)
                {
                    _isClearing = true;
                    ClearOpenSlot(owner);
                }
                else if (lastCard.cardData.Value == prevCard.cardData.Value)
                {
                    _isClearing = true;
                    ClearOpenSlot(owner);
                }
            }
        }

        
        private void ClearOpenSlot(Player whoCleared)
        {
            int scoreIncrease = playedCards.Count * 10;
            whoCleared.score += scoreIncrease;
            Debug.Log($"{whoCleared.name} {playedCards.Count} kart için +{scoreIncrease} puan kazandı. Yeni skor: {whoCleared.score}");

            Sequence vanishSeq = DOTween.Sequence();
            List<Card> vanishCards = new List<Card>();
            vanishCards.AddRange(playedCards);

            Card closed1 = closedCardSlot1.GetComponentInChildren<Card>();
            if (closed1 != null)
                vanishCards.Add(closed1);
            Card closed2 = closedCardSlot2.GetComponentInChildren<Card>();
            if (closed2 != null)
                vanishCards.Add(closed2);

            Transform targetMatchSlot = whoCleared.isBot ? botMatchSlot : playerMatchSlot;

            float yOffset = 0.2f;
            for (int i = 0; i < vanishCards.Count; i++)
            {
                Vector3 targetPosition = targetMatchSlot.position + new Vector3(0, yOffset * i, 0);
                vanishSeq.Insert(i * 0.02f, vanishCards[i].transform.DOMove(targetPosition, .4f).SetEase(Ease.InOutQuad));
                vanishSeq.Join(vanishCards[i].transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f).SetEase(Ease.InOutQuad));
            }

            vanishSeq.OnComplete(() =>
            {
                foreach (Card c in vanishCards)
                {
                    c.gameObject.SetActive(false);
                }
                playedCards.Clear();
                _isClearing = false;
                whoCleared.UpdateScoreUI();
            });
        }

    } 
}

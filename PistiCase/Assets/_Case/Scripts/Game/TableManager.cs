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
                
                // Table kartı veya bot kartı ise bir şey yapma.
                if (card.isTableCard) return;
                if (card.cardDisplay != null && card.cardDisplay.spriteRenderer.sprite == botCardBackSprite) return;

                // Mevcut dünya ölçeğini sakla (lossyScale).
                Vector3 originalWorldScale = cardObj.transform.lossyScale;
                
                // Kartı animasyon başlamadan önce parent'ından ayır ki DOMove'u dünyada yapabilesin.
                // worldPositionStays = true diyerek, kartın anlık world position/rotation/scale'ını koruyoruz.
                cardObj.transform.SetParent(null, true);
                
                // DOTween Sequence oluştur.
                Sequence seq = DOTween.Sequence();

                // 1) Kartın konumunu openCardSlot'un dünya konumuna taşı (dünyada hareket).
                seq.Join(
                    cardObj.transform.DOMove(openCardSlot.position, moveDuration)
                        .SetEase(Ease.InOutQuad)
                );

                // 2) Aynı anda hafif bir z ekseni dönmesi ekle.
                //    Örneğin 20 derece sağa dönüp geri gelsin, Yoyo loop ile 2 kere.
                seq.Join(
                    cardObj.transform.DORotate(new Vector3(0, 0, 20f), moveDuration, RotateMode.Fast)
                        .SetEase(Ease.InOutQuad)
                        .SetLoops(2, LoopType.Yoyo)
                );

                // Animasyon bittiğinde...
                seq.OnComplete(() =>
                {
                    // Kartı openCardSlot altına al, local pozisyonu ve rotasyonu sıfırla.
                    cardObj.transform.SetParent(openCardSlot, false);
                    cardObj.transform.localPosition = Vector3.zero;
                    cardObj.transform.localRotation = Quaternion.identity;

                    // Kartın global (dünya) ölçeği değişmesin istiyoruz.
                    // Ancak artık parent'ı openCardSlot olduğu için localScale hesaplamamız lazım.
                    // Parent'ın lossyScale değerine bölerek orantılı şekilde local scale'ı ayarlarız.
                    Vector3 parentWorldScale = openCardSlot.lossyScale;
                    cardObj.transform.localScale = new Vector3(
                        originalWorldScale.x / parentWorldScale.x,
                        originalWorldScale.y / parentWorldScale.y,
                        originalWorldScale.z / parentWorldScale.z
                    );
                    
                    // Kartı oynanan kartlar listesine ekleyelim.
                    AddPlayedCard(cardObj.GetComponent<Card>());
                });
            }



        private void AddPlayedCard(Card card)
        {
            playedCards.Add(card);

            Player player = card.GetComponentInParent<Player>();
            if (player && player.myCards.Contains(card))
            {
                player.myCards.Remove(card);

                if (player.myCards.Count == 0)
                {
                    PlayerManager.Instance.GiveCard(4);
                }
            }
        }
    }
}
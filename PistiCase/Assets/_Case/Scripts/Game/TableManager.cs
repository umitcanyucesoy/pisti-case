using System;
using System.Collections.Generic;
using _Case.Scripts.Cards;
using _Case.Scripts.Players;
using _Case.Scripts.UI;
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
        public List<Transform> botMatchSlots;
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
            
            CreateTableCard(openCardSlot, null, 0);
            CreateTableCard(openCardSlot, null, 0);
            CreateTableCard(closedCardSlot1, botCardBackSprite, 15f);
            CreateTableCard(closedCardSlot2, botCardBackSprite, -10f);
            CreateTableCard(deckSlot, botCardBackSprite, 0);
        }

        private void Start()
        {
            // 1) En az 23 kart olsun
            EnsureAtLeast23Cards();

            // 2) Masa kartlarını oluştur
            

            // Open Card
            Card openCard = openCardSlot.GetComponentInChildren<Card>();
            if (openCard != null)
            {
                openCard.cardDisplay.UpdateDisplay();
                openCard.transform.localPosition = Vector3.zero;
                openCard.transform.localRotation = Quaternion.identity;
            }

            // Deck Slot'a 2 kart eklemek istediğimizi varsayıyoruz
        }

        /// <summary>
        /// Eğer kart havuzu (CardPool) 23'ten az ise, deste/karıştırma işlemini tekrarlar.
        /// Bu örnekte Shuffle()'ın yeni kart eklediğini varsayıyoruz.
        /// </summary>
        private void EnsureAtLeast23Cards()
        {
            if (cardManager == null)
                return;

            // CardPool null ise en azından boş bir Stack oluştur.
            if (cardManager.CardPool == null)
            {
                cardManager.CardPool = new Stack<Card>();
            }

            // Eğer 23'ten azsa tekrar Shuffle() veya elinizde varsa ResetDeck() gibi bir metot çağırabilirsiniz.
            if (cardManager.CardPool.Count < 24)
            {
                Debug.Log($"CardPool'da {cardManager.CardPool.Count} kart var. 23'e tamamlamak için Shuffle çağrılıyor.");
                cardManager.Shuffle();
            }
        }

        private void CreateTableCard(Transform slot, Sprite sprite, float rotation)
        {
            if (cardManager == null || slot == null)
            {
                Debug.LogWarning("CardManager veya slot eksik.");
                return;
            }

            // Eğer kart havuzu boşsa, yeniden karıştır.
            if (cardManager.CardPool == null || cardManager.CardPool.Count == 0)
            {
                Debug.Log("CardPool boş, yeniden Shuffle yapılıyor...");
                cardManager.Shuffle();
            }

            // Yeniden kontrol: Kart varsa devam et.
            if (cardManager.CardPool.Count > 0)
            {
                Card card = cardManager.CardPool.Pop();

                if (slot == openCardSlot)
                {
                    // Open slot için kartın TableCard flag'ını false yapıp oynanan kartlar listesine ekliyoruz.
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
            else
            {
                Debug.LogWarning("Yeterli kart bulunamadı! Slot: " + slot.name);
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

            if (TurnManager.Instance.players[TurnManager.Instance.currentPlayerIndex] != owner)
            {
                Debug.Log("Sıra size ait değil, oynamaz.");
                return;
            }

            if (owner.isBot && !automated)
            {
                Debug.Log("Bot kartları otomatik oynanıyor, elle oynanamaz.");
                return;
            }

            if (!owner.isBot)
            {
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
            seq.Append(
                cardObj.transform.DOMove(openCardSlot.position, moveDuration)
                    .SetEase(Ease.InOutQuad)
            );
            seq.Join(
                // Kartın rota animasyonunu, hedef açısı olan (0,0,0)'a yavaşça geçecek şekilde ayarlıyoruz.
                cardObj.transform.DORotate(Vector3.zero, moveDuration)
                    .SetEase(Ease.InOutQuad)
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

                TurnManager.Instance.NextTurn();
            });
        }

        private void AddPlayedCard(Card card, Player owner)
        {
            if (card == null)
            {
                Debug.LogWarning("AddPlayedCard => card is null!");
                return;
            }
            if (owner == null)
            {
                Debug.LogWarning($"AddPlayedCard => owner is null! Card: {card.name}");
                return;
            }
            if (owner.myCards == null)
            {
                Debug.LogWarning($"AddPlayedCard => owner.myCards is null! Owner: {owner.name}");
                return;
            }

            playedCards.Add(card);

            if (owner.myCards.Contains(card))
            {
                owner.myCards.Remove(card);
                Debug.Log($"Kart çıkarıldı: {owner.name} elinde artık {owner.myCards.Count} kart var.");
            }

            Player bot = PlayerManager.Instance.players.Find(p => p.isBot);
            Player user = PlayerManager.Instance.players.Find(p => !p.isBot);

            if (bot == null || user == null)
            {
                Debug.LogWarning("Bot veya user bulunamadı!");
                return;
            }
            if (bot.myCards == null || user.myCards == null)
            {
                Debug.LogWarning("Bot veya user myCards listesi null! Dağıtım adımı atlanıyor.");
                return;
            }

            bool allHandsEmpty = PlayerManager.Instance.players.TrueForAll(p => p.myCards.Count == 0);
            if (allHandsEmpty)
            {
                PlayerManager.Instance.GiveCard(4);

                allHandsEmpty = PlayerManager.Instance.players.TrueForAll(p => p.myCards.Count == 0);
                if (allHandsEmpty)
                {
                    ResultPanelManager.Instance.ShowResult();
                    return;
                }
            }

            if (playedCards.Count >= 2)
            {
                Card lastCard = playedCards[playedCards.Count - 1];
                Card prevCard = playedCards[playedCards.Count - 2];

                // Vale (11) veya son 2 kart aynı değerse Clear
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

        // Hedef vanish alanını belirleyelim:
        Transform targetMatchSlot = null;
        if (whoCleared.isBot)
        {
            // Sadece botları filtreleyelim
            List<Player> botPlayers = PlayerManager.Instance.players.FindAll(p => p.isBot);
            int botIndex = botPlayers.IndexOf(whoCleared);
            if (botMatchSlots != null && botMatchSlots.Count > botIndex && botIndex >= 0)
            {
                targetMatchSlot = botMatchSlots[botIndex];
            }
            else
            {
                // Eğer liste boşsa veya index uymazsa, fallback olarak ilk elemanı kullanın.
                targetMatchSlot = botMatchSlots != null && botMatchSlots.Count > 0 ? botMatchSlots[0] : null;
            }
        }
        else
        {
            targetMatchSlot = playerMatchSlot;
        }

        if(targetMatchSlot == null)
        {
            Debug.LogWarning("Hedef match slot bulunamadı!");
            return;
        }

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

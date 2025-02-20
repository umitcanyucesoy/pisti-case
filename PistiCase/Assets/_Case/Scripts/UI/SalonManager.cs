using System.Globalization;
using _Case.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Case.Scripts.UI
{
    public class SalonManager : MonoBehaviour
    {
        [Header("----- Room Settings -----")]
        [SerializeField] private string roomName;
        [SerializeField] private int minBet;
        [SerializeField] private int maxBet;

        [Header("----- UI References -----")]
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private Button playNowButton;

        [Header("----- Bet Slider UI -----")]
        [SerializeField] private Slider betSlider;                  
        [SerializeField] private TextMeshProUGUI currentBetText;      
        [SerializeField] private TextMeshProUGUI minBetText;          
        [SerializeField] private TextMeshProUGUI maxBetText;       

        [Header("----- Insufficient Funds Popup -----")]
        [SerializeField] private GameObject insufficientFundsPopup;
        [SerializeField] private Button popupExitButton;

        private void Start()
        {
            if (playNowButton != null)
                playNowButton.onClick.AddListener(OnPlayNowClicked);

            if (popupExitButton != null)
                popupExitButton.onClick.AddListener(() =>
                {
                    if (insufficientFundsPopup != null)
                        insufficientFundsPopup.SetActive(false);
                });

            if (insufficientFundsPopup != null)
                insufficientFundsPopup.SetActive(false);

            
            if (betSlider != null)
            {
                betSlider.minValue = minBet;
                betSlider.maxValue = maxBet;
                betSlider.value = minBet;
                betSlider.onValueChanged.AddListener(UpdateCurrentBetText);
            }

            if (minBetText != null)
                minBetText.text = minBet.ToString("N0");
            if (maxBetText != null)
                maxBetText.text = maxBet.ToString("N0");

            UpdateCurrentBetText(betSlider != null ? betSlider.value : minBet);
        }

        private void UpdateCurrentBetText(float value)
        {
            if (currentBetText != null)
                currentBetText.text = value.ToString("N0");
        }

        private void OnPlayNowClicked()
        {
            string moneyStr = moneyText.text.Replace(".", "");
            int playerMoney;
            if (!int.TryParse(moneyStr, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out playerMoney))
            {
                Debug.LogError("Player money parse edilemedi: " + moneyText.text);
                return;
            }

            int selectedBet = Mathf.RoundToInt(betSlider.value);
            Debug.Log("Seçilen bahis: " + selectedBet);

            if (playerMoney >= minBet && playerMoney <= maxBet)
            {
                Debug.Log("Giriş başarılı! Salon: " + roomName);

                LobbyData.SelectedRoomMaxBet = maxBet;
                LobbyData.PlayerMoney = playerMoney;

                SceneManager.LoadScene("GameBoard");
            }
            else
            {
                Debug.Log("Paranız bu salon için uygun değil. Gerekli aralık: " + minBet + " - " + maxBet);
                if (insufficientFundsPopup != null)
                    insufficientFundsPopup.SetActive(true);
            }
        }
    }
}

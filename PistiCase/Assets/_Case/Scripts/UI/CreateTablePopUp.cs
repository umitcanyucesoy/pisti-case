using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Case.Scripts.Data;  // LobbyData'nın bulunduğu namespace

namespace _Case.Scripts.UI
{
    [System.Serializable]
    public class RoomBetRange
    {
        public string roomName;  
        public int minBet;      
        public int maxBet;      
    }

    public class CreateTablePopUp : MonoBehaviour
    {
        [Header("----- Room Bet Ranges -----")]
        [SerializeField] private RoomBetRange[] roomBetRanges;

        [Header("----- UI References -----")]
        [SerializeField] private Slider betSlider;                  
        [SerializeField] private TextMeshProUGUI currentBetText;      
        [SerializeField] private TextMeshProUGUI minBetText;          
        [SerializeField] private TextMeshProUGUI maxBetText;          
        [SerializeField] private TextMeshProUGUI lobbyMoneyText;
        [SerializeField] private GameObject notEnoughBetPanel; // Bu panel, oyuncunun parası yetersizse gösterilecek.


        [Header("----- Button References -----")]
        [SerializeField] private Button confirmButton;              
        [SerializeField] private Button exitButton;                 

        [Header("----- Player Count Selection -----")]
        [SerializeField] private Toggle twoPlayersToggle;
        [SerializeField] private Toggle fourPlayersToggle;

        [Header("----- Salon Selection Reference -----")]
        [SerializeField] private SalonPanelSwitcher salonPanelSwitcher;
        private int _selectedRoomIndex = 0;
      

        private void Start()
        {
            if (exitButton != null)
                exitButton.onClick.AddListener(ClosePopup);

            if (betSlider != null)
                betSlider.onValueChanged.AddListener(UpdateCurrentBetText);

            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirm);
        }
        
        public void OpenPopup()
        {
            if (salonPanelSwitcher != null)
                _selectedRoomIndex = salonPanelSwitcher.GetCurrentPanelIndex();
            else
            {
                Debug.LogWarning("SalonPanelSwitcher referansı eksik! Varsayılan salon kullanılacak.");
                _selectedRoomIndex = 0;
            }

            if (roomBetRanges == null || roomBetRanges.Length <= _selectedRoomIndex)
            {
                Debug.LogError("Room bet range bilgisi eksik veya geçersiz, index: " + _selectedRoomIndex);
                return;
            }

            RoomBetRange range = roomBetRanges[_selectedRoomIndex];
            if (betSlider != null)
            {
                betSlider.minValue = range.minBet;
                betSlider.maxValue = range.maxBet;
                betSlider.value = range.minBet;
                UpdateCurrentBetText(betSlider.value);
            }

            if (minBetText != null)
                minBetText.text = range.minBet.ToString("N0", CultureInfo.InvariantCulture);
            if (maxBetText != null)
                maxBetText.text = range.maxBet.ToString("N0", CultureInfo.InvariantCulture);
            
            if(twoPlayersToggle != null && fourPlayersToggle != null)
            {
                twoPlayersToggle.isOn = true;
                fourPlayersToggle.isOn = false;
            }

            gameObject.SetActive(true);
        }

        private void UpdateCurrentBetText(float value)
        {
            if (currentBetText != null)
                currentBetText.text = value.ToString("N0", CultureInfo.InvariantCulture);
        }
        
        public void OnConfirm()
        {
            int selectedBet = Mathf.RoundToInt(betSlider.value);
            RoomBetRange selectedRoom = roomBetRanges[_selectedRoomIndex];
            Debug.Log($"Salon oluşturuluyor. Seçilen bahis: {selectedBet} | Salon: {selectedRoom.roomName}");

            // Önce LobbyMoneyText'ten oyuncu parasını oku.
            int playerMoney = 0;
            if (lobbyMoneyText != null)
            {
                string moneyStr = lobbyMoneyText.text.Replace(".", "");
                if (!int.TryParse(moneyStr, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out playerMoney))
                {
                    Debug.LogWarning("Lobby money parse edilemedi: " + lobbyMoneyText.text);
                }
            }
            LobbyData.PlayerMoney = playerMoney;
    
            if (playerMoney < selectedBet)
            {
                Debug.Log("Oyuncunun parası seçilen bahis değerinden düşük.");
                if (notEnoughBetPanel != null)
                {
                    ClosePopup();
                    notEnoughBetPanel.SetActive(true);
                }
                return;
            }

            LobbyData.SelectedRoomMaxBet = selectedRoom.maxBet;
            LobbyData.SelectedBet = selectedBet;
    
            if (twoPlayersToggle != null && twoPlayersToggle.isOn)
            {
                LobbyData.SelectedPlayersCount = 2;
            }
            else if (fourPlayersToggle != null && fourPlayersToggle.isOn)
            {
                LobbyData.SelectedPlayersCount = 4;
            }
            else
            {
                LobbyData.SelectedPlayersCount = 2;
            }
    
            SceneManager.LoadScene("GameBoard");
            ClosePopup();
        }

        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }
    }
}

using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Button confirmButton;              
        [SerializeField] private Button exitButton;                 

        [Header("----- Salon Selection Reference -----")]
        [SerializeField] private SalonPanelSwitcher salonPanelSwitcher;
        private int _selectedRoomIndex = 0;
        
        
        private void Start()
        {
            gameObject.SetActive(false);

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
            // Salon oluşturma işlemlerini başlatın veya sahne geçişini gerçekleştirin.
            // Örneğin: SceneManager.LoadScene("GameBoard");
            ClosePopup();
        }

        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }
    }
}

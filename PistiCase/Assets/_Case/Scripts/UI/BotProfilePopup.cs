using System.Globalization;
using _Case.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Case.Scripts.UI
{
    public class BotProfilePopup : MonoBehaviour
    {
        [Header("----- UI References -----")]
        [SerializeField] private TextMeshProUGUI winText;  
        [SerializeField] private TextMeshProUGUI loseText;   
        [SerializeField] private TextMeshProUGUI botMoneyText;   
        [SerializeField] private Button exitButton;          

        private void Start()
        {
            if (exitButton)
                exitButton.onClick.AddListener(ClosePopup);
        }
        
        public void OpenPopup()
        {
            UpdateProfile(LobbyData.PlayerWins, LobbyData.PlayerLosses);
            
            if (botMoneyText != null)
                botMoneyText.text = LobbyData.PlayerMoney.ToString("N0", CultureInfo.InvariantCulture);
            
            gameObject.SetActive(true);
        }


        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }
        
        private void UpdateProfile(int wins, int losses)
        {
            if (winText != null)
                winText.text = wins.ToString();
            if (loseText != null)
                loseText.text = losses.ToString();
        }
    }
}
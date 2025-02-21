using _Case.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Case.Scripts.UI
{
    public class GameBoardUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] botMoneyTexts;    
        [SerializeField] private TextMeshProUGUI playerMoneyText; 
        [SerializeField] private TextMeshProUGUI betText;
        
        [Header("Game Settings Panel")]
        [SerializeField] private GameObject settingsPanel;   
        [SerializeField] private Button gameSettingsButton;     
        [SerializeField] private Button backToLobbyButton;        
        [SerializeField] private Button newGameButton;  
        [SerializeField] private Button exitGameButton;  

        private void Start()
        {
            if (LobbyData.SelectedPlayersCount == 4 && botMoneyTexts != null)
            {
                foreach (TextMeshProUGUI botText in botMoneyTexts)
                {
                    botText.text = LobbyData.SelectedRoomBet.ToString("N0");
                }
            }
            else if (botMoneyTexts != null && botMoneyTexts.Length > 0)
            {
                botMoneyTexts[0].text = LobbyData.SelectedRoomBet.ToString("N0");
            }
            
            playerMoneyText.text = LobbyData.PlayerMoney.ToString("N0");

            int betToDisplay = LobbyData.SelectedBet > 0 
                ? LobbyData.SelectedBet 
                : LobbyData.SelectedRoomBet;

            betText.text = betToDisplay.ToString("N0");
            
            if (settingsPanel)
                settingsPanel.SetActive(false);

            if (gameSettingsButton)
                gameSettingsButton.onClick.AddListener(ToggleSettingsPanel);

            if (backToLobbyButton)
                backToLobbyButton.onClick.AddListener(BackToLobby);

            if (newGameButton)
                newGameButton.onClick.AddListener(NewGame);
            
            if (exitGameButton)
                exitGameButton.onClick.AddListener(ClosePopup);
        }

        private void ToggleSettingsPanel()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

        private void DeductBet()
        {
            int betValue = LobbyData.SelectedBet > 0 
                ? LobbyData.SelectedBet 
                : LobbyData.SelectedRoomBet;
            LobbyData.PlayerMoney -= betValue;
            if(LobbyData.PlayerMoney < 0)
                LobbyData.PlayerMoney = 0;
        }

        private void BackToLobby()
        {
            if (settingsPanel)
                settingsPanel.SetActive(false);
            DeductBet();
            SceneManager.LoadScene("Lobby");
        }

        private void NewGame()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
            DeductBet();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ClosePopup()
        {
            settingsPanel.SetActive(false);
        }
    }
}
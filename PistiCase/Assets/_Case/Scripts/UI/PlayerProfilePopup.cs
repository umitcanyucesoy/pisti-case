using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Case.Scripts.UI
{
    public class PlayerProfilePopup : MonoBehaviour
    {
        [Header("----- UI References -----")]
        [SerializeField] private TextMeshProUGUI winText;  
        [SerializeField] private TextMeshProUGUI loseText;   
        [SerializeField] private Button exitButton;          

        private void Start()
        {
            if (exitButton)
                exitButton.onClick.AddListener(ClosePopup);
        }
        
        public void OpenPopup()
        {
            gameObject.SetActive(true);
        }


        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }
        
        public void UpdateProfile(int wins, int losses)
        {
            if (winText != null)
                winText.text = wins.ToString();
            if (loseText != null)
                loseText.text = losses.ToString();
        }
    }
}
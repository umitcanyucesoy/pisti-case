using UnityEngine;

namespace _Case.Scripts.Cards
{
    public class CardDisplay : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite front;

        public void UpdateDisplay()
        {
            spriteRenderer.sprite = front;
        }
    }
}
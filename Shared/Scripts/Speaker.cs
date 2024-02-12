using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class Speaker : MonoBehaviour, ISpeaker
    {
        // Fields
        [SerializeField, ConversationPopup] private string m_conversation;
        
        // Properties
        public string conversation
        {
            get => m_conversation;
            private set => m_conversation = value;
        }
        
        // Methods
        public virtual string GetConversationName()
        {
            return m_conversation;
        }

        public virtual Transform GetSpeaker()
        {
            return transform;
        }
    }
}

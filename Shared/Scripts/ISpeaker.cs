using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public interface ISpeaker
    {
        public string GetConversationName();

        public Transform GetSpeaker();
    }
}

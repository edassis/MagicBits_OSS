using System;
using System.Collections.Generic;
using UnityEngine.Events;

// Sem isso n�o aparece no editor!
namespace MagicBits_OSS.Shared.Scripts
{
    [Serializable]
    public class TCCDialogueEvent : UnityEvent<List<string>> {}
}

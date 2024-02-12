using System.Collections.Generic;

namespace MagicBits_OSS.Shared.Scripts
{
    public interface ISaveable
    {
        public string GetId();
        public Dictionary<string, string> GetSaveData();
        public void SetSaveData(Dictionary<string, string> dict);
    }
}

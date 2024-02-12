using System.Collections.Generic;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public abstract class Saveable: MonoBehaviour, ISaveable
    {
        [SerializeField, ReadOnly] public string id;
        
        protected IdGenerator idGenerator;
        
        private void Awake()
        {
            idGenerator = gameObject.AddComponent<IdGenerator>();
        }

        public virtual string GetId()
        {
            return idGenerator.GetId();
        }
        
        // Como essas classes são especificas para cada caso, uma classe geral para
        // salvar os dados perde o sentido.
        public abstract Dictionary<string, string> GetSaveData();

        public abstract void SetSaveData(Dictionary<string, string> dict);
    }
}

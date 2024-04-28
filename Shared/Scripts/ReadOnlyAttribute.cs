using UnityEngine;

// http://answers.unity.com/answers/801283/view.html
namespace MagicBits_OSS.Shared.Scripts
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    {}
}

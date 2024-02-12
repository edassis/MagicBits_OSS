using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts.Editor
{
    public class EditorMisc : UnityEditor.Editor
    {
        protected SerializedObject so;

        protected virtual void OnEnable()
        {
            so = new SerializedObject(target);
        }
        
        // http://answers.unity.com/answers/1897391/view.html
        public static void PropertiesToBottom(SerializedObject so, params string[] properties)
        {
            // Properties from base class to exclude
            // var exclude = new string[] { "m_Script", "myExampleProp" };
            var exclude = new[] { "m_Script" };
            exclude = exclude.Concat(properties).ToArray();
            // Draw script header, optional but nice to have
            GUI.enabled = false; // Make properties appear faded.
            var scriptProp = so.FindProperty("m_Script");
            EditorGUILayout.PropertyField(scriptProp);
            GUI.enabled = true;

            // Draw subclass props
            DrawPropertiesExcluding(so, exclude);

            foreach (var each in properties)
            {
                // Draw base class props
                var myExampleProp = so.FindProperty(each);
                EditorGUILayout.PropertyField(myExampleProp);
            }
        }
    }
}

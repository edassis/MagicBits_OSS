using UnityEngine;

// http://answers.unity.com/answers/927240/view.html
namespace MagicBits_OSS.Shared.Scripts
{
    public class ConsoleToGUI : MonoBehaviour
    {
        string myLog = "* LOG MESSAGE *";
        string filename = "";
        bool doShow = true;
        int kChars = 1500;
        void OnEnable() { Application.logMessageReceived += Log; }
        void OnDisable() { Application.logMessageReceived -= Log; }
        void Update() { if (Input.GetKeyDown(KeyCode.Space)) { doShow = !doShow; } }
        public void Log(string logString, string stackTrace, LogType type)
        {
            // for onscreen...
            myLog = myLog + "\n" + logString;
            if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }

            // for the file ...
            if (filename == "")
            {
                string d = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Desktop) + "/UNITY_LOGS";
                System.IO.Directory.CreateDirectory(d);
                filename = d + "/Log " + System.DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + ".txt";
            }
            try { System.IO.File.AppendAllText(filename, logString + "\n"); }
            catch { }
        }

        void OnGUI()
        {
            if (!doShow) { return; }
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
                new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
            GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
        }
    }
}

using System.Collections.Generic;
using Scorm;
using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public class Moodle : MonoBehaviour
    {
        private static IScormService scormService;
        public static bool InitScorm()
        {
            if(scormService != null)
                return true;
#if UNITY_EDITOR
            scormService = new ScormPlayerPrefsService();
#else
            scormService = new ScormService();
#endif

            Version version = Version.Scorm_1_2;
            bool started = scormService.Initialize(version);
            return started;
        }

        public static string GetLearnerId()
        {
            return scormService.GetLearnerId();
        }

        public static string GetLearnerName()
        {
            return scormService.GetLearnerName();
        }

        public static LessonStatus GetLessonStatus()
        {
            return scormService.GetLessonStatus();
        }

        public static void SetLessonStatus(LessonStatus status)
        {
            scormService.SetLessonStatus(status);
            scormService.Commit();
        }

        public static float GetRawScore()
        {
            return scormService.GetRawScore();
        }

        public static float GetMaxScore()
        {
            return scormService.GetMaxScore();
        }

        public static float GetMinScore()
        {
            return scormService.GetMinScore();
        }

        public static void SetRawScore(float value)
        {
            scormService.SetRawScore(value);
            scormService.Commit();
        }

        public static void SetMaxScore(float value)
        {
            scormService.SetMaxScore(value);
            scormService.Commit();
        }

        public static void SetMinScore(float value)
        {
            scormService.SetMinScore(value);
            scormService.Commit();
        }

        public static int GetTotalTime()
        {
            return scormService.GetTotalTime();
        }

        public static bool SetTotalTime(int milliseconds)
        {
            return scormService.SetSessionTime(milliseconds);
        }

        public static LessonMode GetLessonMode()
        {
            return scormService.GetLessonMode();
        }

        public static List<ObjectiveData> GetObjectives()
        {
            return scormService.GetObjectives();
        }

        public static void SetObjective(ObjectiveData objective)
        {
            scormService.SetObjective(objective);
            scormService.Commit();
        }

        public static void SetExitReason(ExitReason value)
        {
            scormService.SetExitReason(value);
            scormService.Commit();
        }

        public static void Finish()
        {
            scormService.Finish();
        }
    }
}

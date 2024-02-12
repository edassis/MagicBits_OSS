using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Scorm.Editor;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MagicBits_OSS.Shared.Scripts.Editor
{
    public class BuildProject
    {
        const string m_minigameParentDir = "Assets/MagicBits";
        
        /// <summary>
        /// Para cada arquivo Level*.unity pertence ao módulo da cena aberta, builda e gera pacote SCORM.
        /// </summary>
        [MenuItem("Build/Compile Module")]
        public static void BuildWeb()
        {
            string minigameName = SceneManager.GetActiveScene().path.Split('/').ToList()
                .Find(new Regex(@"Minigame_").IsMatch);
            string scenesPath = $"{m_minigameParentDir}/{minigameName}/Scenes/";
            const string buildDir = "Build/";
            
            AdjustScormSettings();

            DirectoryInfo sceneDir = new DirectoryInfo(scenesPath);
            FileInfo[] levelInfo = sceneDir.GetFiles("Level*.unity");
            foreach (FileInfo levelFile in levelInfo)
            {
                string levelName = levelFile.Name.Replace(levelFile.Extension, "");
                string buildPath = Path.Combine(Application.dataPath, "../", buildDir, minigameName, levelName);
                
                if (!Directory.Exists(buildPath)) Directory.CreateDirectory(buildPath);

                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions.scenes = new[]
                {
                    $"{m_minigameParentDir}/Shared/Scenes/SelectIsland.unity",
                    $"{m_minigameParentDir}/{minigameName}/Scenes/{levelFile.Name}"
                };
                buildPlayerOptions.locationPathName = buildPath;
                buildPlayerOptions.target = BuildTarget.WebGL;
                buildPlayerOptions.options = BuildOptions.None;

                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                BuildSummary summary = report.summary;

                if (summary.result == BuildResult.Succeeded)
                {
                    UnityEngine.Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                    ScormPostprocessor.GenerateScorm(buildPath, $"{minigameName}_{levelName}");
                }

                if (summary.result == BuildResult.Failed)
                {
                    UnityEngine.Debug.Log("Build failed");
                    break;
                }
            }

            RestoreScormSettings();

            // Copy a file from the project folder to the build folder, alongside the built game.
            // FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

            // // Run the game (Process class from System.Diagnostics).
            // Process proc = new Process();
            // proc.StartInfo.FileName = path + "/BuiltGame.exe";
            // proc.Start();
        }
        
        // [MenuItem("Build/Build Test")]
        public static void BuildTest()
        {
            string scenesPath = $"{m_minigameParentDir}/Minigame_2_x/Scenes";
            const string buildDir = "Build/";
            string buildPath = Path.Combine(Application.dataPath, "../", buildDir);

            AdjustScormSettings();

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[]
            {
                $"{m_minigameParentDir}/Shared/Scenes/SelectIsland.unity",
                $"{scenesPath}/evel1.unity"
            };
            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.target = BuildTarget.WebGL;
            buildPlayerOptions.options = BuildOptions.CleanBuildCache;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                UnityEngine.Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                UnityEngine.Debug.Log("Build failed");
            }

            RestoreScormSettings();
        }
        
        /// <summary>
        /// Cria backup do ScormPublishSettings e desliga Enabled do atual.
        /// </summary>
        private static void AdjustScormSettings()
        {
            ScormPublishSettings settings = Resources.Load<ScormPublishSettings>(ScormPublishSettings.RelativePath);
            var set2 = Object.Instantiate(settings);
            set2.Enabled = false;

            string path = $"Assets/Resources/{ScormPublishSettings.RelativePath}.asset";
            string backupPath = $"Assets/Resources/{ScormPublishSettings.RelativePath}BKP.asset";
            AssetDatabase.MoveAsset(path, backupPath);
            AssetDatabase.CreateAsset(set2, path);
        }

        /// <summary>
        /// Restaura ScormPublishSettings do backup.
        /// </summary>
        private static void RestoreScormSettings()
        {
            string path = $"Assets/Resources/{ScormPublishSettings.RelativePath}.asset";
            string backupPath = $"Assets/Resources/{ScormPublishSettings.RelativePath}BKP.asset";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.MoveAsset(backupPath, path);
        }
    }
}

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ProjectBuilder
{
    static string[] SCENES = FindEnabledEditorScene();
    static string TARGET_DIR = "Build";

    private static string[] FindEnabledEditorScene()
    {
        List<string> EditorScenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_path, BuildTargetGroup buildTargetGroup, BuildTarget build_target, BuildOptions build_options, string buildReportFileName = "BuildReport")
    {
        // 날짜 포맷 결정 방법은 아래 참고.
        // http://www.csharpstudy.com/Tip/Tip-datetime-format.aspx
        DateTime currentTIme = DateTime.Now;
        string dateToFileName = string.Format("{0:yyyy-MM-dd-HHmmss}", currentTIme);

        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, build_target);
        UnityEditor.Build.Reporting.BuildReport res = BuildPipeline.BuildPlayer(scenes, target_path, build_target, build_options);

        // 빌드 번호 설정해줘야 함.
        BuildReportMaker buildReportMaker = new BuildReportMaker(buildReportFileName, res);
    }

    [MenuItem("Custom/CI/Build PC")]
    static void PerformPCBuildClient()
    {
        string pcDir = "/PC";
        BuildOptions opt = BuildOptions.None;

        char sep = Path.DirectorySeparatorChar;
        string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + pcDir + string.Format("/PCBuild_{0}.exe", PlayerSettings.bundleVersion);
        GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, opt, "PC_BuildReport");
    }

    [MenuItem("Custom/CI/Build_Android")]
    static void PerformAndroidBuildClient()
    {
        string androidDir = "/Android";
        BuildOptions opt = BuildOptions.None;

        char sep = Path.DirectorySeparatorChar;
        string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + androidDir + string.Format("/AndroidBuild_{0}.apk", PlayerSettings.bundleVersion);
        GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTargetGroup.Android, BuildTarget.Android, opt, "Android_BuildReport");
    }
}

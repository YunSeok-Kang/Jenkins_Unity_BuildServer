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

    static void GenericBuild(string[] scenes, string target_path, BuildTarget build_target, BuildOptions build_options)
    {
        // 날짜 포맷 결정 방법은 아래 참고.
        // http://www.csharpstudy.com/Tip/Tip-datetime-format.aspx
        DateTime currentTIme = DateTime.Now;
        string dateToFileName = string.Format("{0:yyyy-MM-dd-HHmmss}", currentTIme);

        CSVWriteAndRead csvRW = new CSVWriteAndRead(string.Format("build_report_test_{0}.csv", dateToFileName));

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, build_target);
        UnityEditor.Build.Reporting.BuildReport res = BuildPipeline.BuildPlayer(scenes, target_path, build_target, build_options);

        // res.summary의 내용을 여기(빌드 결과)와 아래의 Summary 영역 두 개로 나눔.


        // 클래스화 시켜야.
        csvRW.AppendLine("빌드 결과");
        csvRW.AppendLine("성공 여부, 결과물 크기(byte), 빌드 시간, TotalWarnings, TotalErrors");
        csvRW.AppendLine(res.summary.result + ", " + res.summary.totalSize + ", " + res.summary.totalTime + ", " + res.summary.totalWarnings + ", " + res.summary.totalErrors);
        csvRW.AppendLine("");

        csvRW.AppendLine("Summary");
        csvRW.AppendLine("빌드 끝난 시각, 빌드 시작 시각, 빌드의 Application.buildGUID, Options, OutputPath, Platform, PlatformGroup");
        csvRW.AppendLine(string.Format("{0:yyyy-MM-dd-HH:mm:ss}", res.summary.buildEndedAt)
                         + ", " + string.Format("{0:yyyy-MM-dd-HH:mm:ss}", res.summary.buildStartedAt)
                         + ", " + res.summary.guid
                         + ", " + res.summary.options.ToString()
                         + ", " + res.summary.outputPath
                         + ", " + res.summary.platform.ToString()
                         + ", " + res.summary.platformGroup.ToString()
                         );
        csvRW.AppendLine("");

        if (res.files != null)
        {
            csvRW.AppendLine("Files");
            csvRW.AppendLine("경로, 역할, 크기");
            foreach (UnityEditor.Build.Reporting.BuildFile buildFile in res.files)
            {
                string fileInfoStr = buildFile.path + ", " + buildFile.role + ", " + buildFile.size;
                csvRW.AppendLine(fileInfoStr);

                //Debug.Log(buildFile.path);
                //Debug.Log(buildFile.role);
                //Debug.Log(buildFile.size);
            }
        }

        //Debug.Log(csvRW.ForDebugShowData());
        csvRW.AppendLine("");

        Debug.Log("------------------------------------------- steps ------------------------------------------- ");
        if (res.steps != null)
        {
            csvRW.AppendLine("Steps");
            csvRW.AppendLine("depth, duration, name");
            foreach (UnityEditor.Build.Reporting.BuildStep buildStep in res.steps)
            {
                Debug.Log(buildStep.depth);
                Debug.Log(buildStep.duration);
                Debug.Log(buildStep.messages);

                string buildStepStr = buildStep.depth + ", " + buildStep.duration + ", " + buildStep.name;
                csvRW.AppendLine(buildStepStr);

                if (buildStep.messages != null)
                {
                    // 전체적으로 한 칸씩 들여쓰기 함.
                    csvRW.AppendLine(", Messages");
                    csvRW.AppendLine(", content, type");
                    foreach (UnityEditor.Build.Reporting.BuildStepMessage buildMsg in buildStep.messages)
                    {
                        string buildStepMessagesStr = ", " + buildMsg.content + ", " + buildMsg.type;
                        csvRW.AppendLine(buildStepMessagesStr);
                    }
                }
            }
        }

        csvRW.AppendLine("");

        Debug.Log("------------------------------------------- strippingInfo ------------------------------------------- ");
        if (res.strippingInfo != null)
        {
            csvRW.AppendLine("StrippingInfo");
            csvRW.AppendLine("모듈 이름, 추가 사유");
            foreach (string moduleName in res.strippingInfo.includedModules)
            {
                string moduleInfoStr = moduleName;
                foreach (string reasonStr in res.strippingInfo.GetReasonsForIncluding(moduleName))
                {
                    moduleInfoStr += ", " + reasonStr;
                }

                csvRW.AppendLine(moduleInfoStr);
            }            
        }

        csvRW.AppendLine("");

        csvRW.SaveData();
        //if (res.Length > 0)
        //{
        //    throw new Exception("BuildPlayer failure: " + res);
        //}
    }

    [MenuItem("Custom/CI/Build PC")]
    static void PerformPCBuildClient()
    {
        string pcDir = "/PC";
        BuildOptions opt = BuildOptions.None;

        char sep = Path.DirectorySeparatorChar;
        string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + pcDir + string.Format("/PCBuild_{0}.exe", PlayerSettings.bundleVersion);
        GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTarget.StandaloneWindows64, opt);
    }

    [MenuItem("Custom/CI/Build_Android")]
    static void PerformAndroidBuildClient()
    {
        string androidDir = "/Android";
        BuildOptions opt = BuildOptions.None;

        char sep = Path.DirectorySeparatorChar;
        string BUILD_TARGET_PATH = Path.GetFullPath(".") + sep + TARGET_DIR + androidDir + string.Format("/AndroidBuild_{0}.apk", PlayerSettings.bundleVersion);
        GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTarget.Android, opt);
    }
}

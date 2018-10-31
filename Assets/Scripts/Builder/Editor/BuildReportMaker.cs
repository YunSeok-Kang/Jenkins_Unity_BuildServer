using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using UnityEngine;
using UnityEditor;

using UnityEditor.Build.Reporting;

//using UnityEditor.Build.Reporting;

public class BuildReportMaker
{
    // -------------------------------------------------------------- constructors/destructors --------------------------------------------------------------
    public BuildReportMaker(string fileName, BuildReport buildReport, string path = "")
    {
        MakeReportCSV(fileName, buildReport, path);
    }

    ~BuildReportMaker()
    {

    }

    // -------------------------------------------------------------- private methods --------------------------------------------------------------
    
    /// <summary>
    /// 파일 이름 결정. 형식까지 지정해주지는 않는다.
    /// </summary>
    /// <param name="prefix_name"></param>
    /// <returns></returns>
    public string MakeFileNameFromDate(string prefix_name)
    {
        DateTime currentTIme = DateTime.Now;
        string dateToFileName = string.Format("{0:yyyy-MM-dd-HHmmss}", currentTIme);

        return prefix_name + string.Format("_{0}", dateToFileName);
    }


    // -------------------------------------------------------------- public methods -------------------------------------------------------------- 
    public void MakeReportCSV(string fileName, BuildReport buildReport, string path = "")
    {
        // 설정된 path가 있다면 디렉토리를 만들어주기위해 / 삽입.
        if (path != "")
        {
            path += "/";
        }

        CSVWriteAndRead csvRW = new CSVWriteAndRead(path + MakeFileNameFromDate(fileName) + ".csv");

        // res.summary의 내용을 여기(빌드 결과)와 아래의 Summary 영역 두 개로 나눔.

        csvRW.AppendLine("빌드 결과");
        csvRW.AppendLine("성공 여부, 결과물 크기(byte), 빌드 시간, TotalWarnings, TotalErrors");
        csvRW.AppendLine(buildReport.summary.result + ", " + buildReport.summary.totalSize + ", " + buildReport.summary.totalTime + ", " + buildReport.summary.totalWarnings + ", " + buildReport.summary.totalErrors);
        csvRW.AppendLine("");

        csvRW.AppendLine("Summary");
        csvRW.AppendLine("빌드 끝난 시각, 빌드 시작 시각, 빌드의 Application.buildGUID, Options, OutputPath, Platform, PlatformGroup");
        csvRW.AppendLine(string.Format("{0:yyyy-MM-dd-HH:mm:ss}", buildReport.summary.buildEndedAt)
                         + ", " + string.Format("{0:yyyy-MM-dd-HH:mm:ss}", buildReport.summary.buildStartedAt)
                         + ", " + buildReport.summary.guid
                         + ", " + buildReport.summary.options.ToString()
                         + ", " + buildReport.summary.outputPath
                         + ", " + buildReport.summary.platform.ToString()
                         + ", " + buildReport.summary.platformGroup.ToString()
                         );
        csvRW.AppendLine("");

        if (buildReport.files != null)
        {
            csvRW.AppendLine("Files");
            csvRW.AppendLine("경로, 역할, 크기");
            foreach (UnityEditor.Build.Reporting.BuildFile buildFile in buildReport.files)
            {
                string fileInfoStr = buildFile.path + ", " + buildFile.role + ", " + buildFile.size;
                csvRW.AppendLine(fileInfoStr);
            }
        }
        csvRW.AppendLine("");

        if (buildReport.steps != null)
        {
            csvRW.AppendLine("Steps");
            csvRW.AppendLine("depth, duration, name");
            foreach (UnityEditor.Build.Reporting.BuildStep buildStep in buildReport.steps)
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

        if (buildReport.strippingInfo != null)
        {
            csvRW.AppendLine("StrippingInfo");
            csvRW.AppendLine("모듈 이름, 추가 사유");
            foreach (string moduleName in buildReport.strippingInfo.includedModules)
            {
                string moduleInfoStr = moduleName;
                foreach (string reasonStr in buildReport.strippingInfo.GetReasonsForIncluding(moduleName))
                {
                    moduleInfoStr += ", " + reasonStr;
                }

                csvRW.AppendLine(moduleInfoStr);
            }
        }

        csvRW.AppendLine("");

        csvRW.SaveData();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

/// <summary>
/// 한 인스턴스는 하나의 파일을 관리합니다.
/// </summary>
public class CSVWriteAndRead
{
    private string _fileFullName = "";

    private StreamReader _streamReader = null;
    private string _appendingString = "";

    // -------------------------------------------------------------- constructors --------------------------------------------------------------
    private CSVWriteAndRead() { }

    /// <summary>
    /// 읽고 쓸 파일 생성
    /// </summary>
    /// <param name="fileName"> 파일 이름. csv 확장자까지 붙여주세요. 예) myfile.csv </param>
    /// <param name="path"> 파일 경로. 없으면 비워두세요. </param>
    public CSVWriteAndRead(string fileName, string path = "")
    {
        StreamReader sr = null;

        _fileFullName = MakePath(fileName, path);

        if (File.Exists(_fileFullName))
        {
            sr = new StreamReader(fileName, System.Text.Encoding.UTF8);
        }
        else
        {
            // 파일이 없으면 생성
            StreamWriter sw = new StreamWriter(File.Open(_fileFullName, System.IO.FileMode.Create));
            sw.Close();
        }

        // 파일을 찾지 못했을 시 오류 대처
        try
        {
            sr = new StreamReader(fileName, System.Text.Encoding.UTF8);
        }
        catch(System.ArgumentNullException nullE)
        {
            Console.WriteLine(nullE);
        }

        _streamReader = sr;

        // 기존의 파일을 여기서 읽어야하는데, 아직 이 기능 제작안함 ㅋㅋㅋㅋㅋ

        // 그 후 스트림 close.
        _streamReader.Close();
    }

    ~CSVWriteAndRead()
    {
        SaveData();
    }

    // -------------------------------------------------------------- private methods --------------------------------------------------------------
    private string MakePath(string fileName, string path)
    {
        if (path == "" || path == null)
        {
            return fileName;
        }
        else
        {
            return path + "/" + fileName;
        }
    }

    // -------------------------------------------------------------- public methods -------------------------------------------------------------- 
    public void AppendLine(string str)
    {
        _appendingString += str + ",\n";
    }

    public void SaveData()
    {
        StreamWriter sw = new StreamWriter(File.Open(_fileFullName, System.IO.FileMode.Append));
        sw.Write(_appendingString);

        _appendingString = "";

        sw.Close();
    }

    public string ForDebugShowData()
    {
        return _appendingString;
    }
}

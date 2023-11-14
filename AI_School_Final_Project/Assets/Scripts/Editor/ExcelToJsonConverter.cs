using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using ExcelDataReader;
using Newtonsoft.Json;
using UnityEditor;
// 기존의 json 직렬화/역직렬화를 위해 유니티에 내장되어있는 JsonUtility를 사용했음
//  -> 파싱하고자 하는 데이터가 상속관계를 갖는다면 베이스에 있는 필드를 정상적으로 인식하지 못함

// 하지만 JsonUtility는 기능적으로 부족한 부분이 많아 일반적으로 간단한 프로젝트에서만 사용함
// 실제 프로덕션 단계에서는 주로 NewtonsoftJson, LitJson 등을 많이 이용
namespace AI_Project.Editor
{
    public class ExcelToJsonConvert
    {
        private readonly List<FileInfo> srcFiles;
        private readonly List<bool> isUseFiles;
        private readonly string savePath;
        private readonly int sheetCount;
        private readonly int headerRows;

        public ExcelToJsonConvert(string filePath, string savePath)
        {
            srcFiles = new List<FileInfo>();
            srcFiles.Add(new FileInfo(filePath));
            isUseFiles = new List<bool>();
            isUseFiles.Add(true);
            this.savePath = savePath;
            sheetCount = 1;
            headerRows = 2;
        }

        public int SaveJsonFiles()
        {
            return ReadAllTables(SaveSheetJson);
        }

        #region Read Table

        int ReadAllTables(Func<DataTable, string, int> exportFunc)
        {
            if (srcFiles == null || srcFiles.Count <= 0)
            {
                Debug.LogError("Error! No Excel Files!");
                return -1;
            }

            int result = 0;
            for (var i = 0; i < srcFiles.Count; i++)
            {
                if (isUseFiles[i])
                {
                    var file = srcFiles[i];
                    result += ReadTable(file.FullName, FileNameNoExt(file.Name), exportFunc);
                }
            }

            return result;
        }

        int ReadTable(string path, string fileName, Func<DataTable, string, int> exportFunc)
        {
            int result = 0;
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int tableSheetNum = reader.ResultsCount;
                    if (tableSheetNum < 1)
                    {
                        Debug.LogError("Excel file is empty: " + path);
                        return -1;
                    }

                    var dataSet = reader.AsDataSet();

                    int checkCount = sheetCount <= 0 ? tableSheetNum : sheetCount;
                    for (int i = 0; i < checkCount; i++)
                    {
                        if (i < tableSheetNum)
                        {
                            string name = checkCount == 1 ?
                                fileName :
                                fileName + "_" + dataSet.Tables[i].TableName;
                            //result += SaveJson(dataSet.Tables[i], name);
                            result += exportFunc(dataSet.Tables[i], name);
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Save Json Files

        int SaveSheetJson(DataTable sheet, string fileName)
        {
            if (sheet.Rows.Count <= 0)
            {
                Debug.LogError("Excel Sheet is empty: " + sheet.TableName);
                return -1;
            }

            int columns = sheet.Columns.Count;
            int rows = sheet.Rows.Count;
            
            // Dictionary 객체는 엑셀에 하나의 row(행)을 나타내고 있음, 하나의 행은 결국 하나의 데이터셋을 의미함
            // 결과적으로 List는 복수의 데이터셋(Dictionary)을 들고있음 
            
            List<Dictionary<string, object>> tData = new List<Dictionary<string, object>>();

            for (int i = headerRows; i < rows; i++)
            {
                Dictionary<string, object> rowData = new Dictionary<string, object>();
                for (int j = 0; j < columns; j++)
                {
                    // 필드 이름을 읽어옴 (엑셀 첫번째 행)
                    string key = sheet.Rows[0][j].ToString();
                    // 데이터 타입을 읽어옴 (엑셀 두번째 행)
                    string type = sheet.Rows[1][j].ToString();

                    rowData[key] = SetObjectFiled(type, sheet.Rows[i][j].ToString());
                }

                tData.Add(rowData);
            }

            string json = JsonConvert.SerializeObject(tData, Formatting.Indented);

            // save to file
            string dstFolder = savePath;
            if (!Directory.Exists(dstFolder))
            {
                Directory.CreateDirectory(dstFolder);
            }

            string path = $"{dstFolder}/{fileName}.json";
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(json);
                    Debug.Log("File saved: " + path);
                    return 1;
                }
            }
        }

        // 해당 로컬함수 내에 case 로 명시된 타입만 현재 파싱 가능함
        // 추후 필요한 데이터타입이 있다면 해당 타입을 case로 명시 후
        // 해당 타입에 맞는 파싱작업을 진행하면 됨
        object SetObjectFiled(string type, string param)
        {
            object pObj = param;
            switch (type.ToLower())
            {
                case "string":
                    break;
                case "string[]":
                    pObj = param.Split(',');
                    break;
                case "bool":
                    pObj = bool.Parse(param);
                    break;
                case "byte":
                    pObj = byte.Parse(param);
                    break;
                case "int":
                    pObj = int.Parse(param);
                    break;
                case "int[]":
                    pObj = Array.ConvertAll(param.Split(','), element => int.Parse(element));
                    break;
                case "short":
                    pObj = short.Parse(param);
                    break;
                case "long":
                    pObj = long.Parse(param);
                    break;
                case "float":
                    pObj = float.Parse(param);
                    break;
                case "float[]":
                    pObj = Array.ConvertAll(param.Split(','), element => float.Parse(element));
                    break;
                case "double":
                    pObj = double.Parse(param);
                    break;
                case "decimal":
                    pObj = decimal.Parse(param);
                    break;
                default:
                    Assembly assembly = Assembly.Load("Assembly-CSharp");
                    var t = assembly.GetType(type);
                    if (t != null)
                    {
                        if (t.IsEnum)
                        {
                            pObj = Enum.Parse(t, param);
                        }
                    }
                    break;
            }

            return pObj;
        }
        #endregion

        string FileNameNoExt(string filename)
        {
            int length;
            if ((length = filename.LastIndexOf('.')) == -1)
                return filename;
            return filename.Substring(0, length);
        }
    }
}

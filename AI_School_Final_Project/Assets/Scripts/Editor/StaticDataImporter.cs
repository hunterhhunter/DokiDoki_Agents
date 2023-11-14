using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AI_Project.Editor
{
    /// <summary>
    /// 엑셀 파일이 추가되었을 때 후처리를 진행
    ///  -> excel 파일의 추가/변경을 감지하고, json으로 변환
    /// </summary>
    public static class StaticDataImporter
    {
        /// <summary>
        /// 에셋 후처리기에서 파일 변경감지 콜백이 실행될 시, 호출할 메서드
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        public static void Import(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            ImportNewOrModified(importedAssets);
            Delete(deletedAssets);
            Move(movedAssets, movedFromAssetPaths);
        }

        /// <summary>
        /// 파일을 삭제한 경우 실행할 기능
        /// </summary>
        /// <param name="deletedAssets">삭제한 에셋 정보</param>
        private static void Delete(string[] deletedAssets)
        { 
            ExcelToJson(deletedAssets, true);
        }

        /// <summary>
        /// 파일이 이동 됐을 때
        /// </summary>
        /// <param name="movedAssets">새로운 경로(이동 후)의 에셋 정보</param>
        /// <param name="movedFromAssetPaths">이전 경로(이동 전)의 에셋 정보</param>
        private static void Move(string[] movedAssets, string[] movedFromAssetPaths)
        {
            // 이전 경로 에셋 삭제 기능 실행
            Delete(movedFromAssetPaths);
            // 새로운 경로 에셋 수정
            ImportNewOrModified(movedAssets);
        }

        /// <summary>
        /// 파일을 새로 불러오거나 수정했을 때
        /// </summary>
        /// <param name="importedAssets">불러오거나 수정한 에셋 정보</param>
        private static void ImportNewOrModified(string[] importedAssets)
        {
            ExcelToJson(importedAssets, false);
        }

        /// <summary>
        /// 엑셀 -> json 변환 기능
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="isDeleted"></param>
        private static void ExcelToJson(string[] assets, bool isDeleted)
        {
            // 파라미터로 전달받은 assets 배열의 데이터 중
            // 엑셀 파일인 에셋의 경로만 담을 리스트
            List<string> staticDataAssets = new List<string>();

            // 에셋 경로에서 기획데이터인 엑셀파일만 걸러낸다
            foreach (var asset in assets)
            {
                if (IsStaticData(asset, isDeleted))
                    staticDataAssets.Add(asset);
            }

            // 걸러낸 excel 기획데이터를 json으로 변환
            foreach (var staticDataAsset in staticDataAssets)
            {
                try
                {
                    var rootPath = Application.dataPath;
                    // 문자열에서 마지막 / 가 존재하는 부분부터 뒤쪽 문자열을 전부 지움
                    // 결과적으로 Assets 폴더 경로가 지워짐
                    rootPath = rootPath.Remove(rootPath.LastIndexOf('/'));

                    // 절대경로를 구함
                    var absolutePath = $"{rootPath}/{staticDataAsset}";

                    var converter = new ExcelToJsonConvert(absolutePath, $"{rootPath}/{Define.StaticData.SDJsonPath}");

                    // 변환 실행 및 결과를 반환받아 성공했는지 확인
                    if (converter.SaveJsonFiles() > 0)
                    {
                        // 경로에서 파일이름과 확장자만 남긴다.
                        var fileName = staticDataAsset.Substring(staticDataAsset.LastIndexOf('/') + 1);
                        // 확장제를 제거해서 파일이름만 남긴다.
                        fileName = fileName.Remove(fileName.LastIndexOf('.'));

                        // json을 파일을 생성하여 프로젝트 폴더 내에 위치시켰을 뿐
                        // 에디터에 상에서 로드하여 인식시키는 작업은 하지 않았으므로
                        // 에디터에서 인식할 수 있도록 임포트한다.
                        AssetDatabase.ImportAsset($"{Define.StaticData.SDJsonPath}/{fileName}.json");
                        Debug.Log($"##### StaticData {fileName} reimported");
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogErrorFormat("Couldn't convert assets = {0}", staticDataAsset);
                    EditorUtility.DisplayDialog("Error Convert", 
                        string.Format("Couldn't convert assets = {0}", staticDataAsset), "OK");
                }
            }

        }

        /// <summary>
        /// 파일이 엑셀 파일이면서 기획데이터인지 체크
        /// </summary>
        /// <param name="path">해당 파일 경로</param>
        /// <param name="isDeleted">삭제 이벤트인지?</param>
        /// <returns></returns>
        private static bool IsStaticData(string path, bool isDeleted)
        {
            // xlsx 확장자가 아니라면 리턴
            if (path.EndsWith(".xlsx") == false)
                return false;

            // 파일 존재여부 확인을 위해, 파일의 전체경로를 구함
            //  -> 에디터상에서 dataPath: 드라이브부터 프로젝트 에셋폴더 경로까지
            //     파라미터로 전달받은 경로는 에셋폴더부터
            var absolutePath = Application.dataPath + path.Remove(0, "Assets".Length);

            // Assets/StaticData/Excel 폴더에 존재하는 엑셀파일은 기획데이터 라고
            // 규칙을 정했음
            // 따라서, 해당 경로에 존재하지 않는다면 기획데이터가 아님

            // 삭제이벤트 이거나 존재하는 파일이어야하고, 경로는 excel 데이터 경로에 있어야한다.
            return (isDeleted || File.Exists(absolutePath)) && path.StartsWith(Define.StaticData.SDExcelPath);
        }
    }
}

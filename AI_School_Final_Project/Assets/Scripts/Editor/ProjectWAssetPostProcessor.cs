using UnityEditor;

namespace AI_Project.Editor
{
    /// <summary>
    /// 프로젝트 내 에셋 폴더에서 파일의 변경사항을 감지하여
    /// 콜백을 실행시키는 클래스
    /// </summary>
    public class ProjectWAssetPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            StaticDataImporter.Import(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
        }
    }
}
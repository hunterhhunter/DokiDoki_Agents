using UnityEditor;

namespace AI_Project.Editor
{
    /// <summary>
    /// ������Ʈ �� ���� �������� ������ ��������� �����Ͽ�
    /// �ݹ��� �����Ű�� Ŭ����
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
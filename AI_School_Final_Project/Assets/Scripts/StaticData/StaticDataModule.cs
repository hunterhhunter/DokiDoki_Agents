using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AI_Project.SD
{
    /// <summary>
    /// ��� ��ȹ �����͸� ������ Ŭ����
    /// ��ȹ �����͸� �ε��ϰ� ��� �ֱ⸸ �� ���̹Ƿ�
    /// ��븦 ��� ���� �ʿ䰡 ����
    /// </summary>
    // ��븦 ���� �ʴ� Ŭ������ �ν����Ϳ� �����Ű�� ���� ����ȭ
    [Serializable]
    public class StaticDataModule
    {
        public List<SDCharacter> sdCharacters;
        public List<SDGrowthStat> sdGrowthStats;
        public List<SDStage> sdStages;
        public List<SDMonster> sdMonsters;
        public List<SDItem> sdItems;
        public List<SDNPC> sdNPCS;
        public List<SDString> sdStrings;
        public List<SDQuest> sdQuests;

        public void Intialize()
        {
            var loader = new StaticDataLoader();

            loader.Load(out sdCharacters);
            loader.Load(out sdGrowthStats);
            loader.Load(out sdStages);
            /*loader.Load(out sdMonsters);
            loader.Load(out sdItems);
            loader.Load(out sdNPCS);
            loader.Load(out sdStrings);
            loader.Load(out sdQuests);*/
        }

        /// <summary>
        /// ��ȹ �����͸� �ҷ��� �δ�
        /// </summary>
        private class StaticDataLoader
        {
            private string path;

            public StaticDataLoader()
            {
                path = $"{Application.dataPath}/StaticData/Json";
            }

            /// <summary>
            /// ��ȹ������ json�� �о�� TŸ�� ������ ����Ʈ�� �Ľ��ϴ� ��� 
            /// </summary>
            /// <typeparam name="T">��ȯ�ϰ��� �ϴ� Ÿ��</typeparam>
            /// <param name="data">��ȯ�� TŸ���� �����͵��� ���� ����Ʈ</param>
            public void Load<T>(out List<T> data) where T : StaticData
            {
                // json �����̸��� TŸ�� �̸��� ���ؼ� ���Ѵ�.
                // �� �� ��� ��ȹ�����ʹ� SD ��� ���ξ�� �����ϹǷ� SD ���ξ �����
                // json �����̸��� ������
                var fileName = typeof(T).Name.Remove(0, "SD".Length);

                // json ������ �о��
                var json = File.ReadAllText($"{path}/{fileName}.json");

                // �Ķ���� data�� List<T>�� �ƴ� out List<T>�� ������ ����?
                // List �� ���� Ÿ���̹Ƿ� ���� ��, ��� �����͸� �Ҵ��ص� �״�� ���� �ȴٰ�
                // ���������� �������� ������ ������.
                //  -> ����Ÿ���� ��ü�� �ʵ忡 �����Ͽ� �������Ҵ��� �����ϳ�,
                //     ���޹��� ���� ��ü�� �Ҵ��ϴ� �۾��� outŰ���尡 ���ٸ� �Ұ���
                //     ���� out Ű���带 �����ϰ� data �� ����Ʈ�� �Ҵ��Ͽ���
                //     �������� ����

                // �о�� json�� TŸ�� ����Ʈ ���·� ��ȯ
                data = JsonConvert.DeserializeObject<List<T>>(json);
            }
        }
    }
}
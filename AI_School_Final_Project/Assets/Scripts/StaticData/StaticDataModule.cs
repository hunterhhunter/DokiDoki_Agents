using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AI_Project.SD
{
    /// <summary>
    /// 모든 기획 데이터를 관리할 클래스
    /// 기획 데이터를 로드하고 들고 있기만 할 것이므로
    /// 모노를 상속 받을 필요가 없음
    /// </summary>
    // 모노를 갖지 않는 클래스를 인스펙터에 노출시키기 위해 직렬화
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
        /// 기획 데이터를 불러올 로더
        /// </summary>
        private class StaticDataLoader
        {
            private string path;

            public StaticDataLoader()
            {
                path = $"{Application.dataPath}/StaticData/Json";
            }

            /// <summary>
            /// 기획데이터 json을 읽어와 T타입 데이터 리스트로 파싱하는 기능 
            /// </summary>
            /// <typeparam name="T">변환하고자 하는 타입</typeparam>
            /// <param name="data">변환된 T타입의 데이터들을 담을 리스트</param>
            public void Load<T>(out List<T> data) where T : StaticData
            {
                // json 파일이름을 T타입 이름을 통해서 구한다.
                // 이 때 모든 기획데이터는 SD 라는 접두어로 시작하므로 SD 접두어만 지우면
                // json 파일이름과 동일함
                var fileName = typeof(T).Name.Remove(0, "SD".Length);

                // json 파일을 읽어옴
                var json = File.ReadAllText($"{path}/{fileName}.json");

                // 파라미터 data를 List<T>가 아닌 out List<T>로 선언한 이유?
                // List 는 참조 타입이므로 전달 시, 어떠한 데이터를 할당해도 그대로 유지 된다고
                // 생각하지만 세부적인 조건이 존재함.
                //  -> 참조타입의 객체의 필드에 접근하여 데이터할당은 가능하나,
                //     전달받은 참조 자체를 할당하는 작업은 out키워드가 없다면 불가능
                //     실제 out 키워드를 제외하고 data 에 리스트를 할당하여도
                //     유지되지 않음

                // 읽어온 json은 T타입 리스트 형태로 변환
                data = JsonConvert.DeserializeObject<List<T>>(json);
            }
        }
    }
}
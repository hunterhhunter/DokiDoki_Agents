using AI_Project.DB;
using AI_Project.Define;
using AI_Project.Object;
using AI_Project.Resource;
using AI_Project.UI;
using AI_Project.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AI_Project
{
    /// <summary>
    /// 스테이지 관련 기능들을 제어할 클래스
    /// 주로 스테이지 전환 시 처리작업 (리소스 로드 및 인스턴스 생성)
    /// </summary>
    public class StageManager : Singleton<StageManager>
    {
        /// <summary>
        /// 스테이지 전환 시, 스테이지 전환에 필요한 모든 작업이 완료된 상태인지를 나타내는 필드
        /// </summary>
        private bool isReady;

        /// <summary>
        /// 이전 몬스터 스폰 시간
        /// </summary>
        private float prevSpawnTime;
        /// <summary>
        /// 현재 몬스터 스폰 체크 시간 (체크 시간은 스폰을 한 번 할 때마다 랜덤하게 변경됨)
        /// </summary>
        private float checkSpawnTime;

        /// <summary>
        /// 현재 스테이지의 인스턴스
        /// </summary>
        private GameObject currentStage;
        /// <summary>
        /// 현재 스테이지 내에서 몬스터 스폰 영역에 대한 정보를 들고 있는 딕셔너리
        /// </summary>
        private Dictionary<int, Bounds> spawnAreaBounds = new Dictionary<int, Bounds>();

        private void Update()
        {
            if (!isReady)
                return;

            CheckSpawnTime();
        }

        /// <summary>
        /// 스테이지 전환 시 필요한 리소스를 불러오고 인스턴스 생성 및 데이터 바인딩 작업
        ///  -> 이 메서드를 호출하는 시점은 로딩 씬이 활성화되어있는 상태
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator ChangeStage()
        {
            isReady = false;

            // 외부(서버)에서  새로 불러올 스테이지 정보를 이미 받은 상태
            // 그리고 해당 데이터는 게임매니저의 boUser 필드에 존재함
            // 따라서, 새로 로드할 스테이지에 대한 기획정보를 불러옴
            var sdStage = GameManager.User.boStage.sdStage;

            var resourceManager = ResourceManager.Instance;

            // 현재 스테이지 객체가 존재한다면
            if (currentStage != null)
                // 새로운 스테이지 객체를 생성할 것으므로 파괴
                Destroy(currentStage);

            // 새로운 스테이지 객체를 생성
            //  -> 문제가 있음, 해당 객체를 생성하는 시점은 로딩 씬이 활성화 되어있고
            //     변경하고자 하는 씬은 비활성화 되어있는 상태, 이 때 객체를 생성 시 생성되는 객체는
            //     활성화된 씬에 종속된다.
            //     따라서, 최종적으로 인게임 씬으로 전환되었을 때 스테이지가 보이지 않음
            currentStage = Instantiate(resourceManager.LoadObject(sdStage.resourcePath));

            // 위의 문제를 해결하고자 생성한 객체를 로딩 씬에서 변경하고자하는 씬으로 이동시킨다.
            SceneManager.MoveGameObjectToScene(currentStage, SceneManager.GetSceneByName(SceneType.Ingame.ToString()));

            // 이전 스테이지에서 사용하던 리소스들을 비우는 작업
            spawnAreaBounds.Clear(); // 이전 스테이지 스폰 영역 데이터 비우기
            ObjectPoolManager.Instance.ClearPool<Object.Monster>(); // 이전 스테이지 몬스터 데이터 비우기
            IngameManager.Instance.ClearNPC(); // 이전 스테이지 NPC 데이터 비우기
            UIWindowManager.Instance.GetWindow<UIIngame>()?.Clear(); // 이전 스테이지에서 사용하던 인게임 UI 요소 비우기

            var sd = GameManager.SD;

            var spawnAreaHolder = currentStage.transform.Find("SpawnAreaHolder");

            // 바뀐 현재 스테이지에서 사용될 리소스를 부르고 인스턴스를 생성하는 작업
            // 바뀐 스테이지에서 사용되는 몬스터의 종류만큼 반복
            for (int i = 0; i < sdStage.genMonsters.Length; ++i)
            {
                // 몬스터 기획 데이터를 하나씩 불러온다.
                var sdMonster = sd.sdMonsters.Where(_ => _.index == sdStage.genMonsters[i]).SingleOrDefault();

                // 기획 데이터가 존재한다면
                if (sdMonster != null)
                    // 해당 몬스터 프리팹을 부르고 몬스터 풀에 등록
                    resourceManager.LoadPoolableObject<Object.Monster>(sdMonster.resourcePath, 10);
                else
                    continue;

                // 해당 몬스터의 스폰 구역에 대한 정보를 가져온다.
                //  -> 현재 데이터는 (몬스터 종류) 배열만큼 (스폰 구역) 배열이 존재함 (배열의 길이가 동일)
                var spawnAreaIndex = sdStage.spawnArea[i];
                // 해당 스폰 영역의 인덱스가 딕셔너리에 존재하는지 체크 (중복되는 영역을 사용하는 몬스터가 존재할 수 있으므로)
                if (!spawnAreaBounds.ContainsKey(spawnAreaIndex))
                {
                    // 존재하지 않는다면 딕셔너리에 등록
                    var spawnArea = spawnAreaHolder.GetChild(spawnAreaIndex);
                    spawnAreaBounds.Add(spawnAreaIndex, spawnArea.GetComponent<Collider>().bounds);
                }
            }

            yield return null;
        }

        /// <summary>
        /// 위의 ChangeStage 메서드가 씬 전환 도중에 실행되는 작업이라면
        /// OnChangeStageComplete은 씬 전환이 완료된 후 에 실행될 작업
        /// ex) 캐릭터, 몬스터 스폰 등
        /// </summary>
        public void OnChangeStageComplete()
        {
            ClearSpawnTime();
            SpawnCharacter();
            SpawnMonster();
            SpawnNPC();

            isReady = true;
        }

        /// <summary>
        /// 플레이어의 캐릭터 생성 또는 스테이지 이동 시 플레이어 위치 설정
        /// </summary>
        private void SpawnCharacter()
        {
            // 인게임 씬에서 플레이어 컨트롤러 참조를 찾는다. 
            var playerController = FindObjectOfType<PlayerController>();

            // 플레이어의 캐릭터 인스턴스가 이미 존재한다면,
            // 타이틀 -> 인게임 씬 변경이 아닌, 스테이지 전환을 했다는 것
            // 따라서, 이러한 경우에는 플레이어 위치 설정을 실행되도록
            if (playerController.PlayerCharacter != null)
            {
                // 새로 이동한 스테이지에 이전 스테이지와 연결된 워프의 EntryPos를 찾는다
                var entryPos = currentStage.transform
                    .Find($"WarpHolder/{GameManager.User.boStage.prevStageIndex}/EntryPos").transform;

                // 플레이어를 해당 워프의 입장 위치로 설정
                playerController.PlayerCharacter.transform.position = entryPos.position;
                playerController.PlayerCharacter.transform.forward = entryPos.forward;
                // 플레이어가 워프로 인해 갑작스럽게 이동하였으므로, 카메라도 동일하게 강제로 이동시켜준다.
                playerController.cameraController.SetForceDefaultView();
                return;
            }    

            // 유저의 캐릭터 정보를 받아온다.
            var boCharacter = GameManager.User.boCharacter;

            // 캐릭터 인스턴스 생성
            var character = Instantiate(ResourceManager.Instance
                .LoadObject(boCharacter.sdCharacter.resourcePath));

            // 유저의 캐릭터가 이전에 위치했던 좌표로 이동
            character.transform.position = GameManager.User.boStage.lastPos;

            // 캐릭터 객체가 갖는 캐릭터 컴포넌트에 접근하여 초기화시켜준다.
            //  -> 초기화 시 캐릭터 데이터를 전달
            var characterComp = character.GetComponent<Character>();
            characterComp.Initialize(boCharacter);

            // 생성 후 초기화가 끝난 캐릭터 객체를 유저가 제어할 수 있게 플레이어 컨트롤러에 등록
            playerController.Initialize(characterComp);

            // 모든 초기화가 끝난 캐릭터 객체를 정상적으로 업데이트 할 수 있게 인게임 매니저에 
            // 활성 캐릭터 목록에 등록한다.
            IngameManager.Instance.AddActor(characterComp);
        }

        /// <summary>
        /// 몬스터 스폰 시간을 체크하는 기능
        /// </summary>
        private void CheckSpawnTime()
        {
            if (currentStage == null)
                return;

            if (Time.time - prevSpawnTime >= checkSpawnTime)
            {
                ClearSpawnTime();
                SpawnMonster();
            }
        }

        /// <summary>
        /// 몬스터 스폰시간 초기화
        /// </summary>
        private void ClearSpawnTime()
        {
            prevSpawnTime = Time.time;
            checkSpawnTime = Random.Range(Define.Monster.MinSpawnTime, Define.Monster.MaxSpawnTime);
        }

        /// <summary>
        /// 몬스터를 생성하는 기능
        /// </summary>
        private void SpawnMonster()
        {
            // 현재 스테이지 기획데이터를 받아옴
            var sdStage = GameManager.User.boStage.sdStage;

            // 해당 스테이지가 몬스터를 스폰하지 않는 스테이지라면 종료
            if (sdStage.genMonsters[0] == -1)
                return;

            // 미리 정해둔 최소, 최대 스폰 수 사이에서 랜덤 값을 받는다.
            var monsterSpawnCnt = Random.Range(Define.Monster.MinSpawnCnt, Define.Monster.MaxSpawnCnt);
            // 몬스터 풀을 받아온다.
            var monsterPool = ObjectPoolManager.Instance.GetPool<Object.Monster>();

            var sd = GameManager.SD;
            var ingameManager = IngameManager.Instance;

            // 랜덤한 스폰 카운트만큼 반복
            for (int i = 0; i < monsterSpawnCnt; ++i)
            {
                // 현재 스테이지에서 생성할 수 있는 몬스터 중에 랜덤하게 생성
                // 생성할 수 있는 몬스터의 기획데이터 배열 상의 인덱스를 가져온다.
                var randIndex = Random.Range(0, sdStage.genMonsters.Length);
                var genMonsterIndex = sdStage.genMonsters[randIndex];

                // 생성할 몬스터의 기획데이터를 가져옴
                var sdMonster = sd.sdMonsters.Where(_ => _.index == genMonsterIndex).SingleOrDefault();

                // 이제 몬스터 풀에서 몬스터 객체를 하나 가져온 후, 데이터를 채워서 사용하기만 하면 됨
                //  -> 하지만, 현재 우리가 사용중인 몬스터 풀에는 여러 종류의 몬스터가 존재함
                //     그럼 몬스터 풀에서 몬스터를 가져올 때, 내가 생성할 몬스터 객체와 동일한 종류의 객체를 찾아야 함.
                var monsterName = sdMonster.resourcePath.Remove(0, sdMonster.resourcePath.LastIndexOf('/')+1);
                // 현재 몬스터의 이름으로 동일한 객체를 풀에서 찾음..
                // 하지만 반복문에서 새로운 문자열을 생성하는 방식이 좋지 않기 떄문에, 나중에 바꾸시는걸 추천
                //  -> 현재 그냥 편하게 찾으려고 임의로 이름으로 찾음
                
                var monster = monsterPool.GetObj(_ => _.name == monsterName);
                if (monster == null)
                    continue;

                // 몬스터의 위치 설정 (스폰 영역 내에서 랜덤하게)
                var bounds = spawnAreaBounds[sdStage.spawnArea[randIndex]];
                var spawnPosX = Random.Range(-bounds.size.x * .5f, bounds.size.x * .5f);
                var spawnPosZ = Random.Range(-bounds.size.z  * .5f, bounds.size.z * .5f);

                monster.transform.position = bounds.center + new Vector3(spawnPosX, 0, spawnPosZ);
                monster.Initialize(new BoMonster(sdMonster));

                // 활성 몬스터 목록에 등록
                ingameManager.AddActor(monster);
            }
        }

        /// <summary>
        /// 현재 스테이지에 사용되는 NPC들을 생성하는 기능
        /// </summary>
        private void SpawnNPC()
        {
            // 현재 스테이지 정보를 참조하여 NPC 테이블에 접근해서
            // 현재 스테이지에 존재하는 NPC들의 정보를 받아온다.
            var sdStage = GameManager.User.boStage.sdStage;
            var npcs = GameManager.SD.sdNPCS.Where(_ => _.stageRef == sdStage.index).ToList();

            // 인게임 활성 NPC 목록 참조
            var activeNPCs = IngameManager.Instance.NPCs;
            
            var resourceManager = ResourceManager.Instance;

            // npc 데이터만큼 순회
            for (int i = 0; i < npcs.Count; ++i)
            {
                // npc 데이터의 프리팹 경로를 이용하여 npc 객체 생성
                var npcObj = Instantiate(resourceManager.LoadObject(npcs[i].resourcePath));
                // 생성한 npc 객체의 npc 컴포넌트 참조에 접근하여 초기화
                var npc = npcObj.GetComponent<Object.NPC>();
                // npc 기획데이터를 기반으로 bo 데이터 생성
                npc.Initialize(new BoNPC(npcs[i]));

                // 활성 npc 목록에 추가
                activeNPCs.Add(npc);
            }
        }

        /// <summary>
        /// 몬스터 인덱스를 받아 해당 몬스터의 스폰 구역을 찾아
        /// 해당 스폰 구역 내에서 랜덤한 위치를 반환
        /// </summary>
        /// <param name="monsterIndex"></param>
        public Vector3 GetRandPosInArea(int monsterIndex)
        {
            // 현재 스테이지 정보
            var sdStage = GameManager.User.boStage.sdStage;

            // 현재 스테이지 정보에서 해당 스테이지가 스폰할 수 있는 몬스터 정보에 접근
            // 기획데이터 상에서 스폰할 수 있는 몬스터 정보는 배열형태로 되어있음
            // 동일하게 스폰구역을 나타내는 정보도 몬스터 정보와 동일한 길이의 배열형태로 되어있음
            // 즉, 기획데이터 상의 배열 인덱스를 구하면 스폰 구역 데이터에 접근할 수 있다는 의미
            var arrayIndex = -1;

            for (int i = 0; i < sdStage.genMonsters.Length; ++i)
            {
                if (sdStage.genMonsters[i] == monsterIndex)
                {
                    // 스폰 데이터 접근할 인덱스 값을 찾았으므로 반복을 종료
                    arrayIndex = i;
                    break;
                }
            }

            var bounds = spawnAreaBounds[sdStage.spawnArea[arrayIndex]];
            var spawnPosX = Random.Range(-bounds.size.x * .5f, bounds.size.x * .5f);
            var spawnposZ = Random.Range(-bounds.size.z * .5f, bounds.size.z * .5f);

            return bounds.center + new Vector3(spawnPosX, 0, spawnposZ);
        }
    }
}
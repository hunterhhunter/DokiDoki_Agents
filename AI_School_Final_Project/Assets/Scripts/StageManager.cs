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
    /// �������� ���� ��ɵ��� ������ Ŭ����
    /// �ַ� �������� ��ȯ �� ó���۾� (���ҽ� �ε� �� �ν��Ͻ� ����)
    /// </summary>
    public class StageManager : Singleton<StageManager>
    {
        /// <summary>
        /// �������� ��ȯ ��, �������� ��ȯ�� �ʿ��� ��� �۾��� �Ϸ�� ���������� ��Ÿ���� �ʵ�
        /// </summary>
        private bool isReady;

        /// <summary>
        /// ���� ���� ���� �ð�
        /// </summary>
        private float prevSpawnTime;
        /// <summary>
        /// ���� ���� ���� üũ �ð� (üũ �ð��� ������ �� �� �� ������ �����ϰ� �����)
        /// </summary>
        private float checkSpawnTime;

        /// <summary>
        /// ���� ���������� �ν��Ͻ�
        /// </summary>
        private GameObject currentStage;
        /// <summary>
        /// ���� �������� ������ ���� ���� ������ ���� ������ ��� �ִ� ��ųʸ�
        /// </summary>
        private Dictionary<int, Bounds> spawnAreaBounds = new Dictionary<int, Bounds>();

        private void Update()
        {
            if (!isReady)
                return;

            CheckSpawnTime();
        }

        /// <summary>
        /// �������� ��ȯ �� �ʿ��� ���ҽ��� �ҷ����� �ν��Ͻ� ���� �� ������ ���ε� �۾�
        ///  -> �� �޼��带 ȣ���ϴ� ������ �ε� ���� Ȱ��ȭ�Ǿ��ִ� ����
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator ChangeStage()
        {
            isReady = false;

            // �ܺ�(����)����  ���� �ҷ��� �������� ������ �̹� ���� ����
            // �׸��� �ش� �����ʹ� ���ӸŴ����� boUser �ʵ忡 ������
            // ����, ���� �ε��� ���������� ���� ��ȹ������ �ҷ���
            var sdStage = GameManager.User.boStage.sdStage;

            var resourceManager = ResourceManager.Instance;

            // ���� �������� ��ü�� �����Ѵٸ�
            if (currentStage != null)
                // ���ο� �������� ��ü�� ������ �����Ƿ� �ı�
                Destroy(currentStage);

            // ���ο� �������� ��ü�� ����
            //  -> ������ ����, �ش� ��ü�� �����ϴ� ������ �ε� ���� Ȱ��ȭ �Ǿ��ְ�
            //     �����ϰ��� �ϴ� ���� ��Ȱ��ȭ �Ǿ��ִ� ����, �� �� ��ü�� ���� �� �����Ǵ� ��ü��
            //     Ȱ��ȭ�� ���� ���ӵȴ�.
            //     ����, ���������� �ΰ��� ������ ��ȯ�Ǿ��� �� ���������� ������ ����
            currentStage = Instantiate(resourceManager.LoadObject(sdStage.resourcePath));

            // ���� ������ �ذ��ϰ��� ������ ��ü�� �ε� ������ �����ϰ����ϴ� ������ �̵���Ų��.
            SceneManager.MoveGameObjectToScene(currentStage, SceneManager.GetSceneByName(SceneType.Ingame.ToString()));

            // ���� ������������ ����ϴ� ���ҽ����� ���� �۾�
            spawnAreaBounds.Clear(); // ���� �������� ���� ���� ������ ����
            ObjectPoolManager.Instance.ClearPool<Object.Monster>(); // ���� �������� ���� ������ ����
            IngameManager.Instance.ClearNPC(); // ���� �������� NPC ������ ����
            UIWindowManager.Instance.GetWindow<UIIngame>()?.Clear(); // ���� ������������ ����ϴ� �ΰ��� UI ��� ����

            var sd = GameManager.SD;

            var spawnAreaHolder = currentStage.transform.Find("SpawnAreaHolder");

            // �ٲ� ���� ������������ ���� ���ҽ��� �θ��� �ν��Ͻ��� �����ϴ� �۾�
            // �ٲ� ������������ ���Ǵ� ������ ������ŭ �ݺ�
            for (int i = 0; i < sdStage.genMonsters.Length; ++i)
            {
                // ���� ��ȹ �����͸� �ϳ��� �ҷ��´�.
                var sdMonster = sd.sdMonsters.Where(_ => _.index == sdStage.genMonsters[i]).SingleOrDefault();

                // ��ȹ �����Ͱ� �����Ѵٸ�
                if (sdMonster != null)
                    // �ش� ���� �������� �θ��� ���� Ǯ�� ���
                    resourceManager.LoadPoolableObject<Object.Monster>(sdMonster.resourcePath, 10);
                else
                    continue;

                // �ش� ������ ���� ������ ���� ������ �����´�.
                //  -> ���� �����ʹ� (���� ����) �迭��ŭ (���� ����) �迭�� ������ (�迭�� ���̰� ����)
                var spawnAreaIndex = sdStage.spawnArea[i];
                // �ش� ���� ������ �ε����� ��ųʸ��� �����ϴ��� üũ (�ߺ��Ǵ� ������ ����ϴ� ���Ͱ� ������ �� �����Ƿ�)
                if (!spawnAreaBounds.ContainsKey(spawnAreaIndex))
                {
                    // �������� �ʴ´ٸ� ��ųʸ��� ���
                    var spawnArea = spawnAreaHolder.GetChild(spawnAreaIndex);
                    spawnAreaBounds.Add(spawnAreaIndex, spawnArea.GetComponent<Collider>().bounds);
                }
            }

            yield return null;
        }

        /// <summary>
        /// ���� ChangeStage �޼��尡 �� ��ȯ ���߿� ����Ǵ� �۾��̶��
        /// OnChangeStageComplete�� �� ��ȯ�� �Ϸ�� �� �� ����� �۾�
        /// ex) ĳ����, ���� ���� ��
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
        /// �÷��̾��� ĳ���� ���� �Ǵ� �������� �̵� �� �÷��̾� ��ġ ����
        /// </summary>
        private void SpawnCharacter()
        {
            // �ΰ��� ������ �÷��̾� ��Ʈ�ѷ� ������ ã�´�. 
            var playerController = FindObjectOfType<PlayerController>();

            // �÷��̾��� ĳ���� �ν��Ͻ��� �̹� �����Ѵٸ�,
            // Ÿ��Ʋ -> �ΰ��� �� ������ �ƴ�, �������� ��ȯ�� �ߴٴ� ��
            // ����, �̷��� ��쿡�� �÷��̾� ��ġ ������ ����ǵ���
            if (playerController.PlayerCharacter != null)
            {
                // ���� �̵��� ���������� ���� ���������� ����� ������ EntryPos�� ã�´�
                var entryPos = currentStage.transform
                    .Find($"WarpHolder/{GameManager.User.boStage.prevStageIndex}/EntryPos").transform;

                // �÷��̾ �ش� ������ ���� ��ġ�� ����
                playerController.PlayerCharacter.transform.position = entryPos.position;
                playerController.PlayerCharacter.transform.forward = entryPos.forward;
                // �÷��̾ ������ ���� ���۽����� �̵��Ͽ����Ƿ�, ī�޶� �����ϰ� ������ �̵������ش�.
                playerController.cameraController.SetForceDefaultView();
                return;
            }    

            // ������ ĳ���� ������ �޾ƿ´�.
            var boCharacter = GameManager.User.boCharacter;

            // ĳ���� �ν��Ͻ� ����
            var character = Instantiate(ResourceManager.Instance
                .LoadObject(boCharacter.sdCharacter.resourcePath));

            // ������ ĳ���Ͱ� ������ ��ġ�ߴ� ��ǥ�� �̵�
            character.transform.position = GameManager.User.boStage.lastPos;

            // ĳ���� ��ü�� ���� ĳ���� ������Ʈ�� �����Ͽ� �ʱ�ȭ�����ش�.
            //  -> �ʱ�ȭ �� ĳ���� �����͸� ����
            var characterComp = character.GetComponent<Character>();
            characterComp.Initialize(boCharacter);

            // ���� �� �ʱ�ȭ�� ���� ĳ���� ��ü�� ������ ������ �� �ְ� �÷��̾� ��Ʈ�ѷ��� ���
            playerController.Initialize(characterComp);

            // ��� �ʱ�ȭ�� ���� ĳ���� ��ü�� ���������� ������Ʈ �� �� �ְ� �ΰ��� �Ŵ����� 
            // Ȱ�� ĳ���� ��Ͽ� ����Ѵ�.
            IngameManager.Instance.AddActor(characterComp);
        }

        /// <summary>
        /// ���� ���� �ð��� üũ�ϴ� ���
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
        /// ���� �����ð� �ʱ�ȭ
        /// </summary>
        private void ClearSpawnTime()
        {
            prevSpawnTime = Time.time;
            checkSpawnTime = Random.Range(Define.Monster.MinSpawnTime, Define.Monster.MaxSpawnTime);
        }

        /// <summary>
        /// ���͸� �����ϴ� ���
        /// </summary>
        private void SpawnMonster()
        {
            // ���� �������� ��ȹ�����͸� �޾ƿ�
            var sdStage = GameManager.User.boStage.sdStage;

            // �ش� ���������� ���͸� �������� �ʴ� ����������� ����
            if (sdStage.genMonsters[0] == -1)
                return;

            // �̸� ���ص� �ּ�, �ִ� ���� �� ���̿��� ���� ���� �޴´�.
            var monsterSpawnCnt = Random.Range(Define.Monster.MinSpawnCnt, Define.Monster.MaxSpawnCnt);
            // ���� Ǯ�� �޾ƿ´�.
            var monsterPool = ObjectPoolManager.Instance.GetPool<Object.Monster>();

            var sd = GameManager.SD;
            var ingameManager = IngameManager.Instance;

            // ������ ���� ī��Ʈ��ŭ �ݺ�
            for (int i = 0; i < monsterSpawnCnt; ++i)
            {
                // ���� ������������ ������ �� �ִ� ���� �߿� �����ϰ� ����
                // ������ �� �ִ� ������ ��ȹ������ �迭 ���� �ε����� �����´�.
                var randIndex = Random.Range(0, sdStage.genMonsters.Length);
                var genMonsterIndex = sdStage.genMonsters[randIndex];

                // ������ ������ ��ȹ�����͸� ������
                var sdMonster = sd.sdMonsters.Where(_ => _.index == genMonsterIndex).SingleOrDefault();

                // ���� ���� Ǯ���� ���� ��ü�� �ϳ� ������ ��, �����͸� ä���� ����ϱ⸸ �ϸ� ��
                //  -> ������, ���� �츮�� ������� ���� Ǯ���� ���� ������ ���Ͱ� ������
                //     �׷� ���� Ǯ���� ���͸� ������ ��, ���� ������ ���� ��ü�� ������ ������ ��ü�� ã�ƾ� ��.
                var monsterName = sdMonster.resourcePath.Remove(0, sdMonster.resourcePath.LastIndexOf('/')+1);
                // ���� ������ �̸����� ������ ��ü�� Ǯ���� ã��..
                // ������ �ݺ������� ���ο� ���ڿ��� �����ϴ� ����� ���� �ʱ� ������, ���߿� �ٲٽô°� ��õ
                //  -> ���� �׳� ���ϰ� ã������ ���Ƿ� �̸����� ã��
                
                var monster = monsterPool.GetObj(_ => _.name == monsterName);
                if (monster == null)
                    continue;

                // ������ ��ġ ���� (���� ���� ������ �����ϰ�)
                var bounds = spawnAreaBounds[sdStage.spawnArea[randIndex]];
                var spawnPosX = Random.Range(-bounds.size.x * .5f, bounds.size.x * .5f);
                var spawnPosZ = Random.Range(-bounds.size.z  * .5f, bounds.size.z * .5f);

                monster.transform.position = bounds.center + new Vector3(spawnPosX, 0, spawnPosZ);
                monster.Initialize(new BoMonster(sdMonster));

                // Ȱ�� ���� ��Ͽ� ���
                ingameManager.AddActor(monster);
            }
        }

        /// <summary>
        /// ���� ���������� ���Ǵ� NPC���� �����ϴ� ���
        /// </summary>
        private void SpawnNPC()
        {
            // ���� �������� ������ �����Ͽ� NPC ���̺� �����ؼ�
            // ���� ���������� �����ϴ� NPC���� ������ �޾ƿ´�.
            var sdStage = GameManager.User.boStage.sdStage;
            var npcs = GameManager.SD.sdNPCS.Where(_ => _.stageRef == sdStage.index).ToList();

            // �ΰ��� Ȱ�� NPC ��� ����
            var activeNPCs = IngameManager.Instance.NPCs;
            
            var resourceManager = ResourceManager.Instance;

            // npc �����͸�ŭ ��ȸ
            for (int i = 0; i < npcs.Count; ++i)
            {
                // npc �������� ������ ��θ� �̿��Ͽ� npc ��ü ����
                var npcObj = Instantiate(resourceManager.LoadObject(npcs[i].resourcePath));
                // ������ npc ��ü�� npc ������Ʈ ������ �����Ͽ� �ʱ�ȭ
                var npc = npcObj.GetComponent<Object.NPC>();
                // npc ��ȹ�����͸� ������� bo ������ ����
                npc.Initialize(new BoNPC(npcs[i]));

                // Ȱ�� npc ��Ͽ� �߰�
                activeNPCs.Add(npc);
            }
        }

        /// <summary>
        /// ���� �ε����� �޾� �ش� ������ ���� ������ ã��
        /// �ش� ���� ���� ������ ������ ��ġ�� ��ȯ
        /// </summary>
        /// <param name="monsterIndex"></param>
        public Vector3 GetRandPosInArea(int monsterIndex)
        {
            // ���� �������� ����
            var sdStage = GameManager.User.boStage.sdStage;

            // ���� �������� �������� �ش� ���������� ������ �� �ִ� ���� ������ ����
            // ��ȹ������ �󿡼� ������ �� �ִ� ���� ������ �迭���·� �Ǿ�����
            // �����ϰ� ���������� ��Ÿ���� ������ ���� ������ ������ ������ �迭���·� �Ǿ�����
            // ��, ��ȹ������ ���� �迭 �ε����� ���ϸ� ���� ���� �����Ϳ� ������ �� �ִٴ� �ǹ�
            var arrayIndex = -1;

            for (int i = 0; i < sdStage.genMonsters.Length; ++i)
            {
                if (sdStage.genMonsters[i] == monsterIndex)
                {
                    // ���� ������ ������ �ε��� ���� ã�����Ƿ� �ݺ��� ����
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
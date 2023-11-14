using AI_Project.DB;
using AI_Project.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI_Project.Object
{
    public class NPC : MonoBehaviour
    {
        public BoNPC boNPC;

        private Collider coll;

        public void Initialize(BoNPC boNPC)
        {
            this.boNPC = boNPC;
            coll ??= GetComponent<Collider>();

            var stageTrans = boNPC.sdNPC.stagePos;
            // 위치 및 회전 설정
            transform.position = new Vector3(stageTrans[0], stageTrans[1], stageTrans[2]);
            transform.eulerAngles = new Vector3(stageTrans[3], stageTrans[4], stageTrans[5]);
        }

        public void NPCUpdate()
        {
            CheckInteraction();
        }

        /// <summary>
        /// NPC의 인터렉션 영역을 설정하고, 해당 영역에 플레이어가 들어왔는지 확인한 후
        /// 들어왔다면, 상호작용 키를 눌렀는지 여부에 따라 대화창을 활성화하는 기능
        /// </summary>
        private void CheckInteraction()
        {
            var colls = Physics.OverlapBox(coll.bounds.center, coll.bounds.extents * 1.25f, transform.rotation,
                1 << LayerMask.NameToLayer("Character"));

            // colls의 길이가 0이라면 플레이어가 인터렉션 영역에 없다는 뜻
            if (colls.Length == 0)
            {
                // 해당 npc가 이미 상호작용중이라면
                // (상호작용 중이었는데 플레이어가 영역을 벗어났을 경우)
                if (boNPC.isInteraction)
                {
                    boNPC.isInteraction = false;
                    // 대화창을 종료시킴
                    UIWindowManager.Instance.GetWindow<UIDialouge>()?.Close();
                }

                return;
            }

            // 아래의 코드가 호출된다는 것은 영역에 플레이어가 존재한다는 뜻
            // E(상호작용 키)를 눌렀고, 현재 npc의 상호작용 플래그가 꺼져있을 때만 대화창을 활성화시킴
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 이미 상호작용 중인 상태
                if (boNPC.isInteraction)
                {
                    boNPC.isInteraction = UIWindowManager.Instance.GetWindow<UIDialouge>().NextDialogue();
                }
                else
                {
                    boNPC.isInteraction = true;
                    // 대화창 활성화
                    OnDialogue();
                }
            }
        }

        /// <summary>
        /// 상호작용을 통해 대화창 활성화 시, 대화창에 필요한 데이터를 생성 및 전달하는 기능
        /// </summary>
        public void OnDialogue()
        {
            // UI 다이얼로그 참조를 가져옴
            var uiDialogue = UIWindowManager.Instance.GetWindow<UIDialouge>();

            // BoDialogue 데이터 생성 및 설정
            var boDialogue = new BoDialogue();
            // 이름 설정
            boDialogue.speaker = boNPC.sdNPC.name;
            // NPC가 갖는 대화 중 하나를 랜덤하게 선택
            var randIndex = Random.Range(0, boNPC.sdNPC.speechRef.Length);
            var speechRef = boNPC.sdNPC.speechRef[randIndex];
            var speech = GameManager.SD.sdStrings.Where(_ => _.index == speechRef).SingleOrDefault()?.kr;

            // 현재 대사 데이터는 특정문자를 이용하여 대사를 여러개로 나눈 상태
            // 슬래시를 이용하여, 한 번의 출력에 보여줄 양을 결정하고 있음
            // 그럼 지금 랜덤하게 뽑은 대사를 특정문자를 이용하여 여러개의 문자열 배열로 나눈 후
            // boDialogue에 전달해준다.
            boDialogue.speeches = speech.Contains("/") ? speech.Split('/') : new string[] { speech };

            

            // 유저의 퀘스트 데이터를 가져옴
            var boQuest = GameManager.User.boQuest;

            // NPC에게서 수주할 수 있는 퀘스트 목록만 추림
            // 유저가 이미 진행중인 퀘스트라면 제외
            var canOrderQuests = boNPC.sdNPC.questRef.Except(boQuest.progressQuests.Select(_ => _.sdQuest.index));
            // 유저가 이미 완료한 퀘스트라면 제외
            canOrderQuests = canOrderQuests.Except(boQuest.completedQuests.Select(_ => _.index));

            // 유저가 진행 중이 아니고, 완료하지 않은 퀘스트만 남아있음
            // 남은 퀘스트 중에 선행 퀘스트를 완료해야만 진행할 수 있는 퀘스트인지?
            var orderQuests = canOrderQuests.ToList();
            var sdQuests = GameManager.SD.sdQuests;

            for (int i = 0; i < orderQuests.Count; ++i)
            {
                // NPC에서 추려낸 퀘스트가 선행 퀘스트 데이터가 존재하는지 확인
                // 그리고 만약 존재한다면 선행퀘스트 목록을 가져옴
                var antecedentQuest = 
                    sdQuests.Where(_ => _.index == orderQuests[i]).SingleOrDefault()?.antecedentQuest;

                // 선행 퀘스트 목록의 첫번째 원소의 값이 -1이라면?
                // -> 선행퀘스트가 존재하지 않는다는 뜻
                if (antecedentQuest[0] == -1)
                    continue;

                // 선행퀘스트가 존재한다는 뜻
                //  선행퀘스트를 완료하지 않았다면 orderQuests 에서 제외시킴

                // 선행퀘스트 목록과 유저의 완료퀘스트 목록의 교집합을 구한다.
                // -> 이 때 구한 교집합의 길이가 선행퀘스트 목록의 길이와 동일하다면?
                //    선행퀘스트를 모두 완료했다는 뜻
                if (antecedentQuest.Length !=
                    antecedentQuest.Intersect(boQuest.completedQuests.Select(_ => _.index)).Count())
                {
                    // 이 안으로 들어왔다는 것은, 선행퀘스트를 전부 완료하지 않았다는 뜻
                    // 따라서, 오더 퀘스트 목록에서 지운다.
                    orderQuests.RemoveAt(i);
                    --i;
                }
            }

            // 선행 퀘스트 여부까지 걸러 최종적으로 수주할 수 있는 퀘스트 목록을 구했으므로
            // boDialogue.quest에 전달한다.
            boDialogue.quests = orderQuests.ToArray();

            // 설정된 다이얼로그 데이터를 UI다이얼로그에 적용
            uiDialogue.SetDialogue(boDialogue);

            // Linq Except 두 집합의 차집합을 구하는 메서드
            // (반환되는 요소는 Except을 호출하는 집합을 기준으로 두번째 집합에 없는 요소들이 반환) 

            // Linq Intersect 두 집합의 교집합을 구하는 메서드
        }
    }
}

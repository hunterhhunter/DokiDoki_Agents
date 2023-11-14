using System.Linq;
using UnityEngine;

namespace AI_Project.Object
{
    public class Warp : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // 플레이어 캐릭터가 아니라면 리턴
            if (other.gameObject.tag != "Player")
                return;

            // 현재 워프 객체의 하이라키상의 부모는 해당 워프가 이동할 수 있는 스테이지 인덱스로
            // 이름이 지어진다는 규칙성이 있음.
            // 따라서, 부모의 객체의 이름을 int로 파싱했을 때 해당 워프가 연결된 스테이지 인덱스 값을
            // 얻을 수 있음
            var warpStageIndex = int.Parse(transform.parent.name);
            // 유저의 스테이지 데이터를 받아둠
            var boStage = GameManager.User.boStage;

            // 스테이지 이동을 할 것이므로, 이전 스테이지 인덱스에 현재 스테이지 인덱스를 넣는다. (아직 이동전)
            boStage.prevStageIndex = boStage.sdStage.index;
            // 이전 스테이지 인덱스를 담아뒀으므로, 현재 스테이지 데이터를 다음 스테이지로 변경한다.
            //  -> 스테이지 전체 기획 데이터에서 다음 스테이지 인덱스와 동일한 데이터를 찾는다.
            boStage.sdStage = GameManager.SD.sdStages.Where(_ => _.index == warpStageIndex).SingleOrDefault();

            var stageManager = StageManager.Instance;

            // 데이터를 전부 갖췄으므로, 스테이지 전환 실행
            //  -> 아래의 작업을 통해 이전 스테이지에서 사용하던 리소스를 해제한 후, 현재 스테이지 리소스를 불러온다.
            GameManager.Instance.OnAdditiveLoadingScene(stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
        }
    }
}
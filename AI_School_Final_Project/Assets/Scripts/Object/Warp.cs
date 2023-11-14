using System.Linq;
using UnityEngine;

namespace AI_Project.Object
{
    public class Warp : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // �÷��̾� ĳ���Ͱ� �ƴ϶�� ����
            if (other.gameObject.tag != "Player")
                return;

            // ���� ���� ��ü�� ���̶�Ű���� �θ�� �ش� ������ �̵��� �� �ִ� �������� �ε�����
            // �̸��� �������ٴ� ��Ģ���� ����.
            // ����, �θ��� ��ü�� �̸��� int�� �Ľ����� �� �ش� ������ ����� �������� �ε��� ����
            // ���� �� ����
            var warpStageIndex = int.Parse(transform.parent.name);
            // ������ �������� �����͸� �޾Ƶ�
            var boStage = GameManager.User.boStage;

            // �������� �̵��� �� ���̹Ƿ�, ���� �������� �ε����� ���� �������� �ε����� �ִ´�. (���� �̵���)
            boStage.prevStageIndex = boStage.sdStage.index;
            // ���� �������� �ε����� ��Ƶ����Ƿ�, ���� �������� �����͸� ���� ���������� �����Ѵ�.
            //  -> �������� ��ü ��ȹ �����Ϳ��� ���� �������� �ε����� ������ �����͸� ã�´�.
            boStage.sdStage = GameManager.SD.sdStages.Where(_ => _.index == warpStageIndex).SingleOrDefault();

            var stageManager = StageManager.Instance;

            // �����͸� ���� �������Ƿ�, �������� ��ȯ ����
            //  -> �Ʒ��� �۾��� ���� ���� ������������ ����ϴ� ���ҽ��� ������ ��, ���� �������� ���ҽ��� �ҷ��´�.
            GameManager.Instance.OnAdditiveLoadingScene(stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
        }
    }
}
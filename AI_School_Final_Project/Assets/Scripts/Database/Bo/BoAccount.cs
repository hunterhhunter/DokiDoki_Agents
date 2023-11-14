using System;

namespace AI_Project.DB
{
    /// <summary>
    /// �Ϲ������� ��ſ� ����ϴ� Dto�� �ΰ��ӿ��� ��� ��
    /// Bo�� ��ȯ�Ͽ� ���
    ///  -> ���� �� Dto�� ��ſ� �˸´� ���·� �����͸� ����ȭ�ϱ� ������
    ///     �ΰ��ӿ��� �ٷ� ����� �� ���� ��� ���� �־..
    /// 
    /// Bo�� �ΰ��� ���������� ���ǹǷ�, ����ȭ�� �ʿ䰡 ����
    /// ������, �۾��������� �����͸� ���ϰ� Ȯ���ϱ� ���� �ν����Ϳ� ����
    ///  -> �ν����� ������ ���� ����ȭ
    /// </summary>
#if UNITY_EDITOR
    [Serializable]
    // ����Ƽ �����Ϳ����� �ش� �ڵ尡 �����ϵ���
#endif
    public class BoAccount
    {
        public string nickname;
        public int gold;

        public BoAccount(DtoAccount dtoAccount)
        {
            nickname = dtoAccount.nickname;
            gold = dtoAccount.gold;
        }
    }
}
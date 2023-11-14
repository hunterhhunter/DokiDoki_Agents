using System;
using AI_Project.Network;

namespace AI_Project.DB
{
    /// <summary>
    /// ������ ���������� ��Ÿ���� �����ͼ�
    ///  -> ������ Ŭ���̾�Ʈ ���� ������ ��� ��, ���� ����������
    ///     �ش� Ŭ���� ���·� �ٷ��.
    /// </summary>
    [Serializable]
    public class DtoAccount : DtoBase
    {
        /// <summary>
        /// ���� �г���
        /// </summary>
        public string nickname;
        /// <summary>
        /// ������ ���
        /// </summary>
        public int gold;
    }
}
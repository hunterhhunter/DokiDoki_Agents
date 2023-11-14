using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace AI_Project.Resource
{
    using AtlasType = Define.Resource.AtlasType;

    /// <summary>
    /// ���ӿ� ���Ǵ� ��� ��Ʋ�󽺸� �����ϸ�, ��Ʋ�󽺸� ����
    /// ��Ÿ�ӿ� �ʿ��� ��������Ʈ�� �ҷ����� ����� ���
    /// ��Ʋ�󽺶�?
    ///  ���� �ؽ��ĸ� �ϳ��� �ؽ��ķ� ����� ����ϴ� �� (�޸� ����ȭ�� ����)
    /// </summary>
    public static class SpriteLoader
    {
        // ���� ��Ʋ�󽺸� �з��ϴ� ���?
        // ������ �Ը� �Ǵ� �帣�� ���� �޶��� �� ����
        // �Ϲ������� �� ������ ����
        // (ex: �κ�,�ΰ��Ӿ��� ���� �ؽ��ĸ� ��Ƶ� ��Ʋ��)
        //  ���� ������ ���Ǵ� �ؽ��ĵ��� ���� �з�
        // (ex: commonAtlas (���������� �پ��� ������ ���Ǵ� ��Ʋ��) ��)
        private static Dictionary<AtlasType, SpriteAtlas> atlasDic = new Dictionary<AtlasType, SpriteAtlas>();

        /// <summary>
        /// �Ű������� ���� ��Ʋ�� ����� ��Ʋ�󽺵��� ��ųʸ��� ���
        /// </summary>
        /// <param name="atlases"></param>
        public static void Initialize(SpriteAtlas[] atlases)
        {
            for (int i = 0; i < atlases.Length; ++i)
            {
                // ���� ��ųʸ� Ű ���� �̸� �����ص� ��Ʋ�� Ÿ�� ������
                //  -> ��Ʋ�� Ÿ�� �������� ��Ʋ�� �̸��� �����ϴ�
                // ����, �̸��� ������ Ÿ������ ĳ�����Ͽ� �ڵ����� Ű���� ����ϵ���
                var key = (AtlasType)Enum.Parse(typeof(AtlasType), atlases[i].name);

                atlasDic.Add(key, atlases[i]);
            }
        }

        /// <summary>
        /// Ư�� ��Ʋ�󽺿��� ���� ���ϴ� ��������Ʈ�� ã�Ƽ� ��ȯ���ִ� ���
        /// </summary>
        /// <param name="type">ã�����ϴ� ��������Ʈ�� ����ִ� ��Ʋ���� ��ųʸ� Ű ��</param>
        /// <param name="spriteKey">ã�����ϴ� ��������Ʈ �̸�</param>
        /// <returns></returns>
        public static Sprite GetSprite(AtlasType type, string spriteKey)
        {
            // ��Ʋ�� Ű�� ��ųʸ��� �������� �ʴ´ٸ� ����
            if (!atlasDic.ContainsKey(type))
                return null;

            return atlasDic[type].GetSprite(spriteKey);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace AI_Project.Resource
{
    using AtlasType = Define.Resource.AtlasType;

    /// <summary>
    /// 게임에 사용되는 모든 아틀라스를 관리하며, 아틀라스를 통해
    /// 런타임에 필요한 스프라이트를 불러오는 기능을 담당
    /// 아틀라스란?
    ///  여러 텍스쳐를 하나의 텍스쳐로 만들어 사용하는 것 (메모리 최적화를 위해)
    /// </summary>
    public static class SpriteLoader
    {
        // 보통 아틀라스를 분류하는 방법?
        // 게임의 규모 또는 장르에 따라 달라질 수 있음
        // 일반적으로 씬 단위로 관리
        // (ex: 로비,인게임씬에 사용될 텍스쳐를 모아둔 아틀라스)
        //  여러 씬에서 사용되는 텍스쳐들은 따로 분류
        // (ex: commonAtlas (공통적으로 다양한 씬에서 사용되는 아틀라스) 등)
        private static Dictionary<AtlasType, SpriteAtlas> atlasDic = new Dictionary<AtlasType, SpriteAtlas>();

        /// <summary>
        /// 매개변수로 받은 아틀라스 목록의 아틀라스들을 딕셔너리에 등록
        /// </summary>
        /// <param name="atlases"></param>
        public static void Initialize(SpriteAtlas[] atlases)
        {
            for (int i = 0; i < atlases.Length; ++i)
            {
                // 현재 딕셔너리 키 값은 미리 정의해둔 아틀라스 타입 열거형
                //  -> 아틀라스 타입 열거형은 아틀라스 이름과 동일하다
                // 따라서, 이름을 열거형 타입으로 캐스팅하여 자동으로 키값을 등록하도록
                var key = (AtlasType)Enum.Parse(typeof(AtlasType), atlases[i].name);

                atlasDic.Add(key, atlases[i]);
            }
        }

        /// <summary>
        /// 특정 아틀라스에서 내가 원하는 스프라이트를 찾아서 반환해주는 기능
        /// </summary>
        /// <param name="type">찾고자하는 스프라이트가 들어있는 아틀라스의 딕셔너리 키 값</param>
        /// <param name="spriteKey">찾고자하는 스프라이트 이름</param>
        /// <returns></returns>
        public static Sprite GetSprite(AtlasType type, string spriteKey)
        {
            // 아틀라스 키가 딕셔너리에 존재하지 않는다면 종료
            if (!atlasDic.ContainsKey(type))
                return null;

            return atlasDic[type].GetSprite(spriteKey);
        }
    }
}
using AI_Project.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.DB
{
    // 유저의 전체 아이템 데이터
    [Serializable]
    public class DtoItem : DtoBase
    {
        // 유저가 가지고 있는 모든 아이템 정보를 들고 있을 컬렉션
        public List<DtoItemElement> items;

        public DtoItem() { }
        /// <summary>
        /// db 저장시 사용 (현재 전체 유저 아이템 정보를 넘겨 데이터 덮어쓰기 중)
        /// </summary>
        /// <param name="boItems"></param>
        public DtoItem(List<BoItem> boItems)
        {
            items = new List<DtoItemElement>();

            for (int i = 0; i < boItems.Count; ++i)
                items.Add(new DtoItemElement(boItems[i]));
        }
    }

    // 아이템은 종류에 따라 일반적으로 데이터가 다름
    // 예를 들어 소모품, 장비 아이템은 서로 갖는 데이터가 다름
    //   -> 장비에는 강화 수치 등

    // 아이템 종류와 무관하게 공통으로 갖는 데이터를
    // 아이템 개별 데이터 베이스 클래스를 작성하여 해당 클래스에 정의 후
    // 종류에 따라 달라지는 데이터를 파생 클래스에서 작성

    // -> 하지만 현재 저희는 아래의 데이터셋 하나를 통해 모든 종류의
    //    아이템이 가질 수 있는 데이터를 전부 나열하여 처리하고 있음
    //    추천하는 방식은 아님

    // 유저의 개별 아이템 데이터
    [Serializable]
    public class DtoItemElement
    {
        // 실제 출시 게임 기준
        // 아이템마다 서버에서 부여한 고유한 키 값이 존재할 수 있음
        public int slotIndex;
        public int index; // 아이템의 기획 데이터 상의 인덱스
        public int amount; // 아이템 개수
        public int reinforceValue; // 장비 타입의 아이템일 경우, 사용하는 강화 수치
        public bool isEquip; // 장비 타입의 아이템일 경우, 캐릭터가 착용중인지?

        public DtoItemElement() { }
        public DtoItemElement(BoItem boItem)
        {
            slotIndex = boItem.slotIndex;
            index = boItem.sdItem.index;
            amount = boItem.amount;

            if (boItem is BoEquipment)
            {
                var boEquipment = boItem as BoEquipment;
                reinforceValue = boEquipment.reinforceValue;
                isEquip = boEquipment.isEquip;
            }
        }
    }

    [Serializable]
    public class DtoDropItem : DtoBase
    {
        public int[] index;
        public int[] amount;
        public float[] dropPos;
    }
}

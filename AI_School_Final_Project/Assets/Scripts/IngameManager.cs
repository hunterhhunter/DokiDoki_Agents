using AI_Project.Network;
using AI_Project.Object;
using AI_Project.UI;
using AI_Project.Util;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project
{
    /// <summary>
    /// 인게임 내 활성화된 객체들의 컬렉션을 갖고 제어할 클래스
    /// </summary>
    public class IngameManager : Singleton<IngameManager>
    {
        private int idCounter = 0; 

        // 활성화된 몬스터 객체들을 하이라키 상에서 가지고 있을 부모
        private Transform monsterHolder; 

        // 현재 활성화된 액터들을 갖는 컬렉션
        public List<Actor> Characters { get; private set; } = new List<Actor>();
        public List<Actor> Monsters { get; private set; } = new List<Actor>();
        public List<NPC> NPCs { get; private set; } = new List<NPC>();
        public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();

        public IngameHandler IngameHandler { get; private set; } = new IngameHandler();

        /// <summary>
        /// 활성화된 액터(주로 새로 생성하거나 풀에서 가져온 액터)들을
        /// 활성 액터 리스트에 등록하는 기능
        /// </summary>
        /// <param name="actor"></param>
        public void AddActor(Actor actor)
        {
            actor.id = ++idCounter;

            switch (actor.boActor.type)
            {
                case Define.Actor.Type.Character:
                    Characters.Add(actor);
                    break;
                case Define.Actor.Type.Monster:
                    monsterHolder ??= new GameObject("MonsterHolder").transform;
                    actor.transform.SetParent(monsterHolder);
                    Monsters.Add(actor);
                    // 몬스터에게 hpbar 추가
                    UIWindowManager.Instance.GetWindow<UIIngame>().AddHpBar(actor);
                    // 몬스터 같은 경우 오브젝트 풀에서 가져와서 사용하기 때문에
                    // 객체를 풀에서 막 가져왔을 경우(비활성화된 상태), 따라서 사용을 위해 활성화 
                    actor.gameObject.SetActive(true);
                    break;
            }
        }

        private void FixedUpdate()
        {
            ActorUpdate(Characters);
            ActorUpdate(Monsters);
            NPCUpdate();
            ProjectileUpdate();
        }

        private void ActorUpdate(List<Actor> actors)
        { 
            for (int i = 0; i < actors.Count; ++i)
            {
                // 액터가 죽지 않았다면 업데이트
                if (actors[i].State != Define.Actor.State.Dead)
                    actors[i].Execute();
                else
                {
                    // 죽었다면 액터 컬렉션에서 제거
                    actors.RemoveAt(i);
                    --i;
                }
            }
        }

        private void NPCUpdate()
        {
            for (int i = 0; i < NPCs.Count; ++i)
            {
                if (NPCs[i] != null)
                    NPCs[i].NPCUpdate();
                else
                {
                    NPCs.RemoveAt(i);
                    --i;
                }
            } 
        }

        /// <summary>
        /// 스테이지 전환 시, 이전 스테이지에 사용하던 NPC들을 파괴하고, 활성 NPC 목록에서 제거함
        /// </summary>
        public void ClearNPC()
        {
            for (int i = 0; i < NPCs.Count; ++i)
            {
                if (NPCs[i] != null)
                    Destroy(NPCs[i].gameObject);
            }

            NPCs.Clear();
        }

        private void ProjectileUpdate()
        {
            ObjectPool<Projectile> pool = null;

            for (int i = 0; i < Projectiles.Count; ++i)
            {
                // 충돌하지 않았거나 지속시간이 끝나지 않았다면 
                if (!Projectiles[i].isEnd)
                    // 업데이트
                    Projectiles[i].Execute();
                // 아니라면
                else 
                {
                    // 풀 참조가 없다면 참조를 가져온 후, 풀에 반환
                    pool ??= ObjectPoolManager.Instance.GetPool<Projectile>();
                    pool.Return(Projectiles[i]);
                }
            }
        }
    }
}

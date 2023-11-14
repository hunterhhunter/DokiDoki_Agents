namespace AI_Project.Define
{
    /// <summary>
    /// 게임에 사용되는 씬 종류
    /// </summary>
    public enum SceneType { Title, Ingame, Loading }

    /// <summary>
    /// 타이틀 씬에서 순차적을 수행할 작업
    /// </summary>
    public enum IntroPhase
    {
        Start,
        AppSetting,
        Server,
        StaticData,
        UserData,
        Resource,
        UI,
        Complete
    }

    public class Input
    {
        public const string AxisX = "Horizontal";
        public const string AxisZ = "Vertical";
        public const string MouseX = "Mouse X";
        public const string MouseY = "Mouse Y";
        public const string FrontCam = "Fire3";
        public const string Jump = "Jump";
        public const string MouseLeft = "Fire1";
        public const string MouseRight = "Fire2";
    }

    public class Camera
    {
        public enum View { Default, Front }
        public const float RotSpeed = 3f;
        public const string CamPosPath = "Prefabs/CamPos";
    }

    public class Actor
    {
        /// <summary>
        /// 액터의 종류
        /// </summary>
        public enum Type { Character, Monster }

        /// <summary>
        /// 액터의 일반공격 타입
        /// Normal 근접공격, Projectile 발사체를 이용한 공격
        /// </summary>
        public enum AttackType { Normal, Projectile }

        /// <summary>
        /// 액터의 상태
        /// </summary>
        public enum State { Idle, Walk, Jump, Attack, Dead }
    }

    public class NPC
    {
        // NPC의 타입 (ex: 강화 NPC인지?, 잡화판매 NPC인지? 등)
        public enum Type { None, }
    }

    public class Monster
    {
        public const int MinSpawnCnt = 1;
        public const int MaxSpawnCnt = 5;
        public const float MinSpawnTime = 10f;
        public const float MaxSpawnTime = 20f;
        public const float MinPatrolWaitTime = 1f;
        public const float MaxPatrolWaitTime = 3f;
    }

    public class Item
    {
        public enum Type { Equipment, Expendables, Quest, Etc }
    }

    public class Quest
    {
        public enum QuestType { Hunt, Collect, Adventure } 
    }

    public class UI
    {
        public enum DialogueBtnType { Shop, Quest,  }
        public enum QuestWindow { List, Order }
        public enum QuestTab { Progress, Completed }
    }

    public class Resource
    {
        public enum AtlasType { ItemAtlas, IconAtlas }
    }

    public class StaticData
    {
        public const string SDPath = "Assets/StaticData";
        public const string SDExcelPath = "Assets/StaticData/Excel";
        public const string SDJsonPath = "Assets/StaticData/Json";
    }
}

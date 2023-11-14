using AI_Project.Network;
using AI_Project.Util;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace AI_Project.Dummy
{
    /// <summary>
    /// 더미서버의 역할을 수행할 클래스
    /// 더미서버에서 사용할 db를 갖는다.
    /// </summary>
    public class DummyServer : Singleton<DummyServer>
    {
        /// <summary>
        /// 더미서버에서 갖는 유저데이터 (유저 DB)
        /// </summary>
        public UserDataSo userData;
        /// <summary>
        /// 더미서버의 통신 기능을 갖는 모듈
        /// </summary>
        public INetworkClient dummyModule;

        private Coroutine saveCoroutine;

        public void Initialize()
        {
            dummyModule = new ServerModuleDummy(this);
        }

        /// <summary>
        /// 더미 유저 데이터를 저장하는 기능
        /// 런타임에 db 데이터에 변동사항이 생겼을 경우, 변경사항을 저장하는 기능
        /// </summary>
        public void Save()
        {
            // 현재 db 데이터는 스크립터블 오브젝트를 기반으로 작성되었기 때문에
            // 저장기능은 에디터에서만 가능
            // 또, 해당 기능은 성능이 좋은 편은 아님

            // 저장시킬 유저 데이터를 더티 플래그로 설정
            //  유니티에서 런타임에 사용되는 (프리팹 또는 스크립터블 오브젝트) 등은
            //  일반적으로 휘발성 데이터이고, 변동사항을 원본에 저장하는 목적으로 사용되는
            //  데이터가 아님 
            //  하지만, 런타임 시 사용되던 데이터를 저장하고 싶을 때, 디스크에 쓸 수 있게
            //  더티 플래그를 설정하면 가능함 

            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
                saveCoroutine = null;
            }

            saveCoroutine = StartCoroutine(SaveProgress());

            IEnumerator SaveProgress()
            {
                EditorUtility.SetDirty(userData);
                AssetDatabase.SaveAssets();

                yield return null;
            }
        }
    }
}
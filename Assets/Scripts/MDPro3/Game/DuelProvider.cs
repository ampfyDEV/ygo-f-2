using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MDPro3;
using MDPro3.Servant;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MDPro3
{
    public class DuelProvider : MonoBehaviour
    {
        [Header("Public References")]
        public Transform container_3D;
        public Transform container_2D;
        public CardRenderer cardRenderer;


        public OcgCore ocgcore;
        public RoomServant room;

        private readonly List<Servant.Servant> servants = new();

        public static DuelProvider instance;

        private void InitializeDuelServants()
        {
            servants.Add(ocgcore);
            servants.Add(room);
            foreach (Servant.Servant servant in servants)
                servant.Initialize();
        }

        private void Awake()
        {
            instance = this;
            _ = Initialize();
        }

        private async UniTask Initialize()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (!ABLoader.mdCached)
                await ABLoader.CacheMasterDuelOutDuelBundles();

            cardRenderer = (await Addressables.InstantiateAsync("Prefab/CardRenderer.prefab")).GetComponent<CardRenderer>();
            InitializeDuelServants();

        }

    }

}

using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace ItemTweaks
{
    [BepInPlugin("aoe.top.ItemTweaks", "[戴森球计划]物品叠加 Mod By:小莫", "1.1.0")]
    public class ItemTweaks : BaseUnityPlugin
    {
        // 默认资源叠加倍率
        public static int Count = 10;

        public static ConfigEntry<int> userCount;

        // 启动脚本时会自动调用Awake()方法
        private void Awake()
        {
            // InitializeGUI();
        }

        // 注入脚本时会自动调用Start()方法 执行在Awake()方法后面
        [Obsolete]
        private void Start()
        {
            // 初始化UI 需安装 BepInEx.ConfigurationManager 才能显示UI
            userCount = Config.AddSetting("调整叠加倍率", "倍率:", 10, new ConfigDescription("你可以根据自己的需求,自由的调整物品叠加的倍率", new AcceptableValueRange<int>(1, 1000)));
            Count = userCount.Value;

            // 注入补丁
            Harmony.CreateAndPatchAll(typeof(ItemTweaks), null);

        }
        // 插件将自动循环Update()方法中的内容
        private void Update()
        {
            // 如果用户修改了资源倍数的值
            if (Count != userCount.Value)
            {
                //Debug.Log(String.Format("用户修改资源倍率为 {0}", userCount.Value));
                //Count = userCount.Value;

                ////StorageComponent Sc = new StorageComponent(Configs.freeMode.playerPackageSize);
                ////Sc.InitConn();
                ////Sc.NotifyStorageChange();

                //Player pl = new Player();

                //pl.OnDraw();

                //MyLoadStatic(Sc);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StorageComponent), "LoadStatic")]
        public static bool MyLoadStatic(StorageComponent __instance)
        {
            Debug.Log("自定义 LoadStatic 被调用");

            if (!StorageComponent.staticLoaded)
            {
                StorageComponent.itemIsFuel = new bool[12000];
                StorageComponent.itemStackCount = new int[12000];
                for (int i = 0; i < 12000; i++)
                {
                    StorageComponent.itemStackCount[i] = 1000;
                }
                ItemProto[] dataArray = LDB.items.dataArray;
                for (int j = 0; j < dataArray.Length; j++)
                {
                    StorageComponent.itemIsFuel[dataArray[j].ID] = (dataArray[j].HeatValue > 0L);
                    StorageComponent.itemStackCount[dataArray[j].ID] = dataArray[j].StackSize * Count;
                }
                StorageComponent.staticLoaded = true;
            }
            return false;
        }

    }
}


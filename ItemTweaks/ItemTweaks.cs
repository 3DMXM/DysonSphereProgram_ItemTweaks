using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace ItemTweaks
{
    [BepInPlugin("aoe.top.ItemTweaks", "[戴森球计划] 物品叠加 Mod By:小莫", "1.2.1")]
    public class ItemTweaks : BaseUnityPlugin
    {
        // 默认资源叠加倍率
        public static int Count = 10;

        public static ConfigEntry<int> userCount;

        // 启动脚本时会自动调用Awake()方法
        //private void Awake()
        //{
        //}

        // 注入脚本时会自动调用Start()方法 执行在Awake()方法后面
        [Obsolete]
        private void Start()
        {
            // 初始化UI 需安装 BepInEx.ConfigurationManager 才能显示UI
            userCount = Config.AddSetting("调整叠加倍率", "倍率:", 10, new ConfigDescription("你可以根据自己的需求,自由的调整物品叠加的倍率", new AcceptableValueRange<int>(1, 1000)));
            Count = userCount.Value;

            // 修改堆叠数量
            //ChangeItemTweaks();   // 不能在启动时调用，否则会报错
            Debug.Log(String.Format("初始化,当前叠加倍率是{0}倍", Count));
            // 注入补丁
            Harmony.CreateAndPatchAll(typeof(ItemTweaks), null);

        }


        // 插件将自动循环Update()方法中的内容
        private void Update()
        {
            // 如果用户修改了资源倍数的值
            if (Count != userCount.Value)
            {
                //Debug.Log(String.Format("用户修改资源倍率为 {0},旧倍率为{1}", userCount.Value, Count));
                ChangeItemTweaks(true, Count, userCount.Value);
                Count = userCount.Value;
            }
        }
        /// <summary>
        /// 修改叠加倍率
        /// </summary>
        /// <param name="Change">是否是修改</param>
        /// <param name="oldCount">旧叠加倍率</param>
        /// <param name="newCount">新叠加倍率</param>
        static void ChangeItemTweaks(bool Change = false, int oldCount = -1, int newCount = -1)
        {
            //for (int i = 0; i < 12000; i++)
            //{
            //    StorageComponent.itemStackCount[i] = 1000 * Count;
            //}
            /*
             * LDB.items.dataArray[1].StackSize = 100;
             * ->  dataArray[j].StackSize = dataArray[j].StackSize * 100;
             * -> LDB.items.dataArray[1].StackSize = 10000;
             * -> dataArray[j].StackSize = dataArray[j].StackSize * 10;
             * -> LDB.items.dataArray[1].StackSize / 100 * 10
             * */

            ItemProto[] dataArray = LDB.items.dataArray;
            for (int j = 0; j < dataArray.Length; j++)
            {
                if (Change)
                {
                    if (oldCount > 0 && newCount > 0)
                    {
                        dataArray[j].StackSize = dataArray[j].StackSize / oldCount * newCount;

                        Debug.Log(String.Format("修改物品ID{0}的叠加倍率为{1}", j, dataArray[j].StackSize));
                        StorageComponent.itemStackCount[dataArray[j].ID] = dataArray[j].StackSize;
                    }
                }
                else
                {
                    dataArray[j].StackSize = dataArray[j].StackSize * Count;
                    Debug.Log(String.Format("修改物品ID{0}的叠加倍率为{1}", j, dataArray[j].StackSize));
                    StorageComponent.itemStackCount[dataArray[j].ID] = dataArray[j].StackSize;
                }
            }

        }

        static bool change = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StorageComponent), "LoadStatic")]
        public static void MyLoadStatic(StorageComponent __instance)
        {
            if (!change)
            {
                // 让它只执行一次
                ChangeItemTweaks();
                change = true;
            }
        }



    }
}


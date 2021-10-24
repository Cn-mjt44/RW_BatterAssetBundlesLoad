using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BatterAssetBundlesLoad
{

    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(ShaderDatabase))]
    [HarmonyPatch("LoadShader")]
    public static class PatchShaderDatabase
    {
        static PatchShaderDatabase()
        {
            harmony.PatchAll();
            ModContentPack_allAssetNamesInBundleCached = typeof(ModContentPack).GetField("allAssetNamesInBundleCached", AccessTools.all);
            List<ModContentPack> mods = LoadedModManager.RunningModsListForReading;
            foreach(ModContentPack pack in mods)
            {
                if (!pack.assetBundles.loadedAssetBundles.NullOrEmpty())
                {
                    List<AssetBundle> cache = pack.assetBundles.loadedAssetBundles;
                    pack.assetBundles.loadedAssetBundles = new List<AssetBundle>(cache.Count * 3);
                    for (int i = 0; i < cache.Count; i++)
                    {
                        pack.assetBundles.loadedAssetBundles.Add(cache[i]);
                        pack.assetBundles.loadedAssetBundles.Add(cache[i]);
                        pack.assetBundles.loadedAssetBundles.Add(cache[i]);
                    }
                }
                ModContentPack_allAssetNamesInBundleCached.SetValue(pack, null);
            }
            #region test
            /**
            test = true;
            try
            {
                ContentFinder<Texture2D>.Get("");
            }
            catch { }
            try
            {
                ShaderDatabase.LoadShader("");
            }
            catch { }
            try
            {
                ContentFinder<Texture2D>.GetAllInFolder("").ToList();
            }
            catch { }
            test = false;
            **/
            #endregion
        }

        [HarmonyPrefix]
        public static bool LoadShaderPrefix(string shaderPath,ref Shader __result)
        {
            Dictionary<string, Shader> lookup = (Dictionary<string, Shader>)Dictionary_lookup.GetValue(null);
            if (lookup == null)
            {
                lookup = new Dictionary<string, Shader>();
                Dictionary_lookup.SetValue(null, lookup);
            }
            if (!lookup.TryGetValue(shaderPath, out __result))
            {
                List<ModContentPack> runningModsListForReading = LoadedModManager.RunningModsListForReading;
                foreach (ModContentPack pack in runningModsListForReading)
                {
                    if(!pack.assetBundles.loadedAssetBundles.NullOrEmpty())
                    {
                        foreach (AssetBundle assetBundle in pack.assetBundles.loadedAssetBundles)
                        {
                            string fullPath = Path.Combine(Path.Combine(Path.Combine(basePath, pack.FolderName), "Materials/"), shaderPath);
                            Shader result = (Shader)assetBundle.LoadAsset<Shader>(fullPath + ".shader");
                            if (result != null)
                            {
                                lookup[shaderPath] = result;
                                __result = result;
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private static readonly string basePath = Path.Combine("Assets", "Data");

        private static FieldInfo Dictionary_lookup = typeof(ShaderDatabase).GetField("lookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        private static FieldInfo ModContentPack_allAssetNamesInBundleCached;

        //public static bool test = false;//test

        public static Harmony harmony = new Harmony("BatterAssetBundlesLoad");
    }

    #region Patch

    [HarmonyPatch(typeof(ModContentPack))]
    [HarmonyPatch("get_FolderName")]
    public static class PatchModContentPackFor_ContentFinder
    {
        [HarmonyPostfix]
        public static void get_FolderNamePostfix(ref string __result, ModContentPack __instance)
        {
            StackTrace trace = new StackTrace();
            Type type = trace.GetFrame(2).GetMethod().DeclaringType;
            if (type.FullName.StartsWith("Verse.ContentFinder`1") ||
                type == typeof(PatchShaderDatabase))
            {
                //if(PatchShaderDatabase.test) Log.Message(type.FullName + " : " + i);//test
                switch (i)
                {
                    case 0:
                        ++i;
                        return;
                    case 1:
                        ++i;
                        __result = __instance.Name;
                        return;
                    default:
                        i = 0;
                        __result = __instance.PackageId;
                        return;
                }
            }
        }
        private static int i = 0;
    }


    [HarmonyPatch(typeof(ModAssetBundlesHandler))]
    [HarmonyPatch("ReloadAll")]
    public static class PatchModAssetBundlesHandlerFor_ContentFinder_ReloadAll
    {
        [HarmonyPostfix]
        public static void ReloadAllPostfix(ModAssetBundlesHandler __instance)
        {
            if(!__instance.loadedAssetBundles.NullOrEmpty())
            {
                List<AssetBundle> cache = __instance.loadedAssetBundles;
                __instance.loadedAssetBundles = new List<AssetBundle>(cache.Count * 3);
                for(int i = 0; i < cache.Count;i++)
                {
                    __instance.loadedAssetBundles.Add(cache[i]);
                    __instance.loadedAssetBundles.Add(cache[i]);
                    __instance.loadedAssetBundles.Add(cache[i]);
                }
            }
        }
    }


    [HarmonyPatch(typeof(ModAssetBundlesHandler))]
    [HarmonyPatch("<ClearDestroy>b__6_0")]
    public static class PatchModAssetBundlesHandlerFor_ContentFinder_b__6_0
    {
        [HarmonyPrefix]
        public static void b__6_0Prefix(ModAssetBundlesHandler __instance)
        {
            if (!__instance.loadedAssetBundles.NullOrEmpty())
            {
                List<AssetBundle> cache = __instance.loadedAssetBundles;
                __instance.loadedAssetBundles = new List<AssetBundle>(cache.Count / 3);
                for (int i = 0; i < cache.Count; i+=3)
                {
                    __instance.loadedAssetBundles.Add(cache[i]);
                }
            }
        }
    }
    #endregion
}

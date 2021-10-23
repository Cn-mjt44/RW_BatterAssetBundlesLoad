using BatterAssetBundlesLoad;
using System;
using System.Reflection;
using HarmonyLib;

namespace Test
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main");
            Exception ex;

            TestClass<int>.add(5);
            MethodInfo method = typeof(TestClass<int>).GetMethod("add", AccessTools.all);
            long methodStart = Memory.GetMethodStart(method, out ex);
            Console.WriteLine("TestClass<int>.add() Memory postion:0x" + methodStart.ToString("x").PadLeft(16,'0'));

            TestClass<float>.add(5);
            method = typeof(TestClass<float>).GetMethod("add", AccessTools.all);
            methodStart = Memory.GetMethodStart(method, out ex);
            Console.WriteLine("TestClass<float>.add() Memory postion:0x" + methodStart.ToString("x").PadLeft(16, '0'));

            TestClass<long>.add(5);
            method = typeof(TestClass<long>).GetMethod("add", AccessTools.all);
            methodStart = Memory.GetMethodStart(method, out ex);
            Console.WriteLine("TestClass<long>.add() Memory postion:0x" + methodStart.ToString("x").PadLeft(16, '0'));
            Console.ReadLine();

            harmony.PatchAll();

            TestClass<int>.add(5);
            method = typeof(TestClass<int>).GetMethod("add", AccessTools.all);
            methodStart = Memory.GetMethodStart(method, out ex);
            Console.WriteLine("TestClass<int>.add() Memory postion:0x" + methodStart.ToString("x").PadLeft(16, '0'));

            TestClass<float>.add(5);
            method = typeof(TestClass<float>).GetMethod("add", AccessTools.all);
            methodStart = Memory.GetMethodStart(method, out ex);
            Console.WriteLine("TestClass<float>.add() Memory postion:0x" + methodStart.ToString("x").PadLeft(16, '0'));

            TestClass<long>.add(5);
            method = typeof(TestClass<long>).GetMethod("add", AccessTools.all);
            methodStart = Memory.GetMethodStart(method, out ex);
            Console.WriteLine("TestClass<long>.add() Memory postion:0x" + methodStart.ToString("x").PadLeft(16, '0'));
            Console.ReadLine();
        }

        public static Harmony harmony = new Harmony("Program");
    }
    #region patchTest
    /**
    [HarmonyPatch(typeof(TestClass<int>))]
    [HarmonyPatch("add")]
    public static class PatchTestClass_int
    {
        [HarmonyPrefix]
        public static bool addPrefix(int obj)
        {
            Console.WriteLine("[Patched]TestClass<int>.add(" + obj + ")");
            return true;
        }
    }
    [HarmonyPatch(typeof(TestClass<float>))]
    [HarmonyPatch("add")]
    public static class PatchTestClass_float
    {
        [HarmonyPrefix]
        public static bool addPrefix(float obj)
        {
            Console.WriteLine("[Patched]TestClass<float>.add(" + obj + ")");
            return true;
        }
    }
    [HarmonyPatch(typeof(TestClass<long>))]
    [HarmonyPatch("add")]
    public static class PatchTestClass_long
    {
        [HarmonyPrefix]
        public static bool addPrefix(long obj)
        {
            Console.WriteLine("[Patched]TestClass<long>.add(" + obj + ")");
            return true;
        }
    }
    **/
    #endregion
}

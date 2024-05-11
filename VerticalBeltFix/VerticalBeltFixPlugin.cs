using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using VerticalBeltFix.Patches;

namespace VerticalBeltFix
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class VerticalBeltFixPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.equinox.VerticalBeltFix";
        private const string PluginName = "VerticalBeltFix";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            Harmony.CreateAndPatchAll(typeof(ConveyorInstancePatch));

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void Update() {
            // ToDo: Delete If Not Needed
        }
    }
}

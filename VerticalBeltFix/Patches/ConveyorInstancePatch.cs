using EquinoxsModUtils;
using FluffyUnderware.DevTools.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VerticalBeltFix.Patches
{
    internal class ConveyorInstancePatch
    {
        [HarmonyPatch(typeof(ConveyorInstance), "CanAcceptResourceFromInserter")]
        [HarmonyPrefix]
        static bool ReplaceCheck(ref ConveyorInstance me, int resId, in Vector3Int posToAddAt, ref bool __result) {
            if (me.beltShape != ConveyorInstance.BeltShape.Vertical) return true;

            float start = me.gridInfo.minPos.y;
            if (!me.inputBottom) start += me.gridInfo.dims.y;
            start -= posToAddAt.y;

            int countInRange = 0;
            foreach (ResTransportData data in me.curResPositions) {
                if (data.resType == 0) continue;
                if (data.beltPos >= start && data.beltPos <= start + 1) ++countInRange;
            }

            __result = countInRange < 3;

            return false;
        }

        [HarmonyPatch(typeof(ConveyorInstance), "FindInserterDropSpot")]
        [HarmonyPrefix]
        static bool ReplaceFindSpot(ConveyorInstance __instance, ref bool __result, out float beltPos, out int index, float minBeltRange, float maxBeltRange) {
            index = 0;
            beltPos = (minBeltRange + maxBeltRange) * 0.5f;

            if (__instance.beltShape != ConveyorInstance.BeltShape.Vertical) return true;
            if (__instance.curResCount == __instance.curResPositions.Length) return true;

            List<ResTransportData> resInSegment = __instance.curResPositions.Where(data => data.beltPos > minBeltRange && data.beltPos <= maxBeltRange).ToList();
            if(resInSegment.Count == 0) {
                __result = true;
                return false;
            }

            bool foundPos = false;
            foreach(ResTransportData data in resInSegment) {
                if (data.beltPos + 0.42f <= maxBeltRange) {
                    beltPos = data.beltPos + 0.42f;
                    foundPos = true;
                    break;
                }
                else if (data.beltPos - 0.42f >= minBeltRange) {
                    beltPos = data.beltPos - 0.42f;
                    foundPos = true;
                    break;
                }
            }

            __result = foundPos;

            if (Player.instance.interaction.TargetMachineRef.IsValid())
                if (Player.instance.interaction.TargetMachineRef.GetCommonInfo().instanceId == __instance.commonInfo.instanceId)
                    ModUtils.PacedLog($"minBeltRange: {minBeltRange} | maxBeltRange: {maxBeltRange} | beltPos: {beltPos} | foundPos: {foundPos}");
            
            return false;
        }
    }
}

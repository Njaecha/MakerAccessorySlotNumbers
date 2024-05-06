using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using ChaCustom;
using System.Reflection.Emit;

namespace MakerAccessorySlotNumbers
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class MakerAccessorySlotNumbers : BaseUnityPlugin
    {
        public const string PluginName = "MakerAccessorySlotNumbers";
        public const string GUID = "org.njaecha.plugins.makeraccessoryslotnumbers";
        public const string Version = "1.0.0";

        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Hooks));
        }
    }

    internal class Hooks
    {
        [HarmonyPatch(typeof(ChaCustom.CvsAccessory), nameof(ChaCustom.CvsAccessory.UpdateSlotName)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UpdateSlotNameTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);

            CodeMatcher cm = new CodeMatcher(instructions, generator);
            cm.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ChaInfo), nameof(ChaInfo.infoAccessory))));
            cm.Advance(-1);
            cm.RemoveInstructions(9);
            cm.InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hooks), nameof(SlotNameUpdater))));

            return cm.Instructions();
        }

        [HarmonyPatch(typeof(ChaCustom.CustomAcsChangeSlot), nameof(ChaCustom.CustomAcsChangeSlot.UpdateSlotNames)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UpdateSlotNameTranspiler2(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);

            CodeMatcher cm = new CodeMatcher(instructions, generator);
            cm.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ChaInfo), nameof(ChaInfo.infoAccessory))));
            cm.Advance(-1);
            cm.RemoveInstructions(2);
            cm.Advance(2);
            cm.RemoveInstructions(5);
            cm.InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hooks), nameof(SlotNameUpdater2))));

            return cm.Instructions();
        }

        public static void SlotNameUpdater(CvsAccessory cvs)
        {
            cvs.chaCtrl.infoAccessory.SafeProc(cvs.nSlotNo, delegate (ListInfoBase info)
            {
                cvs.textSlotName.text = (cvs.nSlotNo + 1).ToString("00") + " " + info.Name;
            });
        }

        public static void SlotNameUpdater2(CustomAcsChangeSlot cacs, int i)
        {
            cacs.chaCtrl.infoAccessory.SafeProc(i, delegate (ListInfoBase info)
            {
                cacs.textSlotNames[i].text = (i + 1).ToString("00") + " " + info.Name;
            });
        }
    }
}

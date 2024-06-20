using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using ChaCustom;
using System.Reflection.Emit;
using KKAPI.Utilities;

namespace MakerAccessorySlotNumbers
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class MakerAccessorySlotNumbers : BaseUnityPlugin
    {
        public const string PluginName = "MakerAccessorySlotNumbers";
        public const string GUID = "org.njaecha.plugins.makeraccessoryslotnumbers";
        public const string Version = "1.0.1";

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
#if KKS
            cm.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ChaInfo), nameof(ChaInfo.infoAccessory))));
            cm.Advance(-1);
            cm.RemoveInstructions(9);
            cm.InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hooks), nameof(SlotNameUpdater))));
#elif KK
            cm.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ListInfoBase), nameof(ListInfoBase.Name))));
            cm.Advance(-7);
            cm.RemoveInstructions(9);
            cm.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hooks), nameof(SlotNameUpdater)))
            );
#endif
            return cm.Instructions();
        }

        [HarmonyPatch(typeof(ChaCustom.CustomAcsChangeSlot), nameof(ChaCustom.CustomAcsChangeSlot.UpdateSlotNames)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UpdateSlotNameTranspiler2(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);

            CodeMatcher cm = new CodeMatcher(instructions, generator);
#if KKS
            cm.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ChaInfo), nameof(ChaInfo.infoAccessory))));
            cm.Advance(-1);
            cm.RemoveInstructions(2);
            cm.Advance(2);
            cm.RemoveInstructions(5);
            cm.InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hooks), nameof(SlotNameUpdater2))));
#elif KK
            cm.MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ListInfoBase), nameof(ListInfoBase.Name))));
            cm.Advance(-8);
            cm.RemoveInstructions(10);
            cm.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hooks), nameof(SlotNameUpdater2)))
            );
#endif
            return cm.Instructions();
        }

        public static void SlotNameUpdater(CvsAccessory cvs)
        {
#if KKS
            cvs.chaCtrl.infoAccessory.SafeProc(cvs.nSlotNo, delegate (ListInfoBase info)
            {
                TranslationHelper.TranslateAsync(info.Name, t => cvs.textSlotName.text = (cvs.nSlotNo + 1).ToString("00") + " " + t);
            });
#elif KK
            TranslationHelper.TranslateAsync(cvs.chaCtrl.infoAccessory[cvs.nSlotNo].Name, t => cvs.textSlotName.text = (cvs.nSlotNo + 1).ToString("00") + " " + t);
#endif
        }

        public static void SlotNameUpdater2(CustomAcsChangeSlot cacs, int i)
        {
#if KKS
            cacs.chaCtrl.infoAccessory.SafeProc(i, delegate (ListInfoBase info)
            {
                TranslationHelper.TranslateAsync(info.Name, t => cacs.textSlotNames[i].text = (i + 1).ToString("00") + " " + t);
            });
#elif KK   
            TranslationHelper.TranslateAsync(cacs.chaCtrl.infoAccessory[i].Name, t => cacs.textSlotNames[i].text = (i + 1).ToString("00") + " " + t);
#endif
        }
    }
}

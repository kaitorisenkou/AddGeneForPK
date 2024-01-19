using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AddGeneForPK {
    [StaticConstructorOnStartup]
    public class AddGeneForPK {
        static AddGeneForPK() {
            Log.Message("[AddGeneForPK] Now active");
            var harmony = new Harmony("kaitorisenkou.AddGeneForPK");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("[AddGeneForPK] Harmony patch complete!");
        }
    }

    [HarmonyPatch(typeof(PawnGenerator),"GenerateGenes")]
    public static class Patch_GenerateGenes {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var instructionList = instructions.ToList();
            int stage = 0;
            MethodInfo targetInfo = AccessTools.Method(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.GetMelaninGene));
            //CodeInstruction ldargas = null;
            for (int i = 0; i < instructionList.Count; i++) {
                /*
                if (stage < 1) {
                    if (instructionList[i].opcode == OpCodes.Ldarga_S) {
                        ldargas = new CodeInstruction(instructionList[i]);
                        stage++;
                    }
                    continue;
                }*/
                if (instructionList[i].opcode == OpCodes.Callvirt && (MethodInfo)instructionList[i].operand == targetInfo) {
                    i -= 1;
                    instructionList.InsertRange(i, new CodeInstruction[] {
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(Patch_GenerateGenes),nameof(AddAddidionalGenes))),
                        new CodeInstruction(OpCodes.Ldarg_0)
                    });
                    stage++;
                    break;
                }
            }
            if (stage < 1) {
                Log.Error("[AddGeneForPK] Patch_GenerateGenes failed (stage:" + stage + ")");
            }
            return instructionList;
        }

        public static void AddAddidionalGenes(Pawn pawn,PawnGenerationRequest request) {
            var kindDef = request.KindDef;
            if (!kindDef.HasModExtension<PKAdditionalGenes>())
                return;
            foreach(var i in kindDef.GetModExtension<PKAdditionalGenes>().geneSets) {
                i.AddGenes(pawn);
            }
            return;
        }
    }
}

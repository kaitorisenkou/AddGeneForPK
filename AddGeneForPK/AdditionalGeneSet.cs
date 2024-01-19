using System.Collections.Generic;
using Verse;

namespace AddGeneForPK {
    public class AdditionalGeneSet {
        public bool isXenogene = false;
        public float chance = 1.0f;
        public List<GeneDef> geneDefs = new List<GeneDef>();
        public IEnumerable<GeneDef> GetGeneDefs() {
            if (chance < UnityEngine.Random.Range(0f, 1f)) {
                yield break;
            }
            foreach (var i in geneDefs)
                yield return i;
        }
        public void AddGenes(Pawn pawn) {
            var genes = pawn.genes;
            foreach (var i in GetGeneDefs()) {
                genes.AddGene(i, isXenogene);
            }
        }
    }
}

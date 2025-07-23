using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace Androids
{
    public class AndroidLikeHediff : HediffWithComps
    {
        public float energyTracked;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref energyTracked, "energyTracked");
        }

        public override void Tick()
        {
            base.Tick();

            if (pawn.needs.TryGetNeed<Need_Energy>() is Need_Energy energy)
            {
                energyTracked = energy.CurLevel;
            }
            if (!pawn.Dead)
            {
                //Tick Android HediffGivers and remove bleeding effects.
                List<HediffGiverSetDef> hediffGiverSets = ThingDefOf.ChjAndroid.race.hediffGiverSets;
                if (hediffGiverSets != null && pawn.IsHashIntervalTick(60))
                {
                    for (int k = 0; k < hediffGiverSets.Count; k++)
                    {
                        List<HediffGiver> hediffGivers = hediffGiverSets[k].hediffGivers;
                        for (int l = 0; l < hediffGivers.Count; l++)
                        {
                            hediffGivers[l].OnIntervalPassed(pawn, null);
                            if (pawn.Dead)
                            {
                                return;
                            }
                        }
                    }
                }

                //Remove bleeding.
                pawn.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def == RimWorld.HediffDefOf.BloodLoss);
            }
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            //Log.Message("Pawn died: " + pawn);
            //Log.Message("Parent Holder: " + pawn.ParentHolder);

            if (pawn.health.hediffSet.HasHediff(HediffDefOf.ChjAndroidLike) && ThingDefOf.ChjAndroid.race.deathAction != null)
            {
                //Log.Message("Is Android");
                if (pawn.Corpse != null)
                {
                    //Log.Message("Pre: Death action worker");
                    ThingDefOf.ChjAndroid.race.DeathActionWorker.PawnDied(pawn.Corpse, pawn.GetLord());
                    //Log.Message("Post: Death action worker");
                }
            }
        }
    }
}

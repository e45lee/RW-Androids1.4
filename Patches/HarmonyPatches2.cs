using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Androids
{
    [HarmonyPatch(typeof(JobGiver_GetNeuralSupercharge))]
    [HarmonyPatch("GetPriority")]

    internal static class SuperchargePatch
    {
        public static void Prefix(Pawn pawn, ref JobGiver_GetNeuralSupercharge __instance, ref float __result)
        {
            if (pawn.RaceProps.FleshType.defName == "ChJDroid")
            {
                __result = 0;
            }
        }
    }
    [HarmonyPatch(typeof(MetalhorrorUtility))]
    [HarmonyPatch("CanBeInfected")]

    internal static class CanBeInfectedPatch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn.RaceProps.FleshType.defName == "ChJDroid")
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(DaysWorthOfFoodCalculator))] 
    [HarmonyPatch(nameof(DaysWorthOfFoodCalculator.ApproxDaysWorthOfFood))]
    [HarmonyPatch(new Type[] {
    typeof(List<Pawn>),
    typeof(List<ThingDefCount>),
    typeof(PlanetTile),
    typeof(IgnorePawnsInventoryMode),
    typeof(Faction),
    typeof(WorldPath),
    typeof(float),
    typeof(int),
    typeof(bool)
})]
    public static class ApproxDaysWorthOfFood_Patch
    {
        public static bool Prefix(
            List<Pawn> pawns,
            List<ThingDefCount> extraFood,
            PlanetTile tile,
            IgnorePawnsInventoryMode ignoreInventory,
            Faction faction,
            WorldPath path,
            float nextTileCostLeft,
            int caravanTicksPerMove,
            bool assumeCaravanMoving)
        {
            List<Pawn> modifiedPawnsList = new List<Pawn>(pawns);
            modifiedPawnsList.RemoveAll(pawn => pawn.def.HasModExtension<MechanicalPawnProperties>());

            pawns = modifiedPawnsList;
            // Pass-through: do nothing and allow original method to execute
            return true;
        }
    }

    [HarmonyPatch(typeof(SocialInteractionUtility))]
    [HarmonyPatch(nameof(SocialInteractionUtility.CanInitiateInteraction))]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(InteractionDef) })]
    public static class CanInitiateInteraction_Patch
    {
        public static bool Prefix(Pawn pawn, InteractionDef interactionDef, ref bool __result)
        {
            if (pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(SocialInteractionUtility))]
    [HarmonyPatch(nameof(SocialInteractionUtility.CanReceiveInteraction))]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(InteractionDef) })]
    public static class CanReceiveInteraction_Patch
    {
        public static bool Prefix(Pawn pawn, InteractionDef interactionDef, ref bool __result)
        {
            if (pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker))] 
    [HarmonyPatch(nameof(Pawn_InteractionsTracker.SocialFightPossible))]
    [HarmonyPatch(new[] { typeof(Pawn) })]
    public static class SocialFightPossible_Patch
    {
        public static bool Prefix(Pawn otherPawn, Pawn_InteractionsTracker __instance)
        {
            if (__instance.pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker))] 
    [HarmonyPatch(nameof(Pawn_InteractionsTracker.InteractionsTrackerTickInterval))]
    [HarmonyPatch(new[] { typeof(int) })]
    public static class InteractionsTrackerTickInterval_Patch
    {
        public static bool Prefix(int delta, Pawn_InteractionsTracker __instance)
        {
            if (__instance.pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker))] 
    [HarmonyPatch(nameof(Pawn_InteractionsTracker.CanInteractNowWith))]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(InteractionDef) })]
    public static class CanInteractNowWith_Patch
    {
        public static bool Prefix(Pawn recipient, InteractionDef interactionDef, Pawn_InteractionsTracker __instance)
        {
            if (__instance.pawn.def.GetModExtension<MechanicalPawnProperties>() is MechanicalPawnProperties properties && !properties.canSocialize)
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InspirationHandler))] // Replace with the actual class name
    [HarmonyPatch(nameof(InspirationHandler.InspirationHandlerTickInterval))]
    [HarmonyPatch(new[] { typeof(int) })]
    public static class InspirationHandlerTickInterval_Patch
    {
        public static bool Prefix(int delta, InspirationHandler __instance)
        {
            if (__instance.pawn.def.HasModExtension<MechanicalPawnProperties>())
            {
                return false;
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using RimWorld;
using Verse;

namespace FerrexBusiness
{
    public class StockGenerator_Employee : StockGenerator
    {
        [DebuggerHidden]
        public override IEnumerable<Thing> GenerateThings(int forTile)
        {

                int count = this.countRange.RandomInRange;
                for (int i = 0; i < count; i++)
                {
                Faction ferrexBizNet =  Find.FactionManager.FirstFactionOfDef(FRBFactionDefOf.FerrexBizNet);

                    PawnGenerationRequest request = new PawnGenerationRequest(FRBPawnKindDefOf.FRBEmployee, ferrexBizNet, PawnGenerationContext.NonPlayer, forTile, false, false, false, false, true, false, 1f, !this.trader.orbital, true, true, false, false, null, null, null, null, null, null);
                    yield return PawnGenerator.GeneratePawn(request);
                }
            
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike;
        }
    }
}


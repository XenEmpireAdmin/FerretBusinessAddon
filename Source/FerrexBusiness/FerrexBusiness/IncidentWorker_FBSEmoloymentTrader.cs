using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace FerrexBusiness
{
    class IncidentWorker_FRBEmploymentTrader : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (map.passingShipManager.passingShips.Count >= 5)
            {
                return false;
            }           
            {
                FRBShip ftradeShip = new FRBShip(FRBTraderDefOf.Orbital_FRBStaffer);
                if (map.listerBuildings.allBuildingsColonist.Any((Building b) => b.def.IsCommsConsole && b.GetComp<CompPowerTrader>().PowerOn))
                {
                    Find.LetterStack.ReceiveLetter(ftradeShip.def.LabelCap, "TraderArrival".Translate(new object[]
                    {
                        ftradeShip.name,
                        ftradeShip.def.label
                    }), LetterDefOf.Good, null);
                }
                map.passingShipManager.AddShip(ftradeShip);
                ftradeShip.GenerateThings();
                return true;
            }
            throw new InvalidOperationException();
        }
    }
}

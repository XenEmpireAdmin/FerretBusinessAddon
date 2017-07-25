using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace FerrexBusiness
{
    class JobDriver_UseFRB_ETerminal : JobDriver
    {
        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn(delegate (Toil to)
            {
                Building_FRB_ETerminal building_FRB_ETerminal = (Building_FRB_ETerminal)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing;
                return !building_FRB_ETerminal.CanUseCommsNow;
            });
            yield return new Toil
            {
                initAction = delegate
                {
                    Pawn actor = this.GetActor();
                    Building_FRB_ETerminal building_FRB_ETerminal = (Building_FRB_ETerminal)actor.jobs.curJob.GetTarget(TargetIndex.A).Thing;
                    if (building_FRB_ETerminal.CanUseCommsNow)
                    {
                        actor.jobs.curJob.commTarget.TryOpenComms(actor);
                    }
                }
            };
        }
    }
}



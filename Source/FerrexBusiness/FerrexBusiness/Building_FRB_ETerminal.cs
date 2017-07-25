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
    class Building_FRB_ETerminal : Building
    {
        private CompPowerTrader powerComp;

        public bool callflag;

        public bool CanUseCommsNow
        {
            get
            {
                return (!base.Spawned || !base.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare)) && this.powerComp.PowerOn;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = base.GetComp<CompPowerTrader>();
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.BuildOrbitalTradeBeacon, OpportunityType.GoodToKnow);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.OpeningComms, OpportunityType.GoodToKnow);
        }

        private void UseAct(Pawn myPawn, ICommunicable commTarget)
        {
            Job job = new Job(JobDefOf.UseFRBConsole, this);
            job.commTarget = commTarget;
            myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Some, false, TraverseMode.ByPawn))
            {
                FloatMenuOption item = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                return new List<FloatMenuOption>
                {
                    item
                };
            }
            if (base.Spawned && base.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare))
            {
                FloatMenuOption item2 = new FloatMenuOption("CannotUseSolarFlare".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                return new List<FloatMenuOption>
                {
                    item2
                };
            }
            if (!this.powerComp.PowerOn)
            {
                FloatMenuOption item3 = new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                return new List<FloatMenuOption>
                {
                    item3
                };
            }
            if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
            {
                FloatMenuOption item4 = new FloatMenuOption("CannotUseReason".Translate(new object[]
                {
                    "IncapableOfCapacity".Translate(new object[]
                    {
                        PawnCapacityDefOf.Talking.label
                    })
                }), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                return new List<FloatMenuOption>
                {
                    item4
                };
            }
            if (!this.CanUseCommsNow)
            {
                Log.Error(myPawn + " could not use comm console for unknown reason.");
                FloatMenuOption item5 = new FloatMenuOption("Cannot use now", null, MenuOptionPriority.Default, null, null, 0f, null, null);
                return new List<FloatMenuOption>
                {
                    item5
                };
            }
            if (callflag == false)
            {
                Action callup = delegate
                {
                    IncidentParms incidentParms = new IncidentParms();
                    incidentParms.target = Find.VisibleMap;
                    var instance = new IncidentWorker_FRBEmploymentTrader();
                    instance.TryExecute(incidentParms);
                    callflag = true;
                };
                FloatMenuOption item6 = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Request Eployee Roster", callup, MenuOptionPriority.InitiateSocial, null, null, 0f, null, null), myPawn, this, "ReservedBy");
                {
                    return new List<FloatMenuOption>
                    {
                        item6                   
                    };
                    
                }
            }
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            IEnumerable<ICommunicable> enumerable = myPawn.Map.passingShipManager.passingShips.Cast<ICommunicable>();
            foreach (ICommunicable commTarget in enumerable)
            {
                    ICommunicable localCommTarget = commTarget;
                    string text = "CallOnRadio".Translate(new object[]
                    {
                    localCommTarget.GetCallLabel()
                    });
                    Faction faction = localCommTarget as Faction;
                    if (faction != null)
                    {
                        if (faction.IsPlayer)
                        {
                            continue;
                        }
                        if (!Building_FRB_ETerminal.LeaderIsAvailableToTalk(faction))
                        {
                            string str;
                            if (faction.leader != null)
                            {
                                str = "LeaderUnavailable".Translate(new object[]
                                {
                                faction.leader.LabelShort
                                });
                            }
                            else
                            {
                                str = "LeaderUnavailableNoLeader".Translate();
                            }
                            list.Add(new FloatMenuOption(text + " (" + str + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                            continue;
                        }
                    }
                    Action action = delegate
                     {
                    if (commTarget is TradeShip && !Building_OrbitalTradeBeacon.AllPowered(this.Map).Any<Building_OrbitalTradeBeacon>())
                    {
                        Messages.Message("MessageNeedBeaconToTradeWithShip".Translate(), this, MessageSound.RejectInput);
                        return;
                    }
                    Job job = new Job(JobDefOf.UseFRBConsole, this);
                    job.commTarget = commTarget;
                    myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);
                };
                list.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Call FSB", action, MenuOptionPriority.InitiateSocial, null, null, 0f, null, null), myPawn, this, "ReservedBy"));
            }
            return list;
        }

        public static bool LeaderIsAvailableToTalk(Faction fac)
        {
            return fac.leader != null && (!fac.leader.Spawned || (!fac.leader.Downed && !fac.leader.IsPrisoner && fac.leader.Awake() && !fac.leader.InMentalState));
        }
    }
}
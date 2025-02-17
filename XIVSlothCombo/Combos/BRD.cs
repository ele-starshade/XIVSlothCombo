using System;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class BRD
    {
        public const byte ClassID = 5;
        public const byte JobID = 23;

        public const uint
            HeavyShot = 97,
            StraightShot = 98,
            VenomousBite = 100,
            RagingStrikes = 101,
            QuickNock = 106,
            Barrage = 107,
            Bloodletter = 110,
            Windbite = 113,
            MagesBallad = 114,
            ArmysPaeon = 116,
            RainOfDeath = 117,
            BattleVoice = 118,
            EmpyrealArrow = 3558,
            WanderersMinuet = 3559,
            IronJaws = 3560,
            Sidewinder = 3562,
            PitchPerfect = 7404,
            Troubadour = 7405,
            CausticBite = 7406,
            Stormbite = 7407,
            RefulgentArrow = 7409,
            BurstShot = 16495,
            ApexArrow = 16496,
            Shadowbite = 16494,
            Ladonsbite = 25783,
            BlastArrow = 25784,
            RadiantFinale = 25785;


        public static class Buffs
        {
            public const ushort
                StraightShotReady = 122,
                Troubadour = 1934,
                BlastArrowReady = 2692,
                ShadowbiteReady = 3002,
                WanderersMinuet = 865,
                MagesBallad = 135,
                ArmysPaeon = 137,
                RadiantFinale = 2722,
                BattleVoice = 141,
                Barrage = 128,
                RagingStrikes = 125;
        }

        public static class Debuffs
        {
            public const ushort
                VenomousBite = 124,
                Windbite = 129,
                CausticBite = 1200,
                Stormbite = 1201;
        }

        public static class Levels
        {
            public const byte
                StraightShot = 2,
                RagingStrikes = 4,
                VenomousBite = 6,
                Bloodletter = 12,
                Windbite = 30,
                MagesBallad = 30,
                ArmysPaeon = 40,
                RainOfDeath = 45,
                Barrage = 38,
                BattleVoice = 50,
                PitchPerfect = 52,
                EmpyrealArrow = 54,
                IronJaws = 56,
                WanderersMinuet = 52,
                Sidewinder = 60,
                Troubadour = 62,
                CausticBite = 64,
                StormBite = 64,
                BiteUpgrade = 64,
                RefulgentArrow = 70,
                Shadowbite = 72,
                BurstShot = 76,
                ApexArrow = 80,
                Ladonsbite = 82,
                BlastArrow = 86,
                RadiantFinale = 90;
        }

        public static class Config
        {
            public const string
                RagingJawsRenewTime = "ragingJawsRenewTime",
                NoWasteHPPercentage = "noWasteHpPercentage";
        }

        internal static bool SongIsNotNone(Song value)
        {
            return value != Song.NONE;
        }
        
        internal static bool SongIsNone(Song value)
        {
            return value == Song.NONE;
        }

        internal static bool SongIsWandererMinuet(Song value)
        {
            return value == Song.WANDERER;
        }
    }

    // Replace HS/BS with SS/RA when procced.
    internal class BardStraightShotUpgradeFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardStraightShotUpgradeFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            {
                if (IsEnabled(CustomComboPreset.BardApexFeature))
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                        return BRD.ApexArrow;
                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;
                }

                if (IsEnabled(CustomComboPreset.BardDoTMaintain))
                {
                    var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                    var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);
                    var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                    var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                    if (inCombat)
                    {
                        var useIronJaws = (
                            level >= BRD.Levels.IronJaws &&
                            ((venomous && venomousDuration.RemainingTime < 4) || (caustic && causticDuration.RemainingTime < 4)) ||
                            ((windbite && windbiteDuration.RemainingTime < 4) || (stormbite && stormbiteDuration.RemainingTime < 4))
                        );

                        if (useIronJaws)
                            return BRD.IronJaws;
                        if (level < BRD.Levels.IronJaws && venomous && venomousDuration.RemainingTime < 4)
                            return BRD.VenomousBite;
                        if (level < BRD.Levels.IronJaws && windbite && windbiteDuration.RemainingTime < 4)
                            return BRD.Windbite;
                    }

                }

                if (HasEffect(BRD.Buffs.StraightShotReady))
                {
                    return (level >= BRD.Levels.RefulgentArrow) ? BRD.RefulgentArrow : BRD.StraightShot;
                }
            }

            return actionID;
        }
    }

    internal class BardIronJawsFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardIronJawsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws)
            {
                if (IsEnabled(CustomComboPreset.BardIronJawsApexFeature) && level >= BRD.Levels.ApexArrow)
                {
                    var gauge = GetJobGauge<BRDGauge>();

                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady)) return BRD.BlastArrow;
                    if (gauge.SoulVoice == 100 && IsOffCooldown(BRD.ApexArrow)) return BRD.ApexArrow;
                }


                if (level < BRD.Levels.IronJaws)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous is not null && windbite is not null)
                    {
                        if (level >= BRD.Levels.VenomousBite && venomous.RemainingTime < windbite.RemainingTime)
                        {
                            return BRD.VenomousBite;
                        }

                        if (level >= BRD.Levels.Windbite)
                        {
                            return BRD.Windbite;
                        }
                    }

                    if (level >= BRD.Levels.VenomousBite && (level < BRD.Levels.Windbite || windbite is not null))
                    {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite)
                    {
                        return BRD.Windbite;
                    }
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (level >= BRD.Levels.IronJaws && venomous && windbite)
                    {
                        return BRD.IronJaws;
                    }

                    if (level >= BRD.Levels.VenomousBite && windbite)
                    {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite)
                    {
                        return BRD.Windbite;
                    }
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (level >= BRD.Levels.IronJaws && caustic && stormbite)
                {
                    return BRD.IronJaws;
                }

                if (level >= BRD.Levels.CausticBite && stormbite)
                {
                    return BRD.CausticBite;
                }

                if (level >= BRD.Levels.StormBite)
                {
                    return BRD.Stormbite;
                }
            }

            return actionID;
        }
    }
    internal class BardIronJawsAlternateFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardIronJawsAlternateFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws)
            {
                if (level < BRD.Levels.IronJaws)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous is not null && windbite is not null)
                    {
                        if (level >= BRD.Levels.VenomousBite && venomous.RemainingTime < windbite.RemainingTime)
                        {
                            return BRD.VenomousBite;
                        }

                        if (level >= BRD.Levels.Windbite)
                        {
                            return BRD.Windbite;
                        }
                    }

                    if (level >= BRD.Levels.VenomousBite && (level < BRD.Levels.Windbite || windbite is not null))
                    {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite)
                    {
                        return BRD.Windbite;
                    }
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (level >= BRD.Levels.IronJaws && venomous && windbite && (venomousDuration.RemainingTime < 4 || windbiteDuration.RemainingTime < 4))
                    {
                        return BRD.IronJaws;
                    }

                    if (level >= BRD.Levels.VenomousBite && windbite)
                    {
                        return BRD.VenomousBite;
                    }

                    if (level >= BRD.Levels.Windbite)
                    {
                        return BRD.Windbite;
                    }
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (level >= BRD.Levels.IronJaws && caustic && stormbite && (causticDuration.RemainingTime < 4 || stormbiteDuration.RemainingTime < 4))
                {
                    return BRD.IronJaws;
                }

                if (level >= BRD.Levels.CausticBite && stormbite)
                {
                    return BRD.CausticBite;
                }

                if (level >= BRD.Levels.StormBite)
                {
                    return BRD.Stormbite;
                }
            }

            return actionID;
        }
    }

    internal class BardApexFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardApexFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.QuickNock)
            {
                var gauge = GetJobGauge<BRDGauge>();

                if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                    return BRD.ApexArrow;
            }

            return actionID;
        }
    }

    internal class BardoGCDAoEFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardoGCDAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.RainOfDeath)
            {
                var gauge = GetJobGauge<BRDGauge>();

                if (level >= BRD.Levels.WanderersMinuet && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return OriginalHook(BRD.WanderersMinuet);
                if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow))
                    return BRD.EmpyrealArrow;
                if (level >= BRD.Levels.Bloodletter && IsOffCooldown(BRD.Bloodletter))
                    return BRD.RainOfDeath;
                if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder))
                    return BRD.Sidewinder;

            }

            return actionID;
        }
    }

    internal class BardSimpleAoEFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardSimpleAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Ladonsbite || actionID == BRD.QuickNock)
            {
                var gauge = GetJobGauge<BRDGauge>();
                var soulVoice = gauge.SoulVoice;
                var canWeave = CanWeave(actionID);

                if (IsEnabled(CustomComboPreset.SimpleAoESongOption) && canWeave)
                {
                    var songTimerInSeconds = gauge.SongTimer / 1000;

                    if (songTimerInSeconds < 3 || gauge.Song == Song.NONE)
                    {
                        if (level >= BRD.Levels.WanderersMinuet && 
                            IsOffCooldown(BRD.WanderersMinuet) && !(JustUsed(BRD.MagesBallad) || JustUsed(BRD.ArmysPaeon)) && !IsEnabled(CustomComboPreset.SimpleAoESongOptionExcludeWM))
                            return BRD.WanderersMinuet;
                        if (level >= BRD.Levels.MagesBallad &&
                            IsOffCooldown(BRD.MagesBallad) && !(JustUsed(BRD.WanderersMinuet) || JustUsed(BRD.ArmysPaeon)))
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.ArmysPaeon &&
                            IsOffCooldown(BRD.ArmysPaeon) && !(JustUsed(BRD.MagesBallad) || JustUsed(BRD.WanderersMinuet)))
                            return BRD.ArmysPaeon;
                    }
                }

                if (canWeave)
                {
                    if (level >= BRD.Levels.PitchPerfect && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                        return OriginalHook(BRD.WanderersMinuet);
                    if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow))
                        return BRD.EmpyrealArrow;
                    if (level >= BRD.Levels.RainOfDeath && GetRemainingCharges(BRD.RainOfDeath) > 0)
                        return BRD.RainOfDeath;
                    if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder))
                        return BRD.Sidewinder;
                }

                if (level >= BRD.Levels.Shadowbite && HasEffect(BRD.Buffs.ShadowbiteReady))
                    return BRD.Shadowbite;
                if (level >= BRD.Levels.ApexArrow && soulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                    return BRD.ApexArrow;
                if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                    return BRD.BlastArrow;

            }

            return actionID;
        }
    }

    internal class BardoGCDSingleTargetFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardoGCDSingleTargetFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Bloodletter)
            {
                var gauge = GetJobGauge<BRDGauge>();

                if (IsEnabled(CustomComboPreset.BardSongsFeature) && (gauge.SongTimer < 1 || gauge.Song == Song.ARMY))
                {
                    if (level >= BRD.Levels.WanderersMinuet && IsOffCooldown(BRD.WanderersMinuet))
                        return BRD.WanderersMinuet;
                    if (level >= BRD.Levels.MagesBallad && IsOffCooldown(BRD.MagesBallad))
                        return BRD.MagesBallad;
                    if (level >= BRD.Levels.ArmysPaeon && IsOffCooldown(BRD.ArmysPaeon))
                        return BRD.ArmysPaeon;
                }

                if (gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return OriginalHook(BRD.WanderersMinuet);
                if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow))
                    return BRD.EmpyrealArrow;
                if (level >= BRD.Levels.Bloodletter && IsOffCooldown(BRD.Bloodletter))
                    return BRD.Bloodletter;
                if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder))
                    return BRD.Sidewinder;
            }

            return actionID;
        }
    }
    internal class BardAoEComboFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardAoEComboFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
            {
                if (IsEnabled(CustomComboPreset.BardApexFeature))
                {
                    if (level >= BRD.Levels.ApexArrow && GetJobGauge<BRDGauge>().SoulVoice == 100 && !IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                        return BRD.ApexArrow;

                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;
                }

                if (IsEnabled(CustomComboPreset.BardAoEComboFeature) && level >= BRD.Levels.Shadowbite && HasEffectAny(BRD.Buffs.ShadowbiteReady))
                {
                    return BRD.Shadowbite;
                }
            }

            return actionID;
        }
    }
    internal class SimpleBardFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SimpleBardFeature;
        internal static bool inOpener = false;
        internal static bool openerFinished = false;
        internal static byte step = 0;
        internal static byte subStep = 0;

        internal static bool usedStraightShotReady = false;
        internal static bool usedPitchPerfect = false;

        internal delegate bool DotRecast(int value);

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            {
                var inCombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<BRDGauge>();

                var canWeave = CanWeave(actionID);
                var canWeaveBuffs = CanWeave(actionID, 0.6);
                var canWeaveDelayed = CanDelayedWeave(actionID);

                if (!inCombat && (inOpener || openerFinished))
                {
                    openerFinished = false;
                }

                if (IsEnabled(CustomComboPreset.BardSimpleInterrupt) && CanInterruptEnemy() && IsOffCooldown(All.HeadGraze))
                {
                    return All.HeadGraze;
                }
                
                var isEnemyHealthHigh = IsEnabled(CustomComboPreset.BardSimpleNoWasteMode) ?
                    CustomCombo.EnemyHealthPercentage() > Service.Configuration.GetCustomIntValue(BRD.Config.NoWasteHPPercentage) : true;

                if (IsEnabled(CustomComboPreset.SimpleSongOption) && canWeave && isEnemyHealthHigh)
                {
                    var songTimerInSeconds = gauge.SongTimer / 1000;

                    // Limit optimisation to only when you are high enough to benefit from it.
                    if (level >= BRD.Levels.WanderersMinuet)
                    {
                        // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army Peon    
                        var minuetOffCooldown = IsOffCooldown(BRD.WanderersMinuet);
                        var balladOffCooldown = IsOffCooldown(BRD.MagesBallad);
                        var paeonOffCooldown = IsOffCooldown(BRD.ArmysPaeon);

                        if (gauge.Song == Song.NONE)
                        {
                            // Do logic to determine first song

                            if (minuetOffCooldown && !(JustUsed(BRD.MagesBallad)        || JustUsed(BRD.ArmysPaeon)) )      return BRD.WanderersMinuet;
                            if (balladOffCooldown && !(JustUsed(BRD.WanderersMinuet)    || JustUsed(BRD.ArmysPaeon)) )      return BRD.MagesBallad;
                            if (paeonOffCooldown  && !(JustUsed(BRD.MagesBallad)        || JustUsed(BRD.WanderersMinuet)) ) return BRD.ArmysPaeon;
                        }

                        if (gauge.Song == Song.WANDERER)
                        {
                            // Spend any repertoire before switching to next song
                            if (songTimerInSeconds < 3 && gauge.Repertoire > 0)
                            {
                                return OriginalHook(BRD.WanderersMinuet);
                            }
                            // Move to Mage's Ballad if < 3 seconds left on song
                            if (songTimerInSeconds < 3 && balladOffCooldown)
                            {
                                return BRD.MagesBallad;
                            }
                        }

                        if (gauge.Song == Song.MAGE)
                        {
                            // Move to Army's Paeon if < 12 seconds left on song
                            if (songTimerInSeconds < 12 && paeonOffCooldown)
                            {
                                // Very special case for Empyreal, it needs to be cast before you change to it to avoid drift!!!
                                if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow)) 
                                    return BRD.EmpyrealArrow;

                                return BRD.ArmysPaeon;
                            }
                        }

                        if (gauge.Song == Song.ARMY)
                        {
                            // Move to Wanderer's Minuet if < 3 seconds left on song or WM off CD
                            if (songTimerInSeconds < 3 || minuetOffCooldown)
                            {
                                return BRD.WanderersMinuet;
                            }
                        }
                    }
                    else if ( songTimerInSeconds < 3 )
                    {
                        if (level >= BRD.Levels.MagesBallad && IsOffCooldown(BRD.MagesBallad))
                            return BRD.MagesBallad;
                        if (level >= BRD.Levels.ArmysPaeon && IsOffCooldown(BRD.ArmysPaeon))
                            return BRD.ArmysPaeon;
                    }
                }

                if (IsEnabled(CustomComboPreset.BardSimpleBuffsFeature)  && gauge.Song != Song.NONE && isEnemyHealthHigh)
                {
                    if (canWeaveDelayed && level >= BRD.Levels.RagingStrikes && IsOffCooldown(BRD.RagingStrikes) &&
                        (GetCooldown(BRD.BattleVoice).CooldownRemaining < 4.5 || IsOffCooldown(BRD.BattleVoice)))
                    {
                        return BRD.RagingStrikes;
                    }
                    if (IsEnabled(CustomComboPreset.BardSimpleBuffsRadiantFeature) && level >= BRD.Levels.RadiantFinale && canWeaveBuffs &&
                        IsOffCooldown(BRD.RadiantFinale) && (Array.TrueForAll(gauge.Coda, BRD.SongIsNotNone) || Array.Exists(gauge.Coda, BRD.SongIsWandererMinuet)) &&
                        (IsOffCooldown(BRD.BattleVoice) || GetCooldownRemainingTime(BRD.BattleVoice) < 0.5) && (GetBuffRemainingTime(BRD.Buffs.RagingStrikes) <= 16 || openerFinished ))
                    { 
                       if (!JustUsed(BRD.RagingStrikes)) return BRD.RadiantFinale; 
                    }

                    if (canWeaveBuffs && level >= BRD.Levels.BattleVoice && IsOffCooldown(BRD.BattleVoice) && (GetBuffRemainingTime(BRD.Buffs.RagingStrikes) <= 16 || openerFinished))
                    {
                        if (!JustUsed(BRD.RagingStrikes)) return BRD.BattleVoice; 
                    }
                    if (canWeaveBuffs && level >= BRD.Levels.Barrage && IsOffCooldown(BRD.Barrage) && !HasEffect(BRD.Buffs.StraightShotReady) && HasEffect(BRD.Buffs.RagingStrikes))
                    {
                        if (level >= BRD.Levels.RadiantFinale && HasEffect(BRD.Buffs.RadiantFinale))
                            return BRD.Barrage;
                        else if (level >= BRD.Levels.BattleVoice && HasEffect(BRD.Buffs.BattleVoice))
                            return BRD.Barrage;
                        else if (level < BRD.Levels.BattleVoice && HasEffect(BRD.Buffs.RagingStrikes))
                            return BRD.Barrage;
                    }
                }

                
                if (canWeave)
                {
                    var bvProtection = IsOffCooldown(BRD.BattleVoice) || GetCooldownRemainingTime(BRD.BattleVoice) > 2.5;

                    if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow) && bvProtection)
                            
                        return BRD.EmpyrealArrow;

                    if (level >= BRD.Levels.PitchPerfect && gauge.Song == Song.WANDERER && 
                        (gauge.Repertoire == 3 || (gauge.Repertoire == 2 && GetCooldown(BRD.EmpyrealArrow).CooldownRemaining < 2)) && bvProtection)
                            
                        return OriginalHook(BRD.WanderersMinuet);
                            
                    if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder) && bvProtection)
                        if (IsEnabled(CustomComboPreset.BardSimplePooling))
                        {
                            if (gauge.Song == Song.WANDERER)
                            {
                                if (
                                    (HasEffect(BRD.Buffs.RagingStrikes) || GetCooldown(BRD.RagingStrikes).CooldownRemaining > 10) &&
                                    (HasEffect(BRD.Buffs.BattleVoice) || GetCooldown(BRD.BattleVoice).CooldownRemaining > 10) &&
                                    (
                                        HasEffect(BRD.Buffs.RadiantFinale) || GetCooldown(BRD.RadiantFinale).CooldownRemaining > 10 ||
                                        level < BRD.Levels.RadiantFinale
                                    )
                                    )
                                {
                                    return BRD.Sidewinder;
                                }
                            }
                            else return BRD.Sidewinder;
                        } else return BRD.Sidewinder;
                    if (level >= BRD.Levels.Bloodletter)
                    {
                        var bloodletterCharges = GetRemainingCharges(BRD.Bloodletter);

                        if (IsEnabled(CustomComboPreset.BardSimplePooling) && level >= BRD.Levels.WanderersMinuet)
                        {
                            if (gauge.Song == Song.WANDERER)
                            {
                                if (
                                    ((HasEffect(BRD.Buffs.RagingStrikes) || GetCooldown(BRD.RagingStrikes).CooldownRemaining > 10) &&
                                    (
                                        HasEffect(BRD.Buffs.BattleVoice)   || GetCooldown(BRD.BattleVoice).CooldownRemaining > 10 ||
                                        level < BRD.Levels.BattleVoice
                                    ) && 
                                    (
                                        HasEffect(BRD.Buffs.RadiantFinale) || GetCooldown(BRD.RadiantFinale).CooldownRemaining > 10 ||
                                        level < BRD.Levels.RadiantFinale
                                    ) &&
                                    bloodletterCharges > 0) ||
                                    (bloodletterCharges > 2 && bvProtection)
                                )
                                {
                                    return BRD.Bloodletter;
                                }
                            }
                            if (gauge.Song == Song.ARMY && (bloodletterCharges == 3 || ((gauge.SongTimer / 1000) > 30 && bloodletterCharges > 0))) return BRD.Bloodletter;
                            if (gauge.Song == Song.MAGE && bloodletterCharges > 0) return BRD.Bloodletter;
                            if (gauge.Song == Song.NONE && bloodletterCharges == 3) return BRD.Bloodletter;
                        }
                        else if (bloodletterCharges > 0)
                        {
                            return BRD.Bloodletter;
                        }
                    }
                }
                

                if (isEnemyHealthHigh)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                    var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);

                    var venomousDuration = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbiteDuration = FindTargetEffect(BRD.Debuffs.Windbite);
                    var causticDuration = FindTargetEffect(BRD.Debuffs.CausticBite);
                    var stormbiteDuration = FindTargetEffect(BRD.Debuffs.Stormbite);

                    var ragingStrikesDuration = FindEffect(BRD.Buffs.RagingStrikes);

                    var ragingJawsRenewTime = Service.Configuration.GetCustomIntValue(BRD.Config.RagingJawsRenewTime);

                    DotRecast poisonRecast = delegate (int duration)
                    {
                        return (venomous && venomousDuration.RemainingTime < duration) || (caustic && causticDuration.RemainingTime < duration);
                    };
                    DotRecast windRecast = delegate (int duration)
                    {
                        return (windbite && windbiteDuration.RemainingTime < duration) || (stormbite && stormbiteDuration.RemainingTime < duration);
                    };

                    var useIronJaws = (
                        (level >= BRD.Levels.IronJaws && poisonRecast(4)) ||
                        (level >= BRD.Levels.IronJaws && windRecast(4)) ||
                        (level >= BRD.Levels.IronJaws && IsEnabled(CustomComboPreset.BardSimpleRagingJaws) &&
                            HasEffect(BRD.Buffs.RagingStrikes) && ragingStrikesDuration.RemainingTime < ragingJawsRenewTime &&
                            poisonRecast(40) && windRecast(40))
                    );

                    var dotOpener = (IsEnabled(CustomComboPreset.BardSimpleDotOpener) && !openerFinished || !IsEnabled(CustomComboPreset.BardSimpleDotOpener));

                    if (level < BRD.Levels.BiteUpgrade)
                    {
                        if (useIronJaws)
                        {
                            openerFinished = true;
                            return BRD.IronJaws;
                        }

                        if (level < BRD.Levels.IronJaws)
                        {
                            if (windbite && windbiteDuration.RemainingTime < 4)
                            {
                                openerFinished = true;
                                return BRD.Windbite;
                            }
                            if (venomous && venomousDuration.RemainingTime < 4)
                            {
                                openerFinished = true;
                                return BRD.VenomousBite;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.SimpleDoTOption))
                        {
                            if (level >= BRD.Levels.Windbite && !windbite && dotOpener )
                                return BRD.Windbite;
                            if (level >= BRD.Levels.VenomousBite && !venomous && dotOpener)
                                return BRD.VenomousBite;
                        }
                    }
                    else { 

                        if (useIronJaws)
                        {
                            openerFinished = true;
                            return BRD.IronJaws;
                        }

                        if (IsEnabled(CustomComboPreset.SimpleDoTOption))
                        {
                            if (level >= BRD.Levels.StormBite && !stormbite && dotOpener )
                                return BRD.Stormbite;
                            if (level >= BRD.Levels.CausticBite && !caustic && dotOpener )
                                return BRD.CausticBite;
                                
                        }
                    }
                }

                if (!IsEnabled(CustomComboPreset.BardRemoveApexArrowFeature))
                {
                    if (level >= BRD.Levels.BlastArrow && HasEffect(BRD.Buffs.BlastArrowReady))
                        return BRD.BlastArrow;
                    if (level >= BRD.Levels.ApexArrow)
                    {
                        var songTimerInSeconds = gauge.SongTimer / 1000;

                        if (gauge.Song == Song.MAGE && gauge.SoulVoice == 100) return BRD.ApexArrow;
                        if (gauge.Song == Song.MAGE && gauge.SoulVoice >= 80 && songTimerInSeconds > 18 && songTimerInSeconds < 22) return BRD.ApexArrow;
                        if (gauge.Song == Song.WANDERER && HasEffect(BRD.Buffs.RagingStrikes) && HasEffect(BRD.Buffs.BattleVoice) && 
                            (HasEffect(BRD.Buffs.RadiantFinale) || level < BRD.Levels.RadiantFinale) && gauge.SoulVoice >= 80) return BRD.ApexArrow;
                    }
                }

                if (HasEffect(BRD.Buffs.StraightShotReady))
                {
                    return (level >= BRD.Levels.RefulgentArrow) ? BRD.RefulgentArrow : BRD.StraightShot;
                }

            }

            return actionID;
        }
    }
    internal class BardBuffsFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardBuffsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Barrage)
            {
                if (level >= BRD.Levels.RagingStrikes && IsOffCooldown(BRD.RagingStrikes))
                    return BRD.RagingStrikes;
                if (level >= BRD.Levels.BattleVoice && IsOffCooldown(BRD.BattleVoice))
                    return BRD.BattleVoice;
            }

            return actionID;
        }
    }
    internal class BardOneButtonSongs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardOneButtonSongs;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.WanderersMinuet)
            { // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.
                if (level >= BRD.Levels.WanderersMinuet && IsOffCooldown(BRD.WanderersMinuet))
                    return BRD.WanderersMinuet;
                if (level >= BRD.Levels.MagesBallad && IsOffCooldown(BRD.MagesBallad))
                    return BRD.MagesBallad;
                if (level >= BRD.Levels.ArmysPaeon && IsOffCooldown(BRD.ArmysPaeon))
                    return BRD.ArmysPaeon;
            }

            return actionID;
        }
    }
}

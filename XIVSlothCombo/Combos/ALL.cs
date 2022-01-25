﻿using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
namespace XIVSlothComboPlugin.Combos
{
    internal static class All
    {
        public const byte JobID = 99;

        public const uint
            Swiftcast = 7561,
            Resurrection = 173,
            Verraise = 7523,
            Raise = 125,
            Ascend = 3603,
            Egeiro = 24287,
            SolidReason = 232,
            AgelessWords = 215,
            WiseToTheWorldMIN = 26521,
            WiseToTheWorldBTN = 26522,
            LowBlow = 7540,
            Interject = 7538;

        public static class Buffs
        {
            public const short
                Swiftcast = 167;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Raise = 12;
        }
    }
    internal class InterruptFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.InterruptFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == All.LowBlow)
            {
                var interjectCD = GetCooldown(All.Interject);
                var lowBlowCD = GetCooldown(All.LowBlow);
                if (CanInterruptEnemy() && !interjectCD.IsCooldown)
                    return All.Interject;
            }

            return actionID;
        }
    }

    internal class DoMSwiftcastFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DoMSwiftcastFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.DoMSwiftcastFeature))
            {
                if (actionID == WHM.Raise || actionID == SMN.Resurrection || actionID == SGE.Egeiro || actionID == AST.Ascend || actionID == RDM.Verraise)
                {
                    var swiftCD = GetCooldown(All.Swiftcast);
                    if ((swiftCD.CooldownRemaining == 0 && !HasEffect(RDM.Buffs.Dualcast))
                        || level <= All.Levels.Raise
                        || (level <= RDM.Levels.Verraise && actionID == RDM.Verraise))
                        return All.Swiftcast;
                }
            }

            return actionID;
        }
    }
}

using VsMaker2Core.DataModels;

namespace VsMaker2Core
{
    public static class TrainerPropertyFlags
    {
        public const string DoubleBattle = "Double Battle";
        public const string ChooseMoves = "Choose Moves";
        public const string ChooseItems = "Choose Held Item";
        public const string ChooseAbility_Hge = "Choose Ability";
        public const string ChooseBall_Hge = "Choose Ball";
        public const string SetIvEv_Hge = "Set IV/EV";
        public const string ChooseNature_Hge = "Choose Nature";
        public const string ShinyLock_Hge = "Shiny Lock";
        public const string AdditionalFlags_Hge = "Additional Flags";

        public static List<string> TrainerPropertyFlagNames =>
            RomFile.IsHgEngine ?
            [
                DoubleBattle,
                ChooseMoves,
                ChooseItems,
                ChooseAbility_Hge,
                ChooseBall_Hge,
                SetIvEv_Hge,
                ChooseNature_Hge,
                ShinyLock_Hge,
                AdditionalFlags_Hge,
            ]
            :
            [
                DoubleBattle,
                ChooseMoves,
                ChooseItems,
            ];
    }
}
using VsMaker2Core.DataModels;

namespace Main.Models
{
    public class MainDataModel
    {
        // General Properties
        public List<string> TrainerNames { get; set; } = [];
        public List<string> ClassNames { get; set; } = [];
        public List<string> ClassDescriptions { get; set; } = [];
        public List<string> PokemonNamesFull { get; set; } = [];
        public List<string> MoveNames { get; set; } = [];
        public List<string> AbilityNames { get; set; } = [];
        public List<string> ItemNames { get; set; } = [];
        public List<Species> PokemonSpecies { get; set; } = [];
        public List<Trainer> Trainers { get; set; } = [];
        public List<TrainerClass> Classes { get; set; } = [];
        public List<BattleMessage> BattleMessages { get; set; } = [];
        public List<string> PokemonNames { get; set; } = [];

        public static List<string> SetPokemonNames(List<string> pokemonNamesFull) =>
            pokemonNamesFull
                .Where((_, index) => index < 494 || (index > 543 && index < 1076))
                .Select((name, i) => $"[{i:D4}] {name}")
                .ToList();

        #region HG Engine Only

        public List<string> PokeBallNames { get; set; } = [];
        public List<string> TypeNames { get; set; } = [];
        public List<string> NatureNames { get; set; } = [];
        public List<string> StatusNames { get; set; } = [];

        #endregion

        #region TrainerEditor

        public Trainer SelectedTrainer { get; set; } = new Trainer();
        public Trainer ClipboardTrainer { get; set; } = new Trainer();
        public TrainerProperty ClipboardTrainerProperties { get; set; } = new TrainerProperty();
        public TrainerParty ClipboardTrainerParty { get; set; } = new TrainerParty();

        #endregion

        #region ClassEditor

        public TrainerClass SelectedTrainerClass { get; set; } = new TrainerClass();
        public TrainerClass ClipboardTrainerClass { get; set; } = new TrainerClass();
        public TrainerClassProperty ClipboardClassTrainerProperties { get; set; } = new TrainerClassProperty();

        #endregion

        #region BattleEditor

        public int BattleMessageDisplayIndex { get; set; } = 0;
        public List<string> DisplayBattleMessageText { get; set; } = [];
        public int SelectedBattleMessageRowIndex { get; set; } = -1;

        #endregion
    }
}

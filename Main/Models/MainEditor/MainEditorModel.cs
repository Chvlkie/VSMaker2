using VsMaker2Core.DataModels;

namespace Main.Models
{
    public class MainEditorModel
    {
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

        public static List<string> SetPokemonNames(List<string> pokemonNamesFull) => pokemonNamesFull
                .Where((name, index) => index < 494 || (index > 543 && index < 1076))
                .Select((name, i) => $"[{i:D4}] {name}")
                .ToList();


        #region TrainerEditor

        public Trainer SelectedTrainer { get; set; } = new();
        public Trainer? ClipboardTrainer { get; set; } = new();
        public TrainerProperty? ClipboardTrainerProperties { get; set; } = new();
        public TrainerParty? ClipboardTrainerParty { get; set; } = new();

        #endregion TrainerEditor

        #region ClassEditor

        public TrainerClass SelectedTrainerClass { get; set; } = new();
        public TrainerClass? ClipboardTrainerClass { get; set; } = new();
        public TrainerClassProperty? ClipboardClassTrainerProperties { get; set; } = new();

        #endregion ClassEditor

        #region BattleEditor

        public int BattleMessageDisplayIndex { get; set; } = new();
        public List<string> DisplayBattleMessageText { get; set; } = [];

        public int SelectedBattleMessageRowIndex { get; set; } = -1;

        #endregion BattleEditor

        // Constructor
        public MainEditorModel()
        {
            TrainerNames = [];
            ClassNames = [];
            ClassDescriptions = [];
            PokemonNamesFull = [];
            MoveNames = [];
            AbilityNames = [];
            ItemNames = [];
            PokemonSpecies = [];
            Trainers = [];
            Classes = [];
            BattleMessages = [];
        }
    }
}
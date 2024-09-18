using VsMaker2Core.DataModels;

namespace Main.Models
{
    public class MainEditorModel
    {
        public List<string> TrainerNames { get; set; }
        public List<string> ClassNames { get; set; }
        public List<string> ClassDescriptions { get; set; }
        public List<string> PokemonNamesFull { get; set; }
        public List<string> MoveNames { get; set; }
        public List<string> AbilityNames { get; set; }
        public List<string> ItemNames { get; set; }
        public List<Species> PokemonSpecies { get; set; }
        public List<Trainer> Trainers { get; set; }
        public List<TrainerClass> Classes { get; set; }
        public List<BattleMessage> BattleMessages { get; set; }
        public List<string> PokemonNames { get; set; }

        // Method to get formatted Pokémon names with improved loop
        public static List<string> SetPokemonNames(List<string> pokemonNamesFull)
        {
            var names = new List<string>(pokemonNamesFull.Count + 1) { "----------" };
            for (int i = 1; i < pokemonNamesFull.Count; i++)
            {
                names.Add($"[{i:D4}] {pokemonNamesFull[i]}");
            }
            return names;
        }

        #region TrainerEditor

        public Trainer SelectedTrainer { get; set; }
        public Trainer? ClipboardTrainer { get; set; }
        public TrainerProperty? ClipboardTrainerProperties { get; set; }
        public TrainerParty? ClipboardTrainerParty { get; set; }

        #endregion TrainerEditor

        #region ClassEditor

        public TrainerClass SelectedClass { get; set; }
        public TrainerClass? ClipboardTrainerClass { get; set; }
        public TrainerClassProperty? ClipboardClassTrainerProperties { get; set; }

        #endregion ClassEditor

        #region BattleEditor

        public int BattleMessageDisplayIndex { get; set; }
        public List<string> DisplayBattleMessageText { get; set; } = new List<string>();

        public int SelectedBattleMessageRowIndex { get; set; } = -1;

        #endregion BattleEditor

        // Constructor
        public MainEditorModel()
        {
            TrainerNames = new List<string>();
            ClassNames = new List<string>();
            ClassDescriptions = new List<string>();
            PokemonNamesFull = new List<string>();
            MoveNames = new List<string>();
            AbilityNames = new List<string>();
            ItemNames = new List<string>();
            PokemonSpecies = new List<Species>();
            Trainers = new List<Trainer>();
            Classes = new List<TrainerClass>();
            BattleMessages = new List<BattleMessage>();
        }
    }
}

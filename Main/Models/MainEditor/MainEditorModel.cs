using VsMaker2Core.DataModels;

namespace Main.Models
{
    public class MainEditorModel
    {
        public List<string> TrainerNames { get; set; }
        public List<string> ClassNames { get; set; }
        public List<string> ClassDescriptions { get; set; }
        public List<string> PokemonNames { get; set; }
        public List<string> MoveNames { get; set; }
        public List<string> AbilityNames { get; set; }
        public List<string> ItemNames { get; set; }
        public List<Species> PokemonSpecies { get; set; }
        public List<Trainer> Trainers { get; set; }
        public List<TrainerClass> Classes { get; set; }
        public List<BattleMessage> BattleMessages { get; set; }

        public int BattleMessageDisplayIndex { get; set; }
        public List<string> DisplayBattleMessageText { get; set; }

        public int SelectedBattleMessageRowIndex { get; set; }

        public MainEditorModel()
        {
            SelectedBattleMessageRowIndex = -1;
            BattleMessageDisplayIndex = 0;
            DisplayBattleMessageText = [];
            TrainerNames = [];
            ClassNames = [];
            ClassDescriptions = [];
            PokemonNames = [];
            MoveNames = [];
            ItemNames = [];
            PokemonSpecies = [];
            Trainers = [];
            Classes = [];
            BattleMessages = [];
        }
    }
}
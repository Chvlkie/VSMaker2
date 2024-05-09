﻿using VsMaker2Core.DataModels;

namespace Main.Models
{
    public class MainEditorModel
    {
        public List<string> TrainerNames { get; set; }
        public List<string> ClassNames { get; set; }
        public List<string> PokemonNames { get; set; }
        public List<string> MoveNames { get; set; }
        public List<string> AbilityNames { get; set; }
        public List<string> ItemNames { get; set; }
        public List<Species> PokemonSpecies { get; set; }
        public List<Trainer> Trainers { get; set; }
        public List<TrainerClass> Classes { get; set; }
        public List<BattleMessage> BattleMessages { get; set; }

        public MainEditorModel()
        {
            TrainerNames = [];
            ClassNames = [];
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
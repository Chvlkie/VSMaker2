﻿using VsMaker2Core.DataModels;

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

        #region TrainerEditor

        public Trainer SelectedTrainer { get; set; }
        public Trainer? ClipboardTrainer { get; set; }
        public TrainerProperty? ClipboardTrainerProperties { get; set; }
        public TrainerParty? ClipboardTrainerParty { get; set; }

        #endregion TrainerEditor


        #region ClassEditor

        public TrainerClass SelectedClass{ get; set; }
        public TrainerClass? ClipboardTrainerClass { get; set; }
        public TrainerClassProperty? ClipboardClassTrainerProperties { get; set; }

        #endregion TrainerEditor

        #region BattleEditor

        public int BattleMessageDisplayIndex { get; set; }
        public List<string> DisplayBattleMessageText { get; set; }

        public int SelectedBattleMessageRowIndex { get; set; }

        #endregion BattleEditor

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
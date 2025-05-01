using Data.DataModels.Items;
using Data.DataModels.Moves;
using Data.DataModels.Pokemons;
using Data.DataModels.Rom;
using Data.DataModels.Trainers;
using Data.DataModels.TrainerTexts;
using Data.DataModels.TrainerTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataModels.Main
{
    public class MainData
    {
        public List<string> ItemNames => Items.ConvertAll(x => x.ItenName);
        public List<Item> Items { get; set; } = [];
        public List<string> MoveNames => Moves.ConvertAll(x => x.MoveName);
        public List<Move> Moves { get; set; } = [];
        public List<string> PokemonNames => Pokemons.ConvertAll(x => x.PokemonName);
        public List<Pokemon> Pokemons { get; set; } = [];
        public RomData RomData { get; set; } = new RomData();
        public FileData FileData { get; set; } = new FileData();
        public List<Species> Species { get; set; } = [];
        public List<TextOffset> TextOffsets { get; set; } = [];
        public List<string> TrainerNames => Trainers.ConvertAll(x => x.TrainerName);
        public List<Trainer> Trainers { get; set; } = [];
        public List<TrainerText> TrainerTexts { get; set; } = [];
        public List<string> TrainerTypeNames => TrainerTypes.ConvertAll(x => x.TrainerTypeName);
        public List<TrainerType> TrainerTypes { get; set; } = [];

        #region Trainer Editor

        public Trainer? SelectedTrainer { get; set; }

        #region Clipboards

        public PartyData? ClipboardPartyData { get; set; }
        public PartyPokemon? ClipboardPartyPokemon { get; set; }
        public Trainer? ClipboardTrainer { get; set; }
        public TrainerData ClipboardTrainerData { get; set; }

        #endregion Clipboards

        #endregion Trainer Editor

        #region Trainer Types

        public TrainerType? SelectedTrainerType { get; set; }

        #region Clipboards

        public TrainerType? ClipboardTrainerTyoe { get; set; }

        #endregion Clipboards

        #endregion Trainer Types
    }
}
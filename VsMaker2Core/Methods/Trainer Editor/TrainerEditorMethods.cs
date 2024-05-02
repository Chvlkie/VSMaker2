using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using VsMaker2Core.Database;
using VsMaker2Core.DataModels;
using VsMaker2Core.DataModels.Trainers;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class TrainerEditorMethods : ITrainerEditorMethods
    {
        private IRomFileMethods romFileMethods;

        public TrainerEditorMethods()
        {
            romFileMethods = new RomFileMethods();
        }

        public List<Trainer> GetTrainers(RomFile loadedRom)
        {
            var trainerNames = romFileMethods.GetTrainerNames(loadedRom.TrainerNamesTextNumber);
            List<Trainer> trainers = [];
            // Start from i 1 to skip player trainer file
            for (int i = 1; i < trainerNames.Count; i++)
            {
                var trainerData = loadedRom.TrainersData[i];
                var trainerPartyData = loadedRom.TrainersPartyData[i];
                trainers.Add(BuildTrainerData(i, trainerNames[i], trainerData, trainerPartyData, loadedRom.GameFamily != GameFamily.DiamondPearl));
            }
            return trainers;
        }

        public TrainerParty BuildTrainerPartyFromRomData(TrainerPartyData trainerPartyData, int teamSize, bool hasItems, bool chooseMoves, bool hasBallCapsule)
        {
            var trainerParty = new TrainerParty();
            for (int i = 0; i < teamSize; i++)
            {
                var trainerPartyPokemon = trainerPartyData.PokemonData[i];
                var pokemon = new Pokemon
                {
                    DifficultyValue = trainerPartyPokemon.Difficulty,
                    GenderAbilityOverride = trainerPartyPokemon.GenderAbilityOverride,
                    Level = trainerPartyPokemon.Level,
                    PokemonId = (ushort)(trainerPartyPokemon.Species & Pokemon.Constants.PokemonNumberBitMask),
                    FormId = (ushort)((trainerPartyPokemon.Species & Pokemon.Constants.PokemonFormBitMask) >> Pokemon.Constants.PokemonNumberBitSize),
                    HeldItemId = hasItems ? trainerPartyPokemon.ItemId : null,
                    Moves = chooseMoves ? trainerPartyPokemon.MoveIds : null,
                    BallSealId = hasBallCapsule ? trainerPartyPokemon.BallCapsule : null
                };
                trainerParty.Pokemons.Add(pokemon);
            }
            return trainerParty;
        }

        public TrainerProperty BuildTrainerPropertyFromRomData(TrainerData trainerData)
        {
            var trainerProperty = new TrainerProperty()
            {
                Items = new ushort[4],
                ChooseMoves = (trainerData.TrainerType & 1) != 0,
                ChooseItems = (trainerData.TrainerType & 2) != 0,
                TrainerClassId = trainerData.TrainerClassId,
                TeamSize = trainerData.TeamSize,
                DoubleBattle = trainerData.IsDoubleBattle == 2,
            };

            var aiFlags = new BitArray(BitConverter.GetBytes(trainerData.AIFlags));
            for (int i = 0; i < Trainer.Constants.NumberOfTrainerAIFlags; i++)
            {
                trainerProperty.AIFlags.Add(aiFlags[i]);
            }

            return trainerProperty;
        }

        public Trainer BuildTrainerData(int trainerId, string trainerName, TrainerData trainerData, TrainerPartyData trainerPartyData, bool hasBallCapsule)
        {
            var trainerProperties = BuildTrainerPropertyFromRomData(trainerData);
            var trainerParty = BuildTrainerPartyFromRomData(trainerPartyData, trainerProperties.TeamSize, trainerProperties.ChooseItems, trainerProperties.ChooseMoves, hasBallCapsule);

            return new Trainer
            {
                TrainerId = (ushort)trainerId,
                TrainerName = trainerName,
                TrainerParty = trainerParty,
                TrainerProperties = trainerProperties,
            };
        }

        public Trainer GetTrainer(List<Trainer> trainers, int trainerId)
        {
            return trainers.SingleOrDefault(x => x.TrainerId == trainerId);
        }
    }
}
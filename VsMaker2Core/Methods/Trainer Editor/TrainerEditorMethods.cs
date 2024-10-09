using System.Collections;
using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class TrainerEditorMethods : ITrainerEditorMethods
    {
        private readonly IRomFileMethods romFileMethods;

        public TrainerEditorMethods(IRomFileMethods romFileMethods)
        {
            if (romFileMethods is not null)
            {
                romFileMethods = new RomFileMethods();
                this.romFileMethods = romFileMethods;
            }
            else
            {
                throw new ArgumentNullException(nameof(romFileMethods));
            }
        }

        public List<Trainer> GetTrainers(List<string> trainerNames)
        {
            List<Trainer> trainers = new(trainerNames.Count - 1);

            var trainersData = RomFile.TrainersData;
            var trainersPartyData = RomFile.TrainersPartyData;
            bool isNotDiamondPearl = RomFile.IsNotDiamondPearl;

            // Skip the first trainer file (i = 1) as intended
            for (int i = 1; i < trainerNames.Count; i++)
            {
                var trainerData = trainersData[i];
                var trainerPartyData = trainersPartyData[i];

                trainers.Add(BuildTrainerData(i, trainerNames[i], trainerData, trainerPartyData, isNotDiamondPearl));
            }

            return trainers;
        }

        public TrainerParty BuildTrainerPartyFromRomData(TrainerPartyData trainerPartyData, int teamSize, bool hasItems, bool chooseMoves, bool hasBallCapsule)
        {
            var trainerParty = new TrainerParty();

            // If the team size is zero, add six dummy Pokémon and return early
            if (teamSize == 0)
            {
                trainerParty.Pokemons.AddRange(Enumerable.Repeat(new Pokemon(), 6));
                return trainerParty;
            }

            // Process valid Pokémon in the team
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
                    BallCapsuleId = hasBallCapsule ? trainerPartyPokemon.BallCapsule : null
                };

                trainerParty.Pokemons.Add(pokemon);
            }

            // Add dummy Pokémon to fill the remaining slots if the team size is less than 6
            if (teamSize < 6)
            {
                trainerParty.Pokemons.AddRange(Enumerable.Repeat(new Pokemon(), 6 - teamSize));
            }

            return trainerParty;
        }

        public TrainerProperty BuildTrainerPropertyFromRomData(TrainerData trainerData)
        {
            var trainerProperty = new TrainerProperty()
            {
                Items = trainerData.Items,
                ChooseMoves = (trainerData.TrainerType & 1) != 0,   // Checks if the first bit is set
                ChooseItems = (trainerData.TrainerType & 2) != 0,   // Checks if the second bit is set
                TrainerClassId = trainerData.TrainerClassId,
                TeamSize = trainerData.TeamSize,
                DoubleBattle = trainerData.IsDoubleBattle == 2
            };

            var aiFlags = new BitArray(BitConverter.GetBytes(trainerData.AIFlags));

            trainerProperty.AIFlags = [];
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
            var usages = FindTrainerUses(trainerId);

            return new Trainer((ushort)trainerId, trainerName, trainerProperties, trainerParty, usages);
        }

        private static List<TrainerUsage> FindTrainerUses(int trainerId)
        {
            List<TrainerUsage> trainerUsages = [];

            var scriptFiles = RomFile.ScriptFileData.Where(x => !x.IsLevelScript);

            foreach (var scriptFile in scriptFiles)
            {
                foreach (var script in scriptFile.Scripts.Concat(scriptFile.Functions).Where(s => !s.UsedScriptId.HasValue))
                {
                    foreach (var line in script.Lines.Where(x => IsTrainerCommand(x, trainerId)))
                    {
                        var usageType = scriptFile.Scripts.Contains(script) ? TrainerUsageType.Script : TrainerUsageType.Function;
                        trainerUsages.Add(new TrainerUsage(trainerId, scriptFile.ScriptFileId, (int)script.ScriptNumber, usageType));
                    }
                }
            }

            foreach (var eventFile in RomFile.EventFileData.Where(e => e.Overworlds.Any(ow => ow.IsTrainer)))
            {
                foreach (var ow in eventFile.Overworlds.Where(ow => ow.TrainerId == trainerId))
                {
                    trainerUsages.Add(new TrainerUsage(trainerId, eventFile.EventFileId, ow.OverworldId, TrainerUsageType.Event));
                }
            }

            return trainerUsages;
        }

        private static bool IsTrainerCommand(ScriptLine line, int trainerId) => (line.ScriptCommandId == 0x00E5 || line.ScriptCommandId == 0x00D5)
                && line.Parameters[0].Length >= 2
                && BitConverter.ToUInt16(line.Parameters[0], 0) == trainerId;

        public Trainer GetTrainer(List<Trainer> trainers, int trainerId) => trainers.SingleOrDefault(x => x.TrainerId == trainerId);

        public TrainerProperty NewTrainerProperties(byte teamSize, bool chooseMoves, bool chooseItems, bool isDouble, byte trainerClassId, ushort item1, ushort item2, ushort item3, ushort item4, List<bool> aiFlags) => new TrainerProperty
        {
            DoubleBattle = isDouble,
            ChooseItems = chooseItems,
            ChooseMoves = chooseMoves,
            TeamSize = teamSize,
            Items = [item1, item2, item3, item4],
            TrainerClassId = trainerClassId,
            AIFlags = aiFlags
        };

        public TrainerData NewTrainerData(TrainerProperty trainerProperties)
        {
            byte trainerType = (byte)((trainerProperties.ChooseMoves ? 1 : 0) | (trainerProperties.ChooseItems ? 2 : 0));

            uint aiFlags = 0;
            for (int i = 0; i < trainerProperties.AIFlags.Count; i++)
            {
                if (trainerProperties.AIFlags[i])
                {
                    aiFlags |= (uint)1 << i;
                }
            }

            return new TrainerData(
                trainerType,
                trainerProperties.TrainerClassId,
                0,
                trainerProperties.TeamSize,
                trainerProperties.Items,
                aiFlags,
                (uint)(trainerProperties.DoubleBattle ? 2 : 0)
            );
        }

        public TrainerPartyPokemonData NewTrainerPartyPokemonData(Pokemon pokemon, bool chooseMoves, bool chooseItems, bool hasBallCapsule)
        {
            var newPokemonData = new TrainerPartyPokemonData
            {
                Difficulty = pokemon.DifficultyValue,
                GenderAbilityOverride = pokemon.GenderAbilityOverride,
                Species = (ushort)(pokemon.PokemonId + (pokemon.FormId << Pokemon.Constants.PokemonNumberBitSize)),
                Level = pokemon.Level,
                ItemId = chooseItems && pokemon.HeldItemId.HasValue ? pokemon.HeldItemId.Value : null,
                MoveIds = chooseMoves ? pokemon.Moves : null,
                BallCapsule = hasBallCapsule && pokemon.BallCapsuleId.HasValue ? pokemon.BallCapsuleId.Value : null
            };

            return newPokemonData;
        }
    }
}
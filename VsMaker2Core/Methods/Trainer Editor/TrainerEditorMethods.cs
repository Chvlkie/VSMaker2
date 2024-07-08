using System.Collections;
using VsMaker2Core.DataModels;
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

        public List<Trainer> GetTrainers(List<string> trainerNames, RomFile loadedRom)
        {
            List<Trainer> trainers = [];
            // Start from i 1 to skip player trainer file
            for (int i = 1; i < trainerNames.Count; i++)
            {
                var trainerData = loadedRom.TrainersData[i];
                var trainerPartyData = loadedRom.TrainersPartyData[i];
                trainers.Add(BuildTrainerData(i, trainerNames[i], trainerData, trainerPartyData, RomFile.GameFamily != GameFamily.DiamondPearl, loadedRom));
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
                    BallCapsuleId = hasBallCapsule ? trainerPartyPokemon.BallCapsule : null
                };
                trainerParty.Pokemons.Add(pokemon);
            }

            // Create Dummy Mons
            if (teamSize < 6)
            {
                for (int i = 0; i < 6 - teamSize; i++)
                {
                    trainerParty.Pokemons.Add(new Pokemon());
                }
            }
            return trainerParty;
        }

        public TrainerProperty BuildTrainerPropertyFromRomData(TrainerData trainerData)
        {
            var trainerProperty = new TrainerProperty()
            {
                Items = trainerData.Items,
                ChooseMoves = (trainerData.TrainerType & 1) != 0,
                ChooseItems = (trainerData.TrainerType & 2) != 0,
                TrainerClassId = trainerData.TrainerClassId,
                TeamSize = trainerData.TeamSize,
                DoubleBattle = trainerData.IsDoubleBattle == 2,
            };

            var aiFlags = new BitArray(BitConverter.GetBytes(trainerData.AIFlags));
            for (int i = 0; i < Trainer.Constants.NumberOfTrainerAIFlags; i++)
            {
                trainerProperty.AIFlags[i] = aiFlags[i];
            }

            return trainerProperty;
        }

        public Trainer BuildTrainerData(int trainerId, string trainerName, TrainerData trainerData, TrainerPartyData trainerPartyData, bool hasBallCapsule, RomFile loadedRom)
        {
            var trainerProperties = BuildTrainerPropertyFromRomData(trainerData);
            var trainerParty = BuildTrainerPartyFromRomData(trainerPartyData, trainerProperties.TeamSize, trainerProperties.ChooseItems, trainerProperties.ChooseMoves, hasBallCapsule);
            var usages = FindTrainerUses(trainerId, loadedRom);
            return new Trainer((ushort)trainerId, trainerName, trainerProperties, trainerParty, usages);
        }

        public List<TrainerUsage> FindTrainerUses(int trainerId, RomFile loadedRom)
        {
            List<TrainerUsage> scripts = [];

            foreach (var scriptFile in loadedRom.ScriptFileData.Where(x => !x.IsLevelScript))
            {
                foreach (var script in scriptFile.Scripts.Where(x => !x.UsedScriptId.HasValue))
                {
                    foreach (var line in script.Lines.Where(x => (x.ScriptCommandId == 0x00E5 || x.ScriptCommandId == 0x00D5) && x.Parameters[0].Length >= 2 && BitConverter.ToUInt16(x.Parameters[0], 0) == trainerId))
                    {
                        scripts.Add(new TrainerUsage(trainerId, scriptFile.ScriptFileId, (int)script.ScriptNumber, TrainerUsageType.Script));
                    }
                }

                foreach (var function in scriptFile.Functions.Where(x => !x.UsedScriptId.HasValue))
                {
                    foreach (var line in function.Lines.Where(x => (x.ScriptCommandId == 0x00E5 || x.ScriptCommandId == 0x00D5) && x.Parameters[0].Length >= 2 && BitConverter.ToUInt16(x.Parameters[0], 0) == trainerId))
                    {
                        scripts.Add(new TrainerUsage(trainerId, scriptFile.ScriptFileId, (int)function.ScriptNumber, TrainerUsageType.Function));
                    }
                }
            }
            List<TrainerUsage> events = [];
            return [.. scripts, .. events];
        }

        public Trainer GetTrainer(List<Trainer> trainers, int trainerId)
        {
            return trainers.SingleOrDefault(x => x.TrainerId == trainerId);
        }

        public TrainerProperty NewTrainerProperties(byte teamSize, bool chooseMoves, bool chooseItems, bool isDouble, byte trainerClassId, ushort item1, ushort item2, ushort item3, ushort item4, List<bool> aiFlags)
        {
            return new TrainerProperty
            {
                DoubleBattle = isDouble,
                ChooseItems = chooseItems,
                ChooseMoves = chooseMoves,
                TeamSize = teamSize,
                Items = [item1, item2, item3, item4],
                TrainerClassId = trainerClassId,
                AIFlags = aiFlags
            };
        }

        public TrainerData NewTrainerData(TrainerProperty trainerProperties)
        {
            byte trainerType = 0;
            trainerType |= (byte)(trainerProperties.ChooseMoves ? 1 : 0);
            trainerType |= (byte)(trainerProperties.ChooseItems ? 2 : 0);

            BitArray aiFlagsArray = new BitArray(new bool[32] { true, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false,false,false });
            for (int i = 0; i < trainerProperties.AIFlags.Count; i++)
            {
                aiFlagsArray[i] = trainerProperties.AIFlags[i];
            }
            uint aiFlags = 0;
            for (int i = 0; i < aiFlagsArray.Length; i++)
            {
                if (aiFlagsArray[i])
                {
                    aiFlags |= (uint)1 << i;
                }
            }

            return new TrainerData(trainerType, trainerProperties.TrainerClassId, 0, trainerProperties.TeamSize, trainerProperties.Items, aiFlags, (uint)(trainerProperties.DoubleBattle ? 2 : 0));
        }

        public TrainerPartyPokemonData NewTrainerPartyPokemonData(Pokemon pokemon, bool chooseMoves, bool chooseItems, bool hasBallCapsule)
        {
            var newPokemonData = new TrainerPartyPokemonData()
            {
                Difficulty = pokemon.DifficultyValue,
                GenderAbilityOverride = pokemon.GenderAbilityOverride,
                Species = (ushort)(pokemon.PokemonId + (pokemon.FormId << Pokemon.Constants.PokemonNumberBitSize)),
                Level = pokemon.Level,
            };
            if (chooseItems) { newPokemonData.ItemId = pokemon.HeldItemId.Value; }
            if (chooseMoves) { newPokemonData.MoveIds = pokemon.Moves; }
            if (hasBallCapsule) { newPokemonData.BallCapsule = pokemon.BallCapsuleId.Value; }

            return newPokemonData;
        }
    }
}
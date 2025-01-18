using System.Collections;
using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class TrainerEditorMethods : ITrainerEditorMethods
    {
        private readonly IFileSystemMethods fileSystemMethods;
        private readonly IRomFileMethods romFileMethods;

        public TrainerEditorMethods(IRomFileMethods romFileMethods, IFileSystemMethods fileSystemMethods)
        {
            this.romFileMethods = romFileMethods;
            this.fileSystemMethods = fileSystemMethods;
        }

        public Trainer BuildTrainerData(int trainerId, string trainerName, TrainerData trainerData, TrainerPartyData trainerPartyData, bool hasBallCapsule)
        {
            var trainerProperties = BuildTrainerPropertyFromRomData(trainerData);
            var trainerParty = BuildTrainerPartyFromRomData(trainerPartyData, trainerProperties.TeamSize, hasBallCapsule, trainerProperties.PropertyFlags);
            var usages = FindTrainerUses(trainerId);

            return new Trainer((ushort)trainerId, trainerName, trainerProperties, trainerParty, usages);
        }

        public TrainerParty BuildTrainerPartyFromRomData(TrainerPartyData trainerPartyData, int teamSize, bool hasBallCapsule, List<bool> trainerPropertyFlags)
        {
            var trainerParty = new TrainerParty();

            // If the team size is zero, add six dummy Pokémon and return early
            if (teamSize == 0)
            {
                trainerParty.Pokemons.AddRange(Enumerable.Repeat(new Pokemon(), 6));
                return trainerParty;
            }
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
                    HeldItemId = trainerPropertyFlags[2] ? trainerPartyPokemon.ItemId : null,
                    Moves = trainerPropertyFlags[1] ? trainerPartyPokemon.MoveIds : null,
                    BallCapsuleId = hasBallCapsule ? trainerPartyPokemon.BallCapsule : null,
                };

                //    DoubleBattle,
                //ChooseMoves,
                //ChooseItems,
                //ChooseAbility_Hge,
                //ChooseBall_Hge,
                //SetIvEv_Hge,
                //ChooseNature_Hge,
                //ShinyLock_Hge,
                //AdditionalFlags_Hge

                if (RomFile.IsHgEngine)
                {
                    if (trainerPropertyFlags[3])
                    {
                        pokemon.Ability_Hge = trainerPartyPokemon.Ability_Hge ?? 0;
                    }
                    if (trainerPropertyFlags[4])
                    {
                        pokemon.Ball_Hge = trainerPartyPokemon.Ball_Hge ?? 0;
                    }
                    if (trainerPropertyFlags[5])
                    {
                        pokemon.IvNums_Hge = trainerPartyPokemon.IvNums_Hge;
                        pokemon.EvNums_Hge = trainerPartyPokemon.EvNums_Hge;
                    }
                    if (trainerPropertyFlags[6])
                    {
                        pokemon.Nature_Hge = trainerPartyPokemon.Nature_Hge ?? 0;
                    }
                    if (trainerPropertyFlags[7])
                    {
                        pokemon.ShinyLock_Hge = trainerPartyPokemon.ShinyLock_Hge ?? 0;
                    }
                    if (trainerPropertyFlags[8])
                    {
                        pokemon.ChooseStatus_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x01) != 0;
                        pokemon.ChooseHP_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x02) != 0;
                        pokemon.ChooseATK_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x04) != 0;
                        pokemon.ChooseDEF_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x08) != 0;
                        pokemon.ChooseSPEED_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x10) != 0;
                        pokemon.Choose_SpATK_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x20) != 0;
                        pokemon.Choose_SpDEF_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x40) != 0;
                        pokemon.ChooseTypes_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x80) != 0;
                        pokemon.ChoosePP_Hge = (trainerPartyPokemon.AdditionalFlags_Hge & 0x100) != 0;
                        pokemon.ChooseNickname_HGE = (trainerPartyPokemon.AdditionalFlags_Hge & 0x200) != 0;

                        pokemon.Status_Hge = pokemon.ChooseStatus_Hge ? trainerPartyPokemon.Status_Hge : 0;
                        pokemon.Hp_Hge = pokemon.ChooseHP_Hge ? trainerPartyPokemon.Hp_Hge : 0;
                        pokemon.Atk_Hge = pokemon.ChooseATK_Hge ? trainerPartyPokemon.Atk_Hge : 0;
                        pokemon.Def_Hge = pokemon.ChooseDEF_Hge ? trainerPartyPokemon.Def_Hge : 0;
                        pokemon.Speed_Hge = pokemon.ChooseSPEED_Hge ? trainerPartyPokemon.Speed_Hge : 0;
                        pokemon.SpAtk_Hge = pokemon.Choose_SpATK_Hge ? trainerPartyPokemon.SpAtk_Hge : 0;
                        pokemon.SpDef_Hge = pokemon.Choose_SpDEF_Hge ? trainerPartyPokemon.SpDef_Hge : 0;
                        pokemon.Types_Hge = pokemon.ChooseTypes_Hge ? trainerPartyPokemon.Types_Hge : [0, 0];
                        pokemon.PpCounts_Hge = pokemon.ChoosePP_Hge ? trainerPartyPokemon.PpCounts_Hge : [0, 0, 0, 0];
                        pokemon.Nickname_Hge = pokemon.ChooseNickname_HGE ? trainerPartyPokemon.Nickname_Hge : [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    }
                }

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
                ChooseMoves = (trainerData.TrainerType & 0x01) != 0,
                ChooseItems = (trainerData.TrainerType & 0x02) != 0,
                TrainerClassId = trainerData.TrainerClassId,
                TeamSize = trainerData.TeamSize,
                DoubleBattle = trainerData.IsDoubleBattle == 2
            };

            if (RomFile.IsHgEngine)
            {
                trainerProperty.ChooseAbility_Hge = (trainerData.TrainerType & 0x04) != 0;
                trainerProperty.ChooseBall_Hge = (trainerData.TrainerType & 0x08) != 0;
                trainerProperty.SetIvEv_Hge = (trainerData.TrainerType & 0x10) != 0;
                trainerProperty.ChooseNature_Hge = (trainerData.TrainerType & 0x20) != 0;
                trainerProperty.ShinyLock_Hge = (trainerData.TrainerType & 0x40) != 0;
                trainerProperty.AdditionalFlags_Hge = (trainerData.TrainerType & 0x80) != 0;
            }

            var aiFlags = new BitArray(BitConverter.GetBytes(trainerData.AIFlags));

            trainerProperty.AIFlags = [];
            for (int i = 0; i < Trainer.Constants.NumberOfTrainerAIFlags; i++)
            {
                trainerProperty.AIFlags.Add(aiFlags[i]);
            }

            return trainerProperty;
        }

        public Trainer GetTrainer(List<Trainer> trainers, int trainerId) => trainers.SingleOrDefault(x => x.TrainerId == trainerId);

        public List<Trainer> GetTrainers()
        {
            List<Trainer> trainers = [];

            var trainersData = RomFile.TrainersData;
            var trainersPartyData = RomFile.TrainersPartyData;
            bool isNotDiamondPearl = RomFile.IsNotDiamondPearl;

            for (int i = 0; i < trainersData.Count; i++)
            {
                var trainerData = trainersData[i];
                var trainerPartyData = trainersPartyData[i];

                trainers.Add(BuildTrainerData(i, RomFile.TrainerNames[i], trainerData, trainerPartyData, isNotDiamondPearl));
            }

            return trainers;
        }

        public TrainerData NewTrainerData(TrainerProperty trainerProperties)
        {
            byte trainerType = 0x00;
            if (trainerProperties.ChooseMoves)
            {
                trainerType |= 0x01;
            }
            if (trainerProperties.ChooseItems)
            {
                trainerType |= 0x02;
            }
            if (RomFile.IsHgEngine)
            {
                if (trainerProperties.ChooseAbility_Hge)
                {
                    trainerType |= 0x04;
                }
                if (trainerProperties.ChooseBall_Hge)
                {
                    trainerType |= 0x08;
                }
                if (trainerProperties.SetIvEv_Hge)
                {
                    trainerType |= 0x10;
                }
                if (trainerProperties.ChooseNature_Hge)
                {
                    trainerType |= 0x20;
                }
                if (trainerProperties.ShinyLock_Hge)
                {
                    trainerType |= 0x40;
                }
                if (trainerProperties.AdditionalFlags_Hge)
                {
                    trainerType |= 0x80;
                }
            }

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
            return new TrainerPartyPokemonData
            {
                Difficulty = pokemon.DifficultyValue,
                GenderAbilityOverride = pokemon.GenderAbilityOverride,
                Species = (ushort)((pokemon.FormId << Pokemon.Constants.PokemonNumberBitSize) | pokemon.PokemonId),
                Level = pokemon.Level,
                ItemId = chooseItems && pokemon.HeldItemId.HasValue ? pokemon.HeldItemId.Value : null,
                MoveIds = chooseMoves ? pokemon.Moves : null,
                BallCapsule = hasBallCapsule && pokemon.BallCapsuleId.HasValue ? pokemon.BallCapsuleId.Value : null
            };
        }

        public TrainerProperty NewTrainerProperties(byte teamSize, byte trainerClassId, ushort item1, ushort item2, ushort item3, ushort item4, List<bool> aiFlags, List<bool> propertyFlags) => new TrainerProperty
        {
            DoubleBattle = propertyFlags[0],
            ChooseMoves = propertyFlags[1],
            ChooseItems = propertyFlags[2],
            TeamSize = teamSize,
            Items = [item1, item2, item3, item4],
            TrainerClassId = trainerClassId,
            AIFlags = aiFlags,
            ChooseAbility_Hge = RomFile.IsHgEngine && propertyFlags[3],
            ChooseBall_Hge = RomFile.IsHgEngine && propertyFlags[4],
            SetIvEv_Hge = RomFile.IsHgEngine && propertyFlags[5],
            ChooseNature_Hge = RomFile.IsHgEngine && propertyFlags[6],
            ShinyLock_Hge = RomFile.IsHgEngine && propertyFlags[7],
            AdditionalFlags_Hge = RomFile.IsHgEngine && propertyFlags[8],
        };

        public (bool Success, string ErrorMessage) RemoveTrainer(int trainerId)
        {
            var removeTrainer = fileSystemMethods.RemoveTrainerData(trainerId);
            if (!removeTrainer.Success)
            {
                return (false, removeTrainer.ErrorMessage);
            }

            RomFile.TrainerNames.RemoveAt(trainerId);
            var updateTrainerNames = fileSystemMethods.WriteMessage(RomFile.TrainerNames, RomFile.TrainerNamesTextNumber, true);
            if (!updateTrainerNames.Success)
            {
                return (false, updateTrainerNames.ErrorMessage);
            }

            var reorderFiles = fileSystemMethods.ReorderTrainerData();
            if (!reorderFiles.Success)
            {
                return (false, reorderFiles.ErrorMessage);
            }

            RomFile.TrainersData = romFileMethods.GetTrainersData();
            RomFile.TrainersPartyData = romFileMethods.GetTrainersPartyData();
            RomFile.TrainerNames = romFileMethods.GetTrainerNames();
            return (true, "");
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
    }
}
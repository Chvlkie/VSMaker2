using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods.EventFile
{
    public class EventFileMethods : IEventFileMethods
    {
        public EventFileMethods()
        { }

        EventFileData IEventFileMethods.GetEventFileData(int eventFileId)
        {
            string filePath = $"{Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.eventFiles].unpackedDirectory}\\{eventFileId:D4}";
            List<EventSpawnable> spawnables = [];
            List<EventOverworld> overworlds = [];
            List<EventWarp> warps = [];
            List<EventTrigger> triggers = [];

            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new(fileStream))
            {
                /* Read spawnables */
                uint spawnablesCount = reader.ReadUInt32();
                for (int i = 0; i < spawnablesCount; i++)
                {
                    spawnables.Add(GetSpawnable(new MemoryStream(reader.ReadBytes(0x14))));
                }

                /* Read overworlds */
                uint overworldsCount = reader.ReadUInt32();
                for (int i = 0; i < overworldsCount; i++)
                {
                    overworlds.Add(GetOverworld(new MemoryStream(reader.ReadBytes(0x20))));
                }

                /* Read warps */
                uint warpsCount = reader.ReadUInt32();
                for (int i = 0; i < warpsCount; i++)
                {
                    warps.Add(GetWarp(new MemoryStream(reader.ReadBytes(0xC))));
                }

                /* Read triggers */
                uint triggersCount = reader.ReadUInt32();
                for (int i = 0; i < triggersCount; i++)
                {
                    triggers.Add(GetTrigger(new MemoryStream(reader.ReadBytes(0x10))));
                }
            }
            return new EventFileData(eventFileId, spawnables, overworlds, warps, triggers);
        }

        public static EventSpawnable GetSpawnable(Stream data)
        {
            var spawnable = new EventSpawnable();

            using (BinaryReader reader = new(data))
            {
                spawnable.ScriptNumber = reader.ReadUInt16();
                spawnable.SpawnableType = reader.ReadUInt16();
                spawnable.XPosition = (ushort)reader.ReadInt16();
                spawnable.Unknown2 = reader.ReadUInt16();
                spawnable.YPosition = (ushort)reader.ReadInt16();
                spawnable.ZPosition = (uint)reader.ReadInt32();
                spawnable.Unknown4 = reader.ReadUInt16();
                spawnable.Direction = reader.ReadUInt16();
                spawnable.Unknown5 = reader.ReadUInt16();
            }

            return spawnable;
        }

        public static EventOverworld GetOverworld(Stream data)
        {
            var overworld = new EventOverworld();

            using (BinaryReader reader = new(data))
            {
                overworld.OverworldId = reader.ReadUInt16();
                overworld.OverworldTableEntry = reader.ReadUInt16();
                overworld.Movment = reader.ReadUInt16();
                overworld.OverworldType = reader.ReadUInt16();
                overworld.Flag = reader.ReadUInt16();
                overworld.ScriptNumber = reader.ReadUInt16();
                overworld.Orientation = reader.ReadUInt16();
                overworld.SightRange = reader.ReadUInt16();
                overworld.Unknown1 = reader.ReadUInt16();
                overworld.Unknown2 = reader.ReadUInt16();
                overworld.HorizontalRange = reader.ReadUInt16();
                overworld.VerticalRange = reader.ReadUInt16();
                overworld.XPosition = (ushort)reader.ReadInt16();
                overworld.YPosition = (ushort)reader.ReadInt16();
                overworld.ZPosition = (uint)reader.ReadInt32();
            }
            return overworld;
        }

        public static EventWarp GetWarp(Stream data)
        {
            var warp = new EventWarp();
            using (BinaryReader reader = new(data))
            {
                warp.XPosition = (ushort)reader.ReadInt16();
                warp.YPosition = (ushort)reader.ReadInt16();
                warp.Header = reader.ReadUInt16();
                warp.Anchor = reader.ReadUInt16();
                warp.Height = reader.ReadUInt32();
            }

            return warp;
        }

        public static EventTrigger GetTrigger(Stream data)
        {
            var trigger = new EventTrigger();

            using (BinaryReader reader = new(data))
            {
                trigger.ScriptNumber = reader.ReadUInt16();
                trigger.XPosition = (ushort)reader.ReadInt16();
                trigger.YPosition = (ushort)reader.ReadInt16();
                trigger.WidthX = reader.ReadUInt16();
                trigger.HeightY = reader.ReadUInt16();
                trigger.ZPosition = reader.ReadUInt16();
                trigger.ExpectedVarValue = reader.ReadUInt16();
                trigger.VariableWatched = reader.ReadUInt16();
            }

            return trigger;
        }

        List<EventFileData> IEventFileMethods.GetEventFiles()
        {
            List<EventFileData> events = [];

            string directory = Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.eventFiles].unpackedDirectory;

            var files = new DirectoryInfo(directory).EnumerateFiles();

            foreach (var file in files)
            {
                int index = int.Parse(Path.GetFileNameWithoutExtension(file.Name)); // Assuming file names are indices
                events.Add(((IEventFileMethods)this).GetEventFileData(index));
            }

            return events;
        }
    }
}
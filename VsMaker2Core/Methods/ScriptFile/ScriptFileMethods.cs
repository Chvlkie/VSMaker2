using VsMaker2Core.Database;
using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class ScriptFileMethods : IScriptFileMethods
    {
        private ScriptData GetScriptData(BinaryReader reader, uint index, int scriptOffset, ref List<int> functionOffsets, ref List<int> actionOffsets)
        {
            reader.BaseStream.Position = scriptOffset;

            List<ScriptLine> lines = [];
            bool endScript = false;

            const int maxIterations = 1000;
            int iterationCount = 0;

            while (!endScript && iterationCount < maxIterations)
            {
                var line = ReadScriptLine(reader, ref functionOffsets, ref actionOffsets);

                if (line.Parameters != null)
                {
                    lines.Add(line);

                    if (ScriptDatabase.endCodes.Contains((ushort)line.ScriptCommandId))
                    {
                        endScript = true;
                    }
                }

                iterationCount++;
            }

            if (iterationCount >= maxIterations)
            {
                Console.WriteLine("Warning: Max iterations reached in GetScriptData. Possible infinite loop.");
            }

            return new ScriptData(index + 1, ScriptType.Script, lines: lines);
        }

        private ScriptData GetFunctionData(BinaryReader reader, uint index, ref List<int> functionOffsets, ref List<int> actionOffsets)
        {
            List<ScriptLine> lines = [];
            bool endScript = false;
            int iterationCount = 0;
            const int maxIterations = 1000;

            while (!endScript && iterationCount < maxIterations)
            {
                var line = ReadScriptLine(reader, ref functionOffsets, ref actionOffsets);

                if (line.Parameters != null)
                {
                    lines.Add(line);

                    if (ScriptDatabase.endCodes.Contains((ushort)line.ScriptCommandId))
                    {
                        endScript = true;
                    }
                }

                iterationCount++;
            }

            if (iterationCount >= maxIterations)
            {
                Console.WriteLine("Warning: Max iterations reached in GetFunctionData. Potential infinite loop.");
            }

            return new ScriptData(index + 1, ScriptType.Function, lines: lines);
        }

        private static void AddParametersToList(ref List<byte[]> parameterList, ushort id, BinaryReader dataReader)
        {
            Console.WriteLine("Loaded command id: " + id.ToString("X4"));

            try
            {
                if (!RomFile.ScriptCommandParametersDict.TryGetValue(id, out byte[]? value))
                {
                    Console.WriteLine($"Warning: Command ID {id:X4} not found in the ScriptCommandParametersDict.");
                    return;
                }

                foreach (int bytesToRead in value)
                {
                    parameterList.Add(dataReader.ReadBytes(bytesToRead));
                }
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine($"Error reading parameters for command ID {id:X4}: {ex.Message}");
                return;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error while reading parameters for command ID {id:X4}: {ex.Message}");
                return;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine($"NullReferenceException occurred for command ID {id:X4}. Check ScriptCommandParametersDict.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error for command ID {id:X4}: {ex.Message}");
                return;
            }
        }

        private static void ProcessRelativeJump(BinaryReader reader, ref List<byte[]> parameters, ref List<int> offsetsList)
        {
            int relativeOffset = reader.ReadInt32();

            int offsetFromScriptFileStart = (int)(relativeOffset + reader.BaseStream.Position);

            if (!offsetsList.Contains(offsetFromScriptFileStart))
            {
                offsetsList.Add(offsetFromScriptFileStart);
            }

            int functionNumber = offsetsList.IndexOf(offsetFromScriptFileStart);

            if (functionNumber < 0)
            {
                throw new InvalidOperationException($"Offset {offsetFromScriptFileStart} not found in offsetsList.");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters), "Parameters list cannot be null.");
            }

            parameters.Add(BitConverter.GetBytes(functionNumber + 1));
        }

        private static ScriptLine ReadScriptLine(BinaryReader reader, ref List<int> functionOffsets, ref List<int> actionOffsets)
        {
            ushort id = reader.ReadUInt16();

            List<byte[]> parameterList = [];

            /* How to read parameters for different commands for DPPt*/
            switch (RomFile.GameFamily)
            {
                case GameFamily.DiamondPearl:
                case GameFamily.Platinum:
                    switch (id)
                    {
                        case 0x16: //Jump
                        case 0x1A: //Call
                            ProcessRelativeJump(reader, ref parameterList, ref functionOffsets);
                            break;

                        case 0x17: //JumpIfObjID
                        case 0x18: //JumpIfBgID
                        case 0x19: //JumpIfPlayerDir
                        case 0x1C: //JumpIf
                        case 0x1D: //CallIf
                                   //in the case of JumpIf and CallIf, the first param is a comparisonOperator
                                   //for JumpIfPlayerDir it's a directionID
                                   //for JumpIfObjID, it's an EventID
                            parameterList.Add([reader.ReadByte()]);
                            ProcessRelativeJump(reader, ref parameterList, ref functionOffsets);
                            break;

                        case 0x5E: // Movement
                        case 0x2A1: // Movement2
                                    //in the case of Movement, the first param is an overworld ID
                            parameterList.Add(BitConverter.GetBytes(reader.ReadUInt16()));
                            ProcessRelativeJump(reader, ref parameterList, ref actionOffsets);
                            break;

                        case 0x1CF:
                        case 0x1D0:
                        case 0x1D1:
                            {
                                byte parameter1 = reader.ReadByte();
                                parameterList.Add([parameter1]);
                                if (parameter1 == 0x2)
                                {
                                    parameterList.Add(reader.ReadBytes(2)); //Read additional u16 if first param read is 2
                                }
                            }
                            break;

                        case 0x21D:
                            {
                                ushort parameter1 = reader.ReadUInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));

                                switch (parameter1)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                        parameterList.Add(reader.ReadBytes(2));
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 4:
                                    case 5:
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 6:
                                        break;
                                }
                            }
                            break;

                        case 0x235:
                            {
                                short parameter1 = reader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));

                                switch (parameter1)
                                {
                                    case 0x1:
                                    case 0x3:
                                        parameterList.Add(reader.ReadBytes(2));
                                        parameterList.Add(reader.ReadBytes(2));
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 0x4:
                                        parameterList.Add(reader.ReadBytes(2));
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 0x0:
                                    case 0x6:
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    default:
                                        break;
                                }
                            }
                            break;

                        case 0x23E:
                            {
                                short parameter1 = reader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));

                                switch (parameter1)
                                {
                                    case 0x1:
                                    case 0x3:
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 0x5:
                                    case 0x6:
                                        parameterList.Add(reader.ReadBytes(2));
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    default:
                                        break;
                                }
                            }
                            break;

                        case 0x2C4:
                            {
                                byte parameter1 = reader.ReadByte();
                                parameterList.Add([parameter1]);
                                if (parameter1 == 0 || parameter1 == 1)
                                {
                                    parameterList.Add(reader.ReadBytes(2));
                                }
                            }
                            break;

                        case 0x2C5:
                            {
                                if (RomFile.GameVersion == GameVersion.Platinum)
                                {
                                    parameterList.Add(reader.ReadBytes(2));
                                    parameterList.Add(reader.ReadBytes(2));
                                }
                                else
                                {
                                    goto default;
                                }
                            }
                            break;

                        case 0x2C6:
                        case 0x2C9:
                        case 0x2CA:
                        case 0x2CD:
                            if (RomFile.GameVersion == GameVersion.Platinum)
                            {
                                break;
                            }
                            else
                            {
                                goto default;
                            }
                        case 0x2CF:
                            if (RomFile.GameVersion == GameVersion.Platinum)
                            {
                                parameterList.Add(reader.ReadBytes(2));
                                parameterList.Add(reader.ReadBytes(2));
                            }
                            else
                            {
                                goto default;
                            }

                            break;

                        default:
                            AddParametersToList(ref parameterList, id, reader);
                            break;
                    }

                    break;

                case GameFamily.HeartGoldSoulSilver:
                case GameFamily.HgEngine:
                    switch (id)
                    {
                        case 0x16: //Jump
                        case 0x1A: //Call
                            ProcessRelativeJump(reader, ref parameterList, ref functionOffsets);
                            break;

                        case 0x17: //JumpIfObjID
                        case 0x18: //JumpIfBgID
                        case 0x19: //JumpIfPlayerDir
                        case 0x1C: //JumpIf
                        case 0x1D: //CallIf
                            parameterList.Add([reader.ReadByte()]); //in the case of JumpIf and CallIf, the first param is a comparisonOperator
                            ProcessRelativeJump(reader, ref parameterList, ref functionOffsets);
                            break;

                        case 0x5E: // Movement
                            parameterList.Add(BitConverter.GetBytes(reader.ReadUInt16())); //in the case of Movement, the first param is an overworld ID
                            ProcessRelativeJump(reader, ref parameterList, ref actionOffsets);
                            break;

                        case 0x190:
                        case 0x191:
                        case 0x192:
                            {
                                byte parameter1 = reader.ReadByte();
                                parameterList.Add([parameter1]);
                                if (parameter1 == 0x2)
                                {
                                    parameterList.Add(reader.ReadBytes(2));
                                }
                            }
                            break;

                        case 0x1D1: // Number of parameters differ depending on the first parameter value
                            {
                                short parameter1 = reader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));
                                switch (parameter1)
                                {
                                    case 0x0:
                                    case 0x1:
                                    case 0x2:
                                    case 0x3:
                                        parameterList.Add(reader.ReadBytes(2));
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 0x4:
                                    case 0x5:
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 0x6:
                                        break;

                                    case 0x7:
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    default:
                                        break;
                                }
                            }
                            break;

                        case 0x1E9: // Number of parameters differ depending on the first parameter value
                            {
                                short parameter1 = reader.ReadInt16();
                                parameterList.Add(BitConverter.GetBytes(parameter1));
                                switch (parameter1)
                                {
                                    case 0x0:
                                        break;

                                    case 0x1:
                                    case 0x2:
                                    case 0x3:
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 0x4:
                                        break;

                                    case 0x5:
                                    case 0x6:
                                        parameterList.Add(reader.ReadBytes(2));
                                        parameterList.Add(reader.ReadBytes(2));
                                        break;

                                    case 0x7:
                                    case 0x8:
                                        break;

                                    default:
                                        break;
                                }
                            }
                            break;

                        default:
                            AddParametersToList(ref parameterList, id, reader);
                            break;
                    }

                    break;
            }

            return new ScriptLine(id, parameterList);
        }

        #region Get

        public List<ScriptFileData> GetScriptFiles()
        {
            List<ScriptFileData> scripts = [];

            string directory = Path.Combine(VsMakerDatabase.RomData.GameDirectories[NarcDirectory.scripts].unpackedDirectory);

            var files = new DirectoryInfo(directory).EnumerateFiles();

            foreach (var file in files)
            {
                int index = int.Parse(Path.GetFileNameWithoutExtension(file.Name)); // Assuming file names are indices
                scripts.Add(GetScriptFileData(index));
            }

            return scripts;
        }

        public ScriptFileData GetScriptFileData(int scriptFileId)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.scripts].unpackedDirectory}\\{scriptFileId:D4}";

            if (!File.Exists(directory))
            {
                throw new FileNotFoundException($"Script file with ID {scriptFileId} not found in directory: {directory}");
            }

            List<int> scriptOffsets = [];
            List<int> functionOffsets = [];
            List<int> actionOffsets = [];

            List<ScriptData> scripts = [];
            List<ScriptData> functions = [];
            List<ScriptActionData> actions = [];

            using FileStream fileStream = new(directory, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fileStream);

            bool isLevelScript = true;

            try
            {
                // Read and determine if the script is a level script or not
                while (true)
                {
                    uint checker = reader.ReadUInt16();
                    reader.BaseStream.Position -= 0x2;
                    uint value = reader.ReadUInt32();

                    if (value == 0 && scriptOffsets.Count == 0)
                    {
                        isLevelScript = true;
                        break;
                    }

                    if (checker == 0xFD13)
                    {
                        reader.BaseStream.Position -= 0x4;
                        isLevelScript = false;
                        break;
                    }

                    int offsetFromStart = (int)(value + reader.BaseStream.Position);
                    scriptOffsets.Add(offsetFromStart);
                }

                // If not a level script, read the script, function, and action data
                if (!isLevelScript)
                {
                    // Read Script Data
                    for (uint i = 0; i < scriptOffsets.Count; i++)
                    {
                        int offset = scriptOffsets[(int)i];
                        int index = scriptOffsets.FindIndex(x => x == offset);

                        if (index == i)
                        {
                            scripts.Add(GetScriptData(reader, i, offset, ref functionOffsets, ref actionOffsets));
                        }
                        else
                        {
                            scripts.Add(new ScriptData(i + 1, ScriptType.Script, index + 1));
                        }
                    }

                    // Read Function Data
                    for (uint i = 0; i < functionOffsets.Count; i++)
                    {
                        reader.BaseStream.Position = functionOffsets[(int)i];
                        int index = scriptOffsets.IndexOf(functionOffsets[(int)i]);

                        if (index == -1)
                        {
                            functions.Add(GetFunctionData(reader, i, ref functionOffsets, ref actionOffsets));
                        }
                        else
                        {
                            functions.Add(new ScriptData(i + 1, ScriptType.Function, index + 1));
                        }
                    }

                    // Read Action Data
                    for (uint i = 0; i < actionOffsets.Count; i++)
                    {
                        reader.BaseStream.Position = actionOffsets[(int)i];
                        List<ScriptActionLine> lines = [];
                        bool endActions = false;

                        while (!endActions)
                        {
                            ushort id = reader.ReadUInt16();
                            if (id == 0xFE)
                            {
                                lines.Add(new ScriptActionLine(id, 0));
                                endActions = true;
                            }
                            else
                            {
                                lines.Add(new ScriptActionLine(id, reader.ReadUInt16()));
                            }
                        }

                        actions.Add(new ScriptActionData(i + 1, lines));
                    }
                }
            }
            catch (EndOfStreamException)
            {
                if (!isLevelScript)
                {
                    Console.WriteLine("End of stream reached while reading script file.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading script file with ID {scriptFileId}: {ex.Message}");
                throw;
            }

            return new ScriptFileData(scriptFileId, isLevelScript, scripts, functions, actions);
        }

        #endregion Get

        private struct ScriptReference
        {
            public uint Id { get; set; }
            public uint Offset { get; set; }

            public ScriptReference(uint id, uint offset)
            {
                Id = id;
                Offset = offset;
            }
        }

        private struct CallerReference(Enums.ScriptType scriptType, uint callerId, Enums.ScriptType invokedType, uint invokedId, int position)
        {
            public ScriptType ScriptType = scriptType;
            public uint CallerId = callerId;
            public ScriptType InvokedType = invokedType;
            public uint InvokedId = invokedId;
            public int Offset = position;
        }

        private static void AddReference(ref List<CallerReference> callerReferences, ushort commandId, List<byte[]> parameters, int position, ScriptData script)
        {
            if (ScriptDatabase.commandsWithRelativeJump.TryGetValue(commandId, out int parameterWithRelativeJump))
            {
                if (parameterWithRelativeJump < parameters.Count)
                {
                    uint invokedId = BitConverter.ToUInt32(parameters[parameterWithRelativeJump], 0);

                    switch (commandId)
                    {
                        case 0x005E: // Special case for Action Type
                            callerReferences.Add(new CallerReference(script.ScriptType, script.ScriptNumber, ScriptType.Action, invokedId, position - 4));
                            break;

                        default: // Default case for Function Type
                            callerReferences.Add(new CallerReference(script.ScriptType, script.ScriptNumber, ScriptType.Function, invokedId, position - 4));
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"Error: Parameter index {parameterWithRelativeJump} is out of bounds for command {commandId:X4}.");
                }
            }
        }

        #region Write

        public (bool Success, string ErrorMessage) WriteScriptData(ScriptFileData scriptFileData)
        {
            // Initialize lists properly
            List<ScriptReference> scriptOffsets = [];
            List<ScriptReference> functionOffsets = [];
            List<ScriptReference> actionOffsets = [];
            List<CallerReference> callerReferences = [];

            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);

            try
            {
                // Move writer's position for scripts count
                writer.BaseStream.Position += scriptFileData.Scripts.Count * 0x4;
                writer.Write((ushort)0xFD13);

                List<ScriptData> useScripts = [];

                // Write Scripts to Stream
                WriteScripts(scriptFileData, writer, ref scriptOffsets, ref useScripts, ref callerReferences);

                // Handle UseScripts
                HandleUseScripts(ref scriptOffsets, ref useScripts);

                // Write Functions to Stream
                var writeFunctionResult = WriteFunctions(scriptFileData, writer, ref scriptOffsets, ref functionOffsets, ref callerReferences);
                if (!writeFunctionResult.Success)
                {
                    return writeFunctionResult; // Return error if writing functions failed
                }

                // Align Actions
                AlignActions(writer);

                // Write Actions to Stream
                WriteActions(scriptFileData, writer, ref actionOffsets);

                // Write Script Offsets
                WriteScriptOffsets(writer, scriptOffsets);

                // Write Caller References
                WriteCallerReferences(writer, callerReferences, functionOffsets, actionOffsets);

                // Write to the file
                string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.scripts].unpackedDirectory}\\{scriptFileData.ScriptFileId:D4}";
                File.WriteAllBytes(directory, stream.ToArray());

                return (true, "");
            }
            catch (Exception ex)
            {
                // Log or throw a detailed exception
                Console.WriteLine($"Error writing script data: {ex.Message}");
                return (false, ex.Message);
            }
        }

        private static void WriteScripts(ScriptFileData scriptFileData, BinaryWriter writer, ref List<ScriptReference> scriptOffsets, ref List<ScriptData> useScripts, ref List<CallerReference> callerReferences)
        {
            foreach (var script in scriptFileData.Scripts)
            {
                if (!script.UsedScriptId.HasValue)
                {
                    scriptOffsets.Add(new ScriptReference(script.ScriptNumber, (uint)writer.BaseStream.Position));
                    foreach (var line in script.Lines)
                    {
                        writer.Write((ushort)line.ScriptCommandId);
                        List<byte[]> parameters = line.Parameters;
                        foreach (byte[] parameter in parameters)
                        {
                            writer.Write(parameter);
                        }
                        AddReference(ref callerReferences, (ushort)line.ScriptCommandId, parameters, (int)writer.BaseStream.Position, script);
                    }
                }
                else
                {
                    useScripts.Add(script);
                }
            }
        }

        private static void HandleUseScripts(ref List<ScriptReference> scriptOffsets, ref List<ScriptData> useScripts)
        {
            foreach (var useScript in useScripts)
            {
                var matchingScript = scriptOffsets.FirstOrDefault(s => s.Id == useScript.UsedScriptId);

                if (!matchingScript.Equals(default(ScriptReference)))
                {
                    scriptOffsets.Add(new ScriptReference(useScript.ScriptNumber, matchingScript.Offset));
                }
            }
        }

        private static (bool Success, string ErrorMessage) WriteFunctions(ScriptFileData scriptFileData, BinaryWriter writer, ref List<ScriptReference> scriptOffsets, ref List<ScriptReference> functionOffsets, ref List<CallerReference> callerReferences)
        {
            foreach (var function in scriptFileData.Functions)
            {
                if (!function.UsedScriptId.HasValue)
                {
                    functionOffsets.Add(new ScriptReference(function.ScriptNumber, (uint)writer.BaseStream.Position));

                    foreach (var line in function.Lines)
                    {
                        writer.Write((ushort)line.ScriptCommandId);
                        List<byte[]> parameters = line.Parameters;
                        foreach (byte[] parameter in parameters)
                        {
                            writer.Write(parameter);
                        }
                        AddReference(ref callerReferences, (ushort)line.ScriptCommandId, parameters, (int)writer.BaseStream.Position, function);
                    }
                }
                else
                {
                    int useScriptIndex = function.UsedScriptId.Value - 1;
                    if (useScriptIndex >= scriptOffsets.Count)
                    {
                        return (false, $"Function #{function.ScriptNumber} refers to Script {function.UsedScriptId}, which does not exist.\nThis Script File cannot be saved.");
                    }
                    else
                    {
                        functionOffsets.Add(new ScriptReference(function.ScriptNumber, scriptOffsets[useScriptIndex].Offset));
                    }
                }
            }
            return (true, "");
        }

        private static void AlignActions(BinaryWriter writer)
        {
            if (writer.BaseStream.Position % 2 == 1)
            {
                writer.Write((byte)0x00); // Padding byte for halfword alignment
            }
        }

        private static void WriteActions(ScriptFileData scriptFileData, BinaryWriter writer, ref List<ScriptReference> actionOffsets)
        {
            foreach (var action in scriptFileData.Actions)
            {
                actionOffsets.Add(new ScriptReference(action.ActionNumber, (uint)writer.BaseStream.Position));
                foreach (var line in action.Lines)
                {
                    writer.Write((ushort)line.ActionCommandId);
                    writer.Write((ushort)line.Repetitions);
                }
            }
        }

        private static void WriteScriptOffsets(BinaryWriter writer, List<ScriptReference> scriptOffsets)
        {
            writer.BaseStream.Position = 0x0;
            scriptOffsets = scriptOffsets.OrderBy(x => x.Id).ToList();
            foreach (var scriptReference in scriptOffsets)
            {
                writer.Write(scriptReference.Offset - (uint)writer.BaseStream.Position - 0x4);
            }
        }

        private static void WriteCallerReferences(BinaryWriter writer, List<CallerReference> callerReferences, List<ScriptReference> functionOffsets, List<ScriptReference> actionOffsets)
        {
            foreach (var reference in callerReferences)
            {
                writer.BaseStream.Position = reference.Offset;

                ScriptReference result = reference.InvokedType == ScriptType.Action
                    ? actionOffsets.Find(entry => entry.Id == reference.InvokedId)
                    : functionOffsets.Find(entry => entry.Id == reference.InvokedId);

                if (!result.Equals(default(ScriptReference)))
                {
                    int relativeOffset = (int)(result.Offset - reference.Offset - 4);
                    writer.Write(relativeOffset);
                }
            }
        }


        #endregion Write
    }
}
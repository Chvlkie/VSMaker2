using System.Net.Http.Headers;
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

            while (!endScript)
            {
                var line = ReadScriptLine(reader, ref functionOffsets, ref actionOffsets);
                if (line.Parameters is not null)
                {
                    lines.Add(line);
                    if (ScriptDatabase.endCodes.Contains((ushort)line.ScriptCommandId))
                    {
                        endScript = true;
                    }
                }
            }

            return new ScriptData(index + 1, ScriptType.Script, lines: lines);
        }

        private ScriptData GetFunctionData(BinaryReader reader, uint index, ref List<int> functionOffsets, ref List<int> actionOffsets)
        {
            List<ScriptLine> lines = [];
            bool endScript = false;

            while (!endScript)
            {
                var line = ReadScriptLine(reader, ref functionOffsets, ref actionOffsets);
                if (line.Parameters is not null)
                {
                    lines.Add(line);
                    if (ScriptDatabase.endCodes.Contains((ushort)line.ScriptCommandId))
                    {
                        endScript = true;
                    }
                }
            }

            return new ScriptData(index + 1, ScriptType.Function, lines: lines);
        }

        private void AddParametersToList(ref List<byte[]> parameterList, ushort id, BinaryReader dataReader)
        {
            Console.WriteLine("Loaded command id: " + id.ToString("X4"));
            try
            {
                foreach (int bytesToRead in RomFile.ScriptCommandParametersDict[id])
                {
                    parameterList.Add(dataReader.ReadBytes(bytesToRead));
                }
            }
            catch (NullReferenceException)
            {
                parameterList = null;
                return;
            }
            catch
            {
                parameterList = null;
                return;
            }
        }

        private void ProcessRelativeJump(BinaryReader reader, ref List<byte[]> parameters, ref List<int> offsetsList)
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
                throw new InvalidOperationException();
            }

            parameters.Add(BitConverter.GetBytes(functionNumber + 1));
        }

        private ScriptLine ReadScriptLine(BinaryReader reader, ref List<int> functionOffsets, ref List<int> actionOffsets)
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
                            parameterList.Add(new byte[] { reader.ReadByte() });
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
                                parameterList.Add(new byte[] { parameter1 });
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
                                parameterList.Add(new byte[] { parameter1 });
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
                            parameterList.Add(new byte[] { reader.ReadByte() }); //in the case of JumpIf and CallIf, the first param is a comparisonOperator
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
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.scripts].unpackedDirectory}";
            for (int i = 0; i < new DirectoryInfo(directory).GetFiles().Length; i++)
            {
                scripts.Add(GetScriptFileData(i));
            }
            return scripts;
        }
        public ScriptFileData GetScriptFileData(int scriptFileId)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.scripts].unpackedDirectory}\\{scriptFileId:D4}";
            var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);

            List<int> scriptOffsets = [];
            List<int> functionOffsets = [];
            List<int> actionOffsets = [];

            List<ScriptData> scripts = [];
            List<ScriptData> functions = [];
            List<ScriptActionData> actions = [];
            using BinaryReader reader = new(fileStream);
            bool isLevelScript = true;
            try
            {
                while (true)
                {
                    uint checker = reader.ReadUInt16();
                    reader.BaseStream.Position -= 0x2;
                    uint value = reader.ReadUInt32();

                    // Script is a level script
                    if (value == 0 && scriptOffsets.Count == 0)
                    {
                        isLevelScript = true;
                        break;
                    }
                    // Script is not a level script
                    if (checker == 0xFD13)
                    {
                        reader.BaseStream.Position -= 0x4;
                        isLevelScript = false;
                        break;
                    }

                    int offsetFromStart = (int)(value + reader.BaseStream.Position);
                    scriptOffsets.Add(offsetFromStart);
                }

                // Begin reading Script file
                if (!isLevelScript)
                {
                    // Script Data
                    for (uint i = 0; i < scriptOffsets.Count; i++)
                    {
                        int index = scriptOffsets.FindIndex(x => x == scriptOffsets[(int)i]); // Check for UseScript
                        if (index == i)
                        {
                            int offset = scriptOffsets[(int)i];
                            scripts.Add(GetScriptData(reader, i, offset, ref functionOffsets, ref actionOffsets));
                        }
                        else
                        {
                            scripts.Add(new ScriptData(i + 1, ScriptType.Script, index + 1));
                        }
                    }

                    // Function Data
                    for (uint i = 0; i < functionOffsets.Count; i++)
                    {
                        reader.BaseStream.Position = functionOffsets[(int)i];
                        int index = scriptOffsets.IndexOf(functionOffsets[(int)i]); // Check for UseScript

                        if (index == -1)
                        {
                            functions.Add(GetFunctionData(reader, i, ref functionOffsets, ref actionOffsets));
                        }
                        else
                        {
                            functions.Add(new ScriptData(i + 1, ScriptType.Function, index + 1));
                        }
                    }

                    // Action Data
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
                    Console.WriteLine("Could not read script file");
                    reader.Close();
                    fileStream.Close();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();
                fileStream.Close();
                throw;
            }
            return new ScriptFileData(scriptFileId, isLevelScript, scripts, functions, actions);
        }

        #endregion Get

        private struct ScriptReference
        {
            public uint Id;
            public uint Offset;

            public ScriptReference(uint id, uint offset)
            {
                Id = id;
                Offset = offset;
            }
        }

        private struct CallerReference
        {
            public ScriptType ScriptType;
            public uint CallerId;
            public ScriptType InvokedType;
            public uint InvokedId;
            public int Offset;

            public CallerReference(ScriptType scriptType, uint callerId, ScriptType invokedType, uint invokedId, int position)
            {
                ScriptType = scriptType;
                CallerId = callerId;
                InvokedType = invokedType;
                InvokedId = invokedId;
                Offset = position;
            }
        }

        private void AddReference(ref List<CallerReference> callerReferences, ushort commandId, List<byte[]> parameters, int position, ScriptData script)
        {
            if (ScriptDatabase.commandsWithRelativeJump.TryGetValue(commandId, out int parameterWithRelativeJump))
            {
                uint invokedId = BitConverter.ToUInt32(parameters[parameterWithRelativeJump], 0); // Jump, Call

                switch (commandId)
                {
                    case 0x005E:
                        callerReferences.Add(new CallerReference(script.ScriptType, script.ScriptNumber, ScriptType.Action, invokedId, position - 4));
                        break;

                    default:
                        callerReferences.Add(new CallerReference(script.ScriptType, script.ScriptNumber, ScriptType.Function, invokedId, position - 4));
                        break;
                }
            }
        }

        #region Write

        public (bool Success, string ErrorMessage) WriteScriptData(ScriptFileData scriptFileData)
        {
            List<ScriptReference> scriptOffsets = [];
            List<ScriptReference> functionOffsets = [];
            List<ScriptReference> actionOffsets = [];
            List<CallerReference> callerReferences = [];

            MemoryStream stream = new();
            using BinaryWriter writer = new(stream);
            try
            {
                writer.BaseStream.Position += scriptFileData.Scripts.Count * 0x4;
                writer.Write((ushort)0xFD13);
                List<ScriptData> useScripts = [];

                // Write Scripts
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

                foreach (var useScript in useScripts)
                {
                    for (int i = 0; i < scriptOffsets.Count; i++)
                    {
                        var scriptReference = scriptOffsets[i];
                        if (scriptReference.Id == useScript.UsedScriptId)
                        {
                            scriptOffsets.Add(new ScriptReference(useScript.ScriptNumber, scriptReference.Offset));
                        }
                    }
                }

                // Write Functions
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
                        int useScript = function.UsedScriptId.Value - 1;
                        if (useScript >= scriptOffsets.Count)
                        {
                            return (false, $"Function #{function.ScriptNumber}refers to Script {function.UsedScriptId}, which does not exist." +
                                "\nThis Script File cannot be saved.");
                        }
                        else
                        {
                            functionOffsets.Add(new ScriptReference(function.ScriptNumber, scriptOffsets[function.UsedScriptId.Value - 1].Offset));
                        }
                    }
                }

                //Halfword align Actions
                if (writer.BaseStream.Position % 2 == 1)
                {
                    writer.Write((byte)0x00);//Padding
                }

                // Write Actions
                foreach (var action in scriptFileData.Actions)
                {
                    actionOffsets.Add(new ScriptReference(action.ActionNumber, (uint)writer.BaseStream.Position));
                    foreach (var line in action.Lines)
                    {
                        writer.Write((ushort)line.ActionCommandId);
                        writer.Write((ushort)line.Repetitions);
                    }
                }

                // Write Script Offsets
                writer.BaseStream.Position = 0x0;
                scriptOffsets = scriptOffsets.OrderBy(x => x.Id).ToList();
                for (int i = 0; i < scriptOffsets.Count; i++)
                {
                    writer.Write(scriptOffsets[i].Offset - (uint)writer.BaseStream.Position - 0x4);
                }

                for (int i = 0; i < callerReferences.Count; i++)
                {
                    writer.BaseStream.Position = callerReferences[i].Offset;
                    ScriptReference result;
                    if (callerReferences[i].InvokedType == ScriptType.Action)
                    {
                        result = actionOffsets.Find(entry => entry.Id == callerReferences[i].InvokedId);
                        int relativeOffset = (int)(result.Offset - callerReferences[i].Offset - 4);
                        writer.Write(relativeOffset);
                    }
                    else
                    {
                        result = functionOffsets.Find(entry => entry.Id == callerReferences[i].InvokedId);
                        int relativeOffset = (int)(result.Offset - callerReferences[i].Offset - 4);
                        writer.Write(relativeOffset);
                    }
                }

                string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.scripts].unpackedDirectory}\\{scriptFileData.ScriptFileId:D4}";
                File.WriteAllBytes(directory, stream.ToArray());
                writer.Dispose();
                stream.Close();
                return (true, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                writer.Dispose();
                return (false, ex.Message);
            }
        }

        #endregion Write
    }
}
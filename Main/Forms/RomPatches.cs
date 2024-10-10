using Main.Resources.Toolbox;
using System.Text;
using VsMaker2Core.DataModels;
using VsMaker2Core.DsUtils;
using VsMaker2Core.DSUtils;
using VsMaker2Core.Methods;
using VsMaker2Core.RomFiles;
using static Main.Resources.Toolbox.ToolboxDB;
using static VsMaker2Core.Enums;

namespace Main.Forms
{
    public partial class RomPatches : Form
    {
        public static uint ExpandedArmFileId = ToolboxDB.SyntheticOverlayFileNumbersDB[RomFile.GameFamily];
        public static bool LoadOverlay1FromBackup;
        public string BackupSuffix = ".backup";
        private readonly Mainform Main;
        private IRomFileMethods romFileMethods;

        public RomPatches(Mainform main, IRomFileMethods romFileMethods)
        {
            InitializeComponent();
            this.romFileMethods = romFileMethods;
            Main = main;

            btn_TrainerName.Enabled = !RomFile.TrainerNameExpansion;
            checkBox_TrainerNames.Checked = RomFile.TrainerNameExpansion;
            if (RomFile.GameLanguage == GameLanguage.Spanish || RomFile.GameLanguage == GameLanguage.English && !RomFile.Arm9Expanded)
            {
                RomFile.Arm9Expanded = CheckFilesArm9ExpansionApplied();
            }
            patchArm9Btn.Enabled = !RomFile.Arm9Expanded;
            arm9PatchCheckBox.Checked = RomFile.Arm9Expanded;
            btn_expandTrainerClass.Enabled = RomFile.Arm9Expanded && !RomFile.PrizeMoneyExpanded && !RomFile.EyeContactExpanded && !RomFile.ClassGenderExpanded;
            checkBox2.Checked = RomFile.Arm9Expanded && !btn_expandTrainerClass.Enabled;
        }

        public static bool CheckFilesArm9ExpansionApplied()
        {
            ARM9PatchData data = new();

            byte[] branchCode = Arm9.HexStringToByteArray(data.branchString);
            byte[] branchCodeRead = Arm9.ReadBytes(data.branchOffset, data.branchString.Length / 3 + 1);
            if (branchCodeRead.Length != branchCode.Length || !branchCodeRead.SequenceEqual(branchCode))
                return false;

            byte[] initCode = Arm9.HexStringToByteArray(data.initString);
            byte[] initCodeRead = Arm9.ReadBytes(data.initOffset, data.initString.Length / 3 + 1);
            if (initCodeRead.Length != initCode.Length || !initCodeRead.SequenceEqual(initCode))
                return false;

            return true;
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }
        }

        public static bool ExpandTrainerNames()
        {
            using Arm9.Arm9Writer writer = new(RomFile.TrainerNameMaxByteOffset);
            try
            {
                writer.Write((byte)12);
                writer.Close();
                MessageBox.Show("Patch applied!\nYou can now have trainer names up to 16 characters!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RomFile.TrainerNameMaxByte = 12;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "Unable to Patch ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                writer.Close();
                return false;
            }
        }

        public static async Task<(bool Success, string Error)> RepointEyeContactTableAsync()
        {
            uint newPointer = RomFile.SynthOverlayLoadAddress + RomFile.EyeContactRepointOffset;

            try
            {
                uint pointerOffset = RomFile.EyeContactMusicTableOffsetToRam;

                using (FileStream arm9Stream = new(RomFile.Arm9Path, FileMode.Open, FileAccess.Write))
                using (BinaryWriter writer = new(arm9Stream))
                {
                    arm9Stream.Position = pointerOffset;

                    writer.Write(BitConverter.GetBytes(newPointer));
                }
                RomFile.EyeContactExpanded = true;
                return (true, "Eye contact music table successfully repointed.");
            }
            catch (Exception ex)
            {
                return (false, $"Error during repointing: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Error)> RepointClassGenderTableAsync()
        {
            uint newPointer = RomFile.SynthOverlayLoadAddress + RomFile.ClassGenderRepointOffset;

            try
            {
                // ClassGenderOffsetToRamAddress is the offset where the pointer for the table is stored in arm9.bin
                uint pointerOffset = RomFile.ClassGenderOffsetToRamAddress;

                // Now, write the new pointer to the ClassGenderOffsetToRamAddress in the arm9.bin
                using (FileStream arm9Stream = new(RomFile.Arm9Path, FileMode.Open, FileAccess.Write))
                using (BinaryWriter writer = new(arm9Stream))
                {
                    // Seek to the pointer location in arm9.bin
                    arm9Stream.Position = pointerOffset;

                    // Write the new pointer (in little-endian format) to arm9.bin
                    writer.Write(BitConverter.GetBytes(newPointer));
                }
                RomFile.ClassGenderExpanded = true;
                return (true, "Class gender table successfully repointed.");
            }
            catch (Exception ex)
            {
                return (false, $"Error during repointing: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Error)> RepointPrizeMoneyTableAsync()
        {
            uint newPointer = RomFile.SynthOverlayLoadAddress + RomFile.PrizeMoneyRepointOffset; // Example offset: 0x1100
            uint newPointerPlusTwo = newPointer + 2; // For HGSS at offset 0x8384

            uint pointerOffset = RomFile.GameFamily switch
            {
                GameFamily.DiamondPearl => 0x782C,
                GameFamily.Platinum => 0x816C,
                GameFamily.HeartGoldSoulSilver => 0x8380,
                _ => throw new ArgumentException("Unsupported game family")
            };

            try
            {
                string overlayFilePath = Overlay.OverlayFilePath(RomFile.PrizeMoneyTableOverlayNumber);

                // Check if HGSS, then decompress overlay if necessary
                if (RomFile.IsHeartGoldSoulSilver)
                {
                    bool isCompressed = await Overlay.CheckOverlayIsCompressedAsync(RomFile.PrizeMoneyTableOverlayNumber);
                    if (isCompressed)
                    {
                        await Overlay.DecompressOverlayAsync(RomFile.PrizeMoneyTableOverlayNumber);
                        Overlay.SetOverlayCompressionInTable(RomFile.PrizeMoneyTableOverlayNumber, 0);
                    }
                }

                // Backup overlay file before modification
                string backupPath = overlayFilePath + "_backup";
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
                await CopyFileAsync(overlayFilePath, backupPath);

                // Perform the repoint operation
                using (FileStream overlayStream = new(overlayFilePath, FileMode.Open, FileAccess.Write))
                using (BinaryWriter writer = new(overlayStream))
                {
                    // Write the new pointer
                    overlayStream.Position = pointerOffset;
                    writer.Write(BitConverter.GetBytes(newPointer));

                    // For HGSS, write additional pointer at offset 0x8384
                    if (RomFile.IsHeartGoldSoulSilver)
                    {
                        overlayStream.Position = 0x8384;
                        writer.Write(BitConverter.GetBytes(newPointerPlusTwo));
                    }
                }

                RomFile.PrizeMoneyExpanded = true;

                return (true, "Prize money table successfully repointed.");
            }
            catch (Exception ex)
            {
                return (false, $"Error during repointing: {ex.Message}");
            }
        }

        public static async Task WritePrizeMoneyTableAsync(List<PrizeMoneyData> newPrizeMoneyData)
        {
            string synthOverlayPath = RomFile.SynthOverlayFilePath; // The path to the synthetic overlay

            try
            {
                using FileStream stream = new(synthOverlayPath, FileMode.Open, FileAccess.Write);
                using BinaryWriter writer = new(stream);

                uint newPrizeMoneyOffset = RomFile.PrizeMoneyRepointOffset;

                stream.Position = newPrizeMoneyOffset;

                foreach (var item in newPrizeMoneyData)
                {
                    if (RomFile.IsHeartGoldSoulSilver)
                    {
                        writer.Write(item.TrainerClassId);
                        writer.Write(item.PrizeMoney);
                    }
                    else
                    {
                        writer.Write((byte)item.PrizeMoney);
                    }
                }

                await stream.FlushAsync(); // Ensure the data is written to disk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while writing prize money table: {ex.Message}");
                throw;
            }
        }

        public static async Task WriteClassGenderTableAsync(List<ClassGenderData> newClassGenderData)
        {
            string synthOverlayPath = RomFile.SynthOverlayFilePath; // The path to the synthetic overlay

            try
            {
                using FileStream stream = new(synthOverlayPath, FileMode.Open, FileAccess.Write);
                using BinaryWriter writer = new(stream);

                uint offset = RomFile.ClassGenderRepointOffset;

                stream.Position = offset;

                foreach (var item in newClassGenderData)
                {
                    writer.Write(item.Gender);
                }

                await stream.FlushAsync(); // Ensure the data is written to disk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while writing class gender table: {ex.Message}");
                throw;
            }
        }

        public static async Task WriteEyeContactTableAsync(List<EyeContactMusicData> newEyeContactData)
        {
            string synthOverlayPath = RomFile.SynthOverlayFilePath; // The path to the synthetic overlay

            try
            {
                using FileStream stream = new(synthOverlayPath, FileMode.Open, FileAccess.Write);
                using BinaryWriter writer = new(stream);

                uint offset = RomFile.EyeContactRepointOffset;

                stream.Position = offset;

                foreach (var item in newEyeContactData)
                {
                    writer.Write(item.TrainerClassId);
                    writer.Write(item.MusicDayId);
                    if (RomFile.IsHeartGoldSoulSilver)
                    {
                        writer.Write(item.MusicNightId ?? 0);
                    }
                }

                await stream.FlushAsync(); // Ensure the data is written to disk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while writing class gender table: {ex.Message}");
                throw;
            }
        }

        public List<ClassGenderData> CreateClassGenderTable()
        {
            List<ClassGenderData> original = RomFile.ClassGenderData;
            List<ClassGenderData> newData = new(150);

            for (int i = 0; i < 150; i++)
            {
                var index = original.FindIndex(x => x.TrainerClassId == (ushort)i);
                var item = new ClassGenderData();
                if (index > -1)
                {
                    item.Gender = original[index].Gender;
                    item.TrainerClassId = original[index].TrainerClassId;
                }
                else
                {
                    item.Gender = 0;
                    item.TrainerClassId = (ushort)i;
                }
                newData.Add(item);
            }
            return newData;
        }

        public List<EyeContactMusicData> CreateEyeContactTable()
        {
            List<EyeContactMusicData> original = RomFile.EyeContactMusicData;
            List<EyeContactMusicData> newData = new(150);

            for (int i = 0; i < 150; i++)
            {
                var index = original.FindIndex(x => x.TrainerClassId == (ushort)i);
                var item = new EyeContactMusicData();
                if (index > -1)
                {
                    item.MusicDayId = original[index].MusicDayId;
                    item.MusicNightId = RomFile.IsHeartGoldSoulSilver ? original[index].MusicNightId : null;
                    item.TrainerClassId = original[index].TrainerClassId;
                }
                else
                {
                    item.MusicDayId = 0;
                    item.MusicNightId = RomFile.IsHeartGoldSoulSilver ? 0 : null;
                    item.TrainerClassId = (ushort)i;
                }
                newData.Add(item);
            }
            return newData;
        }

        public List<PrizeMoneyData> CreatePrizeMoneyTable()
        {
            List<PrizeMoneyData> originalPrizeMoney = RomFile.PrizeMoneyData;
            List<PrizeMoneyData> newPrizeMoneyData = new(150);

            for (int i = 0; i < 150; i++)
            {
                var index = originalPrizeMoney.FindIndex(x => x.TrainerClassId == (ushort)i);
                var item = new PrizeMoneyData();
                if (index > -1)
                {
                    item.PrizeMoney = originalPrizeMoney[index].PrizeMoney;
                    item.TrainerClassId = originalPrizeMoney[index].TrainerClassId;
                }
                else
                {
                    item.PrizeMoney = 0;
                    item.TrainerClassId = (ushort)i;
                }
                newPrizeMoneyData.Add(item);
            }
            return newPrizeMoneyData;
        }

        private static void ApplyArm9Patch(ARM9PatchData data)
        {
            Arm9.WriteBytes(Arm9.HexStringToByteArray(data.branchString), data.branchOffset);
            Arm9.WriteBytes(Arm9.HexStringToByteArray(data.initString), data.initOffset);
        }

        private static void ModifyExpandedFile()
        {
            string fullFilePath = $"{VsMaker2Core.Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.synthOverlay].unpackedDirectory}\\{ExpandedArmFileId:D4}";

            File.Delete(fullFilePath);
            using BinaryWriter writer = new(File.Create(fullFilePath));

            byte[] buffer = new byte[0x16000];
            writer.Write(buffer);
        }

        private async void btn_expandTrainerClass_Click(object sender, EventArgs e)
        {
            // Show a warning message to the user
            DialogResult result = MessageBox.Show(
                "Warning: This process will modify the ROM and may cause irreversible changes. " +
                "It is strongly recommended that you create a backup before proceeding.\n\n" +
                "This process will repoint the Prize Money Table, Trainer Class Gender Table, " +
                "and Eye Contact Music Table, allowing up to 150 trainer classes. " +
                "This operation will overwrite approximately 2-3 KB in the synthetic overlay.\n\n" +
                "Do you wish to continue?",
                "Caution: ROM Modification",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = true;
                string errorMessage = "";

                string backupPath = RomFile.Arm9Path + "_backup";
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
                await CopyFileAsync(RomFile.Arm9Path, backupPath);

                var prizeMoneyResult = await RepointPrizeMoneyTableAsync();
                if (!prizeMoneyResult.Success)
                {
                    success = false;
                    errorMessage = prizeMoneyResult.Error;
                }

                if (success)
                {
                    var newPrizeMoneyTable = CreatePrizeMoneyTable();
                    await WritePrizeMoneyTableAsync(newPrizeMoneyTable);
                    RomFile.PrizeMoneyData = await romFileMethods.GetPrizeMoneyDataAsync();
                }

                if (success)
                {
                    var genderResult = await RepointClassGenderTableAsync();
                    if (!genderResult.Success)
                    {
                        success = false;
                        errorMessage = genderResult.Error;
                    }
                }

                if (success)
                {
                    var newClassGenderData = CreateClassGenderTable();
                    await WriteClassGenderTableAsync(newClassGenderData);
                    RomFile.ClassGenderData = romFileMethods.GetClassGenders(150, RomFile.ClassGenderOffsetToRamAddress);
                }

                if (success)
                {
                    var eyeContactRepoint = await RepointEyeContactTableAsync();
                    if (!eyeContactRepoint.Success)
                    {
                        success = false;
                        errorMessage = eyeContactRepoint.Error;
                    }
                }

                if (success)
                {
                    var newEyeContactData = CreateEyeContactTable();
                    await WriteEyeContactTableAsync(newEyeContactData);
                    RomFile.EyeContactMusicData = romFileMethods.GetEyeContactMusicData(RomFile.EyeContactMusicTableOffsetToRam, RomFile.GameFamily);
                }

                // Handle the result
                if (success)
                {
                    MessageBox.Show("All tables repointed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Update ROM file status to indicate all tables are expanded
                    RomFile.PrizeMoneyExpanded = true;
                    RomFile.ClassGenderExpanded = true;
                    RomFile.EyeContactExpanded = true;

                    Main.RefreshTrainerClasses();
                    // Disable the patch button since the patch has been applied
                    btn_expandTrainerClass.Enabled = false;
                }
                else
                {
                    MessageBox.Show($"Failed to repoint tables: {errorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Operation canceled. No changes were made.", "Operation Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_TrainerName_Click(object sender, EventArgs e)
        {
            if (ExpandTrainerNames())
            {
                btn_TrainerName.Enabled = !RomFile.TrainerNameExpansion;
                checkBox_TrainerNames.Checked = RomFile.TrainerNameExpansion;
            }
        }

        private void patchArm9Btn_Click(object sender, EventArgs e)
        {
            ARM9PatchData data = new();

            // Prepare the confirmation message using StringBuilder for efficiency
            var message = new StringBuilder();
            message.AppendLine("Confirming this process will apply the following changes:")
                   .AppendLine()
                   .AppendLine($"- Backup ARM9 file (arm9.bin{BackupSuffix} will be created).")
                   .AppendLine()
                   .AppendLine($"- Replace {(data.branchString.Length / 3 + 1)} bytes of data at arm9 offset 0x{data.branchOffset:X} with")
                   .AppendLine(data.branchString)
                   .AppendLine()
                   .AppendLine($"- Replace {(data.initString.Length / 3 + 1)} bytes of data at arm9 offset 0x{data.initOffset:X} with")
                   .AppendLine(data.initString)
                   .AppendLine()
                   .AppendLine($"- Modify file #{ExpandedArmFileId} inside")
                   .AppendLine($"{VsMaker2Core.Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.synthOverlay].unpackedDirectory}")
                   .AppendLine("to accommodate for 88KB of data (no backup).")
                   .AppendLine()
                   .AppendLine("Do you wish to continue?");

            DialogResult result = MessageBox.Show(message.ToString(), "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Copy(RomFile.Arm9Path, RomFile.Arm9Path + BackupSuffix, overwrite: true);

                    ApplyArm9Patch(data);
                    ModifyExpandedFile();
                    UpdateUIControlsAfterPatch();

                    MessageBox.Show("The ARM9's usable memory has been expanded.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Operation failed: {ex.Message}. It is strongly advised that you restore the arm9 backup (arm9.bin{BackupSuffix}).", "Something went wrong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateUIControlsAfterPatch()
        {
            RomFile.Arm9Expanded = true;
            patchArm9Btn.Enabled = false;
            arm9PatchCheckBox.Checked = RomFile.Arm9Expanded;
            btn_expandTrainerClass.Enabled = RomFile.Arm9Expanded && !RomFile.PrizeMoneyExpanded && !RomFile.ClassGenderExpanded && !RomFile.EyeContactExpanded;
        }
    }
}
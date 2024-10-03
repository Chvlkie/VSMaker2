using Main.Resources.Toolbox;
using System.Text;
using VsMaker2Core.DataModels;
using VsMaker2Core.DSUtils;
using static Main.Resources.Toolbox.ToolboxDB;
using static VsMaker2Core.Enums;

namespace Main.Forms
{
    public partial class RomPatches : Form
    {
        public static uint ExpandedArmFileId = ToolboxDB.SyntheticOverlayFileNumbersDB[RomFile.GameFamily];
        public static bool LoadOverlay1FromBackup;
        public string BackupSuffix = ".backup";
        private readonly RomFile LoadedRom;

        public RomPatches(RomFile loadedRom)
        {
            InitializeComponent();
            LoadedRom = loadedRom;
            if (loadedRom != null)
            {
                btn_TrainerName.Enabled = !RomFile.TrainerNameExpansion;
                checkBox_TrainerNames.Checked = RomFile.TrainerNameExpansion;
                if (RomFile.GameLanguage == GameLanguage.Spanish || RomFile.GameLanguage == GameLanguage.English && !RomFile.Arm9Expanded)
                {
                    RomFile.Arm9Expanded = CheckFilesArm9ExpansionApplied();
                }
                patchArm9Btn.Enabled = !RomFile.Arm9Expanded;
                arm9PatchCheckBox.Checked = RomFile.Arm9Expanded;
                btn_expandClassGender.Enabled = RomFile.Arm9Expanded && !RomFile.ClassGenderExpanded;
                btn_ExpandEyeContact.Enabled = RomFile.Arm9Expanded && !RomFile.EyeContactExpanded;
                btn_expandPrizeMoney.Enabled = RomFile.Arm9Expanded && !RomFile.PrizeMoneyExpanded;
            }
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

        public static bool ExpandTrainerNames(RomFile loadedRom)
        {
            using Arm9.Arm9Writer writer = new(loadedRom.TrainerNameMaxByteOffset);
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

        private void btn_TrainerName_Click(object sender, EventArgs e)
        {
            if (ExpandTrainerNames(LoadedRom))
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

        private void UpdateUIControlsAfterPatch()
        {
            RomFile.Arm9Expanded = true;
            patchArm9Btn.Enabled = false;
            arm9PatchCheckBox.Checked = RomFile.Arm9Expanded;
            btn_expandClassGender.Enabled = RomFile.Arm9Expanded && !RomFile.ClassGenderExpanded;
            btn_ExpandEyeContact.Enabled = RomFile.Arm9Expanded && !RomFile.EyeContactExpanded;
            btn_expandPrizeMoney.Enabled = RomFile.Arm9Expanded && !RomFile.PrizeMoneyExpanded;
        }

        private void btn_expandPrizeMoney_Click(object sender, EventArgs e)
        {

        }
    }
}
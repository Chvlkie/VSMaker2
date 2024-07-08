using VsMaker2Core.DataModels;
using VsMaker2Core.DSUtils;

namespace Main.Forms
{
    public partial class RomPatches : Form
    {
        private RomFile LoadedRom;
        public static bool LoadOverlay1FromBackup;
        public RomPatches(RomFile loadedRom)
        {
            InitializeComponent();
            LoadedRom = loadedRom;
            if (loadedRom != null)
            {
                btn_TrainerName.Enabled = !RomFile.TrainerNameExpansion;
                checkBox_TrainerNames.Checked = RomFile.TrainerNameExpansion;
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
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VsMaker2Core.DataModels;
using VsMaker2Core.DSUtils;

namespace Main.Forms
{
    public partial class RomPatches : Form
    {
        private RomFile LoadedRom;
        public RomPatches(RomFile loadedRom)
        {
            InitializeComponent();
            LoadedRom = loadedRom;
            if (loadedRom != null)
            {
                btn_TrainerName.Enabled = !LoadedRom.TrainerNameExpansion;
                checkBox_TrainerNames.Checked = LoadedRom.TrainerNameExpansion;
            }
        }

        private void btn_TrainerName_Click(object sender, EventArgs e)
        {
            if (ExpandTrainerNames(LoadedRom))
            {
                btn_TrainerName.Enabled = !LoadedRom.TrainerNameExpansion;
                checkBox_TrainerNames.Checked = LoadedRom.TrainerNameExpansion;
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
                loadedRom.TrainerNameMaxByte = 12;
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

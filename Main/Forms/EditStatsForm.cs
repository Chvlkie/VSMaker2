﻿namespace Main.Forms
{
    public partial class EditStatsForm : Form
    {
        public ushort[] Stats = new ushort[6];
        private CheckedListBox AdditionalFlags;

        public EditStatsForm(ushort[] stats, CheckedListBox additionalFlags)
        {
            this.Stats = stats;
            this.AdditionalFlags = additionalFlags;
            InitializeComponent();
            SetStatValues();
            EnableDisableItems();
        }

        private void SetStatValues()
        {
            num_StatHp.Value = Stats[0];
            num_StatAtk.Value = Stats[1];
            num_StatDef.Value = Stats[2];
            num_StatSpd.Value = Stats[3];
            num_StatSpAtk.Value = Stats[4];
            num_StatSpDef.Value = Stats[5];
        }

        private void SaveStatValues()
        {
            Stats[0] = (ushort)num_StatHp.Value;
            Stats[1] = (ushort)num_StatAtk.Value;
            Stats[2] = (ushort)num_StatDef.Value;
            Stats[3] = (ushort)num_StatSpd.Value;
            Stats[4] = (ushort)num_StatSpAtk.Value;
            Stats[5] = (ushort)num_StatSpDef.Value;
        }

        private void EnableDisableItems()
        {
            num_StatHp.Enabled = AdditionalFlags.GetItemChecked(1);
            num_StatAtk.Enabled = AdditionalFlags.GetItemChecked(2);
            num_StatDef.Enabled = AdditionalFlags.GetItemChecked(3);
            num_StatSpd.Enabled = AdditionalFlags.GetItemChecked(4);
            num_StatSpAtk.Enabled = AdditionalFlags.GetItemChecked(5);
            num_StatSpDef.Enabled = AdditionalFlags.GetItemChecked(6);
        }

        private void EditStatsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveStatValues();
        }
    }
}
namespace Main.Forms
{
    public partial class HgeIvEvForm : Form
    {
        public ushort[] IVs = new ushort[6];
        public ushort[] EVs = new ushort[6];

        public HgeIvEvForm(ushort[] ivs, ushort[] evs)
        {
            this.IVs = ivs;
            this.EVs = evs;
            InitializeComponent();
            SetIvValues();
            SetEvValues();
        }

        private void SetIvValues()
        {
            num_IvHp.Value = IVs[0];
            num_IvAtk.Value = IVs[1];
            num_IvDef.Value = IVs[2];
            num_IvSpd.Value = IVs[3];
            num_IvSpAtk.Value = IVs[4];
            num_IvSpDef.Value = IVs[5];
        }

        private void SetEvValues()
        {
            num_EvHp.Value = EVs[0];
            num_EvAtk.Value = EVs[1];
            num_EvDef.Value = EVs[2];
            num_EvSpd.Value = EVs[3];
            num_EvSpAtk.Value = EVs[4];
            num_EvSpDef.Value = EVs[5];
        }

        private void SaveIvValues()
        {
            IVs[0] = (ushort)num_IvHp.Value;
            IVs[1] = (ushort)num_IvAtk.Value;
            IVs[2] = (ushort)num_IvDef.Value;
            IVs[3] = (ushort)num_IvSpd.Value;
            IVs[4] = (ushort)num_IvSpAtk.Value;
            IVs[5] = (ushort)num_IvSpDef.Value;
        }

        private void SaveEvValues()
        {
            EVs[0] = (ushort)num_EvHp.Value;
            EVs[1] = (ushort)num_EvAtk.Value;
            EVs[2] = (ushort)num_EvDef.Value;
            EVs[3] = (ushort)num_EvSpd.Value;
            EVs[4] = (ushort)num_EvSpAtk.Value;
            EVs[5] = (ushort)num_EvSpDef.Value;
        }

        private void HgeIvEvForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveIvValues();
            SaveEvValues();
        }
    }
}
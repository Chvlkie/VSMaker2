namespace Main.Forms
{
    public partial class HgeIvEvForm : Form
    {
        public byte[] IVs = new byte[6];
        public byte[] EVs = new byte[6];

        public HgeIvEvForm(byte[] ivs, byte[] evs)
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
            IVs[0] = (byte)num_IvHp.Value;
            IVs[1] = (byte)num_IvAtk.Value;
            IVs[2] = (byte)num_IvDef.Value;
            IVs[3] = (byte)num_IvSpd.Value;
            IVs[4] = (byte)num_IvSpAtk.Value;
            IVs[5] = (byte)num_IvSpDef.Value;
        }

        private void SaveEvValues()
        {
            EVs[0] = (byte)num_EvHp.Value;
            EVs[1] = (byte)num_EvAtk.Value;
            EVs[2] = (byte)num_EvDef.Value;
            EVs[3] = (byte)num_EvSpd.Value;
            EVs[4] = (byte)num_EvSpAtk.Value;
            EVs[5] = (byte)num_EvSpDef.Value;
        }

        private void HgeIvEvForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveIvValues();
            SaveEvValues();
        }
    }
}
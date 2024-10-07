namespace Main.Forms
{
    public partial class MoveSelector : Form
    {
        public ushort MoveId1;
        public ushort MoveId2;
        public ushort MoveId3;
        public ushort MoveId4;

        private bool IsLoadingData = false;
        private List<ComboBox> moveComboBoxes;
        private List<string> MoveNames;

        public MoveSelector(int partyIndex, string pokemonName, ushort[] moves, List<string> moveNames)
        {
            IsLoadingData = true;
            InitializeComponent();
            SetPokemonName(partyIndex, pokemonName);
            SetMoveIds(moves);
            moveComboBoxes =
                [
                move01ComboBox,
                move02ComboBox,
                move03ComboBox,
                move04ComboBox,
                ];

            MoveNames = moveNames;
            PopulateMoves();
            SetSelectedMoves(moves);
            IsLoadingData = false;
        }

        private void SetSelectedMoves(ushort[] moves)
        {
            for (int i = 0; i < moveComboBoxes.Count; i++)
            {
                moveComboBoxes[i].SelectedIndex = moves[i];
            }
        }

        private void PopulateMoves()
        {
            foreach (var comboBox in moveComboBoxes)
            {
                foreach (var move in MoveNames)
                {
                    comboBox.Items.Add(move);
                }
            }
        }

        private void SetMoveIds(ushort[] moves)
        {
            MoveId1 = moves[0];
            MoveId2 = moves[1];
            MoveId3 = moves[2];
            MoveId4 = moves[3];
        }

        private void SetPokemonName(int partyIndex, string pokemonName)
        {
            pokeName.Text = $"{(partyIndex + 1):D2} - {pokemonName}";
        }

        private void move01ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                MoveId1 = (ushort)move01ComboBox.SelectedIndex;
            }
        }

        private void move02ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                MoveId2 = (ushort)move02ComboBox.SelectedIndex;
            }
        }

        private void move03ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                MoveId3 = (ushort)move03ComboBox.SelectedIndex;
            }
        }

        private void move04ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                MoveId4 = (ushort)move04ComboBox.SelectedIndex;
            }
        }
    }
}
namespace Main
{
    public partial class mainform : Form
    {
        public mainform()
        {
            InitializeComponent();
            startupTab.Appearance = TabAppearance.FlatButtons; startupTab.ItemSize = new Size(0, 1); startupTab.SizeMode = TabSizeMode.Fixed;
        }
    }
}

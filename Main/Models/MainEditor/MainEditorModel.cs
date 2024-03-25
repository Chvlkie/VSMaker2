namespace Main.Models
{
    public class MainEditorModel
    {
        public TrainerEditorModel TrainerEditor { get; set; }
        public ClassEditorModel ClassEditor { get; set; }
        public BattleMessageEditorModel BattleMessageEditor { get; set; }

        public MainEditorModel()
        {
            TrainerEditor = new TrainerEditorModel();
            ClassEditor = new ClassEditorModel();
            BattleMessageEditor = new BattleMessageEditorModel();
        }
    }
}
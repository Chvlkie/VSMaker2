using Main.Forms;
using Main.Models;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace Main
{
    // CLASS EDITOR
    public partial class Mainform : Form
    {
        private List<string> UnfilteredClasses = [];
        private bool UnsavedClassChanges;

        private void SetupClassEditor()
        {
            IsLoadingData = true;
            if (class_ClassListBox.Items.Count == 0)
            {
                PopulateClassList(MainEditorModel.Classes);
            }
            IsLoadingData = false;
        }
        private void PopulateClassList(List<TrainerClass> classes)
        {
            class_ClassListBox.Items.Clear();
            UnfilteredClasses = [];
            foreach (var item in classes)
            {
                class_ClassListBox.Items.Add(item.ListName);
                UnfilteredClasses.Add(item.ListName);
            }
        }
    }
}
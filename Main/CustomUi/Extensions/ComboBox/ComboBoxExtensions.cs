using System;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Main.CustomUi.Extensions
{
    public static class ComboBoxExtensions
    {
        // Define a class to manage state for each ComboBox instance
        private class ComboBoxState
        {
            public bool IsUpdating { get; set; }
            public bool IsSettingIndex { get; set; }
            public bool IsRestoringOriginalList { get; set; }
            public Timer DropdownTimer { get; set; }
        }

        // Maintain state for each ComboBox instance
        private static readonly Dictionary<ComboBox, ComboBoxState> ComboBoxStates = new Dictionary<ComboBox, ComboBoxState>();

        // Method to enable search functionality for a ComboBox using KeyUp event
        public static void MakeSearchable(this ComboBox comboBox, Func<List<string>> getDataSource)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;
            comboBox.AutoCompleteMode = AutoCompleteMode.None; // Disable built-in autocomplete

            // Initialize and store state for the ComboBox
            if (!ComboBoxStates.TryGetValue(comboBox, out var state))
            {
                state = new ComboBoxState
                {
                    DropdownTimer = new Timer
                    {
                        Interval = 100 // Short delay before opening dropdown
                    }
                };
                state.DropdownTimer.Tick += (sender, e) =>
                {
                    if (comboBox.IsHandleCreated && !comboBox.DroppedDown && !state.IsUpdating)
                    {
                        comboBox.DroppedDown = true;
                    }
                    state.DropdownTimer.Stop(); // Stop timer after opening dropdown
                };
                ComboBoxStates[comboBox] = state;
            }

            // Attach to the KeyUp event to handle typing and filtering after input
            comboBox.KeyUp += (sender, e) =>
            {
                if (state.IsSettingIndex || state.IsUpdating || state.IsRestoringOriginalList)
                    return;

                // Ensure the focus is on the current ComboBox
                if (comboBox.Focused)
                {
                    // Filter based on the current text in the ComboBox
                    FilterComboBoxItems(comboBox, getDataSource, comboBox.Text, state);
                }
            };

            // Handle the DropDown event to keep focus and ensure cursor visibility
            comboBox.DropDown += (sender, e) =>
            {
                if (state.IsRestoringOriginalList || state.IsSettingIndex || state.IsUpdating)
                    return;

                // Explicitly set focus to ensure cursor visibility
                comboBox.Focus();
            };

            // Restore the full list when an item is selected
            comboBox.SelectedIndexChanged += (sender, e) =>
            {
                if (state.IsRestoringOriginalList || comboBox.SelectedIndex == -1 || state.IsSettingIndex)
                    return;

                // Get the selected item text
                var selectedItem = comboBox.SelectedItem?.ToString() ?? string.Empty;

                // Restore the original list after selection
                RestoreOriginalList(comboBox, getDataSource, selectedItem, state);
            };
        }

        // Helper method to filter ComboBox items
        private static void FilterComboBoxItems(ComboBox comboBox, Func<List<string>> getDataSource, string typedText, ComboBoxState state)
        {
            comboBox.BeginUpdate();
            state.IsUpdating = true;

            // Get the latest data source
            var allItems = getDataSource();

            // Filter items based on the typed text
            var matchingItems = allItems
                .Where(item => item.ToLower().Contains(typedText.ToLower()))
                .ToList();

            // Temporarily clear and re-add filtered items
            comboBox.Items.Clear();
            comboBox.Items.AddRange(matchingItems.ToArray());

            // Restore the typed text
            comboBox.Text = typedText;
            comboBox.SelectionStart = comboBox.Text.Length; // Move cursor to the end
            comboBox.SelectionLength = 0;

            // Use the timer to delay dropdown opening to ensure smooth UI updates
            state.DropdownTimer.Stop();
            state.DropdownTimer.Start();

            comboBox.EndUpdate();
            state.IsUpdating = false;
        }

        // Method to restore the full list after an item is selected
        private static void RestoreOriginalList(ComboBox comboBox, Func<List<string>> getDataSource, string selectedItem, ComboBoxState state)
        {
            state.IsRestoringOriginalList = true;
            comboBox.BeginUpdate();

            // Get the full original list
            var fullList = getDataSource();

            // Restore the original items in the ComboBox
            comboBox.Items.Clear();
            comboBox.Items.AddRange(fullList.ToArray());

            // Find and select the previously selected item in the full list
            var selectedIndex = fullList.IndexOf(selectedItem);
            if (selectedIndex != -1)
            {
                comboBox.SelectedIndex = selectedIndex;
            }

            comboBox.EndUpdate();
            state.IsRestoringOriginalList = false;
        }

        // Method to populate ComboBox programmatically
        public static void PopulateItems(this ComboBox comboBox, List<string> items)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear(); // Clear existing items
            comboBox.Items.AddRange(items.ToArray());
            comboBox.EndUpdate();
        }

        // Method to set the selected index programmatically, safely handling events and avoiding dropdown opening
        public static void SetSelectedIndexSafely(this ComboBox comboBox, int index, Func<List<string>> getDataSource)
        {
            if (index >= 0)
            {
                if (ComboBoxStates.TryGetValue(comboBox, out var state))
                {
                    state.IsSettingIndex = true; // Prevent triggering KeyUp during index set

                    // Repopulate the ComboBox with the full item list before setting the index
                    var fullList = getDataSource();
                    comboBox.Items.Clear();
                    comboBox.Items.AddRange(fullList.ToArray());

                    if (index < comboBox.Items.Count)
                    {
                        comboBox.SelectedIndex = index;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                    }

                    state.IsSettingIndex = false;
                }
                else
                {
                    throw new InvalidOperationException("ComboBox state not initialized.");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
            }
        }
    }

}

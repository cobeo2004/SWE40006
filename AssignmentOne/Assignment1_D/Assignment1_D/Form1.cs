using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Assignment1_D_DataClass;
using Assignment1_D_Services;

namespace Assignment1_D
{
    public partial class Form1 : Form
    {
        private readonly TodoService _todoService;
        private List<TodoDataClass> _currentTodos;

        public Form1()
        {
            InitializeComponent();
            _todoService = new TodoService();
            _currentTodos = new List<TodoDataClass>();
            
            // Initialize the form
            LoadTodos();
            SetupInitialState();
        }

        private void SetupInitialState()
        {
            // Set initial button states
            btnDelete.Enabled = false;
            
            // Set placeholder text or initial state
            txtSelectedDescription.Text = "Select a todo item to view details...\r\n\r\nTip: Double-click a todo item to toggle its completion status.";
            
            // Focus on title textbox
            txtTitle.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("Please enter a title for the todo item.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // Add the todo
                _todoService.AddTodo(txtTitle.Text.Trim(), txtDescription.Text.Trim());
                
                // Clear input fields
                ClearInputFields();
                
                // Refresh the list
                LoadTodos();
                
                // Show success message
                MessageBox.Show("Todo item added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding todo: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstTodos.SelectedItem == null)
                {
                    MessageBox.Show("Please select a todo item to delete.", "Selection Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the selected todo
                var selectedTodo = (TodoDataClass)lstTodos.SelectedItem;
                
                // Confirm deletion
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedTodo.Title}'?", 
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // Delete the todo
                    _todoService.DeleteTodo(selectedTodo.Id);
                    
                    // Refresh the list
                    LoadTodos();
                    
                    // Clear selection details
                    txtSelectedDescription.Text = "Select a todo item to view details...\r\n\r\nTip: Double-click a todo item to toggle its completion status.";
                    btnDelete.Enabled = false;
                    
                    MessageBox.Show("Todo item deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting todo: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadTodos();
                MessageBox.Show("Todo list refreshed!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing todos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstTodos_SelectedIndexChanged(object sender, EventArgs e)  
        {
            try
            {
                if (lstTodos.SelectedItem != null)
                {
                    var selectedTodo = (TodoDataClass)lstTodos.SelectedItem;
                    
                    // Display selected todo details
                    var details = $"Title: {selectedTodo.Title}\r\n\r\n" +
                                 $"Description: {selectedTodo.Description}\r\n\r\n" +
                                 $"Status: {(selectedTodo.IsCompleted ? "Completed" : "Pending")}\r\n" +
                                 $"ID: {selectedTodo.Id}\r\n\r\n" +
                                 $"Tip: Double-click this item to toggle its completion status.";
                    
                    txtSelectedDescription.Text = details;
                    btnDelete.Enabled = true;
                }
                else
                {
                    txtSelectedDescription.Text = "Select a todo item to view details...\r\n\r\nTip: Double-click a todo item to toggle its completion status.";
                    btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying todo details: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstTodos_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                // Check if an item is selected
                if (lstTodos.SelectedItem == null)
                {
                    return; // No item selected, do nothing
                }

                // Get the selected todo
                var selectedTodo = (TodoDataClass)lstTodos.SelectedItem;
                var previousStatus = selectedTodo.IsCompleted;
                
                // Toggle the completion status
                _todoService.ToggleTodoCompletion(selectedTodo.Id);
                
                // Store the currently selected index to restore selection after refresh
                int selectedIndex = lstTodos.SelectedIndex;
                
                // Refresh the list to reflect the change
                LoadTodos();
                
                // Restore the selection if the item still exists
                if (selectedIndex < lstTodos.Items.Count)
                {
                    lstTodos.SelectedIndex = selectedIndex;
                }
                
                // Show feedback message
                var newStatus = !previousStatus ? "completed" : "pending";
                var statusIcon = !previousStatus ? "✓" : "○";
                MessageBox.Show($"{statusIcon} Todo '{selectedTodo.Title}' marked as {newStatus}!", 
                    "Status Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling todo completion: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTodos()
        {
            try
            {
                // Get all todos from the service
                _currentTodos = _todoService.GetAllTodos();
                
                // Clear the current list
                lstTodos.Items.Clear();
                
                // Add todos to the listbox
                foreach (var todo in _currentTodos)
                {
                    lstTodos.Items.Add(todo);
                }
                
                // Update the UI
                UpdateTodoCountDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading todos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTodoCountDisplay()
        {
            var totalCount = _currentTodos.Count;
            var completedCount = _currentTodos.Count(t => t.IsCompleted);
            var pendingCount = totalCount - completedCount;
            
            lblTodos.Text = $"Todo List ({totalCount} total, {pendingCount} pending, {completedCount} completed)";
        }

        private void ClearInputFields()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            txtTitle.Focus();
        }

        // Override the ToString method for TodoData to display nicely in the ListBox
        // Note: This would be better placed in the TodoData class, but I'll add it here for now
        private void Form1_Load(object sender, EventArgs e)
        {
            // This event handler can be used for any initialization that needs to happen after the form is fully loaded
            LoadTodos();
        }
    }
}

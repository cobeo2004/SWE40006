using Assignment1_D_DataClass;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_D_Services
{
    public sealed class TodoService : ITodoService
    {
        private List<TodoDataClass> _dbService;

        public TodoService(string dbFilePath = "database.db", bool willSeedData = true)
        {
            _dbService = new List<TodoDataClass>();

            // Add some sample data for demonstration
            if (willSeedData)
            {
                InitializeSampleData();
            }
        }

        private void InitializeSampleData()
        {
            // Add a few sample todos for demonstration
            AddTodo("Welcome to Todo App", "This is your first todo item. You can add, view, and delete todos using this application.");
            AddTodo("Learn C# Programming", "Study C# fundamentals, object-oriented programming, and Windows Forms development.");
            AddTodo("Complete Assignment", "Finish the todo list application assignment with all required features.");
        }

        public void AddTodo(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            var todo = new TodoDataClass(Guid.NewGuid().ToString(), title, description ?? "", false);
            _dbService.Add(todo);
        }

        public void DeleteTodo(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be empty", nameof(id));

            var todo = _dbService.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                _dbService.Remove(todo);
            }
            else
            {
                throw new InvalidOperationException($"Todo with Id {id} not found.");
            }
        }

        public List<TodoDataClass> GetAllTodos()
        {
            return _dbService.ToList(); // Return a copy to prevent external modification
        }

        public TodoDataClass GetTodoById(string id)
        {
            return _dbService.FirstOrDefault(t => t.Id == id);
        }

        public void ToggleTodoCompletion(string id)
        {
            var todo = GetTodoById(id);
            if (todo != null)
            {
                todo.IsCompleted = !todo.IsCompleted;
            }
            else
            {
                throw new InvalidOperationException($"Todo with Id {id} not found.");
            }
        }

        public List<TodoDataClass> GetCompletedTodos()
        {
            return _dbService.Where(t => t.IsCompleted).ToList();
        }
        public List<TodoDataClass> GetPendingTodos()
        {
            return _dbService.Where(t => !t.IsCompleted).ToList();
        }

        public int GetTotalCount()
        {
            return _dbService.Count;
        }
    }
}

using Assignment1_D_DataClass;
using System.Collections.Generic;

namespace Assignment1_D_Services
{
    internal interface ITodoService
    {
        void AddTodo(string title, string description);
        void DeleteTodo(string id);
        List<TodoDataClass> GetAllTodos();
    }
}

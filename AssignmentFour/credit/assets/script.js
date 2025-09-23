async function markTodoAsCompleted(todoId) {
  const response = await fetch(`/mark-todo-as-completed/${todoId}`, {
    method: "PUT",
  });
  if (response.ok) {
    window.location.reload();
  }
}
async function deleteTodo(todoId) {
  const response = await fetch(`/delete-todo/${todoId}`, {
    method: "DELETE",
  });
  if (response.ok) {
    window.location.reload();
  }
}

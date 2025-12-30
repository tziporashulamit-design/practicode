import React, { useEffect, useState } from 'react';
import service from './service';

function App() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);

  async function getTodos() {
    const items = await service.getTasks();
    setTodos(items || []);
  }

  async function createTodo(e) {
    e.preventDefault();
    if (!newTodo.trim()) return;
    await service.addTask(newTodo);
    setNewTodo("");
    await getTodos();
  }

async function updateCompleted(todo, isComplete) {
  try {
    const id = todo.id || todo.Id || todo.ID;
    await service.setCompleted(id, isComplete);
    
    // זה התיקון: אנחנו לא סומכים על האובייקט הבודד שחזר, 
    // אלא מושכים את כל הרשימה המעודכנת מה-DB
    await getTodos(); 
  } catch (error) {
    console.error("Update failed", error);
  }
}
  async function deleteTodo(id) {
    await service.deleteTask(id);
    await getTodos();
  }

  useEffect(() => {
    getTodos();
  }, []);

  return (
    <section className="todoapp">
      <header className="header">
        <button 
  className="logout-btn"
  onClick={() => { service.logout(); }}
>
  Logout
</button>
        <h1>todos</h1>
        <form onSubmit={createTodo}>
          <input
            className="new-todo"
            placeholder="What needs to be done?"
            value={newTodo}
            onChange={(e) => setNewTodo(e.target.value)}
          />
        </form>
      </header>
      <section className="main" style={{ display: "block" }}>
        <ul className="todo-list">
  {todos.map(todo => {
    // השורה הזו היא "קסם": היא מוצאת את השדה הראשון שהוא טקסט באובייקט
    const name = todo.Name || todo.name || Object.values(todo).find(v => typeof v === 'string') || "No Name Found";
    const id = todo.Id || todo.id || todo.ID;
    const isDone = todo.IsComplete || todo.isComplete || false;

    return (
      <li className={isDone ? "completed" : ""} key={id}>
        <div className="view">
          <input
            className="toggle"
            type="checkbox"
            checked={isDone}
            onChange={(e) => updateCompleted(todo, e.target.checked)}
          />
          <label>{name}</label>
          <button className="destroy" onClick={() => deleteTodo(id)}></button>
        </div>
      </li>
    );
  })}
</ul>
      </section>
    </section>
  );
}

export default App;
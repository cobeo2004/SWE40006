from typing import Annotated
from fastapi import Depends, FastAPI, Request
from fastapi.responses import RedirectResponse
import uvicorn
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from fastapi.middleware.cors import CORSMiddleware
from sqlmodel import SQLModel, Field, Session, create_engine, select
from contextlib import asynccontextmanager


class Todo(SQLModel, table=True):
    id: int | None = Field(default=None, primary_key=True)
    title: str = Field(index=True)
    description: str = Field(index=True)
    completed: bool = Field(default=False)


engine = create_engine("sqlite:///todo.db",
                       connect_args={"check_same_thread": False})


def get_session():
    with Session(engine) as session:
        yield session


SessionDep = Annotated[Session, Depends(get_session)]


@asynccontextmanager
async def lifespan(app: FastAPI):
    SQLModel.metadata.create_all(engine)
    yield
    SQLModel.metadata.drop_all(engine)


app = FastAPI(lifespan=lifespan)
templates = Jinja2Templates(directory="templates")

app.mount("/assets", StaticFiles(directory="assets"), name="assets")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.post("/add-todo")
async def add_todo(request: Request, session: SessionDep):
    form = await request.form()
    title = form.get("title")
    description = form.get("description")
    print(title, description)
    todo = Todo(title=title, description=description, completed=False)
    session.add(todo)
    session.commit()
    return RedirectResponse(url="/", status_code=303)


@app.put("/mark-todo-as-completed/{todo_id}")
async def mark_todo_as_completed(todo_id: int, session: SessionDep):
    print("Marking todo as completed", todo_id)
    todo = session.get(Todo, todo_id)
    todo.completed = not todo.completed
    session.commit()
    return RedirectResponse(url="/", status_code=303)


@app.delete("/delete-todo/{todo_id}")
async def delete_todo(todo_id: int, session: SessionDep):
    print("Deleting todo", todo_id)
    todo = session.get(Todo, todo_id)
    session.delete(todo)
    session.commit()
    return RedirectResponse(url="/", status_code=303)


@app.get("/")
def hello_world(request: Request, session: SessionDep):
    todos = session.exec(select(Todo)).all()
    return templates.TemplateResponse("index.jinja", {"request": request, "todos": todos})


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)

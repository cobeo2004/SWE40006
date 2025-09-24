from contextlib import asynccontextmanager
from datetime import datetime
from fastapi import FastAPI
from sqlmodel import SQLModel, Field, Session, create_engine, select


class Schema(SQLModel, table=True):
    id: int | None = Field(default=None, primary_key=True)
    client_id: int = Field(index=True)
    message: str = Field(index=True)
    created_at: datetime = Field(default=datetime.now())


class MessageRepository:
    def __init__(self):
        self.db = create_engine("sqlite:///message.db",
                                connect_args={"check_same_thread": False})
        self.session = Session(self.db)

    @asynccontextmanager
    async def lifespan(self, app: FastAPI):
        SQLModel.metadata.create_all(self.db)
        yield

    def add_message(self, client_id: int, message: str):
        # Generate a monotonic-ish integer id using time in nanoseconds plus a small random jitter
        add_schema = Schema(client_id=client_id,
                            message=message, created_at=datetime.now())
        self.session.add(add_schema)
        self.session.commit()

    def get_messages(self):
        return self.session.exec(select(Schema)).all()

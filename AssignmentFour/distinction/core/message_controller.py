from typing import List

from fastapi import WebSocket
from starlette.websockets import WebSocketState
from core.db import MessageRepository


class MessageController:
    def __init__(self):
        self.active_conns: List[WebSocket] = []
        self.repo = MessageRepository()

    async def connect(self, ws: WebSocket):
        await ws.accept()
        self.active_conns.append(ws)
        # Send message history to the newly connected client only
        await self.send_history(ws)

    async def disconnect(self, ws: WebSocket):
        if ws in self.active_conns:
            self.active_conns.remove(ws)

    async def send_personal_message(self, message: str, ws: WebSocket):
        try:
            await ws.send_text(message)
        except Exception:
            # If sending fails, consider the connection stale and remove it
            if ws in self.active_conns:
                self.active_conns.remove(ws)

    async def send_history(self, ws: WebSocket):
        for record in self.repo.get_messages():
            try:
                await ws.send_text(f"Client #{record.client_id} says: {record.message}")
            except Exception:
                if ws in self.active_conns:
                    self.active_conns.remove(ws)
                break

    async def persist_and_broadcast(self, client_id: int, message: str):
        # Persist first, then broadcast
        self.repo.add_message(client_id=client_id, message=message)
        await self.broadcast(f"Client #{client_id} says: {message}")

    async def broadcast(self, message: str):
        stale_conns: List[WebSocket] = []
        # Iterate over a copy to allow safe removal while iterating
        for conn in list(self.active_conns):
            # Skip clearly disconnected sockets
            if (
                getattr(conn, "application_state",
                        None) == WebSocketState.DISCONNECTED
                or getattr(conn, "client_state", None) == WebSocketState.DISCONNECTED
            ):
                stale_conns.append(conn)
                continue

            try:
                await conn.send_text(message)
            except Exception:
                # Send failed, mark for cleanup
                stale_conns.append(conn)

        # Cleanup any stale connections
        for conn in stale_conns:
            if conn in self.active_conns:
                self.active_conns.remove(conn)

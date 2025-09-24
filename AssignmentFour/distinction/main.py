import random
from fastapi import FastAPI, Request, WebSocket, WebSocketDisconnect
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
import uvicorn
from core.message_controller import MessageController
from core.db import MessageRepository

session = MessageRepository()

app = FastAPI(lifespan=session.lifespan)

app.mount("/assets", StaticFiles(directory="assets"), name="assets")
templates = Jinja2Templates(directory="templates")

message_controller = MessageController()


@app.get("/")
async def main(req: Request):
    return templates.TemplateResponse(request=req, name="index.jinja", context={"request": req, "client_id": random.randint(1, 1000000)})


@app.websocket("/chat/{client_id}")
async def chat_endpoint(ws: WebSocket, client_id: int):
    await message_controller.connect(ws)
    await message_controller.broadcast(f"Client #{client_id} joined the chat")
    try:
        while True:
            data = await ws.receive_text()
            await message_controller.persist_and_broadcast(client_id, data)
    except WebSocketDisconnect as e:
        message_controller.disconnect(ws)
        await message_controller.broadcast(f"Client #{client_id} left the chat")

if __name__ == "__main__":
    uvicorn.run("main:app", host="0.0.0.0", port=8000, reload=True)

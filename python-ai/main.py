from fastapi import FastAPI
from routers import chat

app = FastAPI(title="AI Shopping Assistant Agent")
app.include_router(chat.router)

@app.get("/health")
def health():
    return {"status": "ok", "service": "ai-shopping-agent"}

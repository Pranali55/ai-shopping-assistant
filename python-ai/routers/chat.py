from fastapi import APIRouter
from models.schemas import AiChatRequest, AiChatResponse
from services.agent import ShoppingAgent

router = APIRouter()
agent = ShoppingAgent()

@router.post("/chat", response_model=AiChatResponse)
async def chat(request: AiChatRequest):
    return await agent.respond(request)

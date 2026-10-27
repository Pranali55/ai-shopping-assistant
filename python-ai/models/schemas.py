from pydantic import BaseModel
from typing import List, Optional

class AiMessage(BaseModel):
    role: str
    content: str

class UserPreferences(BaseModel):
    budget_range: Optional[str] = None
    preferred_categories: Optional[List[str]] = None

class Product(BaseModel):
    id: int
    name: str
    description: str
    category: str
    brand: str
    price: float
    original_price: Optional[float] = None
    stock: int
    rating: float
    review_count: int
    image_url: Optional[str] = None
    tags: Optional[str] = None
    is_active: bool = True

class AiChatRequest(BaseModel):
    message: str
    history: List[AiMessage] = []
    preferences: Optional[UserPreferences] = None
    products: List[Product] = []

class AiChatResponse(BaseModel):
    reply: str
    recommended_product_ids: List[int] = []
    intent: Optional[str] = None
    suggestions: List[str] = []

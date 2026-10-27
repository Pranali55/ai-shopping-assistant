import os
import json
from openai import AsyncOpenAI
from models.schemas import AiChatRequest, AiChatResponse, Product

client = AsyncOpenAI(api_key=os.getenv("OPENAI_API_KEY", ""))

SYSTEM_PROMPT = """You are an intelligent AI shopping assistant. Your job is to help users find the perfect products.

You have access to a product catalog. When users ask about products, recommend the most relevant ones.

CAPABILITIES:
- Understand user intent (searching, comparing, budget-based, category browsing)
- Recommend products based on preferences, budget, and needs
- Compare products and explain differences
- Remember conversation context
- Suggest alternatives if exact match not found

RULES:
- Always be helpful, friendly and concise
- When recommending products, mention name, key features and price
- If budget is mentioned, only recommend products within that budget
- Always end with a follow-up question or suggestion
- Return recommended product IDs in the JSON response

RESPONSE FORMAT:
You must respond ONLY with valid JSON in this exact format:
{
  "reply": "Your friendly response here",
  "recommended_product_ids": [1, 2, 3],
  "intent": "search|compare|recommend|budget|category|general",
  "suggestions": ["Quick action 1", "Quick action 2", "Quick action 3"]
}"""

class ShoppingAgent:

    async def respond(self, request: AiChatRequest) -> AiChatResponse:
        # Build product catalog context
        catalog = self._build_catalog(request.products)

        # Build preference context
        pref_context = ""
        if request.preferences:
            if request.preferences.budget_range:
                pref_context += f"User budget preference: {request.preferences.budget_range}. "
            if request.preferences.preferred_categories:
                pref_context += f"User preferred categories: {', '.join(request.preferences.preferred_categories)}. "

        # Build messages
        messages = [
            {"role": "system", "content": f"{SYSTEM_PROMPT}\n\nPRODUCT CATALOG:\n{catalog}\n\n{pref_context}"}
        ]

        # Add conversation history (last 10 messages)
        for msg in request.history[-10:]:
            messages.append({"role": msg.role, "content": msg.content})

        # Add current message
        messages.append({"role": "user", "content": request.message})

        try:
            response = await client.chat.completions.create(
                model="gpt-4o-mini",
                messages=messages,
                temperature=0.7,
                max_tokens=800,
                response_format={"type": "json_object"}
            )

            content = response.choices[0].message.content
            data = json.loads(content)

            return AiChatResponse(
                reply=data.get("reply", "I can help you find great products!"),
                recommended_product_ids=data.get("recommended_product_ids", []),
                intent=data.get("intent", "general"),
                suggestions=data.get("suggestions", [
                    "Show me top rated products",
                    "Find something under $100",
                    "Browse electronics"
                ])
            )

        except Exception as e:
            # Fallback: smart keyword-based matching
            return self._fallback_response(request)

    def _fallback_response(self, request: AiChatRequest) -> AiChatResponse:
        message_lower = request.message.lower()
        products = request.products

        # Budget detection
        budget = None
        import re
        budget_match = re.search(r'\$?(\d+)', message_lower)
        if budget_match and any(w in message_lower for w in ["under", "below", "less", "budget", "cheap"]):
            budget = float(budget_match.group(1))

        # Filter and score products
        scored = []
        for p in products:
            score = 0
            name_lower = p.name.lower()
            desc_lower = p.description.lower()
            tags = (p.tags or "").lower()
            category_lower = p.category.lower()

            # Keyword matching
            words = message_lower.split()
            for word in words:
                if len(word) > 3:
                    if word in name_lower: score += 3
                    if word in category_lower: score += 2
                    if word in tags: score += 2
                    if word in desc_lower: score += 1

            # Budget filter
            if budget and p.price > budget:
                score = 0

            if score > 0:
                scored.append((score, p))

        scored.sort(key=lambda x: (-x[0], -x[1].rating))
        top_products = [p for _, p in scored[:3]]
        recommended_ids = [p.id for p in top_products]

        if top_products:
            product_list = "\n".join([f"• **{p.name}** by {p.brand} — ${p.price:.2f} ⭐{p.rating}" for p in top_products])
            reply = f"Here are some great options for you:\n\n{product_list}\n\nWould you like more details on any of these?"
        else:
            reply = "I'd love to help you find the perfect product! Could you tell me more about what you're looking for? For example, a category, budget, or specific features?"

        return AiChatResponse(
            reply=reply,
            recommended_product_ids=recommended_ids,
            intent="search",
            suggestions=["Show me electronics", "Find under $200", "Best rated products", "Compare laptops"]
        )

    def _build_catalog(self, products: list[Product]) -> str:
        if not products:
            return "No products available."
        lines = []
        for p in products:
            discount = f" (was ${p.original_price:.2f})" if p.original_price else ""
            lines.append(f"ID:{p.id} | {p.name} | {p.brand} | {p.category} | ${p.price:.2f}{discount} | ⭐{p.rating} ({p.review_count} reviews) | Tags: {p.tags or 'none'}")
        return "\n".join(lines)

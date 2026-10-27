# 🛒 AI Shopping Assistant Agent

An AI-powered shopping assistant built with **.NET 8**, **Python FastAPI**, **OpenAI GPT-4o-mini**, and **Redis** — fully Dockerized. Chat naturally to find products, compare options, and get personalized recommendations.

## ✨ Features

- 🤖 **AI Chat Agent** — powered by OpenAI GPT-4o-mini
- 🧠 **Smart Intent Detection** — search, compare, budget, category
- 💬 **Conversation Memory** — remembers full chat history
- 🎯 **Personalized Recommendations** — based on preferences & budget
- 📦 **12 Seeded Products** — ready to use out of the box
- 🛒 **Shopping Cart** — add, remove, clear
- ❤️ **Wishlist** — save favourite products
- 🔍 **Product Search** — filter by category, price, sort by rating
- ⚡ **Fallback Mode** — works without OpenAI key (keyword matching)
- 🐳 **Fully Dockerized**

## 🏗️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend API | .NET 8 (ASP.NET Core) |
| AI Agent | Python 3.11 + FastAPI + OpenAI |
| Database | SQL Server 2022 |
| Cache | Redis 7 |
| AI Model | GPT-4o-mini |
| Auth | JWT Bearer Tokens |
| Container | Docker + Docker Compose |

## 🚀 Getting Started

1. Clone the repo
2. Create `.env` file:
```env
OPENAI_API_KEY=sk-your-openai-key-here
```
> ⚠️ No OpenAI key? The app still works using smart keyword-based fallback!

3. Run:
```bash
docker-compose up --build
```

4. Open Swagger → [http://localhost:5000/swagger](http://localhost:5000/swagger)
5. AI Agent Docs → [http://localhost:8000/docs](http://localhost:8000/docs)

## 💬 Example Conversations

```
User: "I'm looking for a laptop under $2000"
AI: "Here are some great laptops for you:
     • MacBook Pro 14" by Apple — $1,999.99 ⭐4.9
     • Dell XPS 15 by Dell — $1,799.99 ⭐4.6
     Would you like more details on either of these?"

User: "Compare the MacBook and Dell"
AI: "Great comparison! The MacBook Pro offers Apple's M3 Pro chip...
     The Dell XPS 15 features an OLED display..."

User: "Which one is better for video editing?"
AI: "For video editing, I'd recommend the MacBook Pro..."
```

## 📡 API Endpoints

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register |
| POST | `/api/auth/login` | Login |

### Chat (AI Agent)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/chat` | My chat sessions |
| POST | `/api/chat` | Start new chat |
| GET | `/api/chat/{id}` | Get chat history |
| POST | `/api/chat/{id}/messages` | Send message to AI |
| DELETE | `/api/chat/{id}` | Delete session |

### Products
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Browse products |
| GET | `/api/products/categories` | List categories |
| GET | `/api/products/{id}` | Product details |

### Cart & Wishlist
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cart` | View cart |
| POST | `/api/cart` | Add to cart |
| DELETE | `/api/cart/{id}` | Remove item |
| GET | `/api/wishlist` | View wishlist |
| POST | `/api/wishlist` | Add to wishlist |

## 📁 Project Structure

```
ai-shopping-assistant/
├── backend-dotnet/          # .NET 8 API
│   ├── Controllers/         # Auth, Chat, Products, Cart, Wishlist
│   ├── Services/            # AuthService, ChatService, ProductService
│   ├── Models/
│   ├── DTOs/
│   ├── Data/                # DbContext + 12 seeded products
│   └── Dockerfile
├── python-ai/               # AI Agent (FastAPI + OpenAI)
│   ├── routers/
│   ├── models/
│   ├── services/            # ShoppingAgent with GPT-4o-mini
│   └── Dockerfile
└── docker-compose.yml
```

## 👨‍💻 Author

Built as a freelance portfolio project showcasing AI agent development.

from flask import Flask, request, jsonify
from telegram import Bot
from telegram.ext import ApplicationBuilder, CommandHandler
import os, json, threading, asyncio
from datetime import datetime
from database import init_db, get_connection

TOKEN = os.environ.get("BOT_TOKEN")
bot = Bot(TOKEN)

# Инициализация базы
init_db()

app = Flask(__name__)

# ======== API для фронтенда ========

@app.route("/api/register_user", methods=["POST"])
def register_user():
    data = request.json
    telegram_id = data.get("telegram_id")
    username = data.get("username")
    avatar_url = data.get("avatar_url")
    if not telegram_id: return jsonify({"error":"telegram_id required"}),400

    conn = get_connection()
    cur = conn.cursor()
    cur.execute("INSERT OR IGNORE INTO users (telegram_id, username, avatar_url) VALUES (?,?,?)",
                (telegram_id, username, avatar_url))
    conn.commit()
    cur.execute("SELECT * FROM users WHERE telegram_id=?", (telegram_id,))
    user = dict(cur.fetchone())
    conn.close()
    return jsonify(user)

@app.route("/api/get_profile", methods=["POST"])
def get_profile():
    telegram_id = request.json.get("telegram_id")
    if not telegram_id: return jsonify({"error":"telegram_id required"}),400
    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT * FROM users WHERE telegram_id=?", (telegram_id,))
    user = cur.fetchone()
    if not user: return jsonify({"error":"user not found"}),404
    user_dict = dict(user)
    # количество тренировок
    cur.execute("SELECT COUNT(*) as count FROM workouts WHERE user_id=?", (user_dict["id"],))
    user_dict["total_workouts"] = cur.fetchone()["count"]
    conn.close()
    return jsonify(user_dict)

@app.route("/api/save_workout", methods=["POST"])
def save_workout():
    data = request.json
    telegram_id = data.get("telegram_id")
    title = data.get("title")
    exercises = data.get("exercises")
    if not telegram_id or not title or not exercises:
        return jsonify({"error":"missing fields"}),400

    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT id FROM users WHERE telegram_id=?", (telegram_id,))
    user = cur.fetchone()
    if not user: return jsonify({"error":"user not found"}),404
    user_id = user["id"]
    cur.execute(
        "INSERT INTO workouts (user_id, title, exercises, created_at) VALUES (?,?,?,?)",
        (user_id, title, json.dumps(exercises), datetime.utcnow().isoformat())
    )
    conn.commit()
    conn.close()
    return jsonify({"status":"ok"})

@app.route("/api/get_workouts", methods=["POST"])
def get_workouts():
    telegram_id = request.json.get("telegram_id")
    if not telegram_id: return jsonify({"error":"telegram_id required"}),400
    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT id FROM users WHERE telegram_id=?", (telegram_id,))
    user = cur.fetchone()
    if not user: return jsonify({"error":"user not found"}),404
    user_id = user["id"]
    cur.execute("SELECT * FROM workouts WHERE user_id=? ORDER BY created_at DESC", (user_id,))
    rows = cur.fetchall()
    workouts = []
    for r in rows:
        workouts.append({
            "id": r["id"],
            "title": r["title"],
            "exercises": json.loads(r["exercises"]),
            "created_at": r["created_at"]
        })
    conn.close()
    return jsonify(workouts)

# ======== Telegram Bot ========
async def faq(update, context):
    await update.message.reply_text("Привет! Я бот FitPlan.\nИспользуй приложение для создания тренировок.")

async def support(update, context):
    await update.message.reply_text("Чат поддержки: @fapSupport")

async def channel(update, context):
    await update.message.reply_text("Канал: t.me/fitappplan")

def start_bot():
    app_bot = ApplicationBuilder().token(TOKEN).build()
    app_bot.add_handler(CommandHandler("faq", faq))
    app_bot.add_handler(CommandHandler("support", support))
    app_bot.add_handler(CommandHandler("channel", channel))
    app_bot.run_polling()

# ======== Запуск ========
if __name__ == "__main__":
    # запускаем бот в отдельном потоке
    threading.Thread(target=start_bot, daemon=True).start()
    # запускаем Flask
    app.run(host="0.0.0.0", port=int(os.environ.get("PORT", 5000)))

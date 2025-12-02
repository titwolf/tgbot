from telegram import Update
from telegram.ext import ApplicationBuilder, CommandHandler, ContextTypes
import os

# Получаем токен из переменной окружения
TOKEN = os.environ.get("BOT_TOKEN")

async def faq(update: Update, context: ContextTypes.DEFAULT_TYPE):
    await update.message.reply_text(
        "Привет! Я бот для приложения FitPlan.\n"
        "Здесь вы можете просматривать свои тренировки, создавать новые и вести учет прогресса."
    )

async def support(update: Update, context: ContextTypes.DEFAULT_TYPE):
    await update.message.reply_text(
        "Чат поддержки - @fapSupport"
    )

async def channel(update: Update, context: ContextTypes.DEFAULT_TYPE):
    await update.message.reply_text(
        "Телеграм канал - t.me/fitappplan"
    )

if __name__ == "__main__":
    app = ApplicationBuilder().token(TOKEN).build()

    app.add_handler(CommandHandler("faq", faq))
    app.add_handler(CommandHandler("support", support))
    app.add_handler(CommandHandler("channel", channel))

    print("Бот запущен...")
    app.run_polling()

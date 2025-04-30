Бот для отправки уведомлений в чат Telegram о создание новой заявки на сайте osTicket

**Необходимые данные:**

OSTicketApiUrl - URL-адрес сайта osTicket 

OsticketApiKey - API ключ сайта osTicket

TelegramBotToken - токен вашего Telegram бота (создать бота и получить его токен можно через бота @BotFather)

TelegramChatId - id чата Telegram в который бот будет отправлять уведомления (id чата можно посмотреть через бота @username_to_id_bot)

**Настройка рабочей среды:**

Установка Visual Studio Code через терминал:

    sudo apt install software-properties-common apt-transport-https wget
    wget -q https://packages.microsoft.com/keys/microsoft.asc -O- | sudo apt-key add -
    sudo add-apt-repository "deb [arch=amd64] https://packages.microsoft.com/repos/vscode stable main
    sudo apt update
    sudo apt install code


Настройка расширения для C#:

1. Запустить Visual Studio Code
   
2. Открыть панель расширений (комбинация клавиш Ctrl+Shift+X)

3. В появившейся панели найдити и установить официальное расширение C#

**Установка необходимых пакетов:**

Установка NuGet:

    sudo apt update
    sudo apt install wget
    wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt update; \
    sudo apt install dotnet-sdk-8.0

Установка NuGet пакетов:

    dotnet add package Newtonsoft.Json
    dotnet add package System.Net.Http

*Newtonsoft.Json - библиотека для обработки JSON-данных

*System.Net.Http - набор классов для выполнения HTTP-запросов

**Запуск бота:**

Сборка и запуск:

    dotnet build
    dotnet run

Автозапуск бота:

    [Unit]
    Description=Ticket Bot Service
    After=network.target

    [Service]
    WorkingDirectory=/path/to/your/project
    ExecStart=/usr/bin/dotnet /path/to/your/project/Ticket_BotTG.dll
    Restart=always
    RestartSec=10
    SyslogIdentifier=ticket-bot
    User=your-user
    Environment=ASPNETCORE_ENVIRONMENT=Production

*WorkingDirectory - указание на рабочую директорию, где находится проект

*ExecStart - команда для запуска приложения через dotnet-рантайм с указанием пути к dll-файлу бота

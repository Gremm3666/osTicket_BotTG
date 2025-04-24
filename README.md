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

**Запуск бота:**

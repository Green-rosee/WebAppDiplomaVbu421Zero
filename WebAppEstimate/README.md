//---reset DataBase
Для твоей текущей схемы после изменений в коде запомни цикл:
cd .\WebAppEstimate\

docker build -t webappestimate .
docker rm -f webappestimate-container
docker run --name webappestimate-container -p 8080:8080 webappestimate

А потом в браузере:

Ctrl + F5

//---
Обычный запуск:

docker run `
  --name webappestimate-container `
-p 8080:8080 `
webappestimate

А когда хочешь один раз пересоздать базу:

docker run `
  --name webappestimate-container `
-p 8080:8080 `
  -e ResetAuthorizeDatabase=true `
webappestimate

Переменная Docker перекроет:
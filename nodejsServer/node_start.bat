
set NODE_PATH="C:\node\nodejsServer\node_modules"
set NODE_ENV="development"
set NODE_CMD=node
set SERVER_HOST="localhost"

cd C:\node\nodejsServer\
SET CONFIG=C:\node\nodejsServer\config\development.json

node example.js  --NODE_ENV=%NODE_ENV% --NODE_PATH=%NODE_PATH% --SERVER_HOST=%SERVER_HOST%

pause

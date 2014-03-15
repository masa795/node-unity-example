#!/bin/sh


export NODE_PATH="~/node/nodejsServer/node_modules"
export NODE_ENV="development"
NODE_CMD=node
SERVER_HOST="localhost"

cd ~/node/nodejsServer/

$NODE_CMD ./example.js

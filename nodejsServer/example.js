
var WebSocket = require('websocket').server;
var Http      = require('http');
var Config    = require('config');


console.log("========= Start ===========");
console.log(new Date());
console.log("NODE_ENV="+process.env.NODE_ENV);
console.log("NODE_PATH="+process.env.NODE_PATH);
console.log("port="+Config.websocket.port);
console.log("=================================");


var connectWebSockets   = {};
var accessHeader        = null;

var main = function() {
	var createServer = function(port) {
		return function() {
			var HttpServer = Http.createServer(httpHandler);
			HttpServer.listen(port);
			var ws = new WebSocket({
				httpServer: HttpServer,
				autoAcceptConnections:true
			});
			ws.on('connect',webSocketConnectHandler);
            HttpServer.on('upgrade', function(req, socket, head) {
                 console.log("HttpServer upgrade.");
                accessHeader = req.headers;
            });
        };
	};

	var main1 = createServer(Config.websocket.port);
	main1();
};



var httpHandler = function(req,res) {
    console.log('httpHandler access.' + req.url);
	res.writeHead(200,{"Content-Type": 'text/plain'});
	res.write('Node HTTP Access.');
	res.end();
};


var webSocketConnectHandler = function(con) {
	var fromUser = null;
	var toUser   = null;
    con.on('upgrade', function(req, socket, head) {
        console.log("webSocketConnectHandler upgrade.");
        console.log(req);
    });


	con.on('message', function(data) {
        console.log("webSocketConnectHandler message.");
        console.log("from="+ fromUser + " to="+toUser + " data.utf8Data="+data.utf8Data);

		var packet;
		try {
			packet = JSON.parse(data.utf8Data);
		} catch(ex) {
            console.log("json Parse Error. " + data.utf8Data );
			return;
		}

        var currentFromUser = packet['from'];
		var currentToUser   = packet['to'];
		var type            = packet['type'];

		if(currentFromUser == "") {
			return;
		}

        if (fromUser == null) {
            fromUser = currentFromUser;
        }

		connectWebSockets[fromUser]  = con;
        toUser = currentToUser ;
		var toWebSocket = connectWebSockets[toUser];
        var ret = '';
        try {
            ret = JSON.parse(data.utf8Data);
            ret["serverTime"] =  new Date();
            var json_text = JSON.stringify(ret);
        } catch(ex) {
            console.log("Json Parse Error . " + data.utf8Data );
            return;
        }

        console.log("return json_text="+json_text);
        toWebSocket.send(json_text);
	});

	con.on('close',function(code, desc) {
        console.log("webSocketConnectHandler close.");
		delete connectWebSockets[fromUser];
        console.log("close. code="+code+" desc=" + desc + " from="+fromUser+" to="+toUser);
	});
};

main();




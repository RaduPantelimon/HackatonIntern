function(element, input){
	
	function script() 
	{
		function SocketInjection(...args) {
			if (window.sockets.indexOf(this) === -1){
				window.sockets.push(this);
				this.addEventListener("message", function (event) {
					console.log(event.data);
					//convert message to a JSON Object watch out for the dashes below, this code will behave differently when minimized and saved as a string
					var msgObj = JSON.parse(event.data.substring(3, event.data.length-2).replaceAll("\\",""));
					window.messageQueue.push(event.data);
					
					// process and filter the incoming messages
					if (event.data.indexOf("Started") != -1 && event.data.indexOf("gameId")!= -1)
					{
						//save the game id if this is the Game Started Payload
						window.gameId = msgObj.fields.gameId
						
					}else if(event.data.indexOf("board") != -1)
					{
						// whenver we detect a board update, we will refresh the board and 
						window.gameBoard = msgObj.fields.data.board;
						
						//save all player moves to a special array
						if(msgObj.fields.actionItems)
						{
							var playerMoves = [[],[],[],[]];
							for (var i = 0; i < msgObj.fields.actionItems.length; i++)
							{
								if(msgObj.fields.actionItems[i].action.type == "place")
								{
									playerMoves[msgObj.fields.actionItems[i].action.playerIndex].push(msgObj.fields.actionItems[i].action.pieceIndex)
								}
							}
							window.enemyMoves = playerMoves;
						}
					}else if (event.data.indexOf("id") != -1 && ! isNaN(msgObj.id))
					{
						//save the payload Id if this is an Updated Id message
						window.payloadId = msgObj.id;
						
					}

					//save our output to the body of the page
					var output ={gameId:window.gameId, 
					payloadId:window.payloadId, 
					gameBoard:window.gameBoard, 
					enemyMoves:window.enemyMoves};
					
					document.body.setAttribute("uipath-automation-data",JSON.stringify(output));				
				});
			}
		};	

		console.log("Initializing JS Injection");
		console.log(window);
		
		//throw exception if injection already happened
		if(document.injectionDone) throw "JS Injection Already active";
		
		//initialize variables
		window.sockets = [];
		window.messageQueue = [];
		window.gameId = "";
		window.payloadId = "";
		window.gameBoard = null;
		window.enemyMoves = null;
		document.injectionDone = true;
		
		//initialize random seed generation method
		window.generateRandomSeed = function(){
			x = Package.ddp.DDP.randomStream("/rpc/gamePlayerAction")
			return x.hexString(20);
		};
		//register new WebSockets into our sockets array
		window.originalSend = WebSocket.prototype.send;
		//window.originalOnMessage = WebSocket.prototype.onmessage;
		
		window.WebSocket.prototype.send = function(...args) { 
			SocketInjection.call(this, ...args);
			return window.originalSend.call(this, ...args); 
		};
		/*window.WebSocket.prototype.onmessage = function(...args) { 
			SocketInjection.call(this, ...args);
			return window.originalOnMessage.call(this, ...args); 
		};*/
	}
	
	function inject(fn) {
    const script = document.createElement('script');
    script.text = `(${fn.toString()})();`
    document.documentElement.appendChild(script)
	}

	inject(script);
	
	return true;
}


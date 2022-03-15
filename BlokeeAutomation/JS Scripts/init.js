function(element, input){
	
	function script() 
	{	
		//adding a string.format function, will probably need it a lot here
		String.prototype.format = function() {
		  a = this;
		  for (k in arguments) {
			a = a.replace("{" + k + "}", arguments[k])
		  }
		  return a
		}
		
		//function that will be executed each time I 
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
						window.gameId = msgObj.fields.gameId;
						
					} else if(event.data.indexOf("board") != -1)
					{
						if(event.data.indexOf("INPROGRESS") != -1 && event.data.indexOf("Id")!= -1)
						{
							//save the game id if this is the Game Started Payload
							window.gameId = msgObj.id;
						}
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
		window.WebSocket.prototype.send = function(...args) { 
			SocketInjection.call(this, ...args);
			return window.originalSend.call(this, ...args); 
		};
		
		var Ticker = window.setInterval(function(){
			//update the current player
			var playerSegments= $("div.ticking").parents("div.ui.column");
			if(playerSegments.length >0)
			{
				document.body.setAttribute("uipath-automation-currentPlayer",Array.from(playerSegments[0].parentElement.children).indexOf(playerSegments[0]));
			}
			
			var rawPayload = document.body.getAttribute("uipath-automation-move");
			if(rawPayload)
			{
				var payload = JSON.parse(rawPayload);
				//send a payload to the server, causing the next move to happen
				
				if(payload.pass)
				{
					var request = "[\"{\\\"msg\\\":\\\"method\\\",\\\"method\\\":\\\"gamePlayerAction\\\",\\\"params\\\":[{\\\"gameId\\\":\\\"{0}\\\",\\\"action\\\":{\\\"type\\\":\\\"pass\\\",\\\"playerIndex\\\":{1}}}],\\\"id\\\":\\\"{2}\\\",\\\"randomSeed\\\":\\\"{3}\\\"}\"]".format( 
					window.gameId,
					payload.playerIndex,					
					parseInt(window.payloadId)+1,
					window.generateRandomSeed());
				}
				else{
					var request = "[\"{\\\"msg\\\":\\\"method\\\",\\\"method\\\":\\\"gamePlayerAction\\\",\\\"params\\\":[{\\\"gameId\\\":\\\"{0}\\\",\\\"action\\\":{\\\"type\\\":\\\"place\\\",\\\"playerIndex\\\":{1},\\\"pieceIndex\\\":{2},\\\"orientation\\\":{3},\\\"position\\\":[{4},{5}]}}],\\\"id\\\":\\\"{6}\\\",\\\"randomSeed\\\":\\\"{7}\\\"}\"]".format( 
					window.gameId, 
					payload.playerIndex, 
					payload.pieceIndex,
					payload.orientation,
					payload.positionX,
					payload.positionY,
					parseInt(window.payloadId)+1,
					window.generateRandomSeed());
				}
				console.log("Sending message:");
				console.log(request);
				
				document.body.setAttribute("uipath-automation-move","");
				window.sockets[window.sockets.length -1].send(request);
				
			}
			
		}, 100);
	}
	
	function inject(fn) {
    const script = document.createElement('script');
    script.text = `(${fn.toString()})();`
    document.documentElement.appendChild(script)
	}

	inject(script);
	
	return true;
}


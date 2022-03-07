function(element, input){
	
	var s = document.createElement("script");
    s.type = "text/javascript";
    var code = '!function(e,n){if(console.log("Initializing JS Injection"),console.log(window),document.injectionDone)throw"JS Injection Already active";window.sockets=[],window.messageQueue=[],window.gameId="",window.payloadId="",window.gameBoard=null,window.enemyMoves=null,document.injectionDone=!0,window.generateRandomSeed=function(){return x=Package.ddp.DDP.randomStream("/rpc/gamePlayerAction"),x.hexString(20)},window.originalSend=WebSocket.prototype.send,window.WebSocket.prototype.send=function(...e){return-1===window.sockets.indexOf(this)&&(window.sockets.push(this),this.addEventListener("message",(function(e){console.log(e.data);var n=JSON.parse(e.data.substring(3,e.data.length-2).replaceAll("\\\\",""));if(window.messageQueue.push(e.data),-1!=e.data.indexOf("Started")&&-1!=e.data.indexOf("gameId"))window.gameId=n.fields.gameId;else if(-1!=e.data.indexOf("board")){if(window.gameBoard=n.fields.data.board,n.fields.actionItems){for(var i=[[],[],[],[]],d=0;d<n.fields.actionItems.length;d++)"place"==n.fields.actionItems[d].action.type&&i[n.fields.actionItems[d].action.playerIndex].push(n.fields.actionItems[d].action.pieceIndex);window.enemyMoves=i}}else-1==e.data.indexOf("id")||isNaN(n.id)||(window.payloadId=n.id)}))),window.originalSend.call(this,...e)}}();';
    try {
      s.appendChild(document.createTextNode(code));
      document.body.appendChild(s);
    } catch (e) {
		console.log("Found exception!");
		console.log(e);
		s.text = code;
		document.body.appendChild(s);
	}
	return true;
}


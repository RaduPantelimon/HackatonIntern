function(element, input){
	
	console.log(input);
	var inputArray = input.split(";");
	if(input.startsWith("passgame") || input.startsWith("ragequit"))
	{
		var passPayload = {'pass':true,'playerIndex':inputArray[1]};
		console.log(JSON.stringify(passPayload));
		document.body.setAttribute("uipath-automation-move",JSON.stringify(passPayload));
	}
	else{	
		var movePayload = {'playerIndex':inputArray[0],'pieceIndex':inputArray[1],'orientation':inputArray[2],'positionX':inputArray[3],'positionY':inputArray[4]};
		console.log(JSON.stringify(movePayload));
		document.body.setAttribute("uipath-automation-move",JSON.stringify(movePayload));
	}

	return true;
}


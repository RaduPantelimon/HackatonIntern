function(element, input){
	
	console.log(input);
	var inputArray = input.split(";");	
	var movePayload = {'playerIndex':inputArray[0],'pieceIndex':inputArray[1],'orientation':inputArray[2],'positionX':inputArray[3],'positionY':inputArray[4]};
	console.log(JSON.stringify(movePayload));
	document.body.setAttribute("uipath-automation-move",JSON.stringify(movePayload));
	return true;
}


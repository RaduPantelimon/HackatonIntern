function(element, input){
	console.log("Retrieving game info via JS Injection");
	var data = document.body.getAttribute("uipath-automation-data");
	console.log("Data found: " + data);
	return data;
}
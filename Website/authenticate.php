<?php
session_start();

include 'connect.php';

//function for the json response
//text_response is text to be used in function on client side (either "success" or not)
//content is the actual response to be sent back, either the url of the saved image or nothing if failed
function respondWithJson($text_response, $content="") {

    //create array for use in json response
    $response = array("result"=> $text_response, "content"=>$content );
    //encode json
    echo json_encode($response);

}

//check if username and password were submitted
if ( !isset($_POST['username'], $_POST['password']) ) {
	// Could not get the data that should have been sent.
	respondWithJson('Failure', 'Please fill both the username and password fields!');
}
// prepare SQL statement to prevent sqlinjection.
if ($stmt = $conn->prepare('SELECT id, password FROM accounts WHERE username = ?')) {
	//binding username input to a string to protect from sqlinjection
	$stmt->bind_param('s', $_POST['username']);
	$stmt->execute();
	// Storing result to check if the account exists
	$stmt->store_result();
	
	if ($stmt->num_rows > 0) {
	$stmt->bind_result($id, $password);
	$stmt->fetch();
	// user exists, verify password
	if (password_verify($_POST['password'], $password)) {
		// password verified so create sessions
		session_regenerate_id();
		$_SESSION['loggedin'] = TRUE;
		$_SESSION['name'] = $_POST['username'];
		$_SESSION['id'] = $id;
		respondWithJson('Success','Signed in successfully'); // redirect to homepage
	} else {
		// Incorrect password
		respondWithJson('Failure','Incorrect username and/or password!');
	}
} else {
	// Incorrect username
	respondWithJson('Failure','Incorrect username and/or password!');
}
	$stmt->close();
}
?>
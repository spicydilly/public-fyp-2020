<?php

include 'connect.php';

//function for the json response
//text_response is text to be used in function on client side (either "success" or not)
//content is the actual response to be sent back,
function respondWithJson($text_response, $content="") {

    //create array for use in json response
    $response = array("result"=> $text_response, "content"=>$content );
    //encode json
    echo json_encode($response);
}

// Now we check if the data was submitted, isset() function will check if the data exists.
if (!isset($_POST['username'], $_POST['password'])) {
	// Could not get the data that should have been sent.
	respondWithJson('Failure','Please complete the registration form!');
}
// Make sure the submitted registration values are not empty.
elseif (empty($_POST['username']) || empty($_POST['password'])) {
	// One or more values are empty.
	respondWithJson('Failure','Please complete the registration form');
}
//invalid chars validation
elseif (preg_match('/^[a-zA-Z0-9]+$/', $_POST['username']) == 0) {
    respondWithJson('Failure','Username is not valid!');
}
//char length check
elseif (strlen($_POST['username']) > 20 || strlen($_POST['username']) < 5) {
	respondWithJson('Failure','Username must be between 5 and 20 characters long!');
}
//char length check
elseif (strlen($_POST['password']) > 20 || strlen($_POST['password']) < 5) {
	respondWithJson('Failure','Password must be between 5 and 20 characters long!');
}
// We need to check if the account with that username exists.
elseif ($stmt = $conn->prepare('SELECT id, password FROM accounts WHERE username = ?')) {
	// Bind parameters (s = string, i = int, b = blob, etc), hash the password using the PHP password_hash function.
	$stmt->bind_param('s', $_POST['username']);
	$stmt->execute();
	$stmt->store_result();
	// Store the result so we can check if the account exists in the database.
	if ($stmt->num_rows > 0) {
		// Username already exists
		respondWithJson('Failure','Username exists, please choose another!');
	} else {
		// Username doesnt exists, insert new account
		if ($stmt = $conn->prepare('INSERT INTO accounts (username, password) VALUES (?, ?)')) {
			// We do not want to expose passwords in our database, so hash the password and use password_verify when a user logs in.
			$password = password_hash($_POST['password'], PASSWORD_DEFAULT);
			$stmt->bind_param('ss', $_POST['username'], $password);
			$stmt->execute();
			respondWithJson('Success','You have successfully registered, you can now login!');
		} 
		else {
			// Something is wrong with the sql statement, check to make sure accounts table exists with all 3 fields.
			respondWithJson('Failure','Could not prepare statement!');
		}
	}
	$stmt->close();
} else {
	// Something is wrong with the sql statement, check to make sure accounts table exists with all 3 fields.
	respondWithJson('Failure', 'Could not prepare statement!');
}
$conn->close();
?>
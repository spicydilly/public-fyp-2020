<?php
//this returns json containing the maps that the signed in user has created

//connect to db
include "connect.php";

header('Content-type: application/json');

//for checking session variables
session_start();

//check if logged in
if($_SESSION["loggedin"]) { 

	$sql_stmt = $conn->prepare("SELECT map_id, map_name, map_description FROM Maps WHERE created_by_user = ". $_SESSION['id']);
	$sql_stmt->execute();
	$sql_stmt->bind_result($map_id, $map_name, $map_desc);
	$outp['maps'] = [];
	while($sql_stmt->fetch()) {
			$temp = array("name" => $map_name, "desc" => $map_desc, "locations_json" =>$map_id);
			array_push($outp['maps'], $temp);
		}
		
	$sql_stmt->close();

	//return the data
	echo json_encode($outp);
	
	} else {

	// no need for a fancy response the user is not logged in
	echo 'Not Logged In';
}
?>
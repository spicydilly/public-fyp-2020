<?php
//this returns json of the map the user is requesting to edit in the create map app

//connect to db
include "connect.php";

header('Content-type: application/json');

//store the requested mapID
$map_id = $_POST['id'];

//for checking session variables
session_start();

//check if logged in
if($_SESSION["loggedin"]) { 

	//fetch the requested maps info, only if the user is the one who created that map
	$sql_stmt = $conn->prepare("SELECT Loc.location_id, Loc.location_name, Loc.location_description, Loc.image_360_url, Loc.posx, Loc.posz, Loc.roty, Loc.scalex, Loc.scalez FROM Map_Locations AS Loc JOIN Maps ON Loc.map_id = Maps.map_id WHERE Loc.map_id = ? and Maps.created_by_user = ". $_SESSION['id']);
	$sql_stmt->bind_param('i', $map_id);
	$sql_stmt->execute();
	$sql_stmt->bind_result($loc_id, $loc_name, $loc_desc, $loc_image, $loc_posx, $loc_posz, $loc_roty, $loc_scalex, $loc_scalez);
	$outp['map_locations'] = [];
	$outp['map_base'] = [];
	while($sql_stmt->fetch()) {
		$temp = array("id" => $loc_id, "name" => $loc_name, "desc" => $loc_desc, "image_url" => $loc_image, "posx" =>$loc_posx, "posz" =>$loc_posz, "roty" => $loc_roty, "scalex" => $loc_scalex, "scalez" => $loc_scalez);
		if ($loc_name == "Map Base"){
			array_push($outp['map_base'], $temp);
		}
		else {
			array_push($outp['map_locations'], $temp);
		}	
		
		}
		
	$sql_stmt->close();

	//return the data
	echo json_encode($outp);
	
	} else {

	// no need for a fancy response the user is not logged in
	echo 'Not Logged In';
}

?>
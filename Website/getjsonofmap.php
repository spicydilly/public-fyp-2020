<?php
//this file returns a json file holding the maps locations

header("Content-Type: application/json; charset=UTF-8");
//connect to db
include "connect.php";

//store the requested mapID
$map_id = $_GET['id'];

//this time preparing sql statement to prevent sql injection
$sql_stmt = $conn->prepare("SELECT Loc.location_id, Loc.location_name, Loc.location_description, Loc.image_360_url, Loc.posx, Loc.posz, Loc.roty, Loc.scalex, Loc.scalez FROM Map_Locations AS Loc JOIN Maps ON Loc.map_id = Maps.map_id WHERE Loc.map_id = ? and Maps.published = '1'");
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

//return json, the application will decode
echo json_encode($outp);
$sql_stmt->close();
?>
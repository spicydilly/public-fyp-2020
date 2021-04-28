<?php
//this file returns a json file holding the maps locations

header("Content-Type: application/json; charset=UTF-8");
//connect to db
include "connect.php";

//only return maps that are published
$sql_stmt = $conn->prepare("SELECT map_id, map_name, map_description FROM Maps WHERE published = 1");
$sql_stmt->execute();
$sql_stmt->bind_result($map_id, $map_name, $map_desc);
$outp['maps'] = [];
while($sql_stmt->fetch()) {
		$temp = array("name" => $map_name, "desc" => $map_desc, "locations_json" =>$map_id);
		array_push($outp['maps'], $temp);
	}
	
//return json, the application will decode
echo json_encode($outp);
$sql_stmt->close();
?>
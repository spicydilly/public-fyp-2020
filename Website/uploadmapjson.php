<?php

//for checking session variables
session_start();

include 'connect.php';

//function for the json response
//text_response is text to be used in function on client side (either "success" or not)
//content is the actual response to be sent back
function respondWithJson($text_response, $content="") {

    //create array for use in json response
    $response = array("result"=> $text_response, "content"=>$content );
    //encode json
    echo json_encode($response);

}

function getTimeForMySQL () {
	date_default_timezone_set('Europe/Dublin');
	$date = date("Y/m/d h:i A");
    $final = strtotime($date);
    $time_posted = date("Y/m/d h:i A", $final);
    return $time_posted;
}

//check if logged in
if($_SESSION["loggedin"]) { 

	//store the json data
	$data = json_decode(file_get_contents('php://input'), true);

	//first check if number of placed objects is within limits, return an error otherwise, this is check in App before saving so should only occur if user tries uploading json manually
	if (count($data['map_locations']) == 0) {
		respondWithJson("Failure", "This map has no locations!");
	}
	elseif (count($data['map_locations']) >= 50) {
		respondWithJson("Failure", "Number of uploaded objects exceeds limit!");
	}
	//check if the maps id, if 0 it means the map is new, else it is a map update
	elseif ($data['map_id'] == 0) {

		//lets make sure the map name is not already in use
		if ($sql_stmt_map = $conn->prepare("SELECT * FROM Maps WHERE map_name = ?")) {
			$sql_stmt_map->bind_param('s', $data['map_base'][0]['name']);
			$sql_stmt_map->execute();
			$sql_stmt_map->store_result();
			if ($sql_stmt_map->num_rows > 0) {
				//name already in use
				respondWithJson("Failure", "Map Name : '" . $data['map_base'][0]['name'] . "' - Already in use");
			} else {
				//continue
				//get the mapbase data and attempt to create the new map
				//ensure to escappe data to protect against sql injection
				if ($sql_stmt_map = $conn->prepare("INSERT INTO Maps (map_name, map_description, create_date, created_by_user, published) VALUES (?, ?, ?, ?, ?)")) {

					$sql_stmt_map->bind_param('sssii', $data['map_base'][0]['name'], $data['map_base'][0]['desc'], getTimeForMySQL(), $_SESSION['id'], $data['publish']);
					$sql_stmt_map->execute();
					//map inserted

					//set the map id, is the id of the previous insert
					$map_id = $sql_stmt_map->insert_id;

					//create the sql statement, inserting multiple entries at once
					$sql_stmt_locations = "INSERT INTO Map_Locations (map_id,location_name, location_description, image_360_url, posx, posz, scalex, scalez, roty) VALUES ";
					foreach ($data['map_locations'] as $location) {
						//skip location if it is using the reserved name (Map Base)
						if ( mysqli_real_escape_string($conn, $location['name']) != "Map Base") {
							//ensure to escappe data to protect against sql injection
						$sql_stmt_locations .= "('" . (int)$map_id . "', '" . mysqli_real_escape_string($conn, $location['name']) . "', '" . mysqli_real_escape_string($conn, $location['desc']) ."', '" . mysqli_real_escape_string($conn, $location['image_url']) . "', '" . (float)$location['posx'] . "', '" . (float)$location['posz'] . "', '" . (float)$location['scalex'] . "', '" . (float)$location['scalez'] . "', '" . (float)$location['roty'] . "'),";
						}
						
					}

					//finally add the map base data
					$sql_stmt_locations .= "('" . (int)$map_id . "', 'Map Base', '', '" . mysqli_real_escape_string($conn, $data['map_base'][0]['image_url']) . "', '" . (float)$data['map_base'][0]['posx'] . "', '" . (float)$data['map_base'][0]['posz'] . "', '0', '0', '0')";

					//insert 
					$result = mysqli_query($conn, $sql_stmt_locations);
					if(!$result)
					{
					    //something went wrong
					    respondWithJson("Failure", "Something went wrong while uploading. Please try again later.");
					    //echo mysqli_error($conn); //debugging purposes
					}
					else
					{	
						//succesfully saved, so return the maps ID for the application to handle
					    respondWithJson("Success", $map_id);
					}

				} else {
					//something went wrong, statement could not be prepared
					respondWithJson("Failure", "Something went wrong while uploading. Please try again later.");
				}

			}
				
		} else {
			//something went wrong, statement could not be prepared
			respondWithJson("Failure", "Something went wrong while uploading. Please try again later.");
		}


	} else {
		// this means an existing map is being updated
		//will be done in 3 stages, first update the maps name and description, an insertion of the new data combined with an update of the existing data, and the deletion of the removed objects

		//first update the maps name and description
		$sql_map_stmt = $conn->prepare("UPDATE Maps SET map_name = ?, map_description = ?, published = ? WHERE map_id = ? AND created_by_user = ?");
		$sql_map_stmt->bind_param('ssiii', $data['map_base'][0]['name'], $data['map_base'][0]['desc'], $data['publish'], $data['map_id'], $_SESSION['id']);
		$sql_map_stmt->execute();

		//now fetch that same row, to make sure a row is returned which proves this user is authenticated to edit this map
		$sql_map_stmt = $conn->prepare("SELECT * FROM Maps WHERE map_id = ? AND created_by_user = ?");
		$sql_map_stmt->bind_param('ii', $data['map_id'], $_SESSION['id']);
		$sql_map_stmt->execute();
		$sql_map_stmt->store_result();

		//if a row returned
		if ( $sql_map_stmt->num_rows > 0 ) {
			
			//create the sql statement for inserting multiple entries at once, using ON DUPLICATE KEY so if the ID already exists, then the data is instead updated. Inserting an ID of 0 will automatically trigger mysql to set the id as the next AUTO_INCREMENT value
			$sql_stmt_locations = "INSERT INTO Map_Locations (location_id, map_id,location_name, location_description, image_360_url, posx, posz, scalex, scalez, roty) VALUES ";

			//create the sql statement for removing locations
			$sql_stmt_remove = "DELETE FROM Map_Locations WHERE location_id in(";
			$doRemove = false; //will be true when a remove statement is needed

			foreach ($data['map_locations'] as $location) {
				// if the locations scale isn't 0, it is to be added/updated
				if ( $location['scalex'] != 0) {
					//ensure to escappe data to protect against sql injection
				$sql_stmt_locations .= "('" . (int)$location['id'] ."', '" . (int)$data['map_id'] . "', '" . mysqli_real_escape_string($conn, $location['name']) . "', '" . mysqli_real_escape_string($conn, $location['desc']) ."', '" . mysqli_real_escape_string($conn, $location['image_url']) . "', '" . (float)$location['posx'] . "', '" . (float)$location['posz'] . "', '" . (float)$location['scalex'] . "', '" . (float)$location['scalez'] . "', '" . (float)$location['roty'] . "'),";
				} else {
					//add to remove statment
					$doRemove = true;
					$sql_stmt_remove .= (int)$location['id'] . ",";
				}
			}

			//finish the remove statement is a removal is needed
			if ( $doRemove ) {
				$sql_stmt_remove = rtrim($sql_stmt_remove, ","); //remove the trailing comma
				$sql_stmt_remove .= ")";
			}

			//add the map base data
			$sql_stmt_locations .= "('" . (int)$data['map_base'][0]['id'] . "','" . (int)$data['map_id'] . "', 'Map Base', '', '" . mysqli_real_escape_string($conn, $data['map_base'][0]['image_url']) . "', '" . (float)$data['map_base'][0]['posx'] . "', '" . (float)$data['map_base'][0]['posz'] . "', '0', '0', '0')";
			
			//use ON DUPLICATE KEY to instead update when an existing id is added, also using if statement to prevent editing of an existing location if the stored map_id of the existing isn't the same as the map_id we are uploading
			$sql_stmt_locations .= " ON DUPLICATE KEY UPDATE location_name = IF(map_id = " . (int)$data['map_id'] . ", VALUES(location_name), location_name), location_description = IF(map_id = " . (int)$data['map_id'] . ", VALUES(location_description), location_description), image_360_url = IF(map_id = " . (int)$data['map_id'] . ", VALUES(image_360_url), image_360_url), posx = IF(map_id = " . (int)$data['map_id'] . ", VALUES(posx), posx), posz = IF(map_id = " . (int)$data['map_id'] . ", VALUES(posz), posz), scalex = IF(map_id = " . (int)$data['map_id'] . ", VALUES(scalex), scalex), scalez = IF(map_id = " . (int)$data['map_id'] . ", VALUES(scalez), scalez), roty = IF(map_id = " . (int)$data['map_id'] . ", VALUES(roty), roty)";

			//insert 
			$result = mysqli_query($conn, $sql_stmt_locations);
			if(!$result)
			{
			    //something went wrong
			    respondWithJson("Failure", "Something went wrong while updating the map. Please try again later.");
			    //echo mysqli_error($conn); //debugging purposes
			}
			else
			{
				//now remove locations if needed
				if ($doRemove) {
					//insert 
					$result = mysqli_query($conn, $sql_stmt_remove);
					if(!$result)
					{
					    //something went wrong
					    respondWithJson("Failure", "Something went wrong while deleting old locations. Try again later.");
					    //echo mysqli_error($conn); //debugging purposes
					} else {
					respondWithJson("Success", $data['map_id']);
					}
				} else {
					respondWithJson("Success", $data['map_id']);
				}
			    
			}
		} else {
			//nothing was updated, map either doesn't exist or this user didn't create the original map
			respondWithJson("Failure", "Map doesn't exist, cannot update map.");
		}

	}

} else {

	// no need for a fancy response the user is not logged in
	echo 'Not Logged In';
}

$conn->close();

?>
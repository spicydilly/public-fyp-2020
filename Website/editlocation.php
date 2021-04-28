<?php

//for checking session variables
session_start();

include 'connect.php';

//store the inputs
$map_id = $_POST['mapid'];
$location_id = $_POST['locid'];
$location_name = $_POST['name'];
$location_desc = $_POST['desc'];
$image_url = $_POST['imageurl'];

//function for the json response
//text_response is text to be used in function on client side (either "success" or not)
//content is the actual response to be sent back,
function respondWithJson($text_response, $content="") {

    //create array for use in json response
    $response = array("result"=> $text_response, "content"=>$content );
    //encode json
    echo json_encode($response);

}


//check if logged in
if($_SESSION["loggedin"]) { 

	//make sure the map name isn't "Map Base" as this is reserved
	if ($location_name != "Map Base") {

		$content = "";
		//check if image is attached 
		if ( isset($_FILES["imagefile"]) && $_FILES["imagefile"]["size"] > 0 )
		{
			//will add time and the users id to the saved file making sure no duplicates exist in the directory
			$target_dir = "360mapimages/". time() . '_' . $_SESSION['id'];
			$target_file = $target_dir . basename($_FILES["imagefile"]["name"]);
			$uploadOk = 1;
			$imageFileType = strtolower(pathinfo($target_file,PATHINFO_EXTENSION));

			// Check if image file is a actual image or fake image
			$check = getimagesize($_FILES["imagefile"]["tmp_name"]);
			if($check !== false) {
				$content = "";
			} else {
				$content = "File is not an image.";
			}

			// Check if file already exists, this should not happen but if it does
			// just ask user to try again
			if (file_exists($target_file)) {
				$content .= "Error occured, please try again.";
			}

			// Check file size, ~10mb limit
			if ($_FILES["imagefile"]["size"] > 10000000) {
				$content .= "Sorry, your file is too large.";
			}

			// Allow certain file formats
			if($imageFileType != "jpg" && $imageFileType != "png" && $imageFileType != "jpeg" ) {
				$content .= "Only JPG, JPEG & PNG files are allowed.";
			}

			// if content isn't empty, a failure occured
			if ($content != "") {
				respondWithJson("Failure", $content);
			} else { // if everything is ok, try to upload file
				if (move_uploaded_file($_FILES["imagefile"]["tmp_name"], $target_file)) {
					//will add the local url to the image url
					$imageurl = "https://www.thedylon.com/fyp2020/" . $target_file;
				} else {
		      	//this would be a major error
		      		respondWithJson("Failure","Sorry, there was an error uploading your file.");
		    	}
			}

		} 
			
	  	//first update the maps name and description
		$sql_map_stmt = $conn->prepare("UPDATE Map_Locations SET location_name = ?, location_description = ?, image_360_url = ? WHERE location_id = ? AND map_id = (SELECT map_id FROM Maps WHERE map_id = ? AND created_by_user = ?)");
		$sql_map_stmt->bind_param('sssiii', $location_name, $location_desc, $imageurl, $location_id, $map_id, $_SESSION['id']);
		$sql_map_stmt->execute();

		//completed update
		respondWithJson("Success", "Successfully Updated!");
		
	} else {
		respondWithJson("Failure", "'Map Base' is a resvered name!");
	}

	

} else {

	// user is not logged in
	respondWithJson("Failure", "You are not logged in!");
}

$conn->close();


 ?>
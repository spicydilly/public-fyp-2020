<?php
//for checking session variables
session_start();
//make sure user is logged in

if($_SESSION["loggedin"]) {

	//connect to db
	include "connect.php";

	//store the requested location id
	$location_id = $_POST['id'];

	//this time preparing sql statement to prevent sql injection
	$sql_stmt = $conn->prepare("SELECT location_id, location_name, location_description, image_360_url, map_id FROM Map_Locations WHERE location_id = ?");
	$sql_stmt->bind_param('i', $location_id);
	$sql_stmt->execute();
	$sql_stmt->bind_result($loc_id, $loc_name, $loc_desc, $image_360, $map_id);
	while($sql_stmt->fetch()) {
		//ignore map base
		echo '<input id="mapid" name="mapid" type="hidden" value="' . $map_id . '">
		<input id="locationid" name="locationid" type="hidden" value="' . $loc_id . '">
		<input id="imageurl" name="imageurl" type="hidden" value="' . $image_360 . '">
	  <div class="mb-3">
		<label for="locationName">Location Name</label>
		<input type="text" class="form-control" id="locationName" value="' . $loc_name . '">
	  </div>
	  <div class="mb-3">
		<label for="description">Description</label>
		<textarea type="text" class="form-control" rows="4" id="description" placeholder="Description of location">' . $loc_desc . '</textarea>
		<small id="locationHelp" class="form-text text-muted">A short description of this location.</small>
		</div>';

		if ($image_360==null) {
			echo '<div class="mb-3">
				<label for="imagefile">Upload 360 Image</label>
		    <input class="form-control" type="file" class="form-control-file" id="imagefile" name="imagefile">
		    <small id="imagefileHelp" class="form-text text-muted">This image will be visitable by users via the Portal in the application.</small>
			</div>';
		} else {
			echo '<div class="mb-3">
				<label for="imagefile">Overwrite existing 360 Image</label>
		    <input class="form-control" type="file" class="form-control-file" id="imagefile" name="imagefile">
		    <small id="imagefileHelp" class="form-text text-muted">Max size of 10MB. This image will be visitable by users via the Portal in the application.</small>
			</div>';
		}
	}
	$sql_stmt->close();

	}
else {
	echo 'Not Logged In';
}
?>
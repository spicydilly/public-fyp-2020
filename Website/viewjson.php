<?php

//title of page, is passed to header
$title = "View Maps";
include "header.php";
	
//check if logged in, this page is admin only
if($_SESSION["loggedin"]) {	
	
	$json_string =    file_get_contents("./general/map_locations.json");
	$parsed_json = json_decode($json_string, true);
	
?>
<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
	<h1 class="h2">Dashboard</h1>
</div>
<div class="table-responsive">
<table class="table table-hover table-bordered table-striped table-sm"><thead><tr><th scope="col">Name</th><th scope="col">Description</th><th scope="col">Image URL</th></tr></thead>


<?php 
	foreach ($parsed_json as $key => $value) {
		foreach ($value as $key2 => $value2) {
			//if name is "MapBase" ignore as it is just default entry
			if($value2['name'] != "Map Base") {
				echo '<tr><td>' . $value2['name'] . '</td><td>' . $value2['desc'] . '</td><td>' . $value2['image_url'] . '</td></tr>';
			}		
		}
	}
	echo '</table></div>';
} else //not logged in
{ 
?>
<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
	<h1 class="h2">You must be signed in to view this page</h1>
	</div>
</div>
<?php 
} 
?>

<?php
include "footer.php";
?>
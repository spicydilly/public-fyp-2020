<?php

//connect to db
include "connect.php";

//datatables is required
$datatables = true;

//title of page, is passed to header
$title = "View Maps";
include "header.php";
	
//function fetches all the maps and prints to a table
function getAllMaps ($conn) {
		
	$sql_stmt = "SELECT * FROM Maps WHERE published = 1 OR created_by_user = ". $_SESSION['id'];
	$sql_result = mysqli_query($conn, $sql_stmt);
	if($sql_result) {
		while ($row = mysqli_fetch_assoc($sql_result)) {
			echo '<tr><td>' . $row['map_name'] . '</td><td>' . $row['map_description'] . '</td><td>';
			if ($row['created_by_user'] == $_SESSION['id']) {
				echo '<button type="button" class="btn btn-success btn-sm" data-bs-toggle="tooltip" data-bs-placement="top" title="Created by you"><i class="fas fa-thumbs-up"></i></button>';
			}
			echo '</td><td><button type="button" data-mapname="' . $row['map_name'] . '" data-id="'. $row['map_id'] . '" class="btn btn-info btn-sm mapbtn"><i class="fas fa-database"></i> View Data</a></td></tr>';
		}
	}
}
	
//check if logged in, this page is admin only
if($_SESSION["loggedin"]) {	
?>
<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
	<h1 class="h2">All Maps <small class="text-muted">Showing published maps and maps created by you</small></h1>
</div>
<div class="table-responsive">
<table id="allMaps" class="table table-hover table-bordered table-striped table-sm"><thead><tr><th scope="col">Map</th><th scope="col">Description</th><th scope="col">Created By You</th><th scope="col"></th></tr></thead>

<?php 
	getAllMaps($conn);
?>
</table></div>
<hr />
<!-- map info inserted here -->
<div class="mapviewtable">
</div>
<!-- activate datatables-->
<script>
	$(document).ready(function() {var tableAll = $('#allMaps').removeAttr('width').DataTable( {
        columnDefs: [
            { width: '10%', targets : [-1, -2] },
			{ orderable: false, searchable: false, targets : -1 }
        ],
        fixedColumns: true
    } ); 
	$('.mapbtn').click(function(){
   		var mapname = $(this).data('mapname');
		var location_id = $(this).data('id');
			// AJAX request
			$.ajax({
			url: 'viewthismap.php',
			type: 'post',
			data: {id: location_id},
			success: function(response){ 
				$('.mapviewtable').html(response);
				$('.mapviewheading').html(mapname);
			}
		});
	});
});
</script>

<?php 
} else {
//not logged in	
?>
<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
	<h1 class="h2">You must be signed in to view this page</h1>
	</div>
</div>
<?php 
} 

include "footer.php";
?>
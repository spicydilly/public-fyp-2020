<?php
//for checking session variables
session_start();

//connect to db
include "connect.php";
	
//store the requested mapID
$map_id = $_POST['id'];

	
//function fetches all the maps and prints to a table
function getLocations ($conn, $map_id) {
		
	//this time preparing sql statement to prevent sql injection
	$sql_stmt = $conn->prepare("SELECT Loc.location_id, Loc.location_name, Loc.location_description, Loc.image_360_url , Maps.created_by_user FROM Map_Locations AS Loc JOIN Maps ON Loc.map_id = Maps.map_id WHERE Loc.map_id = ?");
	$sql_stmt->bind_param('i', $map_id);
	$sql_stmt->execute();
	$sql_stmt->bind_result($loc_id, $loc_name, $loc_desc, $image_360 ,$createdby);
	while($sql_stmt->fetch()) {
		//ignore map base
		if ($loc_name != "Map Base") {
			echo '<tr><td id="name' . $loc_id . '">' . $loc_name . '</td><td id="desc' . $loc_id . '">' . $loc_desc . '</td>';
			if ($image_360==null) {
				echo '<td data-order="1"><a href="#" class="btn btn-secondary btn-sm disabled"><i class="fas fa-external-link-alt"></i> View</a></td>';
			} else {
				echo '<td data-order="0"><a href="'. $image_360 .'" class="btn btn-primary btn-sm" target="_blank"><i class="fas fa-external-link-alt"></i> View</a></td>';
			}
			//if created by the logged in user, allow edit
			if ( $createdby == $_SESSION['id']) {
			echo '<td><button type="button" data-id=' . $loc_id . ' class="btn btn-primary btn-sm editbtn"><i class="far fa-edit"></i> Edit</button></td>';
			} else {
				echo '<td></td>';
			}
			echo '</tr>';
		}
	}
	$sql_stmt->close();
}
	
//check if logged in, this page is admin only
if($_SESSION["loggedin"]) {	

?>
<!-- modal for editing -->
<div class="modal fade" id="editLocationModal" tabindex="-1" aria-labelledby="editLocation" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="editLocationLabel">Edit Location</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <form role="form" id="editLocationForm">
      <div class="modal-body">
      	
	      <div id="editModalBody"></div>
	   </div>
		  <div class="modal-footer">
		  	<button type="submit" id="editLocationSubmit" class="btn btn-primary btn-md">Save Changes</button>
		  </div>
		</form>
    </div>
 </div>
</div>
<h2 class="mapviewheading"></h2>
<div class="table-responsive">
<table id="mapLocations" class="table table-hover table-bordered table-striped table-sm">
	<thead>
		<tr>
			<th scope="col">Map</th>
			<th scope="col">Description</th>
			<th scope="col">360 Image</th>
			<th scope="col">Options</th>
		</tr>
	</thead>

<?php 
	//show the locations via function
	getLocations($conn, $map_id);
?>
</table></div>
<!-- activate datatables-->
<!-- ajax request to editmodal -->
<script>$(document).ready(function() {
    var table = $('#mapLocations').removeAttr('width').DataTable( {
        scrollY:        '300px',
        scrollCollapse: true,
        paging:         false,
        columnDefs: [
            { width: '20%', targets : 0 },
			{ orderable: false, targets : -1 },
			{ searchable: false, targets: [-1,-2] }
        ],
        fixedColumns: true
    } );
	$('.editbtn').click(function(){
   
		var location_id = $(this).data('id');
			// AJAX request
			$.ajax({
			url: 'editlocationmodal.php',
			type: 'post',
			data: {id: location_id},
			success: function(response){ 
				// Add response in Modal body
				$('#editModalBody').html(response);
				// Display Modal
				$('#editLocationModal').modal('show'); 
			}
		});
	});
} );</script>
<script>
	$("#editLocationForm").submit(function(event){
	    // cancels the form submission
	    event.preventDefault();
	    submitEdit();
	});
	function submitEdit(){
		// Initiate Variables With Form Content
		var newForm = new FormData();
		newForm.append('imagefile', $('input[type=file]')[0].files[0]);
		newForm.append( 'mapid', $('#mapid').val());
		newForm.append( 'locid', $("#locationid").val());
	    newForm.append( 'name', $("#locationName").val());
	    newForm.append( 'desc', $("#description").val());
	    newForm.append( 'imageurl', $("#imageurl").val());
		var desc = $("#description").val(); 
		$.ajax({
		    type: "post",
	        url: "editlocation.php",
	        enctype: 'multipart/form-data',
	        data: newForm,
	        dataType: "json",
			success: function(response) {
		        if (response["result"] == "Success") {
	            	alert(response["content"]);
	            	//update the tables data
	            	$('#name' + $("#locationid").val()).html($("#locationName").val());
	            	$('#desc' + $("#locationid").val()).html($("#description").val());

	        	} else {
	        	    alert(response["content"]);
	    	    }
		  	},
			cache: false,
			contentType: false,
			processData: false
	   	});
	}
</script>
<?php
} else //not logged in
{ 
?>
<h1 class="h2">You must be signed in to view this page</h1>
<?php 
} 
?>

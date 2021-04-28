<?php

//connect to db
include "connect.php";

//unity required
$unity = true;

//title of page, is passed to header
$title = "Create Map";
include "header.php";

//check if logged in, this page is admin only
if($_SESSION["loggedin"]) {	

?>

<!-- modal for uploading map base images -->
<div class="modal fade" id="uploadMapBaseModal" tabindex="-1" aria-labelledby="uploadImage" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="signInLabel">Upload Map Image</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
      	<form role="form" id="mapfile" enctype="multipart/form-data" method="post" name="mapfile">
			<div class="mb-3">
			    <label for="mapbasefile">Upload your maps image here, this will be used as your maps background</label>
			    <input class="form-control" type="file" class="form-control-file" id="mapbasefile" name="mapbasefile">
			    <div id="mapbaseHelp" class="form-text">Max Image 10MB. Please allow some time for the upload to complete</div>
			</div>
			<div class="mb-3" id="uploadresult">
			</div>
			<div class="modal-footer">
				<button type="button" class="btn btn-info btn-md mapbasebtn">Upload</button>
			</div>
		</form>
	  </div>
    </div>
 </div>
</div>

<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
<h1 class="h2">Create A Map</h1>
<a type="button" onClick="showUploadImage()" class="btn btn-primary">Upload Map Image</a>
</div>

<div class="d-flex justify-content-center">
	<div id="unityContainer" style="width: 960px; height: 600px; margin: auto"></div>
</div>

<!-- image upload for the map base -->
<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
	
</div>

<script src="MapBuild/UnityLoader.js"></script>
	<script>
		var unityInstance = UnityLoader.instantiate("unityContainer", "MapBuild/Builds.json");
	</script>
<script>
function showUploadImage(){
	$('#uploadMapBaseModal').modal('show');
}
$(document).ready(function() { 
	$('.mapbasebtn').click(function(){
   		var mapimage = new FormData(); 
   		mapimage.append('mapfile', $('input[type=file]')[0].files[0]);
			// AJAX request
			$.ajax({
			url: 'mapbaseupload.php',
			type: 'post',
			data: mapimage,
			dataType: "json",
			success: function(response) {
                    if (response["result"] == "Success") {
                        $('#uploadresult').html("Successfully Uploaded.");
                        unityInstance.SendMessage('GameController', 'SetMapImage', response['content']);
                    } else {
                        $('#uploadresult').html(response['content']);
                    }
                },
			cache: false,
			contentType: false,
			processData: false
		});
	});

});
</script>
<!-- This script sends the json data to save the map on the server -->
<script>	
	function SendTheJson (jsonString) {
		    // AJAX request
		    $.ajax({
		    url: 'uploadmapjson.php',
		    type: 'post',
		    dataType: 'json',
		    contentType: 'application/json; charset=utf-8',
		    data: jsonString,
		    success: function(response){ 
		    	if (response["result"] == "Success") {
		    		//send response to unity
		      		unityInstance.SendMessage('GameController', 'SuccessfulSave', response['content']);
		    	} else {
		    		//send response to unity
		      		unityInstance.SendMessage('GameController', 'BrowserSaveResponse', response['content']);
		    	}
		      
		    }
		  });
		}
</script>
<!-- This script retrieves the map data for the logged in user -->
<script>	
	function LoadAllMaps () {
		    // AJAX request
		    $.ajax({
		    url: 'create_loadallmaps.php',
		    type: 'get',
		    dataType: 'json',
		    success: function(response){ 
		      //send response to unity
		      unityInstance.SendMessage('GameController', 'AllTheMaps', JSON.stringify(response));
		    }
		  });
		}
</script>
<!-- This script retrieves the map data for chosen map -->
<script>	
	function LoadAMap (inputID) {
		    // AJAX request
		    $.ajax({
		    url: 'create_loadthismap.php',
		    type: 'post',
		    dataType: 'json',
		    data: {id: inputID},
		    success: function(response){ 
		      //send response to unity
		      unityInstance.SendMessage('GameController', 'MapJson', JSON.stringify(response));
		    }
		  });
		}
</script>


<?php
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
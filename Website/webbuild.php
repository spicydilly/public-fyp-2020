<?php 
//for checking session variables
session_start();

//unity required
$unity = true;

//title of page, is passed to header
$title = "Web Build";
include "header.php";
?>

<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
<h1 class="h2">Unity WebGL Build</h1>
</div>
<div class="d-flex justify-content-center">
	<div id="unityContainer" style="width: 960px; height: 600px; margin: auto"></div>
</div>

<script src="FYPBuild/UnityLoader.js"></script>
	<script>
		var unityInstance = UnityLoader.instantiate("unityContainer", "FYPBuild/Builds.json");
	</script>

<?php 
//Special case where input must be either caught by unitys web gl or by the html page, needed to ensure a user can sign in on the page the app is loade
// only used if the user isn't signed in 
if(!isset($_SESSION["loggedin"]) || $_SESSION["loggedin"] == '') {
?>

<script>
$(document).ready(function() { 
	//detects when modal is closed or not, to disable or enable Unity keyboard input
	$('#signInModal').on('show.bs.modal', function (event){
		//disable unity taking keyboard input
		unityInstance.SendMessage('Player', 'InputNeeded', 0);
	});
	$('#signInModal').on('hidden.bs.modal', function (event) {
		// give unity back the keyboard inputs
		unityInstance.SendMessage('Player', 'InputNeeded', 1);
	});
});
</script>

<?php
	} //end of the if statement 

include "footer.php";
?>
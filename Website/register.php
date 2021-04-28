<?php

//title of page, is passed to header
$title = "Register";
include "header.php";
?>

<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
	<h1 class="h2">Registration</h1>
</div>

<?php
// registratrion allowed if the user isn't signed in
if(!isset($_SESSION["loggedin"]) || $_SESSION["loggedin"] == '') { ?>

<div class="row">
	<div class="col-sm">
		<p>Register for an account to gain access to the map building tools.</p>
	</div>
	<div class="col-sm">
		<form role="form" id="registerForm">
		  	<div class="mb-3">
			<label for="usernameReg" class="form-label">User Name</label>
				<input type="text" class="form-control" id="usernameReg" name="usernameReg" placeholder="Username">
				<small id="usernameHelp" class="form-text text-muted">Must be between 5-20 characters</small>
		  	</div>
		  	<div class="mb-3">
			<label for="passwordReg" class="form-label">Password</label>
				<input type="password" class="form-control" id="passwordReg" name="passwordReg" placeholder="Password">
				<small id="passwordHelp" class="form-text text-muted">Must be between 5-20 characters</small>
		  	</div>
		  	<button type="submit" id="registerSubmit" class="btn btn-primary btn-md">Register</button>
		</form>
	</div>
</div>
<!-- scripts for registering -->
<script>
$("#registerForm").submit(function(event){
    // cancels the form submission
    event.preventDefault();
    submitSignIn();
});
function submitSignIn(){
	// Initiate Variables With Form Content
    var username = $("#usernameReg").val();
	var password = $("#passwordReg").val(); 
	$.ajax({
	    type: "post",
        url: "registeruser.php",
        data: "username=" + username + "&password="+password,
        dataType: "json",
		success: function(response) {
	        if (response["result"] == "Success") {
            	alert(response["content"]);
        	} else {
        	    alert(response["content"]);
    	    }
	  	}
   	});
}
</script>
<?
	} 
	//else the user is already logged in so cannot regsiter
	else {
?>

<p>You are already logged in. If you wish to create a new account, please <a href="logout.php">Sign Out</a></p>

<?php

	}
	
	include "footer.php";
?>
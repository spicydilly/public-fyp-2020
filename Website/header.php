<?php
//for checking session variables
session_start();

?>

<!DOCTYPE html>
<html>
	<head>
		<meta charset="utf-8">
		<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
		<title><?php echo $title; ?></title>
		<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.7.1/css/all.css">
		<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-eOJMYsd53ii+scO/bJGFsiCZc+5NDVN2yr8+0RDqr0Ql0h+rP48ckxlpbzKgwra6" crossorigin="anonymous">
		<?php 
		//check if datatables is needed
		if ( isset($datatables) && ($datatables===TRUE) ) { ?>
			<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/v/dt/dt-1.10.24/datatables.min.css"/>
			<script type="text/javascript" src="https://cdn.datatables.net/v/dt/dt-1.10.24/datatables.min.js" defer="defer"></script>
		<?php } ?>
		<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
		<link href="style.css" rel="stylesheet" type="text/css">
	</head>
	<body>

<?php
	//modal for signing in exists if the user isn't signed in
	if(!isset($_SESSION["loggedin"]) || $_SESSION["loggedin"] == '') { ?>
<!-- modal for signing in -->
<div class="modal fade" id="signInModal" tabindex="-1" aria-labelledby="signIn" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="signInLabel">Sign In</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
      	<form role="form" id="signInForm">
		  <div class="mb-3">
			<label for="username" class="form-label">User Name</label>
				<input type="text" class="form-control" id="username" name="username" placeholder="Username">
		  </div>
		  <div class="mb-3">
			<label for="password" class="form-label">Password</label>
				<input type="password" class="form-control" id="password" name="password" placeholder="Password">
		  </div>
		  </div>
		  <div class="modal-footer">
		  	<button type="submit" id="signInSubmit" class="btn btn-primary btn-md">Sign In</button>
		  </div>
		</form>
	  </div>
    </div>
 </div>
</div>
<!-- scripts for logging in -->
<script>
function showSignIn(){
	$('#signInModal').modal('show');
}
$("#signInForm").submit(function(event){
    // cancels the form submission
    event.preventDefault();
    submitSignIn();
});
function submitSignIn(){
	// Initiate Variables With Form Content
    var username = $("#username").val();
	var password = $("#password").val(); 
	$.ajax({
	    type: "post",
        url: "authenticate.php",
        data: "username=" + username + "&password="+password,
        dataType: "json",
		success: function(response) {
	        if (response["result"] == "Success") {
            	window.location.reload();
        	} else {
        	    alert(response["content"]);
    	    }
	  	}
   	});
}
</script>
<?
	} 
?>
		<header class="navbar navbar-dark sticky-top bg-dark flex-md-nowrap p-0 shadow">
		  <a class="navbar-brand col-md-3 col-lg-2 me-0 px-3" href="index.php">FYP - Dillon Condon</a>
		  <button class="navbar-toggler position-absolute d-md-none collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="false" aria-label="Toggle navigation">
		    <span class="navbar-toggler-icon"></span>
		  </button>
  		<div class="w-100">
		  <ul class="nav px-3">
			<?php
				//login / signout button
				if($_SESSION["loggedin"]) { ?>
				<li class="nav-item dropdown nav-signin">
					<a class="nav-link dropdown-toggle text-white" href="#" id="navbarDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
						Hello, <?php echo $_SESSION["name"]; ?>!
					</a>
					<ul class="dropdown-menu dropdown-menu-md-end" aria-labelledby="navbarDropdown">
					    <li class="dropdown-item"><a href="logout.php">Sign out</a></li>
					</ul>
				</li>
				<?php } else { ?>
				<li class="nav-item text-nowrap nav-signin">
					<a type="button" class="nav-link text-white" onClick="showSignIn()">Sign In</a>
				</li>
				<?php } ?>			
		  </ul>
		</header>
		<div class="container-fluid">
				<div class="row">
				<nav id="sidebarMenu" class="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse">
			  <div class="position-sticky pt-3">
				<ul class="nav flex-column">
				  <li class="nav-item">
					<a class="nav-link <?php if($title == 'Homepage') { echo 'active'; }?>" href="index.php">
					  Homepage
					</a>
				  </li>
				 <?php if(!isset($_SESSION["loggedin"]) || $_SESSION["loggedin"] == '') { //only if not logged in?>
				  <li class="nav-item">
					<a class="nav-link <?php if($title == 'Register') { echo 'active'; }?>" href="register.php">
					  Register
					</a>
				  </li>
				<?php } ?>
				</ul>
				<h6 class="sidebar-heading d-flex justify-content-between align-items-center px-3 mt-4 mb-1 text-muted">
				  Project Builds
				</h6>
				<ul class="nav flex-column mb-2">
				  <li class="nav-item">
					<a class="nav-link <?php if($title == 'Web Build') { echo 'active'; }?>" href="webbuild.php">
					  Web Build
					</a>
				  </li>
				  <li class="nav-item">
					<a class="nav-link <?php if($title == 'Android Build') { echo 'active'; }?>" href="androidbuild.php">
					  Android Build
					</a>
				  </li>
				  <li class="nav-item">
					<a class="nav-link <?php if($title == 'Windows Build') { echo 'active'; }?>" href="windowsbuild.php">
					  Windows/VR Build
					</a>
				  </li>
				</ul>
				<?php
				//admin pages
				if($_SESSION["loggedin"]) { ?>
				<h6 class="sidebar-heading d-flex justify-content-between align-items-center px-3 mt-4 mb-1 text-muted">
				  Admin
				</h6>
				<ul class="nav flex-column mb-2">
				  <li class="nav-item">
					<a class="nav-link <?php if($title == 'Create Map') { echo 'active'; }?>" href="createmap.php">
					  Create Map
					</a>
				  <li class="nav-item">
					<a class="nav-link <?php if($title == 'View Maps') { echo 'active'; }?>" href="viewmaps.php">
					  View Maps
					</a>
				</ul>
				<?php } 
				//end of admin pages
				?>
			  </div>
			</nav>
			<main class="col-md-9 ms-sm-auto col-lg-10 px-md-4">
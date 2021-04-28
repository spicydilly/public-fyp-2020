<?php

//for checking session variables
session_start();

//function for the json response
//text_response is text to be used in function on client side (either "success" or not)
//content is the actual response to be sent back, either the url of the saved image or nothing if failed
function respondWithJson($text_response, $content="") {

    //create array for use in json response
    $response = array("result"=> $text_response, "content"=>$content );
    //encode json
    echo json_encode($response);

}

//check if logged in, upload is admin only
if($_SESSION["loggedin"]) { 
  
  //will add time and the users id to the saved file making sure no duplicates exist
  // in the directory
  $target_dir = "mapbaseimages/". time() . '_' . $_SESSION['id'];
  $target_file = $target_dir . basename($_FILES["mapfile"]["name"]);
  $uploadOk = 1;
  $imageFileType = strtolower(pathinfo($target_file,PATHINFO_EXTENSION));

  // Check if image file is a actual image or fake image
  $check = getimagesize($_FILES["mapfile"]["tmp_name"]);
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
  if ($_FILES["mapfile"]["size"] > 10000000) {
    $content .= "Sorry, your file is too large.";
  }

  // Allow certain file formats
  if($imageFileType != "jpg" && $imageFileType != "png" && $imageFileType != "jpeg" ) {
    $content .= "Only JPG, JPEG & PNG files are allowed.";
  }

  // if content isn't empty, a failure occured
  if ($content != "") {
    respondWithJson("Failure", $content);
  // if everything is ok, try to upload file
  } else {
    if (move_uploaded_file($_FILES["mapfile"]["tmp_name"], $target_file)) {
      respondWithJson("Success", $target_file);
    } else {
      //this would be a major error
      respondWithJson("Failure","Sorry, there was an error uploading your file.");
    }
  }
} else {
  // no need for a fancy response the user is not logged in
  respondWithJson("Failure","Admin Only, please sign in.");
}
?>
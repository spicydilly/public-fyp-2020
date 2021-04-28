<?php 
//for checking session variables
session_start();

//unity required
$unity = true;

//title of page, is passed to header
$title = "Windows Build";
include "header.php";
?>

<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
<h1 class="h2">Unity Windows/VR Build</h1>
</div>
<p>The most recent VR build download can be accessed here <a href="downloadbuild/fyp2020.zip" download>here</a>.</p>

<p>The download will provide you with a .zip file. Simply unzip this file to continue. If you need an unzipping program, try out this : <a href="https://www.win-rar.com/">Win-Rar</a></p>

<p>Inside the unzipped folder will contain a VR_Test.exe which can be run on a windows machine. Running this should automatically open the application with your VR desktop applicaiton and run on your VR device, for example if you are using the Oculus Rift or the Oculus Quest with the Ouclus Link. However, if you have not already set up your Oculus device with your computer, I would recommend doing so via : https://www.oculus.com/setup/ before proceeding.</p>

<p>You may need to allow "Unknown Sources" on the Oculus desktop application, to do this, simply open the Oculus Desktop application then click on "Settings" then "General" and there will be an "Unknown Sources" option here that must be enabled.</p>

<p>How to play :</p>
<ul>
<li>Teleport with the right controller using the right trigger. A "teleport" reticle will appear showing the destination</li>
<li>Continuous movement can be achieved by using the right controllers analog stick</li>
<li>Object placement is done via the left controllers trigger</li>
<li>Can rotate the player by using the left controllers analog stick</li>
</ul>

<?php
include "footer.php";
?>
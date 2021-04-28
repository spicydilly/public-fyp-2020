<?php 
//for checking session variables
session_start();

//unity required
$unity = true;

//title of page, is passed to header
$title = "Android Build";
include "header.php";
?>

<div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
<h1 class="h2">Unity Android Build</h1>
</div>
<p>You can download the .apk file <a href="downloadbuild/fyp2020.apk" download>here</a>.</p>

<p>Just download the .apk file to your Android device, and then locate that file on the device using a file manager. After clicking on the .apk you should be propted to install the application, you may get some notifications which can be ignored such as the applicaiton may be potentially dangerous ( that is fine, this applicaiton is just for development and is not published through the Google Play Store ). You may get another notification indicating you are unable to install unknown sources/apps, this can be fixed by just enabling this setting on the Android device, instructions are provided below :</p>
<ol>
	<li>Navigate to your phone settings menu.</li>
	<li>Click on "Security Settings".</li>
	<li>Enable "Install from Unknown Sources".</li>
</ol>

<p>Once the app is installed, you may be prompted to give access to your Camera, this must be accepted as this is a requirement for running the applicaiton.</p>

<p>How to play :</p>
<ul>
<li>Point your camera towards the floor. You may have to move your device around abit as AR Core needs to gather some information in order to correctly locate the floor. You will see an object appear if the placement area is valid, clicking on the screen will stick this object to the floor. </li>
</ul>

<?php
include "footer.php";
?>
<?php

//title of page, is passed to header
$title = "Homepage";
include "header.php";
?>

  <section class="py-5 text-center container">
    <div class="row py-lg-5">
      <div class="col-lg-6 col-md-8 mx-auto">
        <h1 class="fw-light">Augmented and Virtual Reality based framework for remote viewing of real world locations</h1>
        <p class="lead text-muted">This project sets out to create both Augmented and Virtual Reality applications to allow users to experience real world locations from the comfort of their own homes . The applications have the means to provide a interactive experience for any person which has access to either a web browser, a VR capable device, or a modern android smart phone. The applications allow users to create portals, allowing a user to step through the portal and be transported to this other reality. Users can interact with a map which users can select specific areas from, and a Console which can be used to select which map to display. All of the data the user can access is stored on a MySQL database. The project also provides a web application which allows for users to create their own maps and publish these to the database to provide access to all users. The web interface provides a user-friendly experience for users, built using PHP and utilizing JavaScript and AJAX for the smooth loading of data, complemented by a modern style provided by Bootstrap. The projects applications are built using the Unity Game Engine with C#.</p>
        <h5>Make sure to <a href="register.php">Register</a> to gain access to the Create A Map tool!</h5>
      </div>
    </div>
  </section>



<?php
include "footer.php";
?>
<?php
//connect.php , connects to the mysql database
$DATABASE_HOST = 'localhost';
$DATABASE_USER = '';
$DATABASE_PASS = '';
$DATABASE_NAME = '';

$conn = mysqli_connect($DATABASE_HOST, $DATABASE_USER, $DATABASE_PASS, $DATABASE_NAME);

if ( !$conn )
{
    die('Connection failed: ' . mysqli_connect_error());
}

?>

<?php

// Put your credentials here

$db_host="db_host";
$db_username="db_username";
$db_password="db_password";
$db_name="db_name";

$db_connect = mysqli_connect($db_host, $db_username, $db_password, $db_name);
// Check connection
if (mysqli_connect_error())
{
    echo "Failed to connect to DataBase: " . mysqli_connect_error();
}
?>
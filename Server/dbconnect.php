<?php

$db_host="litesound.com";
$db_username="a0037607_testCreate";
$db_password="3332210";
$db_name="a0037607_testCreate";

$db_connect = mysqli_connect($db_host, $db_username, $db_password, $db_name);
// Check connection
if (mysqli_connect_error())
{
    echo "Failed to connect to DataBase: " . mysqli_connect_error();
}
?>
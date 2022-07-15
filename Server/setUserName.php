<?php

include("dbconnect.php");

$name = $_REQUEST['name'];
$user_hash = $_REQUEST['user_hash'];

$query=mysqli_query($db_connect,"UPDATE users SET name = '$name' WHERE user_hash = '$user_hash'") or die(mysqli_error($db_connect));

mysqli_close($db_connect);

?>
<?php

include("dbconnect.php");

$user_hash = $_REQUEST['user_hash'];

$result=mysqli_query($db_connect,"SELECT name FROM users where user_hash = '$user_hash'") or die(mysqli_error($db_connect));
$user = mysqli_fetch_assoc($result);
mysqli_close($db_connect);

echo $user['name'];

?>
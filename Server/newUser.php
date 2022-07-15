<?php

include("dbconnect.php");

$user_hash = $_REQUEST['user_hash'];
$ip = $_REQUEST['ip'];
$info = $_REQUEST['info'];

$query=mysqli_query($db_connect,"INSERT INTO users (user_hash, ip, last_date, info) VALUES ('$user_hash', '$ip', now(), '$info') ON DUPLICATE KEY UPDATE user_hash = VALUES(user_hash), ip = VALUES(ip), last_date = now(), info = VALUES(info); ") or die(mysqli_error($db_connect));

mysqli_close($db_connect);

?>
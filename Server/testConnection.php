<?php

include("dbconnect.php");

$result=mysqli_query($db_connect,"SELECT txt FROM information order by date DESC LIMIT 1") or die(mysqli_error($db_connect));
$msg = mysqli_fetch_assoc($result);
mysqli_close($db_connect);

echo $msg['txt'];

?>
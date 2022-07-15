<?php

include("dbconnect.php");

$user_hash = $_REQUEST['user_hash'];

$result=mysqli_query($db_connect,"SELECT proxy FROM proxy where user_hash = '$user_hash'") or die(mysqli_error($db_connect));

while ($row = mysqli_fetch_assoc($result)) {
        echo $row['proxy'];
		echo "<br>";
}
mysqli_close($db_connect);

?>
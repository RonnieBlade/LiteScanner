<?php

include("dbconnect.php");

$result=mysqli_query($db_connect,"SELECT users.name AS name, COUNT(*) AS proxies_count FROM users, proxy where name IS NOT null AND users.user_hash = proxy.user_hash GROUP BY users.name ORDER BY proxies_count DESC LIMIT 50") or die(mysqli_error($db_connect));

$place = 1;
while ($row = mysqli_fetch_assoc($result)) {
        echo "{$place}. {$row['name']} - {$row['proxies_count']}";
		echo "<br>";
		$place +=1;
}
mysqli_close($db_connect);

?>
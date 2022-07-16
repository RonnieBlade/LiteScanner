<?php

$proxy = $_REQUEST['proxy'];
$user_hash = $_REQUEST['user_hash'];

// Any fast-loading page
$options[CURLOPT_URL] = 'https://www.freecodecamp.org/the-fastest-web-page-on-the-internet';
$options[CURLOPT_PROXY] = $proxy;
$options[CURLOPT_PORT] = 443;
$options[CURLOPT_FRESH_CONNECT] = true;
$options[CURLOPT_FOLLOWLOCATION] = false;
$options[CURLOPT_FAILONERROR] = true;
$options[CURLOPT_RETURNTRANSFER] = true;
$options[CURLOPT_TIMEOUT] = 10;

$response = "";

$curl = curl_init();
curl_setopt_array($curl, $options);

$response = curl_exec($curl);

curl_close($curl);

if($response != "") {
	
	include("dbconnect.php");
	
	$query=mysqli_query($db_connect,"INSERT INTO proxy (proxy, user_hash) VALUES ('$proxy', '$user_hash') ON DUPLICATE KEY UPDATE proxy = VALUES(proxy), user_hash = VALUES(user_hash); ") or die(mysqli_error($db_connect));

	mysqli_close($db_connect);
	
    echo 'OK';
	
} else {
	echo 'Proxy is not working';
}

?>
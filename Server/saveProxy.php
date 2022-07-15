<?php

$proxy = $_REQUEST['proxy'];
$user_hash = $_REQUEST['user_hash'];

$options[CURLOPT_URL] = 'https://www.freecodecamp.org/the-fastest-web-page-on-the-internet';
$options[CURLOPT_PROXY] = $proxy;
$options[CURLOPT_PORT] = 443;
$options[CURLOPT_FRESH_CONNECT] = true;
$options[CURLOPT_FOLLOWLOCATION] = false;
$options[CURLOPT_FAILONERROR] = true;
$options[CURLOPT_RETURNTRANSFER] = true; // curl_exec will not return true if you use this, it will instead return the request body
$options[CURLOPT_TIMEOUT] = 10;

// Preset $response var to false and output
$fb = "";
$response = false;// don't quote booleans
//echo '<p class="response1">'.$response.'</p>';

$curl = curl_init();
curl_setopt_array($curl, $options);
// If curl request returns a value, I set it to the var here. 
// If the file isn't found (server offline), the try/catch fails and var should stay as false.
$fb = curl_exec($curl);
curl_close($curl);

if($fb != "") {
	
	include("dbconnect.php");
	
	$query=mysqli_query($db_connect,"INSERT INTO proxy (proxy, user_hash) VALUES ('$proxy', '$user_hash') ON DUPLICATE KEY UPDATE proxy = VALUES(proxy), user_hash = VALUES(user_hash); ") or die(mysqli_error($db_connect));

	mysqli_close($db_connect);
	
    echo 'OK';
    $response = $fb;
} else {
	echo 'Proxy is not working';
}
















?>
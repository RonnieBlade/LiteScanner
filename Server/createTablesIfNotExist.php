<?php

include("dbconnect.php");

$query=mysqli_query($db_connect,"CREATE TABLE IF NOT EXISTS `proxy` (`id` int(11) NOT NULL AUTO_INCREMENT, `proxy` varchar(50) COLLATE utf8_unicode_ci NOT NULL, `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP, `user_hash` varchar(512) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'unknown', PRIMARY KEY (`proxy`), UNIQUE KEY `id_UNIQUE` (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;") or die(mysqli_error($db_connect));
$query=mysqli_query($db_connect,"CREATE TABLE IF NOT EXISTS `information` (`id` int(11) NOT NULL, `txt` varchar(512) COLLATE utf8_unicode_ci DEFAULT NULL, `date` datetime DEFAULT CURRENT_TIMESTAMP, PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;") or die(mysqli_error($db_connect));
$query=mysqli_query($db_connect,"CREATE TABLE IF NOT EXISTS `users` (`id` int(11) NOT NULL AUTO_INCREMENT, `user_hash` varchar(512) COLLATE utf8_unicode_ci NOT NULL, `name` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL, `ip` varchar(128) COLLATE utf8_unicode_ci NOT NULL, `last_date` datetime DEFAULT NULL, `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP, `info` varchar(512) COLLATE utf8_unicode_ci DEFAULT NULL, PRIMARY KEY (`user_hash`), UNIQUE KEY `processer_id_UNIQUE` (`user_hash`), UNIQUE KEY `id_UNIQUE` (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;") or die(mysqli_error($db_connect));

mysqli_close($db_connect);

echo $msg['txt'];

?>
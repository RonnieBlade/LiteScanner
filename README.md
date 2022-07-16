# LiteScanner
An effective flexible proxy scanner for Windows (.NETFramework 4.6.1)

## About
**LiteScanner** consists of a **C#** Client console app, **PHP** Server scripts and **MySQL** database. Clients scan IPs according to the previously collected statistics for better efficiency or completely randomly. When a client finds a working proxy, it sends it to the server to check and save it in the database.

<p align="center">
 <img src="https://github.com/RonnieBlade/LiteScanner/blob/d00b0f7bac7b625e40ed437e5ad08eb0c8e4394c/LiteScanner.gif" alt="Trie"/>
</p>

## Features
* Multitask network scanning
* Scanning according to the previously collected statistics for better efficiency 
* Authorization by client **Hardware and Software Fingerprint Hash**
* Double checking proxies on the server side before accepting
* Flexible configuration with command-line arguments
* Colorful console design:)

## Command-line arguments
* manual - user chooses number of scanning tasks in the app manually
* allports - scanning ports according to the statistics
* port:{port} - scanning only one particular port (8080 by default)
* random - scanning IPs randomly not taking statistics into account
* timeout:{number of seconds} - scanning requests timeout in seconds

### Example:
```
150 port:3128 timeout:10 random
```
Starts 150 tasks scanning random IPs connecting only to port 3128 with 10 seconds timeout

Without any given arguments LiteScanner runs 100 tasks scanning IPs according to the statistics connecting only to port 8080 with 5 seconds timeout 

## Notes
Configure your DB credentials in ```Server/dbconnect.php```

## Licence
The code is released under an MIT licence.

## FXEmu v2

1. Install NGINX with stream_ssl_preread_module:  https://www.osradar.com/install-nginx-from-the-source-code-debian-ubuntu/
2. Upload the nginx folder to /etc/nginx, replace every occurence, and modify the ports inside nginx.conf as needed (usually one listing per key & port) 
3. Upload html/ to /var/www/html and modify to your needs.
4. Compile FXEmuSharp, modify the listing ip override, ports & keys, then run it.


Notice: You will have to provide a proper sv_licenseKeyToken, otherwise your server will rarely show.
It does sometimes, that's just a bug with the server list though.
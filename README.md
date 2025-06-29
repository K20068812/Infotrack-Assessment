# InfoTrack-Assessment

1) Download visual studio 2022
2) Download sql server 2022 express
    Select Basic installation type 
3) Download Node.js 
4) Clone the repository into a folder and open the .sln file 
5) Navigate to LandRegistryApi.Api\ReactApp\infotrackui and run an npm install 
6) run "npm run build" in the same directory
7) I ran the migrations using the command line. 
    In the top level folder Infotrack-Assessment, run "dotnet tool install --global dotnet-ef" if you don't have the dotnet tool installed 
    then in the same directory run "dotnet ef database update --project LandRegistryApi.Infrastructure --startup-project LandRegistryApi.Api"
8) run the application on visual studio (in debug mode, the "https" option or IIS express, I tested it with both. I ran it on Windows so it may be different on another OS)


Potential Issues:

Google actively tries to prevent scraping (you are blocked by a cookie consent banner)
The only way I found to avoid this was by setting the cookies in the request
In the appsettings.json, there is a Socs cookie and a SecureEnid cookie. These take a year to expire, so should work okay. I generated them through going on an incognito tab on my browser, going to google, accepting the cookie consent option and copying those cookie values. So if they don't work you may want to do the same.
The __Secure-ENID cookie is used to remember your preferences (language, number of results, safesearch filter etc.) and the Socs cookie is used to store state regarding cookie choices


Future improvements:
A more polished UI 

Video Demo:

https://github.com/user-attachments/assets/4eeb2cdc-062f-4321-9959-c3bfab162320


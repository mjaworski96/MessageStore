# MessageStorer
MessageStorer is application for storing messages (currently from Android phones and Messenger application.

# Deploying application
1. Create own postgreSQL databases
    1. First using scripts (all except clear_db.sql) from db/API,
    1. Second using scripts (all except clear_db.sql) from db/MessengerIntegration
Using other database is possible but requires updating Nuget packages in backend/MessageStorer/API and backend/MessageStorer/MessengerIntegration projects. Also change in appsettings.json will be required.
2. Create your own certificates using commands from certificate/generate.txt and config from certificate/conf.txt. You can change some details e.g. CN, alternative DNS names, passoword. Using exisiting certificates is also posible. Finally you can add root.crt as trusted certificate (It will make messagesender.crt certificate trusted too).
3. Update backend/MessageStorer/API/appsettings.json and backend/MessageStorer/MessengerIntegration/appsettings.json with your db name and credentials and certificate password. You can also change token secret, lifetime and refresh time.
4. If you changed backend port (Urls property) in previous step, update frontend/messages-viewer/proxy.conf.json *
5. Run backend apps (API and MessengerIntegration): 
    1. dotnet run (in project directory) OR
    2. dotnet <project_name>.dll (after building project, in this step you must check relative paths in appsetings.json OR
    3. just run  <project_name>.exe (after building project, in this step you must check relative paths in appsetings.json
6. Run frontend app: npm start for english version or npm run start:pl for polish version. Other option is build application using ng build --prod and then deploy it on http server like nginx. Remeber about configuring proxy (like in step 4).
7. Update your certificate data in MessageSender.Model.Constraints (project MessageSender.Android)
8. Deploy MessageSender.Android aplication on your phone.

*<!-- --> this step can be ommited if you are deploying app on http server.<br/>

# Other information
Deploying project require installed .Net Core 3.1, node.js (for angular 8), and postgreSQL (or other database).


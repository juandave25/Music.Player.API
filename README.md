# Music.Player.API

Before executing this solution, you need to follow these steps:

1. Change ConnectionString in the <code>appsettings.json</code> and <code>Context.cs</code> file with your connection string

   Note: The database was developed with PostgreSQL : This is an example of connection string :
   
   <code>Host=localhost;Database=music-player;Username=yourdbuser;Password=yourdbpassword;Port=5432</code>

3. Execute the next commands in your Nuget package console pointing to Products.Api.Data Class Library :  
  <code>Update-database Initial-Setup</code>

4. Open your visual studio and select default <code>Music.Player.Api</code> as a launch project

5. Open the Postman folder and import collection from Postman execute the <code>localhost:5001/api/auth/login</code> endpoint to get a bearer token and start to test every endpoint. 

   PD: <code>{username: test - password: password}</code> for JSON body in authenticate endpoint. Enpoints to create, update, and delete products need a bearer token, you can get products without authentication and authorization.

6. If you want to use swagger you can run this URL :
   <code>https://localhost:5001/swagger</code>

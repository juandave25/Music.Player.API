# Music.Player.API

Before executing this solution, you need to follow these steps:

1. Change ConnectionString in the <code>appsettings.json</code> and <code>Context.cs</code> file with your connection string

   Note: The database was developed with PostgreSQL: This is an example of a connection string :
   
   <code>Host=localhost;Database=music-player;Username=yourdbuser;Password=yourdbpassword;Port=5432</code>
   
2. You can create your stack using cloud formation I attached a script to make your infrastructure for the S3 bucket and policies automatically or If you want you can make it manually. the script is located in :
   
   <code>(https://github.com/juandave25/Music.Player.API/blob/master/Music.Player.API/audio-streaming-s3-infrastructure.yaml)</code>

4. To execute this script you need to open cmd or PowerShell depending on your operating system in the project :
   
   <code>..\yourrepofolder\Music.Player.API\Music.Player.API\audio-streaming-s3-infrastructure.yaml</code>

5. Execute this command:
   
   <code>aws cloud formation create-stack --stack-name AudioStreamingS3Stack --template-body file://audio-streaming-s3-infrastructure.yaml --parameters ParameterKey=Environment,ParameterValue=development --capabilities CAPABILITY_IAM  --profile your-aws-profile</code>

   Note: Ensure you have a user account created with permissions in AWS condole to execute aws-cli commands. If you do not have aws-cli you can download it from <code>https://awscli.amazonaws.com/AWSCLIV2.msi</code>

6. Execute the next commands in your Nuget package console pointing to Products.Api.Data Class Library :  
  <code>Update-database Initial-Setup</code>

7. Open your visual studio and select default <code>Music.Player.Api</code> as a launch project

8. Open the Postman folder and import collection from Postman execute the <code>https://localhost:5001/api/auth/login</code> endpoint to get a bearer token and start to test every endpoint. 

   PD: <code>{username: test - password: password}</code> for JSON body in authenticate endpoint. Enpoints to create, update, and delete products need a bearer token, you can get products without authentication and authorization.

9. If you want to use swagger you can run this URL :
   <code>https://localhost:5001/swagger</code>

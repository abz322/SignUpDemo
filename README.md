# Sign Up Demo
Single page application demonstrating a sign up form on an ASP NET framework using an MVC architecture. The front end uses bootrap, jquery, ajax and razor for design and validation. POST requests are made on submission to the controller where further validation is made and passwords are hashed for security before inserting into a local database using non ORM methods.

# Setup
- Clone the repo or download the zip file
- Open using Visual Studio
- Setup a new database called "UserDB" on the local server: (localdb)\MSSQLLocalDB
- Within a New Query for the db, run the "DB Table-User Creation Script.sql" script
- Clean/Rebuild Solution and start the project

# Caveats
This solution was made using VS 2019 on .Net Framework 4.7.2, it is assumed that your setup will be running on these versions as well.

If you don't have a local server visible in your VS, you will need to set one up as per below:
- Open SQL Server Object
- Add SQL Server
- Server Name = "(localdb)\MSSQLLocalDB"
- Authentication = "Windows Athentication"
- Database = "default"

  From there you can create the "UserDB" database

# Misc
If you would like to see the results of the database on screen a view has been created for ease of access. You can access this with the steps below
- Go to SignUpDemo > Views > Shared > _Layout.cshtml
- Uncomment the following line: @Html.Action("Test", "Home")

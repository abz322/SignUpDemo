--Once the Users database has been created, created the schema, table and user login
CREATE SCHEMA [UserDB] AUTHORIZATION [dbo];

go

CREATE TABLE [UserDetails]
  (
     [UserID]   INT IDENTITY (1, 1) NOT NULL,
     [Email]    NVARCHAR (50) NOT NULL,
     [Password] BINARY (32) NOT NULL
     CONSTRAINT [PK__UserDetails__7AD0F11A993CC91] PRIMARY KEY CLUSTERED (
     [UserID] ASC )WITH (pad_index = OFF, statistics_norecompute = OFF,
     ignore_dup_key = OFF, allow_row_locks = on, allow_page_locks = on) ON
     [PRIMARY]
  )
ON [PRIMARY]

go

USE [UserDB]

go

--Check to see if the login credential has been created for the user that will be used in the connection string
IF NOT EXISTS(SELECT *
              FROM   sys.server_principals
              WHERE  NAME = 'SignUpDemoDBUser')
  BEGIN
      CREATE login [SignUpDemoDBUser] WITH password=N'Mr/[v3CJxq!xH-dR',
      default_database=[Users], check_expiration=OFF, check_policy=ON
  END;

go

USE [UserDB]

go

CREATE USER [SignUpDemoDBUser] FOR login [SignUpDemoDBUser]

go
-- Giving read and write access for the created user, it doesn't need any higher level of access for this scenario
EXEC Sp_addrolemember
  'db_datawriter',
  'SignUpDemoDBUser'

EXEC Sp_addrolemember
  'db_datareader',
  'SignUpDemoDBUser'

go 

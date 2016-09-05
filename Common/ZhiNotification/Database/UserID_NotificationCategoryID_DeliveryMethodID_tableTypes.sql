USE [ENO_Notification]
GO

/****** Object:  UserDefinedTableType [dbo].[UserID]    Script Date: 11/16/2015 13:29:15 ******/
IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'UserID' AND ss.name = N'dbo')
DROP TYPE [dbo].[UserID]
GO

USE [ENO_Notification]
GO

/****** Object:  UserDefinedTableType [dbo].[UserID]    Script Date: 11/16/2015 13:29:15 ******/
CREATE TYPE [dbo].[UserID] AS TABLE(
	[ID] [int] NOT NULL
)
GO

/****** Object:  UserDefinedTableType [dbo].[NotificationCategoryID]    Script Date: 11/16/2015 13:29:15 ******/
IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'NotificationCategoryID' AND ss.name = N'dbo')
DROP TYPE [dbo].[NotificationCategoryID]
GO

USE [ENO_Notification]
GO

/****** Object:  UserDefinedTableType [dbo].[NotificationCategoryID]    Script Date: 11/16/2015 13:29:15 ******/
CREATE TYPE [dbo].[NotificationCategoryID] AS TABLE(
	[ID] [int] NOT NULL
)
GO

/****** Object:  UserDefinedTableType [dbo].[DeliveryMethodID]    Script Date: 11/16/2015 13:29:15 ******/
IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'DeliveryMethodID' AND ss.name = N'dbo')
DROP TYPE [dbo].[DeliveryMethodID]
GO

USE [ENO_Notification]
GO

/****** Object:  UserDefinedTableType [dbo].[DeliveryMethodID]    Script Date: 11/16/2015 13:29:15 ******/
CREATE TYPE [dbo].[DeliveryMethodID] AS TABLE(
	[ID] [int] NOT NULL
)
GO

USE [ENO_Notification]
GO

/****** Object:  UserDefinedTableType [dbo].[ExcludedUser]    Script Date: 11/24/2015 16:14:23 ******/
CREATE TYPE [dbo].[ExcludedUser] AS TABLE(
	[ChangeID] [bigint] NOT NULL,
	[UserID] [int] NOT NULL
)
GO

USE [ENO_Notification]
GO

/****** Object:  UserDefinedTableType [dbo].[PattNotification]    Script Date: 11/24/2015 16:13:22 ******/
CREATE TYPE [dbo].[PattNotification] AS TABLE(
	[NotificationID] [bigint] NULL,
	[NotificationCategoryID] [int] NULL,
	[ChangeID] [bigint] NULL
)
GO


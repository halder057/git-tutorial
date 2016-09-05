USE ENO_Notification
GO

-- CREATE DeliveryFrequency Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'DeliveryFrequency' AND xtype = 'U')
CREATE TABLE DeliveryFrequency
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Value VARCHAR(64) NOT NULL
);
GO

-- CREATE DeliveryMethod Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'DeliveryMethod' AND xtype = 'U')
CREATE TABLE DeliveryMethod
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Value VARCHAR(64) NOT NULL
);
GO
  
-- CREATE ContentVolume Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'ContentVolume' AND xtype = 'U')
CREATE TABLE ContentVolume
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Value VARCHAR(64) NOT NULL
);
GO

-- CREATE UserNotificationDeliveryAssoc Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'UserNotificationDeliveryAssoc' AND xtype = 'U')
CREATE TABLE UserNotificationDeliveryAssoc
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
UserID INT NOT NULL,
DeliveryMethod INT,
DeliveryFrequency INT,
ContentVolume INT,
ApplicationID INT NOT NULL,
LastDeliveryTimestamp DATETIME NULL,
CONSTRAINT FK_DeliveryMethodID_UserNotyDeliveryAssocDM FOREIGN KEY (DeliveryMethod) REFERENCES DeliveryMethod(ID),
CONSTRAINT FK_DeliveryFrequencyID_UserNotyDeliveryAssocNotyCategoryID FOREIGN KEY (DeliveryFrequency) REFERENCES DeliveryFrequency(ID),
CONSTRAINT FK_ContentVolumeID_UserNotificationDeliveryAssocContentVolume FOREIGN KEY (ContentVolume) REFERENCES ContentVolume(ID),
);
GO

-- CREATE NotificationCategory Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'NotificationCategory' AND xtype = 'U')
CREATE TABLE NotificationCategory
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Name VARCHAR(256) NOT NULL,
Mandatory BIT DEFAULT 0,
AutoApproval BIT DEFAULT 0,
ApplicationID INT NOT NULL,
);
GO

-- Create ChangeStatus TABLE
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'ChangeStatus' AND xtype = 'U')
CREATE TABLE ChangeStatus
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Value VARCHAR(64) NOT NULL
);
GO

-- CREATE Change Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'Change' AND xtype = 'U')
CREATE TABLE Change
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
NotificationCategoryID INT NOT NULL,
ChangedFieldName VARCHAR(128),
PreviousValue VARCHAR(512),
CurrentValue VARCHAR(512),
NotificationMessage VARCHAR(2048) NOT NULL,
GenerationTime DATETIME NOT NULL,
ChangedBy INT,
ApprovedBy INT,
Status INT DEFAULT 1,
NotificationDetails XML,
MCOID INT,
Indication VARCHAR(255),
Drug_Name VARCHAR(255),
CONSTRAINT FK_NotyCategoryID_ChangeNotyCategoryID FOREIGN KEY (NotificationCategoryID) REFERENCES NotificationCategory(ID),
CONSTRAINT FK_ChangeStatusID_ChangeStatus FOREIGN KEY (Status) REFERENCES ChangeStatus(ID)
);
GO

-- CREATE Notification Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'Notification' AND xtype = 'U')
CREATE TABLE Notification
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
NotificationCategoryID INT NOT NULL,
NotificationDetails XML,
Text VARCHAR(2048) NOT NULL,
GenerationTime DATETIME NOT NULL,
MCOID INT,
Indication VARCHAR(255),
Drug_Name VARCHAR(255),
CONSTRAINT FK_NotyCategoryID_NotificationNotyCategoryID FOREIGN KEY (NotificationCategoryID) REFERENCES NotificationCategory(ID)
);
GO
  
-- CREATE NotificationStatus Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'NotificationStatus' AND xtype = 'U')
CREATE TABLE NotificationStatus
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Value VARCHAR(64) NOT NULL
);
GO

-- CREATE UserNotificationAssoc Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'UserNotificationAssoc' AND xtype = 'U')
CREATE TABLE UserNotificationAssoc
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
UserID INT NOT NULL,
NotificationID BIGINT NOT NULL,
Status INT DEFAULT 1,
ApplicationID INT NOT NULL,
CONSTRAINT FK_NotificationID_UserNotyAssocNotyID FOREIGN KEY (NotificationID) REFERENCES Notification(ID),
CONSTRAINT FK_NotyStatusID_UserNotyAssocStatus FOREIGN KEY (Status) REFERENCES NotificationStatus(ID),
);
GO

-- CREATE UserNotificationCategoryAssoc Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'UserNotificationCategoryAssoc' AND xtype = 'U')
CREATE TABLE UserNotificationCategoryAssoc
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
UserID INT NOT NULL,
NotificationCategoryID INT NOT NULL,
CONSTRAINT FK_NotyCategoryNotyCategoryID_UserNotyAssocNotyCategoryID FOREIGN KEY (NotificationCategoryID) REFERENCES NotificationCategory(ID)
);
GO
  
-- CREATE Page Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'Page' AND xtype = 'U')
CREATE TABLE Page
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Value VARCHAR(64) NOT NULL
);
GO

-- CREATE PreferenceType Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'PreferenceType' AND xtype = 'U')
CREATE TABLE PreferenceType
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Value VARCHAR(64) NOT NULL
);
GO

-- CREATE UIPreference Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'UserPreference' AND xtype = 'U')
CREATE TABLE UserPreference
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
UserID INT NOT NULL,
PageID INT NOT NULL,
PreferenceTypeID INT NOT NULL,
PreferenceValue INT NOT NULL
);
GO


-- CREATE NotificationTemplateMessage Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'NotificationTemplateMessage' AND xtype = 'U')
CREATE TABLE NotificationTemplateMessage
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
NotificationCategoryID INT NOT NULL,
Message VARCHAR(1024) NOT NULL,
Active BIT DEFAULT 1,
LastUpdateTime DATETIME DEFAULT GETUTCDATE(),
CONSTRAINT FK_NotyCategoryID_NotyCategoryID FOREIGN KEY (NotificationCategoryID) REFERENCES NotificationCategory(ID)
);
GO

-- CREATE NotificationDetail Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'NotificationDetail' AND xtype = 'U')
CREATE TABLE NotificationDetail
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Name VARCHAR(255) NOT NULL,
[Value] VARCHAR(255)
);
GO

-- CREATE ChangeNotificationDetailAssoc Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'ChangeNotificationDetailAssoc' AND xtype = 'U')
CREATE TABLE ChangeNotificationDetailAssoc
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
ChangeID BIGINT NOT NULL,
NotificationDetailID BIGINT NOT NULL,
CONSTRAINT FK_ChangeID_ChangeID FOREIGN KEY (ChangeID) REFERENCES Change(ID),
CONSTRAINT FK_NotyDetailID_NotyDetailID FOREIGN KEY (NotificationDetailID) REFERENCES NotificationDetail(ID)
);
GO

-- Alter Notification table
IF EXISTS (SELECT * from sysobjects WHERE name = 'Notification' AND xtype = 'U')
BEGIN
ALTER TABLE [Notification] ADD ChangeID BIGINT
ALTER TABLE [Notification] ADD CONSTRAINT FK_ChangeCID_ChangeID FOREIGN KEY (ChangeID) REFERENCES Change(ID)
END
GO

-- CREATE udt_Change table type
IF TYPE_ID(N'udt_Change') IS NULL
CREATE TYPE udt_Change AS TABLE
(
LogId INT,
FieldName VARCHAR(64),
OldValue VARCHAR(512),
NewValue VARCHAR(512),
NotificationMessage VARCHAR(2048),
NotificationCategoryID INT
)
GO

-- CREATE udt_ChangeDetail table type
IF TYPE_ID(N'udt_ChangeDetail') IS NULL
CREATE TYPE udt_ChangeDetail AS TABLE
(
LogId INT,
Name VARCHAR(255),
Value VARCHAR(255)
)
GO

-- CREATE udt_IndicationAbbreviation table type
IF TYPE_ID(N'udt_IndicationAbbreviation') IS NULL
CREATE TYPE udt_IndicationAbbreviation AS TABLE
(
	Indication VARCHAR(255)
)

GO

-- CREATE udt_ChangeDetail table type
IF TYPE_ID(N'udt_IndicationID') IS NULL
CREATE TYPE udt_IndicationID AS TABLE
(
ID INT
)
GO

-- CREATE UserIndicationAssoc Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'UserIndicationAssoc' AND xtype = 'U')
CREATE TABLE UserIndicationAssoc
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
UserID INT NOT NULL,
IndicationID INT NOT NULL,
);
GO

-- Create NotificationColumn Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'NotificationColumn' AND xtype = 'U')
CREATE TABLE NotificationColumn
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Display_Name VARCHAR(128),
Column_Name VARCHAR(128),
Table_Name VARCHAR(128),
Indication VARCHAR(128)
);
GO

-- Create NotificationCategoryNotificationColumnAssoc Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'NotificationCategoryNotificationColumnAssoc' AND xtype = 'U')
CREATE TABLE NotificationCategoryNotificationColumnAssoc
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
NotificationCategoryID INT NOT NULL,
NotificationColumnID INT NOT NULL
);
GO

-- Create UserNotificationCategoryColumnAssoc Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'UserNotificationCategoryColumnAssoc' AND xtype = 'U')
CREATE TABLE UserNotificationCategoryColumnAssoc
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
UserID INT NOT NULL,
NotificationCategoryID INT NOT NULL,
NotificationColumnID INT NOT NULL
);
GO

-- CREATE EligibleEmailAddress Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'EligibleEmailAddress' AND xtype = 'U')
CREATE TABLE EligibleEmailAddress
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
EmailAddress VARCHAR(256) NOT NULL
);
GO

-- CREATE EligibleDomain Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'EligibleDomain' AND xtype = 'U')
CREATE TABLE EligibleDomain
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Domain VARCHAR(256) NOT NULL
);
GO

-- Create udt_BigIntegerID Table
IF TYPE_ID(N'udt_BigIntegerID') IS NULL
CREATE TYPE udt_BigIntegerID AS TABLE
(
ID BIGINT
)
GO

-- Create udt_NotificationCategoryNotificationColumn Table
IF TYPE_ID(N'udt_NotificationCategoryNotificationColumn') IS NULL
CREATE TYPE udt_NotificationCategoryNotificationColumn AS TABLE
(
NotificationCategoryID INT,
NotificationColumnID INT
)
GO

-- Create UserID Table
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'UserID' AND ss.name = N'dbo')
CREATE TYPE [dbo].[UserID] AS TABLE(
	[ID] [int] NOT NULL
)
GO

-- Create NotificationCategoryID Table
IF NOT  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'NotificationCategoryID' AND ss.name = N'dbo')
CREATE TYPE [dbo].[NotificationCategoryID] AS TABLE(
	[ID] [int] NOT NULL
)
GO

-- Create DeliveryMethodID Table
IF NOT  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'DeliveryMethodID' AND ss.name = N'dbo')
CREATE TYPE [dbo].[DeliveryMethodID] AS TABLE(
	[ID] [int] NOT NULL
)
GO

-- Create ExcludedUser Table
IF NOT  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'ExcludedUser' AND ss.name = N'dbo')
CREATE TYPE [dbo].[ExcludedUser] AS TABLE(
	[ChangeID] [bigint] NOT NULL,
	[UserID] [int] NOT NULL
)
GO

-- Create PattNotification Table
IF NOT  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'PattNotification' AND ss.name = N'dbo')
CREATE TYPE [dbo].[PattNotification] AS TABLE(
	[NotificationID] [bigint] NULL,
	[NotificationCategoryID] [int] NULL,
	[ChangeID] [bigint] NULL
)
GO

-- PlanDetailNotAvailableMessage
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'PlanDetailNotAvailableMessage' AND xtype = 'U')
CREATE TABLE PlanDetailNotAvailableMessage
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
FieldName VARCHAR(100) NOT NULL,
Message VARCHAR(1024) NOT NULL,
);
GO

-- NotificationEmailTemplateColumnOrder
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'NotificationEmailTemplateColumnOrder' AND xtype = 'U')
CREATE TABLE NotificationEmailTemplateColumnOrder
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
FieldName VARCHAR(100) NOT NULL,
AppearanceOrder INT NOT NULL,
SortOrder INT NOT NULL
);
GO


-- PATTDataEntry Dev DDL
USE PATTDataEntry_Dev
GO

-- CREATE ScanHistory Table
IF NOT EXISTS (SELECT * from sysobjects WHERE name = 'ScanHistory' AND xtype = 'U')
CREATE TABLE ScanHistory
(
ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
ApplicationID INT NULL,
LogID INT NOT NULL,
ProcessingTimeStamp DATETIME NOT NULL
);
GO

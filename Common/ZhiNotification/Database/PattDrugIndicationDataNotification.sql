USE ENO_Notification
GO

IF TYPE_ID(N'PattDrugIndiacationDataNotification') IS NULL
CREATE TYPE PattDrugIndiacationDataNotification AS TABLE
(
NotificationID BIGINT,
NotificationCategoryID INT,
Drug_Name NVARCHAR(255),
Indication NVARCHAR(255)
)
GO

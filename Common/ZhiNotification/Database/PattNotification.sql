USE ENO_Notification
GO

IF TYPE_ID(N'PattNotification') IS NULL
CREATE TYPE PattNotification AS TABLE
(
NotificationID BIGINT,
NotificationCategoryID INT
)
GO

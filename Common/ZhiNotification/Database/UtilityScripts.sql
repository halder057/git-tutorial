USE ENO_Notification
GO
-- Reset Some Information
UPDATE [ENO_Notification].[dbo].[Change] SET STATUS = 1
DELETE FROM [ENO_Notification].[dbo].[UserNotificationAssoc]
DELETE FROM [ENO_Notification].[dbo].[Notification]

-- Replicate CWP user table
select * into ENO_Notification.dbo.[User] from CWP.dbo.[User]
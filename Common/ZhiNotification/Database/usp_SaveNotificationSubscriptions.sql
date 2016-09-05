USE [ENO_Notification]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_SaveNotificationSubscriptions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_SaveNotificationSubscriptions]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Golam Kibria
-- Create date: 16 Nov 2015
-- Description:	Saves notification subscriptions 
--              for a set of given users
-- =============================================
CREATE PROCEDURE [dbo].[usp_SaveNotificationSubscriptions] 
	
	@UserIDs UserID READONLY,
	@NotificationCategoryNotificationColumns udt_NotificationCategoryNotificationColumn READONLY,
	@DeliveryMethodIDs DeliveryMethodID READONLY,
	@IndicationIDs udt_IndicationID READONLY,
	@AppID INT,
	@DeliveryFrequency INT = NULL,
	@ContentVolume INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @UserID INT
	
	-- Delete existing user notification category assocs
	DELETE unca FROM UserNotificationCategoryAssoc unca 
	INNER JOIN @UserIDs uids ON unca.UserID = uids.ID
	INNER JOIN NotificationCategory nc ON unca.NotificationCategoryID = nc.ID
	WHERE nc.ApplicationID = @AppID

	-- Delete existing user notification category column assocs
	DELETE uncca FROM UserNotificationCategoryColumnAssoc uncca
	INNER JOIN @UserIDs uids ON uncca.UserID = uids.ID
	INNER JOIN NotificationCategory nc ON nc.ID = uncca.NotificationCategoryID 
	WHERE nc.ApplicationID = @AppID
	
	-- Delete existing user notification delivery assocs
	DELETE FROM UserNotificationDeliveryAssoc WHERE ApplicationID = @AppID
	AND UserID IN (SELECT ID FROM @UserIDs)
	
	-- Delete existing user indication assocs
	DELETE FROM UserIndicationAssoc WHERE UserID IN (SELECT ID FROM @UserIDs)
	
	DECLARE UserCursor CURSOR FAST_FORWARD 
	FOR
	SELECT ID FROM @UserIDs
	
	OPEN UserCursor
	FETCH NEXT FROM UserCursor INTO @UserID
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
	-- Insert user notification category assocs
	INSERT INTO UserNotificationCategoryAssoc SELECT DISTINCT @UserID, NotificationCategoryID FROM @NotificationCategoryNotificationColumns
	-- Insert user notification category column assocs
	INSERT INTO UserNotificationCategoryColumnAssoc SELECT @UserID, NotificationCategoryID, NotificationColumnID FROM @NotificationCategoryNotificationColumns
	-- Insert user indication assocs
	INSERT INTO UserIndicationAssoc SELECT @UserID, ID FROM @IndicationIDs
	-- Insert user notification delivery assocs
	INSERT INTO UserNotificationDeliveryAssoc SELECT @UserID, ID, 
	CASE WHEN ID = 2 THEN @DeliveryFrequency ELSE NULL END, -- DeliveryMethodID = 2 denotes the 'Email' delivery method
	CASE WHEN ID = 2 THEN @ContentVolume ELSE NULL END, -- DeliveryMethodID = 2 denotes the 'Email' delivery method
	@AppID, NULL FROM @DeliveryMethodIDs 
	
	FETCH NEXT FROM UserCursor INTO @UserID
	END
	
	CLOSE UserCursor
	DEALLOCATE UserCursor
END
GO

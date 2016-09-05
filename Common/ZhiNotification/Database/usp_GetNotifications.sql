USE [ENO_Notification]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetNotifications]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetNotifications]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ==========================================================
-- Author:		Golam Kibria
-- Create date: 21-09-2015
-- Description:	Get notification, user and delivery details
--              optionally filter to get only pending changes 
--              and/or only about an specific application or
--              of an specific delivery method 
-- ==========================================================
CREATE PROCEDURE [dbo].[usp_GetNotifications] 
	@Status INT = NULL,
	@ApplicationID INT = NULL,
	@DeliveryMethod INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Return resultset#1: The user notification details
    SELECT una.ID AS UserNotificationAssocID, una.UserID, tuser.UserName, tuser.Email, tuser.FirstName, una.NotificationID, una.ApplicationID, noty.[Text], noty.NotificationDetails, noty.GenerationTime FROM UserNotificationAssoc una
    INNER JOIN UserNotificationDeliveryAssoc uda
    ON (una.UserID = uda.UserID AND una.ApplicationID = uda.ApplicationID)
    INNER JOIN [Notification] noty
    ON una.NotificationID = noty.ID
    INNER JOIN [User] tuser
    ON una.UserID = tuser.UserID
    WHERE
    (@Status IS NULL OR una.[Status] = @Status)
    AND
    (@ApplicationID IS NULL OR una.ApplicationID = @ApplicationID)
	AND
	(@DeliveryMethod IS NULL OR uda.DeliveryMethod = @DeliveryMethod)
	-- Return resultset#2: the user delivery details
	SELECT unda.ApplicationID, unda.UserID, unda.DeliveryMethod, unda.DeliveryFrequency, unda.ContentVolume FROM UserNotificationDeliveryAssoc unda
	WHERE
	(@ApplicationID IS NULL OR unda.ApplicationID = @ApplicationID)
	AND
	(@DeliveryMethod IS NULL OR unda.DeliveryMethod = @DeliveryMethod)
END
GO

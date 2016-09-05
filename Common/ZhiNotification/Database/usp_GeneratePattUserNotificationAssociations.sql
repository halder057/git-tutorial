USE [ENO_Notification]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GeneratePattUserNotificationAssociations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GeneratePattUserNotificationAssociations]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- ==========================================================
-- Author:		Golam Kibria
-- Create date: 23-09-2015
-- Description:	Generate user notification associations for
--              PATT notifications
-- ==========================================================
CREATE PROCEDURE [dbo].[usp_GeneratePattUserNotificationAssociations] @PattNotifications PattNotification READONLY
	,@ExcludedUsers ExcludedUser READONLY, @ApplicationID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @HPMFieldName VARCHAR(128)
	DECLARE @HPMLabelName VARCHAR(128)
	SET @HPMFieldName = 'Health plan Management'
	SET @HPMLabelName = 'Healthplan Management'
        
    DECLARE @NotificationID BIGINT;
	DECLARE @NotificationCategoryID INT;
	DECLARE @ChangeID BIGINT;
	    
	DECLARE NotificationCursor CURSOR FAST_FORWARD
	FOR
	SELECT NotificationID
		,NotificationCategoryID
		,ChangeID
	FROM @PattNotifications;

	OPEN NotificationCursor

	FETCH NEXT
	FROM NotificationCursor
	INTO @NotificationID
		,@NotificationCategoryID
		,@ChangeID

	WHILE @@FETCH_STATUS = 0
	BEGIN
	INSERT INTO UserNotificationAssoc
	SELECT DISTINCT unca.UserID
			,@NotificationID
			,1
			,@ApplicationID
	FROM UserNotificationCategoryAssoc unca
		INNER JOIN [User] u ON unca.UserID = u.UserID
		INNER JOIN [Client] c ON u.ClientID = c.ClientID
		INNER JOIN [Change] ch ON ch.NotificationCategoryID = unca.NotificationCategoryID
		INNER JOIN UserNotificationCategoryColumnAssoc uncca ON (uncca.UserID = unca.UserID)
		INNER JOIN UserIndicationAssoc uia ON uia.UserID = unca.UserID
		INNER JOIN Indication indi ON indi.IndicationID = uia.IndicationID
		CROSS APPLY [GetUserLevelMCOFilterWithPlanFilter_table]
		    (
		       unca.UserID, 
		       ISNULL(ch.Indication, (SELECT TOP 1 i.abbreviation FROM Indication i
               INNER JOIN DrugIndication di ON di.IndicationID = i.IndicationID
               INNER JOIN drug d ON d.DrugID = di.DrugID
               WHERE d.name = ch.Drug_Name)), 
		       ch.Drug_Name
		    ) AS usermco
		WHERE 
		ch.ID = @ChangeID 
		AND (unca.NotificationCategoryID = @NotificationCategoryID) 
		AND (indi.Abbreviation = ISNULL(ch.Indication, (SELECT TOP 1 i.abbreviation FROM Indication i
               INNER JOIN DrugIndication di ON di.IndicationID = i.IndicationID
               INNER JOIN drug d ON d.DrugID = di.DrugID
               WHERE d.name = ch.Drug_Name))) -- subscribed to indication
		AND (uncca.NotificationColumnID =      (SELECT ID FROM NotificationColumn 
												WHERE
												(Column_Name = (SELECT TOP 1 nd.Value FROM ChangeNotificationDetailAssoc cna
												INNER JOIN NotificationDetail nd ON nd.ID = cna.NotificationDetailID 
												WHERE cna.ChangeID = ch.ID AND nd.Name = 'ColumnName') AND 
												(indi.Abbreviation IS NULL OR Indication = 'ALL' OR Indication = indi.Abbreviation))
                                               )) -- subscribed to category and column
       AND (ch.ChangedFieldName <> @HPMFieldName 
                                                OR (dbo.GetCustomColumnName(@HPMLabelName, c.Name, indi.Abbreviation) 
                                                = (SELECT TOP 1 nd.Value FROM ChangeNotificationDetailAssoc cna
												INNER JOIN NotificationDetail nd ON nd.ID = cna.NotificationDetailID 
												WHERE cna.ChangeID = ch.ID AND nd.Name = 'ColumnName'))) -- not HPM change or user has access to this HPM
      AND (usermco.mcoid = ch.MCOID)	-- user has access to this plan	
      AND NOT EXISTS 
		(SELECT UserID FROM @ExcludedUsers WHERE ChangeID = @ChangeID AND UserID = unca.UserID) 
      								
	FETCH NEXT
		FROM NotificationCursor
		INTO @NotificationID
			,@NotificationCategoryID
			,@ChangeID
	END
	
	CLOSE NotificationCursor
	DEALLOCATE NotificationCursor
												
END
GO

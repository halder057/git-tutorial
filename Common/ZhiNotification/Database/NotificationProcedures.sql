USE ENO_Notification
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ApproveChangesAndGenerateNotifications]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ApproveChangesAndGenerateNotifications]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- ==========================================================
-- Author:		Golam Kibria
-- Create date: 23-09-2015
-- Description:	Approves changes and generate notifications
--              returns list of generated notification
-- ==========================================================
CREATE PROCEDURE [dbo].[usp_ApproveChangesAndGenerateNotifications] @ChangeIDs udt_BigIntegerID READONLY,
@ApprovedBy INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	CREATE TABLE #tempNotification
	(
	ID BIGINT,
	NotificationCategoryID INT,
	ChangeID BIGINT
	)

	--INSERT INTO [Notification] (NotificationCategoryID, GenerationTime, [Text], ChangeID, Drug_Name, MCOID, Indication)
	MERGE INTO [Notification] AS t1
	USING (SELECT c.NotificationCategoryID, c.GenerationTime, c.NotificationMessage, c.ID, c.Drug_Name, c.MCOID, c.Indication 
	FROM @ChangeIDs cids
	INNER JOIN Change c ON cids.ID = c.ID) src ON 1=0

	WHEN NOT MATCHED BY TARGET THEN
    INSERT(
    NotificationCategoryID, GenerationTime, [Text], ChangeID, Drug_Name, MCOID, Indication
    )
    VALUES
    (
    src.NotificationCategoryID,
    GETUTCDATE(),
    src.NotificationMessage,
    src.ID,
    src.Drug_Name,
    src.MCOID,
    src.Indication
    )
    OUTPUT INSERTED.ID, src.NotificationCategoryID, src.ID AS ChangeID
    INTO #tempNotification(ID, NotificationCategoryID, ChangeID);

	UPDATE c SET [Status] = 2, ApprovedBy = @ApprovedBy
	FROM
	Change c
	INNER JOIN @ChangeIDs cids ON c.ID  = cids.ID

	-- retrun
	SELECT * FROM #tempNotification

	DROP TABLE #tempNotification
												
END
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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetCustomNotificationColumnNames]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetCustomNotificationColumnNames]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_GetCustomNotificationColumnNames] 
 @aliasName  VARCHAR(255)
 ,@client VARCHAR(255)
 ,@indicationAbbreviations udt_IndicationAbbreviation readonly
AS
BEGIN


DECLARE @fieldNames TABLE 
(
  fieldName VARCHAR(255)
)

INSERT INTO @fieldNames  SELECT  FieldName    
FROM tblQueryMasterUnion tqmu
INNER JOIN 
@indicationAbbreviations ia on tqmu.IndicationDrug = ia.Indication WHERE LabelName = @aliasName   AND client = @client

IF( (select COUNT(*) from @fieldNames) < 1  )
	INSERT INTO @fieldNames	SELECT FieldName   FROM tblQueryMasterUnion tqmu  INNER JOIN @indicationAbbreviations ia  on tqmu.IndicationDrug = ia.Indication WHERE LabelName = @aliasName   AND client = 'All'

SELECT DISTINCT fieldName FROM @fieldNames

END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetPattUsersAssociatedWithChange]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetPattUsersAssociatedWithChange]
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
CREATE PROCEDURE [dbo].[usp_GetPattUsersAssociatedWithChange] @ChangeID BIGINT
	,@NotificationCategoryID INT
AS
BEGIN
	    -- SET NOCOUNT ON added to prevent extra result sets from
	    -- interfering with SELECT statements.
	    SET NOCOUNT ON;

		DECLARE @HPMFieldName VARCHAR(128)
		DECLARE @HPMLabelName VARCHAR(128)
		SET @HPMFieldName = 'Health plan Management'
		SET @HPMLabelName = 'Healthplan Management'

		SELECT DISTINCT u.*
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
WHERE d.name = ch.Drug_Name
)), 
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
												
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetUserIndications]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetUserIndications]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Golam Kibria
-- Description:	
--   Inputs: UserID
--   Returns valid list of IndicationID, and Names and Abbreviations accessable to a given user
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetUserIndications] @UserID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	EXEC usp_List_Indications @UserID = @UserID
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_SaveChangesAndDetails]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_SaveChangesAndDetails]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =====================================
-- Author:		Golam Kibria
-- Create date: 30-Nov-2015
-- Description:	Save changes and details
-- =====================================
CREATE PROCEDURE [dbo].[usp_SaveChangesAndDetails]
	@Changes udt_Change READONLY,
	@ChangeDetails udt_ChangeDetail READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
    DECLARE @ChangeChangeLogAssoc TABLE
    (
    ID BIGINT,
    LogId INT
    )
    
    DECLARE @ChangeDetailChangeLogAssoc TABLE
    (
    ID BIGINT,
    LogId INT
    )
    
    -- Change
    MERGE INTO Change AS t1
    USING @Changes AS src ON 1=0
    
    WHEN NOT MATCHED BY TARGET THEN
    INSERT(
    NotificationCategoryID,
    ChangedFieldName,
    PreviousValue,
    CurrentValue,
    NotificationMessage,
	GenerationTime,
    ChangedBy,
    ApprovedBy,
    [Status],
    MCOID,
    Indication,
    Drug_Name
    )
    VALUES
    (
    src.NotificationCategoryID,
    src.FieldName,
    src.OldValue,
    src.NewValue,
    src.NotificationMessage,
    GETUTCDATE(),
    NULL,
    NULL,
    1,
    CAST((SELECT TOP 1 Value FROM @ChangeDetails WHERE LogId = src.LogId AND Name = 'MCOID') AS INT),
    (SELECT TOP 1 Value FROM @ChangeDetails WHERE LogId = src.LogId AND Name = 'TACode'),
    (SELECT TOP 1 Value FROM @ChangeDetails WHERE LogId = src.LogId AND Name = 'Drug')
    )
    OUTPUT INSERTED.ID, src.LogId
    INTO @ChangeChangeLogAssoc(ID, LogId);
    
    -- Notification Details
    MERGE INTO NotificationDetail AS t1
    USING @ChangeDetails AS src ON 1=0
    
    WHEN NOT MATCHED BY TARGET THEN
    INSERT(
    Name,
    Value
    )
    VALUES
    (
    src.Name,
    src.Value
    )
    OUTPUT INSERTED.ID, src.LogId
    INTO @ChangeDetailChangeLogAssoc(ID, LogId);
    
    -- Change Notification Detail Assoc
    INSERT INTO ChangeNotificationDetailAssoc
    SELECT 
    ccla.ID, -- Change ID
    cdcla.ID -- NotificationDetail ID
    FROM @ChangeChangeLogAssoc ccla
    INNER JOIN @ChangeDetailChangeLogAssoc cdcla ON ccla.LogId = cdcla.LogId
END
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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_SavePattChangesAndDetails]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_SavePattChangesAndDetails]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =====================================
-- Author:		Golam Kibria
-- Create date: 30-Nov-2015
-- Description:	Save PATT changes and details
-- =====================================
CREATE PROCEDURE [dbo].[usp_SavePattChangesAndDetails]
	@Changes udt_Change READONLY,
	@ChangeDetails udt_ChangeDetail READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
    CREATE TABLE #ChangeDetailsTemp
    (
    LogId INT,
    Name VARCHAR(255),
    Value VARCHAR(255)
    )
    
    CREATE TABLE #ChangeChangeLogAssoc
    (
    ID BIGINT,
    LogId INT
    )
    
    CREATE TABLE #ChangeDetailChangeLogAssoc
    (
    ID BIGINT,
    LogId INT
    )
    
    -- Change
    MERGE INTO Change AS t1
    USING @Changes AS src ON 1=0
    
    WHEN NOT MATCHED BY TARGET THEN
    INSERT(
    NotificationCategoryID,
    ChangedFieldName,
    PreviousValue,
    CurrentValue,
    NotificationMessage,
	GenerationTime,
    ChangedBy,
    ApprovedBy,
    [Status],
    MCOID,
    Indication,
    Drug_Name
    )
    VALUES
    (
    src.NotificationCategoryID,
    src.FieldName,
    src.OldValue,
    src.NewValue,
    src.NotificationMessage,
    GETUTCDATE(),
    NULL,
    NULL,
    1,
    CAST((SELECT TOP 1 Value FROM @ChangeDetails WHERE LogId = src.LogId AND Name = 'MCOID') AS INT),
    (SELECT TOP 1 Value FROM @ChangeDetails WHERE LogId = src.LogId AND Name = 'TACode'),
    (SELECT TOP 1 Value FROM @ChangeDetails WHERE LogId = src.LogId AND Name = 'Drug')
    )
    OUTPUT INSERTED.ID, src.LogId
    INTO #ChangeChangeLogAssoc(ID, LogId);
    
    INSERT INTO #ChangeDetailsTemp
    SELECT * FROM @ChangeDetails
    
    -- Add associated indication abbreviation in notification details
    INSERT INTO #ChangeDetailsTemp
    SELECT DISTINCT ch.LogId, 
    'SecondaryTA',
    ISNULL(cd.Value, (SELECT DISTINCT
                      CASE
                      WHEN EXISTS (SELECT COUNT(*), name FROM DrugIndication di INNER JOIN drug d ON d.DrugID = di.DrugID 
                        WHERE name = (SELECT TOP 1 Value FROM @ChangeDetails WHERE LogId = ch.LogId AND Name = 'Drug') GROUP BY name HAVING COUNT(*)=1)
                      THEN i.Name
                      ELSE 'N/A'
                      END AS Indicaton
                      FROM dbo.Indication i
                      INNER JOIN DrugIndication di ON di.IndicationID = i.IndicationID
                      INNER JOIN drug d ON d.DrugID = di.DrugID WHERE d.name = 
                         (SELECT TOP 1 Value FROM @ChangeDetails WHERE LogId = ch.LogId AND Name = 'Drug'))) AS Value
    FROM @Changes ch
    INNER JOIN @ChangeDetails cd ON ch.LogId = cd.LogId
    WHERE cd.Name = 'TACode'
    
    -- Notification Details
    MERGE INTO NotificationDetail AS t1
    USING #ChangeDetailsTemp AS src ON 1=0
    
    WHEN NOT MATCHED BY TARGET THEN
    INSERT(
    Name,
    Value
    )
    VALUES
    (
    src.Name,
    src.Value
    )
    OUTPUT INSERTED.ID, src.LogId
    INTO #ChangeDetailChangeLogAssoc(ID, LogId);
    
    -- Change Notification Detail Assoc
    INSERT INTO ChangeNotificationDetailAssoc
    SELECT 
    ccla.ID, -- Change ID
    cdcla.ID -- NotificationDetail ID
    FROM #ChangeChangeLogAssoc ccla
    INNER JOIN #ChangeDetailChangeLogAssoc cdcla ON ccla.LogId = cdcla.LogId
    
    DROP TABLE #ChangeChangeLogAssoc
    DROP TABLE #ChangeDetailChangeLogAssoc
    DROP TABLE #ChangeDetailsTemp
END
GO


USE PATTDataEntry_DEV
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetPattChangesDetailsTemplates]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetPattChangesDetailsTemplates]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- ==========================================================
-- Author:		Golam Kibria
-- Create date: 26-Nov-15
-- Description:	Get latest CWP changes, details and templates
-- ==========================================================
CREATE PROCEDURE [dbo].[usp_GetPattChangesDetailsTemplates] @LastProcessedLogId INT = NULL,
@ApplicationId INT = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
    -- Latet Approval Entry's Update Time
    DECLARE @ScanStartTimeBaseLine DATETIME = '2016-01-01'
    DECLARE @LatestProcessedApprovalTime DATETIME
    
    IF @LastProcessedLogId IS NULL
    SET @LatestProcessedApprovalTime = NULL
    ELSE
    SET @LatestProcessedApprovalTime = (SELECT TOP 1 ulogs.UpdateTime 
                              FROM 
                              (SELECT rowID, UpdateTime FROM UpdateLogs WHERE ID = @LastProcessedLogId) lplog
                              INNER JOIN UpdateLogs ulogs ON ulogs.rowID = lplog.rowID
                              WHERE ulogs.UpdateTime > lplog.UpdateTime AND ulogs.UpdateSource = 8
                              ORDER BY ulogs.UpdateTime
                              )
	
	
    -- Filter out UpdateLogs entries by DE approval process
    ;WITH NonRankedApprovals
    AS
    (
    SELECT rowid,UpdateTime
    FROM UpdateLogs
    WHERE UpdateSource = 8
		AND (
			(
				@LatestProcessedApprovalTime IS NOT NULL
				AND UpdateTime > @LatestProcessedApprovalTime
				)
			OR (
				@LatestProcessedApprovalTime IS NULL
				AND UpdateTime > @ScanStartTimeBaseLine
				)
			)
    )
    
    ,LatestProcessedRowUpdateTimes
    AS -- finds the last times rowIds were processed (or set to Jan 1st, 2016 if not processed yet)
    (
    SELECT nra.rowid, 
    nra.updateTime,  
    CASE WHEN @LatestProcessedApprovalTime IS NULL THEN @ScanStartTimeBaseLine
    ELSE ISNULL((SELECT TOP 1 ul.UpdateTime FROM ScanHistory sh
                 INNER JOIN UpdateLogs ul ON sh.LogID = ul.ID
                 WHERE ul.rowID = nra.rowid
                 AND ul.UpdateTime <= nra.UpdateTime
                 AND ul.UpdateTime <= @LatestProcessedApprovalTime
                 ORDER BY ul.UpdateTime DESC)
                 , @ScanStartTimeBaseLine)
    END AS LastProcessedRowUpdateTime,
    RANK() OVER
    (
    PARTITION BY rowid ORDER BY CASE WHEN @LatestProcessedApprovalTime IS NULL THEN @ScanStartTimeBaseLine
    ELSE ISNULL((SELECT TOP 1 ul.UpdateTime FROM ScanHistory sh
                 INNER JOIN UpdateLogs ul ON sh.LogID = ul.ID
                 WHERE ul.rowID = nra.rowid
                 AND ul.UpdateTime <= nra.UpdateTime
                 AND ul.UpdateTime <= @LatestProcessedApprovalTime
                 ORDER BY ul.UpdateTime DESC)
                 , @ScanStartTimeBaseLine)
    END DESC
    ) AS rank
    FROM NonRankedApprovals nra
    )
    
	,RankedApprovals
    AS --this section finds approvals that happened since jan 1/16 (arbitrary date for now) or since the logs were last processed,
       --and ranks them by rowIds based on UpdateTime
	(
	SELECT rowid
		,UpdateTime
		,RANK() OVER (
			PARTITION BY rowid ORDER BY UpdateTime DESC
			) AS RANK
	FROM NonRankedApprovals
	)
	
	,RANGE
    AS --this uses the results FROM above AND finds the newest and oldest approval rows and outputs one row with start/stop dates for each qualifiying rowid
	(
	SELECT DISTINCT a.rowid
		,a.UpdateTime AS latest_update
		,a.rank AS latest_rank
		,b.LastProcessedRowUpdateTime AS oldest_update
		,b.rank AS oldest_rank
	FROM RankedApprovals a INNER JOIN LatestProcessedRowUpdateTimes b ON a.rowid = b.rowid
	WHERE 
		a.RANK = 1
		AND b.rank = 1
	)
	
	,eligible
    AS -- this joins the above results with the corresponding data row in dataentry to get the MCOID
	(
	SELECT u.id
		,u.rowid
		,tablename
		,ColumnName
		,OldValue
		,newValue
		,u.UpdateTime
		,CASE 
			WHEN tablename = 'ParentDrug'
				THEN p.mco
			WHEN tablename = 'indicationdrugdata'
				THEN i.mco
			ELSE T.mcoid
			END AS mcoid
		,CASE 
			WHEN tablename = 'ParentDrug'
				THEN p.drug_name
			WHEN tablename = 'indicationdrugdata'
				THEN i.indication + '/' + i.drug_name
			ELSE NULL
			END AS IndicationDrug
		,CASE tableName
			WHEN 'tblMCO'
				THEN NULL -- Plan level change, drug not needed
			WHEN 'ParentDrug'
				THEN p.Drug_Name
			WHEN 'IndicationDrugData'
				THEN i.Drug_Name
			ELSE NULL
			END AS Drug
		,CASE tableName
			WHEN 'tblMCO'
				THEN NULL -- Plan level change, indication not needed
			WHEN 'ParentDrug'
				THEN NULL -- Drug level change, indication not needed
			WHEN 'IndicationDrugData'
				THEN i.indication
			ELSE NULL
			END AS Indication
	FROM updatelogs u
	INNER JOIN range r ON u.rowid = r.rowid
	LEFT JOIN tblmco_production t ON T.tblmcoid = u.rowID
	LEFT JOIN parentdrug_production p ON p.ParentDrugID = u.rowID
	LEFT JOIN IndicationDrugDataOne_production i ON i.IndicationDrugDataID = u.rowid
	WHERE u.UpdateTime > r.oldest_update
		AND u.UpdateTime <= r.latest_update
		AND UpdateSource <> 8
	)
	
	,results
    AS -- this joins the above with tblmco to get the mco name
	(
	SELECT e.id
		,e.rowid
		,e.tableName
		,t.mco
		,t.mcoid
		,indicationdrug
		,ColumnName
		,e.OldValue
		,e.newValue
		,e.Drug
		,e.Indication
		,updatetime
		,RANK() OVER (
			PARTITION BY t.mcoid
			,indicationdrug	
			,columnname ORDER BY UpdateTime DESC
			) AS Rank
	FROM eligible e
	INNER JOIN tblmco_production t ON e.mcoid = t.mcoid
	)
	
	,Final
    AS -- final set of entries to be processed
    (
	SELECT rn.id
		,rn.rowid
		,rn.mco
		,rn.mcoid
		,rn.tableName
		,rn.indicationdrug
		,rn.Drug
		,rn.Indication
		,rn.columnname
		,ro.oldvalue
		,REPLACE(rn.newvalue, '''', '') AS newValue
		,rn.updatetime
	FROM results rn
	INNER JOIN results ro ON rn.mcoid = ro.mcoid
		AND rn.indicationdrug = ro.indicationdrug
		AND rn.ColumnName = ro.columnname
	WHERE rn.rank = 1
		AND ro.rank = (
			SELECT MAX(rank)
			FROM results
			WHERE mcoid = rn.mcoid
				AND indicationdrug = rn.indicationdrug
				AND ColumnName = rn.ColumnName
			)
	)
	
	SELECT * INTO #tempUpdateLogs FROM Final 
	WHERE (((OldValue IS NULL OR len(oldValue) <= 512) AND (newValue IS NULL OR len(newValue) <= 512))
	AND ((newValue IS NULL AND OldValue IS NOT NULL) OR (OldValue IS NULL AND newValue IS NOT NULL) OR newValue <> OldValue))
	ORDER BY id

	SELECT ulog.ID
		,ulog.rowID
		,ncolumn.Display_Name AS FieldName
		,ulog.tableName
		,ulog.ColumnName
		,ulog.OldValue
		,ulog.newValue
		,ulog.Drug
		,indi.Name AS Indication
		,ulog.Indication AS IndicationAbbr
		,ulog.MCO AS [Plan]
		,ulog.MCOID
		,dr.DrugID
		,ISNULL(vcdd.PACriteria_DocPath, 'PA Policy Document Not Applicable') AS PlanDetailLink
	INTO #tempLogEntries
	FROM #tempUpdateLogs ulog
	INNER JOIN NotificationColumn ncolumn ON ulog.ColumnName = ncolumn.Column_Name
	LEFT JOIN Drug dr ON (ulog.Drug = dr.Name)
	LEFT JOIN vw_CurrentDocDisplay vcdd ON (ulog.MCOID = vcdd.mcoid AND dr.DrugID = vcdd.DrugID)
	LEFT JOIN Indication indi -- indication
		ON (
			ulog.tableName = 'IndicationDrugData'
			AND indi.Abbreviation = ulog.Indication
			)
	WHERE (
			ncolumn.Indication = 'ALL'
			OR indi.Abbreviation = ncolumn.Indication
			) -- Applicable indications only
	ORDER BY ulog.ID
    
    DROP TABLE #tempUpdateLogs

	-- Add change entries
	SELECT le.Id AS LogId
		,le.FieldName
		,le.OldValue
		,le.NewValue
		,NULL AS NotificationMessage
		,ncnca.NotificationCategoryID
	INTO #tempChanges
	FROM #tempLogEntries le
	INNER JOIN NotificationColumn nc ON nc.Column_Name = le.ColumnName
	INNER JOIN NotificationCategoryNotificationColumnAssoc ncnca ON ncnca.NotificationColumnID = nc.ID

	-- Add change detail entries
	CREATE TABLE #tempChangeDetails
	(
	 LogId INT,
	 Name VARCHAR(255),
	 Value VARCHAR(255)
	)
	-- Drugs
	INSERT INTO #tempChangeDetails
	SELECT Id AS LogId
		,'Drug' AS Name
		,Drug AS Value
	FROM #tempLogEntries

	-- Indications
	INSERT INTO #tempChangeDetails (LogId, Name, Value)
	SELECT Id AS LogId
		,'TA' AS Name
		,Indication AS Value
	FROM #tempLogEntries
	
	-- Indication abbreviations
	INSERT INTO #tempChangeDetails (LogId, Name, Value)
	SELECT Id AS LogId
		,'TACode' AS Name
		,IndicationAbbr AS Value
	FROM #tempLogEntries
	
	-- Table Names
	INSERT INTO #tempChangeDetails (LogId, Name, Value)
	SELECT Id AS LogId
		,'TableName' AS Name
		,TableName AS Value
	FROM #tempLogEntries
	
	-- Column Names
	INSERT INTO #tempChangeDetails (LogId, Name, Value)
	SELECT Id AS LogId
		,'ColumnName' AS Name
		,ColumnName AS Value
	FROM #tempLogEntries

	-- Plans
	INSERT INTO #tempChangeDetails (LogId, Name, Value)
	SELECT Id AS LogId
		,'PlanName' AS Name
		,[Plan] AS Value
	FROM #tempLogEntries
	
	-- Plan IDs
    INSERT INTO #tempChangeDetails (LogId, Name, Value)
	SELECT Id AS LogId
		,'MCOID' AS Name
		,CAST(MCOID AS VARCHAR(255)) AS Value
	FROM #tempLogEntries

	-- PlanDetailLink
	INSERT INTO #tempChangeDetails (LogId, Name, Value)
	SELECT Id AS LogId
		,'PlanDetailLink' AS Name
		,PlanDetailLink AS Value
	FROM #tempLogEntries

    -- Save Scan History
    INSERT INTO ScanHistory
    SELECT 
    @ApplicationId, -- app id
    c.LogId, -- log id
    GETUTCDATE() -- processing timestamp
    FROM #tempChanges c

	-- Returned resultset #1: The changes
	SELECT LogId
		,FieldName
		,OldValue
		,NewValue
		,NotificationMessage
		,NotificationCategoryID
	FROM #tempChanges;

	-- Returned resultset #2: The change details
	SELECT LogId
		,NAME
		,Value
	FROM #tempChangeDetails;
	
	DROP TABLE #tempChangeDetails

	-- Returned resultset #3: The templates
	SELECT Distinct NotificationCategoryID
	INTO #tempNotyCategoryIDs
	FROM #tempChanges
	
	DROP TABLE #tempChanges
	
	SELECT NotificationCategoryID, Message
	FROM NotificationTemplateMessage
	WHERE NotificationCategoryID IN (SELECT DISTINCT NotificationCategoryID FROM #tempNotyCategoryIDs)
		AND Active = 1
	ORDER BY ID
	DROP TABLE #tempNotyCategoryIDs
END
GO

USE [PATTDataEntry_Dev]
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
	
	DECLARE @ProcessableLogEntries TABLE (
	    ID INT,
	    RowID INT,
	    MCO VARCHAR(2048),
	    MCOID INT,
	    TableName VARCHAR(128),
	    IndicationDrug VARCHAR(512),
	    Drug VARCHAR(256),
	    Indication VARCHAR(256),
	    ColumnName VARCHAR(512),
	    OldValue VARCHAR(MAX),
	    NewValue VARCHAR(MAX),
	    UpdateTime DATETIME
	    )
	-- Temporary table to hold latest log entries
	DECLARE @LogEntries TABLE (
		Id INT
		,RowId INT
		,FieldName VARCHAR(64)
		,TableName VARCHAR(100)
		,ColumnName VARCHAR(100)
		,OldValue VARCHAR(MAX)
		,NewValue VARCHAR(MAX)
		,Drug VARCHAR(255)
		,Indication VARCHAR(255)
		,IndicationAbbr VARCHAR(255)
		,[Plan] VARCHAR(255)
		,MCOID INT
		,DrugID INT
		,PlanDetailLink VARCHAR(512)
		)
	-- Temporary table to hold change entries
	DECLARE @Changes TABLE (
		LogId INT
		,FieldName VARCHAR(64)
		,OldValue VARCHAR(MAX)
		,NewValue VARCHAR(MAX)
		,NotificationMessage VARCHAR(2048)
		,NotificationCategoryID INT
		)
	-- Temporary table to hold change detail entries
	DECLARE @ChangeDetails TABLE (
		LogId INT
		,NAME VARCHAR(64)
		,Value VARCHAR(255)
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
	INSERT INTO @ProcessableLogEntries
	SELECT * FROM Final 
	WHERE ((newValue IS NULL AND OldValue IS NOT NULL) OR (OldValue IS NULL AND newValue IS NOT NULL) OR newValue <> OldValue)
	ORDER BY id

	INSERT INTO @LogEntries
	SELECT ulog.ID
		,ulog.rowID
		,ncolumn.Display_Name AS DisplayName
		,ulog.tableName
		,ulog.ColumnName
		,ulog.OldValue
		,ulog.newValue
		,ulog.Drug
		,indi.Name
		,ulog.Indication
		,ulog.MCO
		,ulog.MCOID
		,dr.DrugID
		,ISNULL(vcdd.PACriteria_DocPath, 'PA Policy Document Not Applicable') AS PlanDetailLink
	FROM @ProcessableLogEntries ulog
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
    
	-- Add change entries
	INSERT INTO @Changes
	SELECT le.Id
		,le.FieldName
		,le.OldValue
		,le.NewValue
		,NULL
		,ncnca.NotificationCategoryID -- For PATT, only one notification category: 'Data Change'
	FROM @LogEntries le
	INNER JOIN NotificationColumn nc ON nc.Column_Name = le.ColumnName
	INNER JOIN NotificationCategoryNotificationColumnAssoc ncnca ON ncnca.NotificationColumnID = nc.ID

	-- Add change detail entries
	-- Drugs
	INSERT INTO @ChangeDetails
	SELECT Id
		,'Drug'
		,Drug
	FROM @LogEntries

	-- Indications
	INSERT INTO @ChangeDetails
	SELECT Id
		,'TA'
		,Indication
	FROM @LogEntries
	
	-- Indication abbreviations
	INSERT INTO @ChangeDetails
	SELECT Id
		,'TACode'
		,IndicationAbbr
	FROM @LogEntries
	
	-- Table Names
	INSERT INTO @ChangeDetails
	SELECT Id
		,'TableName'
		,TableName
	FROM @LogEntries
	
	-- Column Names
	INSERT INTO @ChangeDetails
	SELECT Id
		,'ColumnName'
		,ColumnName
	FROM @LogEntries

	-- Plans
	INSERT INTO @ChangeDetails
	SELECT Id
		,'PlanName'
		,[Plan]
	FROM @LogEntries
	
	-- Plan IDs
    INSERT INTO @ChangeDetails
	SELECT Id
		,'MCOID'
		,CAST(MCOID AS VARCHAR(255))
	FROM @LogEntries

	-- PlanDetailLink
	INSERT INTO @ChangeDetails
	SELECT Id
		,'PlanDetailLink'
		,PlanDetailLink
	FROM @LogEntries

    -- Save Scan History
    INSERT INTO ScanHistory
    SELECT 
    @ApplicationId, -- app id
    c.LogId, -- log id
    GETUTCDATE() -- processing timestamp
    FROM @Changes c

	-- Returned resultset #1: The changes
	SELECT LogId
		,FieldName
		,OldValue
		,NewValue
		,NotificationMessage
		,NotificationCategoryID
	FROM @Changes;

	-- Returned resultset #2: The change details
	SELECT LogId
		,NAME
		,Value
	FROM @ChangeDetails;

	-- Returned resultset #3: The templates
	SELECT NotificationCategoryID, Message
	FROM NotificationTemplateMessage
	WHERE NotificationCategoryID IN (SELECT NotificationCategoryID FROM @Changes)
		AND Active = 1
	ORDER BY ID
END
GO

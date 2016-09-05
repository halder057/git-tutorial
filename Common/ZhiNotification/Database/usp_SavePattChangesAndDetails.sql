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
    
    DECLARE @ChangeDetailsTemp TABLE
    (
    LogId INT,
    Name VARCHAR(255),
    Value VARCHAR(255)
    )
    
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
    
    INSERT INTO @ChangeDetailsTemp
    SELECT * FROM @ChangeDetails
    
    -- Add associated indication abbreviation in notification details
    INSERT INTO @ChangeDetailsTemp
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
    WHERE cd.Name = 'TA'
    
    -- Notification Details
    MERGE INTO NotificationDetail AS t1
    USING @ChangeDetailsTemp AS src ON 1=0
    
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

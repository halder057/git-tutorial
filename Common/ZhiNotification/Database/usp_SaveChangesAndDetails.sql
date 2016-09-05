USE [ENO_Notification]
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

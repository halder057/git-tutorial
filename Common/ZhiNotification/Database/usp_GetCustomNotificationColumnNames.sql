USE [ENO_Notification]
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



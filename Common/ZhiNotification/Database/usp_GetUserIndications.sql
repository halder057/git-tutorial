USE [ENO_Notification]
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

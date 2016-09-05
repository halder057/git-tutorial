USE ENO_Notification
GO

CREATE VIEW PattUserDrugIndicationView
AS
SELECT usr.UserID, cdi.DrugID, drg.Name AS DrugName, cdi.IndicationID, indi.Name AS Indication FROM ENO_CWP.dbo.ClientDrugIndication cdi
INNER JOIN ENO_CWP.dbo.Drug drg ON cdi.DrugID = drg.DrugID
INNER JOIN ENO_CWP.dbo.Indication indi ON cdi.IndicationID = indi.IndicationID 
INNER JOIN ENO_CWP.zhi.tblUser usr
ON cdi.ClientID = usr.ClientID

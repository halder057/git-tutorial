USE ENO_Notification
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'NotificationCategory' AND xtype = 'U')
BEGIN
INSERT INTO NotificationCategory VALUES('HPM Change',0,0,1)
INSERT INTO NotificationCategory VALUES('Preferred Status Change',0,0,1)
INSERT INTO NotificationCategory VALUES('Critical Data Change',0,0,1)
INSERT INTO NotificationCategory VALUES('Other Data Change',0,0,1)
INSERT INTO NotificationCategory VALUES('Lives Change',0,0,1)
INSERT INTO NotificationCategory VALUES('MCMM HPM Change',0,0,2)
INSERT INTO NotificationCategory VALUES('MCMM Preferred Status Change',0,0,2)
INSERT INTO NotificationCategory VALUES('MCMM Critical Data Change',0,0,2)
INSERT INTO NotificationCategory VALUES('MCMM Other Data Change',0,0,2)
INSERT INTO NotificationCategory VALUES('MCMM Lives Change',0,0,2)
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'Change' AND xtype = 'U')
BEGIN
DELETE FROM Change
DECLARE @NotyDetails XML = '<patt><Drug>My Drug</Drug><Indication>My Indication</Indication></patt>'
INSERT INTO Change VALUES(1,'HPM',NULL,NULL,'An HPM has been changed','',NULL,NULL,NULL,@NotyDetails)
INSERT INTO Change VALUES(2,'Preferred Status',NULL,NULL,'A preferred status has been changed','',NULL,NULL,NULL,@NotyDetails)
INSERT INTO Change VALUES(3,'Critical Data',NULL,NULL,'A critical data has been changed','',NULL,NULL,NULL,@NotyDetails)
INSERT INTO Change VALUES(4,'Other Data',NULL,NULL,'Other Data Change','',NULL,NULL,NULL,@NotyDetails)
INSERT INTO Change VALUES(9,'HPM',NULL,NULL,'An HPM has been changed','',NULL,NULL,NULL,@NotyDetails)
END
GO


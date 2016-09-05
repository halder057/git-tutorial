USE ENO_Notification
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'ContentVolume' AND xtype = 'U')
BEGIN
INSERT INTO ContentVolume VALUES('Single')
INSERT INTO ContentVolume VALUES('Multiple')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'DeliveryMethod' AND xtype = 'U')
BEGIN
INSERT INTO DeliveryMethod VALUES('Push')
INSERT INTO DeliveryMethod VALUES('Email')
INSERT INTO DeliveryMethod VALUES('Text')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'DeliveryFrequency' AND xtype = 'U')
BEGIN
INSERT INTO DeliveryFrequency VALUES('Instant')
INSERT INTO DeliveryFrequency VALUES('Daily')
INSERT INTO DeliveryFrequency VALUES('Weekly')
INSERT INTO DeliveryFrequency VALUES('Monthly')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'NotificationStatus' AND xtype = 'U')
BEGIN
INSERT INTO NotificationStatus VALUES('Pending Delivery')
INSERT INTO NotificationStatus VALUES('Delivered')
INSERT INTO NotificationStatus VALUES('Acknowledged')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'ChangeStatus' AND xtype = 'U')
BEGIN
INSERT INTO ChangeStatus VALUES('Pending')
INSERT INTO ChangeStatus VALUES('Approved')
INSERT INTO ChangeStatus VALUES('Discarded')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'Page' AND xtype = 'U')
BEGIN
INSERT INTO Page VALUES('PendingChanges')
INSERT INTO Page VALUES('Subscription')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'PreferenceType' AND xtype = 'U')
BEGIN
INSERT INTO PreferenceType VALUES('Application')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'NotificationCategory' AND xtype = 'U')
BEGIN
SET IDENTITY_INSERT [dbo].[NotificationCategory] ON
INSERT [dbo].[NotificationCategory] ([ID], [Name], [Mandatory], [AutoApproval], [ApplicationID]) VALUES (1, N'Data Change', 0, 0, 1)
INSERT [dbo].[NotificationCategory] ([ID], [Name], [Mandatory], [AutoApproval], [ApplicationID]) VALUES (6, N'Healthplan Management Change', 0, 0, 1)
INSERT [dbo].[NotificationCategory] ([ID], [Name], [Mandatory], [AutoApproval], [ApplicationID]) VALUES (7, N'MCMM Preferred Status Change', 0, 0, 2)
INSERT [dbo].[NotificationCategory] ([ID], [Name], [Mandatory], [AutoApproval], [ApplicationID]) VALUES (8, N'MCMM Critical Data Change', 0, 0, 2)
INSERT [dbo].[NotificationCategory] ([ID], [Name], [Mandatory], [AutoApproval], [ApplicationID]) VALUES (9, N'MCMM Other Data Change', 0, 0, 2)
INSERT [dbo].[NotificationCategory] ([ID], [Name], [Mandatory], [AutoApproval], [ApplicationID]) VALUES (10, N'MCMM Lives Change', 0, 0, 2)
SET IDENTITY_INSERT [dbo].[NotificationCategory] OFF
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'NotificationTemplateMessage' AND xtype = 'U')
BEGIN
INSERT INTO NotificationTemplateMessage(NotificationCategoryID, [Message]) VALUES(1, 'For <%PlanName%>, the <%FieldName%> value has changed from <%OldValue%> to <%NewValue%> for <%TA%>/<%Drug%> #-# <%PlanDetailLink%>') -- 1 denotes 'Data Changed' notification category
INSERT INTO NotificationTemplateMessage(NotificationCategoryID, [Message]) VALUES(1, 'For <%PlanName%>, the <%FieldName%> value has set to <%NewValue%> for <%TA%>/<%Drug%> #-# <%PlanDetailLink%>')
INSERT INTO NotificationTemplateMessage(NotificationCategoryID, [Message]) VALUES(6, 'For <%PlanName%>, the <%FieldName%> value has changed from <%OldValue%> to <%NewValue%> for <%TA%>/<%Drug%> #-# <%PlanDetailLink%>') -- 6 denotes 'Healthplan Management Changed' notification category
INSERT INTO NotificationTemplateMessage(NotificationCategoryID, [Message]) VALUES(6, 'For <%PlanName%>, the <%FieldName%> value has set to <%NewValue%> for <%TA%>/<%Drug%> #-# <%PlanDetailLink%>')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'NotificationColumn' AND xtype = 'U')
BEGIN
SET IDENTITY_INSERT [dbo].[NotificationColumn] ON
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (1, N'Place in Therapy', N'Place_in_Therapy', N'IndicationDrugDataOne', N'PRC')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (2, N'Plan Specifies for Genotype', N'Plan_Specifies_for_Genotype', N'IndicationDrugDataOne', N'HepC')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (3, N'Metavir or Equivalent Stated', N'Metavir_or_Equivalent_Stated', N'IndicationDrugDataOne', N'HepC')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (4, N'NCCN Category', N'NCCN_Category', N'IndicationDrugDataTwo', N'MM')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (5, N'Regimens Listed', N'Regimen_Listed', N'IndicationDrugDataTwo', N'MM')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (6, N'Excludes Specific Patient Population', N'Excludes_Specific_Patient_Population', N'IndicationDrugDataTwo', N'MEL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (7, N'Excludes Specific Patient Population', N'Excludes_Specific_Patient_Population', N'IndicationDrugDataTwo', N'NSCLC')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (8, N'Differentiation scale', N'Differential_Scale', N'IndicationDrugDataOne', N'IGCVID')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (9, N'CIDP Motor Neuron Damage', N'CIDP_Motor_Neuron_Damage', N'IndicationDrugDataOne', N'IGCVID')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (10, N'CIDP ADL Deterioration', N'CIDP_ADL_Deterioration', N'IndicationDrugDataOne', N'IGCVID')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (11, N'CIDP CSF Study', N'CIDP_CSF_Study', N'IndicationDrugDataOne', N'IGCVID')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (12, N'Differentiation scale', N'Differential_Scale', N'IndicationDrugDataOne', N'IGCIPD')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (13, N'CIDP Motor Neuron Damage', N'CIDP_Motor_Neuron_Damage', N'IndicationDrugDataOne', N'IGCIPD')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (14, N'CIDP ADL Deterioration', N'CIDP_ADL_Deterioration', N'IndicationDrugDataOne', N'IGCIPD')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (15, N'CIDP CSF Study', N'CIDP_CSF_Study', N'IndicationDrugDataOne', N'IGCIPD')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (16, N'Health plan Management', N'HealthPlan_Management', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (17, N'Tier Placement', N'Tier_Placement', N'IndicationDrugDataTwo', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (18, N'Step Therapy Notes', N'Step_Therapy_Notes', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (19, N'Specific Biologic Failure', N'Specific_Biologic_Failure', N'IndicationDrugDataOne', N'RA')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (20, N'Specific Biologic Failure', N'Specific_Biologic_Failure', N'IndicationDrugDataOne', N'PSO')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (21, N'Liver Function Requirements', N'Liver_Function_Requirements', N'IndicationDrugDataOne', N'Embolization')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (22, N'Notes', N'Notes1,Notes2,Notes3', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (23, N'On Formulary', N'On_Formulary', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (24, N'Step Therapy', N'Step_Therapy_Req', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (25, N'Split Fills', N'Split_Fill_Requirements', N'ParentDrug', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (26, N'Benefit Type', N'Benefit_Type', N'ParentDrug', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (27, N'Change to entry', N'Change_to_Entry', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (28, N'Reason for change', N'Reason_for_Change', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (29, N'Number of Tiers', N'Number_of_Tiers', N'IndicationDrugDataTwo', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (30, N'Health plan Management', N'Amgen_Healthplan_Management', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (31, N'Health plan Management', N'Ariad_HP_Management', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (32, N'Health plan Management', N'Bayer_Healthplan_Management', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (33, N'Health plan Management', N'BMS_Healthplan_Management', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (34, N'Health plan Management', N'DNA_Healthplan_Management', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (35, N'Health plan Management', N'Gilead_Level_of_Control', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (36, N'Health plan Management', N'Novartis_Healthplan_Management', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (37, N'Health plan Management', N'Novo_Healthplan_Management', N'IndicationDrugDataOne', N'ALL')
INSERT [dbo].[NotificationColumn] ([ID], [Display_Name], [Column_Name], [Table_Name], [Indication]) VALUES (38, N'Health plan Management', N'Regeneron_Healthplan_management', N'IndicationDrugDataOne', N'ALL')
SET IDENTITY_INSERT [dbo].[NotificationColumn] OFF
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'NotificationCategoryNotificationColumnAssoc' AND xtype = 'U')
BEGIN
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 1)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 2)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 3)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 4)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 5)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 6)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 7)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 8)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 9)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 10)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 11)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 12)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 13)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 14)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 15)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 16)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 17)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 18)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 19)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 20)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 21)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 22)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 23)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 24)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 25)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 26)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 27)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 28)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(1, 29)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 30)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 31)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 32)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 33)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 34)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 35)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 36)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 37)
INSERT INTO NotificationCategoryNotificationColumnAssoc VALUES(6, 38)
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'PlanDetailNotAvailableMessage' AND xtype = 'U')
BEGIN
INSERT INTO PlanDetailNotAvailableMessage([FieldName], [Message]) VALUES('Default', 'PA Policy Document Not Applicable')
INSERT INTO PlanDetailNotAvailableMessage([FieldName], [Message]) VALUES('Benefit Type', 'Not Applicable')
INSERT INTO PlanDetailNotAvailableMessage([FieldName], [Message]) VALUES('Tier Placement', 'Not Applicable')
INSERT INTO PlanDetailNotAvailableMessage([FieldName], [Message]) VALUES('Number of Tiers', 'Not Applicable')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'NotificationEmailTemplateColumnOrder' AND xtype = 'U')
BEGIN
INSERT INTO NotificationEmailTemplateColumnOrder([FieldName], [AppearanceOrder],[SortOrder]) VALUES('Drug', 1,1)
INSERT INTO NotificationEmailTemplateColumnOrder([FieldName], [AppearanceOrder],[SortOrder])  VALUES('TA', 2,2)
INSERT INTO NotificationEmailTemplateColumnOrder([FieldName], [AppearanceOrder],[SortOrder])  VALUES('FieldName', 3,3)
INSERT INTO NotificationEmailTemplateColumnOrder([FieldName], [AppearanceOrder],[SortOrder])  VALUES('OldValue', 4,4)
INSERT INTO NotificationEmailTemplateColumnOrder([FieldName], [AppearanceOrder],[SortOrder])  VALUES('NewValue', 5,5)
INSERT INTO NotificationEmailTemplateColumnOrder([FieldName], [AppearanceOrder],[SortOrder])  VALUES('PlanDetailLink', 6,6)
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'EligibleDomain' AND xtype = 'U')
BEGIN
INSERT INTO EligibleDomain(Domain) VALUES('zitter.com')
END
GO

IF EXISTS (SELECT * from sysobjects WHERE name = 'EligibleEmailAddress' AND xtype = 'U')
BEGIN
INSERT INTO EligibleEmailAddress(EmailAddress) VALUES('enosis.test2@gmail.com')
END
GO

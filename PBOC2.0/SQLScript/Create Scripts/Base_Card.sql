USE [FunnettStation]
GO

/****** CPU����Ϣ��******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Base_Card](
	[CardNum] [char](16) NOT NULL,
	[CardType] [varchar](2) NOT NULL,
	[ClientId] int NOT NULL,
	[CardState] [int] NOT NULL,
	[UseValidateDate] [datetime] NULL,
	[UseInvalidateDate] [datetime] NULL,
	[Plate] [nvarchar](16) NULL,
	[SelfId] [varchar](50) NULL,
	[CertificatesType] [varchar](2) NULL,
	[PersonalId] [varchar](32) NULL,
	[DriverName] [nvarchar](50) NULL,
	[DriverTel] [varchar](32) NULL,
	[VechileCategory] [nvarchar](8) NULL,
	[SteelCylinderId] [varchar](32) NULL,
	[CylinderTestDate] [datetime] NULL,
	[Remark] [nvarchar](50) NULL,
	[RechargeTotal] [decimal](18, 2) NULL,
	[ConsumeTotal] [decimal](18, 2) NULL,
	[CardBalance] [decimal](18, 2) NULL,
	[AccountBalance] [decimal](18, 2) NULL,
	[CreditsTotal] [int] NULL,
	[R_OilTimesADay] [int] NULL,
	[R_OilVolATime] [decimal](18, 2) NULL,
	[R_OilVolTotal] [decimal](18, 2) NULL,
	[R_OilEndDate] [datetime] NULL,
	[R_Plate] [bit] NULL,
	[R_Oil] [varchar](4) NULL,
	[R_RFID] [bit] NULL,
	[CylinderNum] [int] NULL,
	[FactoryNum] [char](7) NULL,
	[CylinderVolume] [int] NULL,
	[BusDistance] [varchar](10) NULL,
	[FixedGroupID] [varchar](2) NULL,
	[OperateDateTime] [datetime] NULL,
	[KeyGuid] uniqueidentifier NOT NULL,
	[UpLoadStatus] [int] NOT NULL,
 CONSTRAINT [PK_Base_Card] PRIMARY KEY CLUSTERED 
(
	[CardNum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Base_Card_Key](
	[KeyGuid] uniqueidentifier NOT NULL,
	[OrgKey] [char](32) NOT NULL,
	[MasterKey] [char](32) NOT NULL,
	[ApplicationIndex] [int] NOT NULL,
	[AppTendingKey] [char](32) NULL,
	[AppLoadKey] [char] (32) NULL,
	[AppUnlockKey] [char](32) NULL,
	[AppPinUnlockKey] [char](32) NULL,
	[AppPinResetKey] [char](32) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CardNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����ͣ�01-�����û���02-������04-Ա������06-ά�޿���11-�ӿ���21-ĸ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CardType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������λ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'ClientId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��״̬��0-������1-��ʧ��2-�Ѱ�������3-���˿���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CardState'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����Ч����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'UseValidateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ʧЧ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'UseInvalidateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���ƺţ���ΪԱ����ʱ���洢Ա���ţ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'Plate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Ա��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'SelfId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'֤������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CertificatesType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'֤����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'PersonalId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ֿ�������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'DriverName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ֿ��˵绰' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'DriverTel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'VechileCategory'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ƿ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'SteelCylinderId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ƿ��Ч��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CylinderTestDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ע' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ֵ�ܶ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'RechargeTotal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����ܶ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'ConsumeTotal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CardBalance'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�˻���δȦ����ӿ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'AccountBalance'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����ܶ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CreditsTotal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ÿ���޼��ʹ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'R_OilTimesADay'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ÿ���޼�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'R_OilVolATime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ÿ���������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'R_OilVolTotal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���ͽ�ֹ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'R_OilEndDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�޳���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'R_Plate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����Ʒ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'R_Oil'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ޱ�ǩ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'R_RFID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ƿ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CylinderNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ƿ�������ұ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'FactoryNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ƿ�ݻ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'CylinderVolume'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����·��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'BusDistance'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���㵥λ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'FixedGroupID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'OperateDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Ƭ��ԿΨһʶ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'KeyGuid'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Ƿ����ϴ���0-��1-�ǣ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card', @level2type=N'COLUMN',@level2name=N'UpLoadStatus'
GO

----------------------------------ͨ��KeyGuid����Ƭ��ԿΨһʶ���룩�ҵ���Ӧ����Կ-----------------------------------------------------------------------

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Ƭ��ԿΨһʶ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'KeyGuid'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Ƭԭʼ��Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'OrgKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'MasterKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ�ú�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'ApplicationIndex'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ��ά����Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'AppTendingKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ��Ȧ����Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'AppLoadKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ�����������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'AppUnlockKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ��PIN������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'AppPinUnlockKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ��PIN��װ��Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Card_Key', @level2type=N'COLUMN',@level2name=N'AppPinResetKey'
GO

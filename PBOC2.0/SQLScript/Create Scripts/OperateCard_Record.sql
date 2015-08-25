USE [FunnettStation]
GO

/****** ��������¼��******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[OperateCard_Record](
	[Id] [uniqueidentifier] NOT NULL,
	[InvalidCardId] [char](16) NOT NULL,
	[CardType] [varchar](2) NOT NULL,
	[ClientId] int NOT NULL,
	[UseValidateDate] [datetime] NULL,
	[UseInvalidateDate] [datetime] NULL,
	[PersonalId] [varchar](32) NULL,
	[DriverName] [nvarchar](50) NULL,
	[DriverTel] [varchar](32) NULL,
	[CardBalance] [decimal](18, 2) NULL,
	[AccountBalance] [decimal](18, 2) NULL,
	[OperateName] [nvarchar](16) NOT NULL,
	[RePublishCardId] [char](16) NULL,
	[RelatedName] [nvarchar](50) NULL,
	[RelatedPersonalId] [varchar](32) NULL,
	[RelatedTel] [varchar](32) NULL,	
	[OperateDateTime] [datetime] NOT NULL,
	[UpLoadStatus] [int] NOT NULL,
 CONSTRAINT [PK_OperateCard_Record] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ʧЧ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'InvalidCardId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����ͣ�01-�����û���02-������04-Ա������06-ά�޿���11-�ӿ���21-ĸ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'CardType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������λ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'ClientId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����Ч����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'UseValidateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ʧЧ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'UseInvalidateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'֤����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'PersonalId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ֿ�������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'DriverName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ֿ��˵绰' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'DriverTel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'CardBalance'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�˻�����λĸ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'AccountBalance'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�������ƣ���ʧ����ҡ��������˿���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'OperateName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�������ţ���������Ϊ�գ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'RePublishCardId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ͻ���������ʧ�ߡ������ߣ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'RelatedName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ͻ�֤����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'RelatedPersonalId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ͻ��绰' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'RelatedTel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'OperateDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Ƿ����ϴ���0-��1-�ǣ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OperateCard_Record', @level2type=N'COLUMN',@level2name=N'UpLoadStatus'
GO

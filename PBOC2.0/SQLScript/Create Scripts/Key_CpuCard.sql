USE [FunnettStation]
GO

/****** CPU����Կ�� ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Key_CpuCard](
	[KeyId] [int] IDENTITY(1,1) NOT NULL,
	[MasterKey] [char](32) NOT NULL,
	[MasterTendingKey] [char](32) NOT NULL,
	[InternalAuthKey] [char](32) NOT NULL,
	[InfoRemark]  [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Key_CpuCard] PRIMARY KEY CLUSTERED 
(
	[KeyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

--������Ӧ����Կ
CREATE TABLE [dbo].[Key_CARD_ADF](
	[ADFKeyId] [int] IDENTITY(1,1) NOT NULL,
	[RelatedKeyId] [int] NOT NULL,    --������Key_CpuCard���е�KeyId
	[ApplicationIndex] [int] NOT NULL,   --Ӧ�úţ�1-����Ӧ�ã�2-����Ӧ�ã�3-���Ӧ�ã�...
	[ApplicatonMasterKey] [char](32) NOT NULL,
	[ApplicationTendingKey] [char](32) NOT NULL,
	[AppInternalAuthKey] [char](32) NOT NULL,
	[PINResetKey] [char](32) NULL,
	[PINUnlockKey] [char](32) NULL,
	[ConsumerMasterKey] [char](32) NULL,
	[LoadMasterKey] [char](32) NULL,
	[TacMasterKey] [char](32) NULL,
	[UnlockUnloadKey] [char](32) NULL,
	[OverdraftKey] [char](32) NULL,
 CONSTRAINT [PK_Key_CARD_ADF] PRIMARY KEY CLUSTERED 
(
	[ADFKeyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Կ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CpuCard', @level2type=N'COLUMN',@level2name=N'KeyId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Ƭ������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CpuCard', @level2type=N'COLUMN',@level2name=N'MasterKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Ƭά����Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CpuCard', @level2type=N'COLUMN',@level2name=N'MasterTendingKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Ƭ�ڲ���֤��Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CpuCard', @level2type=N'COLUMN',@level2name=N'InternalAuthKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Կ��Ϣ(�ͻ����Ƶȱ��ڲ鿴)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CpuCard', @level2type=N'COLUMN',@level2name=N'InfoRemark'
GO

--------Ӧ����Կ��--------------

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ����Կ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'ADFKeyId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������Կ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'RelatedKeyId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ�ú�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'ApplicationIndex'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ��������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'ApplicatonMasterKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ��ά����Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'ApplicationTendingKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ���ڲ���֤��Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'AppInternalAuthKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PIN��װ��Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'PINResetKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PIN������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'PINUnlockKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'ConsumerMasterKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ȧ������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'LoadMasterKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TAC����Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'TacMasterKey'
GO
	
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������ۡ�Ȧ������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'UnlockUnloadKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�޸�͸֧�޶�����Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_CARD_ADF', @level2type=N'COLUMN',@level2name=N'OverdraftKey'
GO

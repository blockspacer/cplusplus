USE [FunnettStation]
GO

/****** PSAM����¼�� ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Psam_Card](
	[PsamId] [char](16) NOT NULL,	
	[TerminalId] [varchar](6) NOT NULL,
	[ClientId] int NOT NULL,
	[CardState] [int] NOT NULL,		
	[UseValidateDate] [datetime] NULL,
	[UseInvalidateDate] [datetime] NULL,
	[IssueCode] [varchar](8) NULL,
	[RecvCode] [varchar](8) NULL,
	[Remark] [nvarchar](50) NULL,
	[OperateDateTime] [datetime] NULL,	
	[OrgKey]       [char](32) NOT NULL,
	[PsamMasterKey]  [char](32) NOT NULL,
	[UpLoadStatus] [int] NOT NULL,
 CONSTRAINT [PK_Psam_Card] PRIMARY KEY CLUSTERED 
(
	[PsamId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����к�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'PsamId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ն˻����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'TerminalId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������λ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'ClientId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��״̬��0-��ʼ����1-���ƿ���2-����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'CardState'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����Ч����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'UseValidateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ʧЧ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'UseInvalidateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ�÷����߱�ʶ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'IssueCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ӧ�ý����߱�ʶ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'RecvCode'
GO


EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ע' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'OperateDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ƬPSAMԭʼ��Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'OrgKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PSAM������Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'PsamMasterKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Ƿ����ϴ���0-��1-�ǣ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Psam_Card', @level2type=N'COLUMN',@level2name=N'UpLoadStatus'
GO



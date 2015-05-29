USE [FunnettStation]
GO

/****** �ҿ���¼ ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Data_LockGrayRecord](
	[CardNum] [char](16) NOT NULL,
	[GrayTime] datetime NOT NULL,
	[LockMoney] [decimal](18, 2) NOT NULL,
	[TerminalId] [varchar](12) NOT NULL,
	[IsLock] [bit] NULL,
 CONSTRAINT [PK_LockGrayRecord] PRIMARY KEY CLUSTERED 
(
	[CardNum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ҿ�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_LockGrayRecord', @level2type=N'COLUMN',@level2name=N'CardNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ҿ�ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_LockGrayRecord', @level2type=N'COLUMN',@level2name=N'GrayTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ҽ��׽��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_LockGrayRecord', @level2type=N'COLUMN',@level2name=N'LockMoney'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ն˻����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_LockGrayRecord', @level2type=N'COLUMN',@level2name=N'TerminalId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ҿ���ǣ�0-��ҿ�,1-�ҿ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_LockGrayRecord', @level2type=N'COLUMN',@level2name=N'IsLock'
GO



USE [FunnettStation]
GO

/****** ����˵����� ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SC_MenuItem](
	[FID] [int] IDENTITY(1,1) NOT NULL,
	[FIndex] [int] NULL,
	[FName] [varchar](20) NULL,
	[FCaption] [varchar](50) NULL,
	[FSqlType] [int] NULL,
	[FGunno] [int] NULL,
	[FOperatorCard] [int] NULL,
	[FCompanyid] [int] NULL,
	[FDatetime] [int] NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�˵�����λ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SC_MenuItem', @level2type=N'COLUMN',@level2name=N'FIndex'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SC_MenuItem', @level2type=N'COLUMN',@level2name=N'FName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SC_MenuItem', @level2type=N'COLUMN',@level2name=N'FCaption'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ű�ִ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SC_MenuItem', @level2type=N'COLUMN',@level2name=N'FSqlType'
GO



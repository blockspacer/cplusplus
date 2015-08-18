USE [FunnettStation]
GO

/******��ҿ���¼��******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Data_GreyCardRecord](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[StationNo] [char](8) NOT NULL,
	[GunNo] [int] NOT NULL,
	[CardNo] [char](16) NOT NULL,
	[TradeDateTime] [datetime] NOT NULL,
	[GrayPrice] [decimal](18, 2) NOT NULL,
	[GrayGas] [decimal](18, 2) NOT NULL,
	[GrayMoney] [decimal](18, 2) NOT NULL,
	[ResidualAmount] [decimal](18, 2) NOT NULL,	
	[Operator] int NOT NULL,
	[OperateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Data_GreyCardRecord] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ҿ���¼ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'RecordId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'վ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'StationNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ǹ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'GunNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'CardNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'TradeDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Ҽ�¼����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'GrayPrice'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'GrayGas'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'GrayMoney'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'ResidualAmount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����Ա' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'Operator'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_GreyCardRecord', @level2type=N'COLUMN',@level2name=N'OperateTime'
GO



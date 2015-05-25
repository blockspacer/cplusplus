USE [FunnettStation]
GO

/****** ĸ��ת�˼�¼�� ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Data_TransferRecord](
	[RunningNum] [int] IDENTITY(1,1) NOT NULL,
	[MainCardNum] [char](20) NULL,
	[BalBeforeDeduct] [decimal](18, 2) NULL,
	[DestCardNum] [char](20) NULL,
	[TransferVal] [decimal](18, 2) NULL,
	[BalAfterDeduct] [decimal](18, 2) NULL,
	[DestCardAccountBal] [decimal](18, 2) NULL,
	[OperateDateTime] [datetime] NULL,
	[OperatorId] [varchar](50) NULL,
 CONSTRAINT [PK_Data_MotherCardRecord] PRIMARY KEY CLUSTERED 
(
	[RunningNum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ˮ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'RunningNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ĸ������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'MainCardNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ת��ǰ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'BalBeforeDeduct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ӿ�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'DestCardNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ת������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'TransferVal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ת�˺�ĸ�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'BalAfterDeduct'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ת�˺��ӿ��˻����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'DestCardAccountBal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'OperateDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����Ա����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_TransferRecord', @level2type=N'COLUMN',@level2name=N'OperatorId'
GO



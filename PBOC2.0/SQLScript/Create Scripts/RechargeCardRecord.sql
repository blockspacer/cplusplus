USE [FunnettStation]
GO

/****** ��ֵ��¼�� ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Data_RechargeCardRecord](
	[RunningNum] [int] IDENTITY(1,1) NOT NULL,
	[CardNum] [char](16) NULL,
	[OperateType] [nvarchar](2) NULL,
	[ForwardBalance] [decimal](18, 2) NULL,
	[RechargeValue] [decimal](18, 2) NULL,
	[PreferentialVal] [decimal](18, 2) NULL,
	[ReceivedVal] [decimal](18, 2) NULL,
	[CurrentBalance] [decimal](18, 2) NULL,
	[RechargeDateTime] [datetime] NULL,
	[OperatorId] [int] NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[ShiftNum] [varchar](10) NULL,
	[UpLoadStatus] [int] NOT NULL,
 CONSTRAINT [PK_RechargeCardRecord] PRIMARY KEY CLUSTERED 
(
	[RunningNum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ˮ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'RunningNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'CardNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�������ͣ���ֵ��ת��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'OperateType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ֵǰ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'ForwardBalance'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ֵ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'RechargeValue'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Żݽ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'PreferentialVal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ʵ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'ReceivedVal'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ֵ�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'CurrentBalance'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ֵʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'RechargeDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����Ա���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'OperatorId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���ʽ(�ֽ����п�)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'PaymentMethod'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'yyyyMMddnn(nnΪ���)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'ShiftNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Ƿ��Ѿ��ϴ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_RechargeCardRecord', @level2type=N'COLUMN',@level2name=N'UpLoadStatus'
GO


USE [FunnettStation]
GO

/****** ���ݿ�汾 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Funnett_Version](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SoftwareVersion] [varchar](32) NOT NULL,
	[UpgradeTime]   [datetime] NOT NULL,
	[DbVersion] [varchar](32) NOT NULL,
	[DbUpgradeTime]   [datetime] NOT NULL,
	[Info] [nvarchar](256) NULL,	
CONSTRAINT [PK_Funnett_Version] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����汾' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Funnett_Version', @level2type=N'COLUMN',@level2name=N'SoftwareVersion'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�������ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Funnett_Version', @level2type=N'COLUMN',@level2name=N'UpgradeTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���ݿ�汾' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Funnett_Version', @level2type=N'COLUMN',@level2name=N'DbVersion'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���ݿ�����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Funnett_Version', @level2type=N'COLUMN',@level2name=N'DbUpgradeTime'
GO

--ʹ�ð�װ���������ʱ�������������ʱ�䣻 ���ݿ�����ʱ��ֻ�ڰ�װ���ݿ���ֶ�����ʱ����
declare @curTime datetime
set @curTime = GETDATE()
insert into Funnett_Version values('1.07.10.26','2015-10-28 08:15:10', '1.0.0.1', '2015-10-28 08:15:10', '���ݿ��һ��');
insert into Funnett_Version values('1.07.10.28','2015-10-29 14:15:10', '1.0.0.2', '2015-10-29 14:15:10', '����funnett_version,SC_MonitorConfig����FGasVariety');
insert into Funnett_Version values('1.07.10.30','2015-10-30 15:25:30', '1.0.0.3','2015-10-30 15:25:30', '�洢����Pro_SC_ConsumerDetail�޸�');
insert into Funnett_Version values('1.07.10.30',@curTime, '1.0.0.4', @curTime, '�洢����PROC_PublishPsamCard�޸�,��Psam_Card�ṹ�޸�');
GO
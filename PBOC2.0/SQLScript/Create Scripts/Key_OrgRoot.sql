USE [FunnettStation]
GO

/****** ��ʼ����Կ�� ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Key_OrgRoot](
	[KeyId] [int] IDENTITY(1,1) NOT NULL,
	[OrgKey] [char](32) NOT NULL,
	[KeyType] [int] NOT NULL,
	[InfoRemark]  [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Key_OrgRoot] PRIMARY KEY CLUSTERED 
(
	[KeyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Կ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_OrgRoot', @level2type=N'COLUMN',@level2name=N'KeyId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ʼ����Կ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_OrgRoot', @level2type=N'COLUMN',@level2name=N'OrgKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����(0-CPU����1-PSAM��, 2-ͨ��)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_OrgRoot', @level2type=N'COLUMN',@level2name=N'KeyType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��Կ��Ϣ(�ͻ����Ƶȱ��ڲ鿴)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Key_OrgRoot', @level2type=N'COLUMN',@level2name=N'InfoRemark'
GO

insert into Key_OrgRoot values('404142434445464748494A4B4C4D4E4F',2,N'������Կ');
GO
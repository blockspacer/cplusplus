USE [FunnettStation]
GO

/****** �ƿ�������־ ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Log_PublishCard](
	[RunningNum] [int] IDENTITY(1,1) NOT NULL,
	[WhenHappen] [datetime] NOT NULL,
	[LogContent] [nvarchar](1024) NOT NULL,	
	[ClientId] [int] NOT NULL,
	[CardNum] [char](16) NOT NULL,
 CONSTRAINT [PK_Log_PublishCard] PRIMARY KEY CLUSTERED 
(
	[RunningNum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


insert into Log_PublishCard values('2015-07-10 15:00:00',
									N'���ݿ�ṹ�޸ģ�����Ȧ����Կ��,�洢�����޸ģ������Ʊ仯��Ȧ����Կ��Ȧ����Կ�����������Կ��',
									0,
									'0000000000000000');
GO
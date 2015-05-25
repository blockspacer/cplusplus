USE [FunnettStation]
GO

/****** վ����Ϣ�� ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Base_Station](
	[StationId] [char](4) NOT NULL,
	[StationName] [nvarchar](50) NOT NULL,
	[Prov] [char](2) NOT NULL,
	[City] [char](4) NOT NULL,
	[SuperiorId] [char](4) NOT NULL,	
	[ClientId] [int] NOT NULL,
	[IsSelfStation] [bit] NOT NULL,
 CONSTRAINT [PK__Base_Station] PRIMARY KEY CLUSTERED 
(
	[StationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����վ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Station', @level2type=N'COLUMN',@level2name=N'StationId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����վ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Station', @level2type=N'COLUMN',@level2name=N'StationName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ʡ����BCD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Station', @level2type=N'COLUMN',@level2name=N'Prov'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�д���BCD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Station', @level2type=N'COLUMN',@level2name=N'City'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ϼ���λ��˾����BCD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Station', @level2type=N'COLUMN',@level2name=N'SuperiorId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������λID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Station', @level2type=N'COLUMN',@level2name=N'ClientId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Ƿ��Խ�վ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Station', @level2type=N'COLUMN',@level2name=N'IsSelfStation'
GO


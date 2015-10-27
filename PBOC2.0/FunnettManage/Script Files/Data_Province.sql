USE [FunnettStation]
GO

/****** ����վ����ʡ���� ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Data_Province](
	[ProvinceCode] [char](2) NOT NULL,
	[ProvinceName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Data_Province] PRIMARY KEY CLUSTERED 
(
	[ProvinceCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ʡ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_Province', @level2type=N'COLUMN',@level2name=N'ProvinceCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ʡ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Data_Province', @level2type=N'COLUMN',@level2name=N'ProvinceName'
GO
     

insert into Data_Province values('11','������');
insert into Data_Province values('12','�����');
insert into Data_Province values('13','�ӱ�ʡ');
insert into Data_Province values('14','ɽ��ʡ');
insert into Data_Province values('15','���ɹ�������');
insert into Data_Province values('21','����ʡ');
insert into Data_Province values('22','����ʡ');
insert into Data_Province values('23','������');
insert into Data_Province values('31','�Ϻ���');
insert into Data_Province values('32','����ʡ');
insert into Data_Province values('33','�㽭ʡ');
GO

insert into Data_Province values('34','����ʡ');
insert into Data_Province values('35','����ʡ');
insert into Data_Province values('36','����ʡ');
insert into Data_Province values('37','ɽ��ʡ');
insert into Data_Province values('41','����ʡ');
insert into Data_Province values('42','����ʡ');
insert into Data_Province values('43','����ʡ');
insert into Data_Province values('44','�㶫ʡ');
insert into Data_Province values('45','����׳��������');
insert into Data_Province values('46','����ʡ');
insert into Data_Province values('50','������');
GO

insert into Data_Province values('51','�Ĵ�ʡ');
insert into Data_Province values('52','����ʡ');
insert into Data_Province values('53','����ʡ');
insert into Data_Province values('54','����������');
insert into Data_Province values('61','����ʡ');
insert into Data_Province values('62','����ʡ');
insert into Data_Province values('63','�ຣʡ');
insert into Data_Province values('64','���Ļ���������');
insert into Data_Province values('65','�½�ά���������');
insert into Data_Province values('71','̨��ʡ');
insert into Data_Province values('81','����ر�������');
insert into Data_Province values('82','�����ر�������');
GO


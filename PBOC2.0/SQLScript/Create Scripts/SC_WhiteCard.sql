USE [FunnettStation]
GO

/****** Object:  Table [dbo].[SC_WhiteCard]    Script Date: 08/07/2015 08:45:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SC_WhiteCard](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FUserCardNo] [varchar](16) NOT NULL,
 CONSTRAINT [PK_SC_WHITECARD] PRIMARY KEY CLUSTERED 
(
	[FUserCardNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



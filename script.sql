--create database mdb;


USE [mdb]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 09.03.2023 1:12:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[roles](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name_roles] [nvarchar](20) NOT NULL,
 CONSTRAINT [pk_roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[student]    Script Date: 09.03.2023 1:12:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON

/****** Object:  Table [dbo].[tovar]    Script Date: 09.03.2023 1:12:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tovar](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name_t] [nvarchar](20) NOT NULL,
	[prise] [money] NOT NULL,
	[articul] [nvarchar](20) NOT NULL,
	[photo] [nvarchar](max) NULL,
	[quantity] [int] NULL,
 CONSTRAINT [pk_id_tovar] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 09.03.2023 1:12:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[id_roles] [bigint] NULL,
	[login_user] [nvarchar](20) NOT NULL,
	[pass] [nvarchar](20) NOT NULL,
 CONSTRAINT [pk_users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [fk_id_roles] FOREIGN KEY([id_roles])
REFERENCES [dbo].[roles] ([id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [fk_id_roles]
GO

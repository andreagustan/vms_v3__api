USE [VMS_V3]
GO
/****** Object:  Table [dbo].[M_CustomerDetail]    Script Date: 17/11/2023 11:03:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_CustomerDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[customerID] [nvarchar](100) NULL,
	[customerName] [nvarchar](100) NULL,
	[subCustomerID] [nvarchar](100) NULL,
	[subCustomerName] [nvarchar](200) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](100) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](100) NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedDate] [datetime] NULL,
	[DeletedUser] [nvarchar](100) NULL,
 CONSTRAINT [PK__M_Custom__3214EC0707AC3A6A] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_PriceListVoucher]    Script Date: 17/11/2023 11:03:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_PriceListVoucher](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[supplierID] [nvarchar](100) NULL,
	[itemID] [nvarchar](100) NULL,
	[beginQty] [int] NULL,
	[endQty] [int] NULL,
	[price] [float] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](100) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](100) NULL,
	[DeletedDate] [datetime] NULL,
	[DeletedUser] [nvarchar](100) NULL,
 CONSTRAINT [PK__M_PriceL__3214EC0739CD6E79] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_SystemConfig]    Script Date: 17/11/2023 11:03:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_SystemConfig](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NULL,
	[SystemCategory] [varchar](256) NOT NULL,
	[SystemSubCategory] [varchar](256) NOT NULL,
	[SystemCode] [varchar](40) NOT NULL,
	[SystemValue] [varchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateUser] [varchar](500) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [varchar](500) NULL,
	[UpdateDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedUser] [varchar](500) NULL,
	[DeletedDate] [datetime] NULL,
 CONSTRAINT [PK_M_SystemConfig] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemLogs]    Script Date: 17/11/2023 11:03:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Module] [nvarchar](250) NULL,
	[Action] [nvarchar](max) NULL,
	[Request] [nvarchar](max) NULL,
	[Response] [nvarchar](max) NULL,
	[Note1] [nvarchar](max) NULL,
	[ErrorLog] [nvarchar](max) NULL,
	[StatusLog] [nvarchar](max) NULL,
	[CreateUser] [nvarchar](500) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](500) NULL,
	[UpdateDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedUser] [varchar](500) NULL,
	[DeletedDate] [datetime] NULL,
 CONSTRAINT [PK_SystemLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[T_VoucherDetail]    Script Date: 17/11/2023 11:03:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_VoucherDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[refId] [nvarchar](50) NULL,
	[itemID] [nvarchar](50) NULL,
	[startNo] [nvarchar](50) NULL,
	[endNo] [nvarchar](50) NULL,
	[expDateVoucher] [date] NULL,
	[qty] [int] NULL,
	[sources] [nvarchar](20) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](100) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](100) NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedDate] [datetime] NULL,
	[DeletedUser] [nvarchar](100) NULL,
 CONSTRAINT [PK__T_Vouche__3214EC0747FB5CC0] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[M_CustomerDetail] ON 

INSERT [dbo].[M_CustomerDetail] ([Id], [customerID], [customerName], [subCustomerID], [subCustomerName], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (1, N'CRM2017030035', N'PERTAMINA RETAIL, PT - EVENT (PELUMAS)', N'1512521513', N'test sub ptpr', CAST(N'2023-11-01T14:25:04.733' AS DateTime), N'dev.andre', CAST(N'2023-11-10T11:03:22.720' AS DateTime), N'umar.indocyber', 0, NULL, NULL)
INSERT [dbo].[M_CustomerDetail] ([Id], [customerID], [customerName], [subCustomerID], [subCustomerName], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (2, N'CRM2019040077', N'17UQ - PLUS ALTERNATIF CIBUBUR 3', N'214214', N'plus detail', CAST(N'2023-11-01T14:31:23.820' AS DateTime), N'dev.andre', CAST(N'2023-11-07T14:23:25.123' AS DateTime), N'dev.andre', 1, CAST(N'2023-11-10T11:03:22.737' AS DateTime), N'umar.indocyber')
INSERT [dbo].[M_CustomerDetail] ([Id], [customerID], [customerName], [subCustomerID], [subCustomerName], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (3, N'cccc', N'cccc', N'cccc', N'cccc', CAST(N'2023-11-07T14:12:36.460' AS DateTime), N'dev.andre', CAST(N'2023-11-10T11:09:04.117' AS DateTime), N'umar.indocyber', 1, CAST(N'2023-11-10T11:10:00.837' AS DateTime), N'umar.indocyber')
INSERT [dbo].[M_CustomerDetail] ([Id], [customerID], [customerName], [subCustomerID], [subCustomerName], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (4, N'aaaa', N'aaaa', N'aaaa', N'aaaa', CAST(N'2023-11-10T11:03:22.720' AS DateTime), N'umar.indocyber', NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[M_CustomerDetail] ([Id], [customerID], [customerName], [subCustomerID], [subCustomerName], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (5, N'bbbb', N'bbbb', N'bbbb', N'bbbb', CAST(N'2023-11-10T11:07:55.583' AS DateTime), N'umar.indocyber', NULL, NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[M_CustomerDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[M_PriceListVoucher] ON 

INSERT [dbo].[M_PriceListVoucher] ([Id], [supplierID], [itemID], [beginQty], [endQty], [price], [CreateDate], [CreateUser], [IsDeleted], [UpdateDate], [UpdateUser], [DeletedDate], [DeletedUser]) VALUES (1, N'PP0022', N'21201001', 1, 1000, 2500.5, CAST(N'2023-10-26T11:02:43.000' AS DateTime), N'dev.andre', 0, CAST(N'2023-11-01T10:59:07.080' AS DateTime), N'dev.andre', NULL, NULL)
INSERT [dbo].[M_PriceListVoucher] ([Id], [supplierID], [itemID], [beginQty], [endQty], [price], [CreateDate], [CreateUser], [IsDeleted], [UpdateDate], [UpdateUser], [DeletedDate], [DeletedUser]) VALUES (2, N'PP0022', N'21201001', 1001, 10000, 1200.5, CAST(N'2023-10-27T11:13:25.390' AS DateTime), N'dev.andre', 0, CAST(N'2023-11-02T09:30:05.707' AS DateTime), N'dev.andre', NULL, NULL)
INSERT [dbo].[M_PriceListVoucher] ([Id], [supplierID], [itemID], [beginQty], [endQty], [price], [CreateDate], [CreateUser], [IsDeleted], [UpdateDate], [UpdateUser], [DeletedDate], [DeletedUser]) VALUES (3, N'PP0021', N'21201001', 1, 1000, 2000, CAST(N'2023-11-02T09:24:20.760' AS DateTime), N'dev.andre', 1, CAST(N'2023-11-10T15:43:13.083' AS DateTime), N'umar.indocyber', CAST(N'2023-11-15T11:32:39.973' AS DateTime), N'umar.indocyber')
INSERT [dbo].[M_PriceListVoucher] ([Id], [supplierID], [itemID], [beginQty], [endQty], [price], [CreateDate], [CreateUser], [IsDeleted], [UpdateDate], [UpdateUser], [DeletedDate], [DeletedUser]) VALUES (6, N'PP0021', N'21201001', 1001, 2500, 1800.5, CAST(N'2023-11-10T15:43:13.083' AS DateTime), N'umar.indocyber', 0, NULL, NULL, NULL, NULL)
INSERT [dbo].[M_PriceListVoucher] ([Id], [supplierID], [itemID], [beginQty], [endQty], [price], [CreateDate], [CreateUser], [IsDeleted], [UpdateDate], [UpdateUser], [DeletedDate], [DeletedUser]) VALUES (7, N'PP0021', N'21201001', 2501, 3000, 1500.123, CAST(N'2023-11-10T15:43:13.083' AS DateTime), N'umar.indocyber', 0, CAST(N'2023-11-15T11:32:39.973' AS DateTime), N'umar.indocyber', NULL, NULL)
INSERT [dbo].[M_PriceListVoucher] ([Id], [supplierID], [itemID], [beginQty], [endQty], [price], [CreateDate], [CreateUser], [IsDeleted], [UpdateDate], [UpdateUser], [DeletedDate], [DeletedUser]) VALUES (8, N'aaaa', N'aaaa', 1, 1000, 200.55, CAST(N'2023-11-15T11:06:06.390' AS DateTime), N'umar.indocyber', 0, CAST(N'2023-11-15T11:07:17.567' AS DateTime), N'umar.indocyber', CAST(N'2023-11-15T11:08:02.863' AS DateTime), N'umar.indocyber')
INSERT [dbo].[M_PriceListVoucher] ([Id], [supplierID], [itemID], [beginQty], [endQty], [price], [CreateDate], [CreateUser], [IsDeleted], [UpdateDate], [UpdateUser], [DeletedDate], [DeletedUser]) VALUES (9, N'bbbb', N'bbbb', 1, 100, 500.58, CAST(N'2023-11-15T11:32:39.957' AS DateTime), N'umar.indocyber', 0, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[M_PriceListVoucher] OFF
GO
SET IDENTITY_INSERT [dbo].[M_SystemConfig] ON 

INSERT [dbo].[M_SystemConfig] ([Id], [Name], [SystemCategory], [SystemSubCategory], [SystemCode], [SystemValue], [Description], [CreateUser], [CreateDate], [UpdateUser], [UpdateDate], [IsDeleted], [DeletedUser], [DeletedDate]) VALUES (1, N'Common', N'Secret', N'Token', N'S1', N'my top secret key', N'Secret Token JWT Bright API', N'umar.indocyber', CAST(N'2023-11-13T14:32:29.333' AS DateTime), NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[M_SystemConfig] ([Id], [Name], [SystemCategory], [SystemSubCategory], [SystemCode], [SystemValue], [Description], [CreateUser], [CreateDate], [UpdateUser], [UpdateDate], [IsDeleted], [DeletedUser], [DeletedDate]) VALUES (2, N'Common', N'Secret', N'Token', N'S2', N'zcTH8rYj3ixTUjkYqNDKbaAHEww12Y13e12Y13e', N'Secret Token JWT Public API', N'umar.indocyber', CAST(N'2023-11-14T11:02:49.060' AS DateTime), NULL, NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[M_SystemConfig] OFF
GO
SET IDENTITY_INSERT [dbo].[T_VoucherDetail] ON 

INSERT [dbo].[T_VoucherDetail] ([Id], [refId], [itemID], [startNo], [endNo], [expDateVoucher], [qty], [sources], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (6, N'SL-SO-230100004', N'21201001', N'VMS2300001abcd', N'VMS2300020abcd', NULL, 20, NULL, CAST(N'2023-10-18T10:48:31.387' AS DateTime), N'dev.andre', NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[T_VoucherDetail] ([Id], [refId], [itemID], [startNo], [endNo], [expDateVoucher], [qty], [sources], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (7, N'JK31008-DO-230100001', N'00100537', N'VMS2300001abcd', N'VMS2300041abcd', NULL, 41, NULL, CAST(N'2023-10-18T11:22:22.797' AS DateTime), N'dev.andre', NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[T_VoucherDetail] ([Id], [refId], [itemID], [startNo], [endNo], [expDateVoucher], [qty], [sources], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (8, N'SL-GR-230100002', N'00100537', N'VMS2300001abcd', N'VMS2300010abcd', NULL, 10, NULL, CAST(N'2023-10-18T13:22:45.040' AS DateTime), N'dev.andre', NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[T_VoucherDetail] ([Id], [refId], [itemID], [startNo], [endNo], [expDateVoucher], [qty], [sources], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (9, N'HO-PO-230100001', N'21201001', N'vms2300001bcda', N'vms2300010bcda', NULL, 10, NULL, CAST(N'2023-10-24T15:23:49.490' AS DateTime), N'dev.andre', NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[T_VoucherDetail] ([Id], [refId], [itemID], [startNo], [endNo], [expDateVoucher], [qty], [sources], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (10, N'HO-PO-230100002', N'00100537', N'vms2300101bcda', N'vms2300800bcda', CAST(N'2023-10-01' AS Date), 700, NULL, CAST(N'2023-10-31T09:30:21.707' AS DateTime), N'dev.andre', NULL, NULL, 1, CAST(N'2023-11-17T10:23:13.027' AS DateTime), N'umar.indocyber')
INSERT [dbo].[T_VoucherDetail] ([Id], [refId], [itemID], [startNo], [endNo], [expDateVoucher], [qty], [sources], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (11, N'aaaa', N'aaaa', N'aaaa', N'aaaa', CAST(N'2023-11-18' AS Date), 20, N'aaaa', CAST(N'2023-11-17T10:16:29.550' AS DateTime), N'umar.indocyber', CAST(N'2023-11-17T10:23:13.027' AS DateTime), N'umar.indocyber', 0, CAST(N'2023-11-17T10:18:38.020' AS DateTime), N'umar.indocyber')
INSERT [dbo].[T_VoucherDetail] ([Id], [refId], [itemID], [startNo], [endNo], [expDateVoucher], [qty], [sources], [CreateDate], [CreateUser], [UpdateDate], [UpdateUser], [IsDeleted], [DeletedDate], [DeletedUser]) VALUES (12, N'cccc', N'cccc', N'cccc', N'cccc', CAST(N'2023-11-18' AS Date), 20, N'cccc', CAST(N'2023-11-17T10:23:13.027' AS DateTime), N'umar.indocyber', NULL, NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[T_VoucherDetail] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_M_SystemConfig]    Script Date: 17/11/2023 11:03:02 ******/
ALTER TABLE [dbo].[M_SystemConfig] ADD  CONSTRAINT [UC_M_SystemConfig] UNIQUE NONCLUSTERED 
(
	[SystemCategory] ASC,
	[SystemSubCategory] ASC,
	[SystemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[M_CustomerDetail] ADD  CONSTRAINT [DF_M_CustomerDetail_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[M_PriceListVoucher] ADD  CONSTRAINT [DF_M_PriceListVoucher_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[M_SystemConfig] ADD  CONSTRAINT [DF_M_SystemConfig_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SystemLogs] ADD  CONSTRAINT [DF_SystemLogs_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[T_VoucherDetail] ADD  CONSTRAINT [DF_T_VoucherDetail_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

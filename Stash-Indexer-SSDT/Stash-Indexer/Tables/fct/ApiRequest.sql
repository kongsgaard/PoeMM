﻿CREATE TABLE [fct].[ApiRequest]
(
	[DW_SK_ApiRequest]	INT NOT NULL PRIMARY KEY,
	[Change_Start]		Nvarchar(100) NOT NULL,
	[Change_End]		Nvarchar(100) NOT NULL,
	[FilePath]			Nvarchar(256) NOT NULL
)

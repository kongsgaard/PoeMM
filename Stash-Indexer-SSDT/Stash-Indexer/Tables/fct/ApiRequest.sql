CREATE TABLE [fct].[ApiRequest]
(
	[DW_SK_ApiRequest]	INT NOT NULL PRIMARY KEY,
	[Shard]				INT NOT NULL,
	[Change_Start]		INT NOT NULL,
	[Change_End]		INT NOT NULL,
	[FilePath]			Nvarchar(256) NOT NULL
)

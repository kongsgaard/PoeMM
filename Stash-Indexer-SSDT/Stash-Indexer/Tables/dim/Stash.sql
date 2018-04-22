CREATE TABLE [dim].[Stash]
(
	[DW_SK_Stash]	INT NOT NULL PRIMARY KEY,
	[StashID]		Nvarchar(64) NOT NULL,
	[League]		Nvarchar(64) NOT NULL,
	[Type]			Nvarchar(64) NOT NULL
)

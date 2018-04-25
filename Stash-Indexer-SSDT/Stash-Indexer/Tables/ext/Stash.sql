CREATE TABLE [ext].[Stash]
(
	[FromFilePath]	Nvarchar(256) NOT NULL,
	[StashID]		Nvarchar(64) NOT NULL,
	[League]		Nvarchar(64) NOT NULL,
	[Type]			Nvarchar(64) NOT NULL,
	[AccountName]   Nvarchar(100) NOT NULL
)

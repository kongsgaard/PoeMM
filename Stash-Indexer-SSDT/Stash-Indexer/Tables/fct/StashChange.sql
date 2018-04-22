CREATE TABLE [fct].[StashChange]
(
	[DW_SK_StashChange] INT NOT NULL PRIMARY KEY,
	[DW_SK_ApiRequest]	INT NOT NULL,
	[DW_SK_Stash]		INT NOT NULL,
)

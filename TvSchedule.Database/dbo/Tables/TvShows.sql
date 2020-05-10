﻿CREATE TABLE [dbo].[TvShows]
(
	Id INT NOT NULL IDENTITY PRIMARY KEY,
	Name NVARCHAR(128),
	Description NVARCHAR(1024),
	ImageUrl NVARCHAR(1024),
	StartDateUtc DATETIME2,
	ChannelId INT,
	CreatedUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	UpdatedUtc DATETIME2,
	CONSTRAINT FK_TvShows_Channels FOREIGN KEY (ChannelId)  REFERENCES Channels (Id)
)

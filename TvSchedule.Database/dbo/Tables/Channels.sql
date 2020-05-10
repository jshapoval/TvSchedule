CREATE TABLE [dbo].[Channels]
(
	Id INT NOT NULL IDENTITY PRIMARY KEY,
	Name NVARCHAR(128),
	Description NVARCHAR(512),
	CreatedUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	UpdateShowsLockSessionId NVARCHAR(256),
    UpdateShowsLockExpireUtc DATETIME2 NULL,
	UpdatedUtc DATETIME2,
	LockSessionId NVARCHAR(256),
	LockExpirationUtc DATETIME2
)

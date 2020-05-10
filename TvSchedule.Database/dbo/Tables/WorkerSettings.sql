CREATE TABLE [dbo].[WorkerSettings] (
    [Id]    INT             IDENTITY (1, 1) NOT NULL,
    [Key]   NVARCHAR (128)  NULL,
    [Value] NVARCHAR (1024) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


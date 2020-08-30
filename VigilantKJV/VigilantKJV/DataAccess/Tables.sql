/* Disable foreign keys */
PRAGMA foreign_keys = 'off';

/* Begin transaction */
BEGIN;

/* Database properties */
PRAGMA auto_vacuum = 0;
PRAGMA encoding = 'UTF-8';
PRAGMA page_size = 4096;

/* Drop table [Bible] */
DROP TABLE IF EXISTS [main].[Bible];

/* Table structure [Bible] */
CREATE TABLE [main].[Bible](
   [Id] INTEGER PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT NOT NULL UNIQUE, 
  [Label] varchar, 
  [Version] nvarchar, 
  [Information] nvarchar);







DROP TABLE IF EXISTS [main].[Testament];

/* Table structure [Chapters] */
CREATE TABLE [Testament](
  [Id] INTEGER PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT NOT NULL UNIQUE, 
  [Information] NVARCHAR(5000), 
  [TestamentName] NVARCHAR(50));

  
  
  
  
  
  
  
  
  
  
  
/* Drop table [Books] */
DROP TABLE IF EXISTS [main].[Books];

CREATE TABLE [Books](
  [Id] INTEGER PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT NOT NULL UNIQUE, 
  [TestamentId] string NOT NULL, 
  [BookName] nvarchar NOT NULL, 
  [Ordinal] int NOT NULL, 
  [Information] nvarchar);

CREATE UNIQUE INDEX [Books_Name] ON [Books]([BookName]);















/* Drop table [Chapters] */
DROP TABLE IF EXISTS [main].[Chapters];

CREATE TABLE [Chapters](
  [Id] INTEGER CONSTRAINT [PK_Chapters] PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT NOT NULL, 
  [BookId] int NOT NULL CONSTRAINT [FK_Chapters_Books_BookId] REFERENCES [Books]([Id]) ON DELETE CASCADE, 
  [Number] tinyint NOT NULL, 
  [Information] nvarchar);

CREATE UNIQUE INDEX [Chapters_Number_BookId]
ON [Chapters](
  [Number], 
  [BookId]);

CREATE INDEX [IX_Chapters_BookId] ON [Chapters]([BookId]);






/* Drop table [Verses] */
DROP TABLE IF EXISTS [main].[Verses];

CREATE TABLE [Verses](
  [Id] INTEGER CONSTRAINT [PK_Verses] PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT NOT NULL, 
  [Text] varchar NOT NULL, 
  [Number] int NOT NULL, 
  [BookId] int NOT NULL CONSTRAINT [FK_Verses_Books_BookId] REFERENCES [Books]([Id]) ON DELETE CASCADE, 
  [ChapterId] int NOT NULL CONSTRAINT [FK_Verses_Chapters_ChapterId] REFERENCES [Chapters]([Id]) ON DELETE CASCADE, 
  [IsMemorized] bit NOT NULL, 
  [LastRecited] datetime NOT NULL, 
  [Information] nvarchar);

CREATE INDEX [IX_Verses_BookId] ON [Verses]([BookId]);

CREATE UNIQUE INDEX [Verses_ChapterId_Number]
ON [Verses](
  [ChapterId], 
  [Number]);

CREATE INDEX [IX_Verses_ChapterId] ON [Verses]([ChapterId]);


/* Commit transaction */
COMMIT;

/* Enable foreign keys */
PRAGMA foreign_keys = 'on';


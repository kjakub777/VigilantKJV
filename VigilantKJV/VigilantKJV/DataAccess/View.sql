 
   
/* Drop view [BookNameChapter] */
DROP VIEW IF EXISTS [main].[BookNameChapter];

/* View structure [BookNameChapter] */
CREATE VIEW [main].[BookNameChapter]
AS
SELECT 
       [bookname], 
       [number], 
       [c].[id] AS [chapterid], 
       [b].[id] AS [bookid]
FROM   [books] [b]
       INNER JOIN [chapters] [c] ON [b].[id] = [c].[BookId];

/* Drop view [TextKey] */
DROP VIEW IF EXISTS [main].[TextKey];

/* View structure [TextKey] */
CREATE VIEW [main].[TextKey]
AS
SELECT 
       [v].[id] AS [vid], 
       [b].[id] AS [bid], 
    --   replace([b].[BookName],' ','') || [c].[Number] || ':' || [v].[Number] AS [Key]
       [b].[BookName] || [c].[Number] || ':' || [v].[Number] AS [Key]
FROM   [verses] [v]
       INNER JOIN [chapters] [c] ON [c].[id] = [v].[ChapterId]
       INNER JOIN [books] [b] ON [b].[id] = [c].[BookId];


DROP VIEW IF EXISTS [main].[MemorizedVerseDateGroups];

CREATE VIEW [MemorizedVerseDateGroups]
AS
SELECT *
FROM   [verses] [v]
WHERE  STRFTIME ('%m-%d-%Y', DATETIME ([v].[lastrecited])) IN (SELECT DISTINCT STRFTIME ('%m-%d-%Y', DATETIME ([lastrecited])) AS [dateGroup]
       FROM   [verses] [v]
       WHERE  [v].[IsMemorized]
       ORDER  BY [LastRecited])
       AND [v].[IsMemorized];



DROP VIEW IF EXISTS [main].[MemorizedVerseBookGroups];

CREATE VIEW [MemorizedVerseBookGroups]
AS
SELECT *
FROM   [verses] [v]
       INNER JOIN [books] [b] ON [v].[BookId] = [b].[Id]
       INNER JOIN [chapters] [c] ON [c].[id] = [v].[ChapterId]
WHERE  [v].[IsMemorized]
ORDER  BY
          [b].[Ordinal],
          [c].[Number],
          [v].[number];

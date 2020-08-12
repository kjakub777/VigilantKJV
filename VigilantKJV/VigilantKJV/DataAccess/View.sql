drop VIEW if exists TextKey ;
drop VIEW if exists BookNameChapter ;
CREATE VIEW BookNameChapter AS
SELECT name,
number,
c.id AS chapterid,
b.id AS bookid
FROM book b
INNER JOIN
chapter c ON b.id = c.BookId;
CREATE VIEW TextKey AS
SELECT v.id AS vid,
b.id AS bid,
b.Name || c.Number || ':' || v.Number AS [Key]
FROM verse v
INNER JOIN
chapter c ON c.id = v.ChapterId
INNER JOIN
book b ON b.id = c.BookId;

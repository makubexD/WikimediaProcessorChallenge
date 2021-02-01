CREATE PROCEDURE spLanguagePageReport
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.[Language]
        ,p.[Name] AS [Page]
        ,FORMAT(a.[ActivityDate], 'yyyyMM') AS [Period]
        ,SUM(a.[Count]) AS [Count]
    FROM [Pages] p
    INNER JOIN [Activities] a ON p.Id = a.PageId
    GROUP BY p.[Language], p.[Name], FORMAT(a.[ActivityDate], 'yyyyMM')
    ORDER BY p.[Name];
END
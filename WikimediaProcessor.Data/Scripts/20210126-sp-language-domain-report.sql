CREATE PROCEDURE spLanguageDomainReport
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.[Language]
        ,p.[Domain]
        ,FORMAT(a.[ActivityDate], 'yyyyMM') AS [Period]
        ,SUM(a.[Count]) AS [Count]
    FROM [Pages] p
    INNER JOIN [Activities] a ON p.Id = a.PageId
    GROUP BY p.[Language], p.[Domain], FORMAT(a.[ActivityDate], 'yyyyMM')
    ORDER BY p.[Domain];
END